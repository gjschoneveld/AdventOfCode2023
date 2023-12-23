using System.Data;
using System.Text.RegularExpressions;
using Range = (int from, int to);

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

var accepted = new List<Dictionary<string, Range>>();

var toVisit = new Queue<(string target, Dictionary<string, Range> ranges)>(
[
    (
        "in",
        new()
        {
            ["x"] = (1, 4000),
            ["m"] = (1, 4000),
            ["a"] = (1, 4000),
            ["s"] = (1, 4000)
        }
    )
]);

while (toVisit.Count > 0)
{
    var (target, ranges) = toVisit.Dequeue();

    if (target == "A")
    {
        accepted.Add(ranges);

        continue;
    }
    
    if (target == "R")
    {
        continue;
    }

    var next = rules[target].Apply(ranges);

    foreach (var nextItem in next)
    {
        toVisit.Enqueue(nextItem);
    }
}

var answer2 = accepted.Sum(Combinations);
Console.WriteLine($"Answer 2: {answer2}");

long Combinations(Dictionary<string, Range> ranges)
{
    return ranges.Values.Select(r => r.to - r.from + 1L).Aggregate((a, b) => a * b);
}

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

    public List<Dictionary<string, Range>> Split(Dictionary<string, Range> ranges, string key, int value)
    {
        var result = new List<Dictionary<string, Range>>
        {
            ranges.ToDictionary(kv => kv.Key, kv => kv.Value),
            ranges.ToDictionary(kv => kv.Key, kv => kv.Value)
        };

        result[0][key] = (ranges[key].from, value - 1);
        result[1][key] = (value, ranges[key].to);

        return result;
    }

    public List<(string target, Dictionary<string, Range> ranges)> Apply(Dictionary<string, Range> ranges)
    {
        var result = new List<(string target, Dictionary<string, Range> ranges)>();

        foreach (var step in Steps)
        {
            switch (step.Operator)
            {
                case '<':
                    if (ranges[step.Left].to < step.Right)
                    {
                        result.Add((step.Target, ranges));

                        return result;
                    }
                    
                    if (ranges[step.Left].from < step.Right)
                    {
                        var newRanges = Split(ranges, step.Left, step.Right);

                        result.Add((step.Target, newRanges[0]));
                        ranges = newRanges[1];
                    }

                    break;
                case '>':
                    if (ranges[step.Left].from > step.Right)
                    {
                        result.Add((step.Target, ranges));

                        return result;
                    }
                    
                    if (ranges[step.Left].to > step.Right)
                    {
                        var newRanges = Split(ranges, step.Left, step.Right + 1);

                        result.Add((step.Target, newRanges[1]));
                        ranges = newRanges[0];
                    }

                    break;
                default:
                    result.Add((step.Target, ranges));
                    break;
            }
        }

        return result;
    }

    public static Rule Parse(string X)
    {
        var parts = X.Split(new[] { '{', ',', '}' }, StringSplitOptions.RemoveEmptyEntries);

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
