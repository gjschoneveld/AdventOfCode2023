using System.Data;
using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");
var separator = Array.FindIndex(input, string.IsNullOrEmpty);

var rules = input[..separator].Select(Rule.Parse).ToDictionary(r => r.Name);
var parts = input[(separator + 1)..].Select(Part.Parse).ToList();

int total = 0;

foreach (var part in parts)
{
    var rule = "in";

    while (rule != "A" && rule != "R")
    {
        rule = rules[rule].Apply(part);
    }

    if (rule == "A")
    {
        total += part.Values.Values.Sum();
    }
}

var answer1 = total;
Console.WriteLine($"Answer 1: {answer1}");

class Step
{
    public required string Left { get; set; }
    public int Right { get; set; }
    public char Operator { get; set; }

    public required string Target { get; set; }

    public static Step Parse(string x)
    {
        var match = Regex.Match(x, @"((?<left>[xmas])(?<op>[<>])(?<right>\d+):)?(?<target>\w+)");

        var left = match.Groups["left"].Value;
        var op = match.Groups["op"].Value;
        var right = match.Groups["right"].Value;
        var target = match.Groups["target"].Value;

        return new()
        {
            Left = left,
            Right = right.Length > 0 ? int.Parse(right) : 0,
            Operator = op.Length > 0 ? op[0] : '\0',
            Target = target
        };
    }
}

class Rule
{
    public required string Name { get; set; }
    public required List<Step> Steps { get; set; }

    public string Apply(Part part)
    {
        foreach (var step in Steps)
        {
            var match = step.Operator switch
            {
                '<' => part.Values[step.Left] < step.Right,
                '>' => part.Values[step.Left] > step.Right,
                _ => true
            };

            if (match)
            {
                return step.Target;
            }
        }

        throw new();
    }

    public static Rule Parse(string X)
    {
        var parts = X.Split('{', ',', '}');

        return new()
        {
            Name = parts[0],
            Steps = parts[1..].Select(Step.Parse).ToList()
        };
    }
}

class Part
{
    public required Dictionary<string, int> Values { get; set; }

    public static Part Parse(string x)
    {
        var matches = Regex.Matches(x, @"(?<key>[xmas])=(?<value>\d+)");

        var keys = matches.SelectMany(m => m.Groups["key"].Captures.Select(c => c.Value)).ToList();
        var values = matches.SelectMany(m => m.Groups["value"].Captures.Select(c => c.Value)).ToList();

        return new()
        {
            Values = keys.Zip(values.Select(int.Parse), (k, v) => (k, v)).ToDictionary(kv => kv.k, kv => kv.v)
        };
    }
}
