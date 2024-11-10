﻿using Point = (int x, int y);

var input = File.ReadAllLines("input.txt");
var size = input.Length;
var rocks = FindOfType(input, '#');
var start = FindOfType(input, 'S').First();

var answer1 = Simulate(64);
Console.WriteLine($"Answer 1: {answer1}");

//for (int steps = 65; steps < 262 * 50; steps += 262)
//{
//    Console.WriteLine($"{steps}: {Simulate(steps)}");
//    Console.WriteLine($"{steps}: {SimulateMany(steps)}");
//}

var answer2 = SimulateMany(26501365);
Console.WriteLine($"Answer 2: {answer2}");

long SimulateMany(int steps)
{
    // times 2 so it is even resulting in always odd steps being simulated
    var remainder = steps % (size * 2);

    long value = 0;
    long increment = 0;
    long incrementIncrement = 0;

    var index = 0;

    while (index <= steps / (size * 2))
    {
        var nextSteps = index * size * 2 + remainder;
        var nextValue = Simulate(nextSteps);

        var nextIncrement = nextValue - value;
        var nextIncrementIncrement = nextIncrement - increment;

        if (nextIncrementIncrement == incrementIncrement)
        {
            break;
        }

        value = nextValue;
        increment = nextIncrement;
        incrementIncrement = nextIncrementIncrement;

        index++;
    }

    while (index <= steps / (size * 2))
    {
        increment += incrementIncrement;
        value += increment;

        index++;
    }

    return value;
}

int Simulate(int steps)
{
    var toVisit = new List<Point> { start };
    var visited = new HashSet<Point> { start };

    for (int i = 0; i < steps; i++)
    {
        toVisit = toVisit
            .SelectMany(Neighbours)
            .Distinct()
            .Where(nb => !rocks.Contains(Normalize(nb)))
            .Where(nb => !visited.Contains(nb))
            .ToList();

        visited.UnionWith(toVisit);
    }

    return visited.Count(p => (p.x + p.y) % 2 == steps % 2);
}

int Modulo(int x, int mod)
{
    var result = x % mod;

    if (result < 0)
    {
        result += mod;
    }

    return result;
}

Point Normalize(Point position)
{
    return (Modulo(position.x, size), Modulo(position.y, size));
}

List<Point> Neighbours(Point position)
{
    return [
        (position.x - 1, position.y),
        (position.x + 1, position.y),
        (position.x, position.y - 1),
        (position.x, position.y + 1),
    ];
}

HashSet<Point> FindOfType(string[] input, char type)
{
    var result = new HashSet<Point>();

    for (int y = 0; y < input.Length; y++)
    {
        for (int x = 0; x < input[y].Length; x++)
        {
            if (input[y][x] == type)
            {
                result.Add(new Point(x, y));
            }
        }
    }

    return result;
}
