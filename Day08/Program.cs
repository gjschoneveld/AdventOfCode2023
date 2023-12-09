using System.Numerics;

var input = File.ReadAllLines("input.txt");
var directions = input[0];
var nodes = input[2..].Select(Node.Parse).ToDictionary(n => n.Name);

var steps = 0;
var current = "AAA";

while (current != "ZZZ")
{
    var direction = directions[(int)(steps % directions.Length)];
    current = direction == 'L' ? nodes[current].Left : nodes[current].Right;
    steps++;
}

var answer1 = steps;
Console.WriteLine($"Answer 1: {answer1}");


var starts = nodes.Keys.Where(n => n.EndsWith('A')).ToList();
var periods = starts.Select(s => (BigInteger)FindPeriod(nodes, directions, s)).ToList();

var answer2 = periods.Aggregate(LeastCommonMultiple);
Console.WriteLine($"Answer 2: {answer2}");

BigInteger LeastCommonMultiple(BigInteger a, BigInteger b)
{
    return a / BigInteger.GreatestCommonDivisor(a, b) * b;
}

int FindPeriod(Dictionary<string, Node> nodes, string directions, string start)
{
    var steps = 0;
    var directionsIndex = 0;
    var current = start;

    var history = new Dictionary<string, Dictionary<int, int>>();

    while (!history.ContainsKey(current) || !history[current].ContainsKey(directionsIndex))
    {
        if (current.EndsWith('Z'))
        {
            if (!history.ContainsKey(current))
            {
                history[current] = [];
            }

            history[current][directionsIndex] = steps;
        }

        var direction = directions[directionsIndex];
        current = direction == 'L' ? nodes[current].Left : nodes[current].Right;

        steps++;
        directionsIndex = steps % directions.Length;
    }

    var period = steps - history[current][directionsIndex];

    // turns out that all start values end up with a period without
    // a prelude and without other end nodes within the period
    // so, we are only interested in the period
    return period;
}

class Node
{
    public required string Name { get; set; }

    public required string Left { get; set; }
    public required string Right { get; set; }

    public static Node Parse(string x)
    {
        var parts = x.Split((char[])[' ', '=', '(', ',', ')'], StringSplitOptions.RemoveEmptyEntries);

        return new()
        { 
            Name = parts[0],
            Left = parts[1],
            Right = parts[2]
        };
    }
}
