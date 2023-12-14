using Point = (int x, int y);

var input = File.ReadAllLines("input.txt");
var lengthX = input[0].Length;
var lengthY = input.Length;

var rocks = Parse(input);
TiltNorth(rocks, lengthX, lengthY);

var answer1 = CalculateLoad(rocks, lengthY);
Console.WriteLine($"Answer 1: {answer1}");


rocks = Parse(input);

var history = new Dictionary<Dictionary<Point, RockType>, int>(new RockComparer());
var cycles = 1_000_000_000;
var cycle = 1;

while (!history.ContainsKey(rocks))
{
    history[rocks] = cycle;
    Cycle(rocks, lengthX, lengthY);
    cycle++;
}

var period = cycle - history[rocks];
var fullPeriodsLeft = (cycles - cycle) / period;
cycle += fullPeriodsLeft * period;

while (cycle <= cycles)
{
    Cycle(rocks, lengthX, lengthY);
    cycle++;
}

var answer2 = CalculateLoad(rocks, lengthY);
Console.WriteLine($"Answer 2: {answer2}");

int CalculateLoad(Dictionary<Point, RockType> rocks, int lengthY)
{
    return rocks.Where(kv => kv.Value == RockType.Rounded).Sum(kv => lengthY - kv.Key.y);
}

void Cycle(Dictionary<Point, RockType> rocks, int lengthX, int lengthY)
{
    TiltNorth(rocks, lengthX, lengthY);
    TiltWest(rocks, lengthX, lengthY);
    TiltSouth(rocks, lengthX, lengthY);
    TiltEast(rocks, lengthX, lengthY);
}

void TiltNorth(Dictionary<Point, RockType> rocks, int lengthX, int lengthY)
{
    for (int x = 0; x < lengthX; x++)
    {
        TiltLine(rocks, lengthY, p => (x, p));
    }
}

void TiltWest(Dictionary<Point, RockType> rocks, int lengthX, int lengthY)
{
    for (int y = 0; y < lengthY; y++)
    {
        TiltLine(rocks, lengthX, p => (p, y));
    }
}

void TiltSouth(Dictionary<Point, RockType> rocks, int lengthX, int lengthY)
{
    for (int x = 0; x < lengthX; x++)
    {
        TiltLine(rocks, lengthY, p => (x, lengthY - p - 1));
    }
}

void TiltEast(Dictionary<Point, RockType> rocks, int lengthX, int lengthY)
{
    for (int y = 0; y < lengthY; y++)
    {
        TiltLine(rocks, lengthX, p => (lengthX - p - 1, y));
    }
}

void TiltLine(Dictionary<Point, RockType> rocks, int length, Func<int, Point> position)
{
    int empty = 0;
    int rock = 0;

    while (true)
    {
        // find next empty spot
        while (empty < length && rocks.ContainsKey(position(empty)))
        {
            empty++;
        }

        // find next rock
        rock = Math.Max(rock, empty + 1);

        while (rock < length && !rocks.ContainsKey(position(rock)))
        {
            rock++;
        }

        if (rock >= length)
        {
            // no more rocks in this line
            return;
        }

        switch (rocks[position(rock)])
        {
            case RockType.CubeShaped:
                empty = rock + 1;
                break;
            case RockType.Rounded:
                rocks.Remove(position(rock));
                rocks[position(empty)] = RockType.Rounded;

                empty++;
                rock++;
                break;
        };
    }
}

Dictionary<Point, RockType> Parse(string[] input)
{
    var result = new Dictionary<Point, RockType>();

    for (int y = 0; y < input.Length; y++)
    {
        for (int x = 0; x < input[y].Length; x++)
        {
            if (input[y][x] == 'O')
            {
                result[(x, y)] = RockType.Rounded;
            }
            else if (input[y][x] == '#')
            {
                result[(x, y)] = RockType.CubeShaped;
            }
        }
    }

    return result;
}

enum RockType
{
    Rounded,
    CubeShaped
}

class RockComparer : IEqualityComparer<Dictionary<Point, RockType>>
{
    static IOrderedEnumerable<Point> GetPoints(Dictionary<(int x, int y), RockType>? rocks)
    {
        return rocks!.Where(kv => kv.Value == RockType.Rounded).Select(kv => kv.Key).OrderBy(p => p.x).ThenBy(p => p.y);
    }

    public bool Equals(Dictionary<(int x, int y), RockType>? a, Dictionary<(int x, int y), RockType>? b)
    {
        return GetPoints(a).SequenceEqual(GetPoints(b));
    }

    public int GetHashCode(Dictionary<(int x, int y), RockType> obj)
    {
        var hashCode = new HashCode();

        foreach (var point in GetPoints(obj))
        {
            hashCode.Add(point);
        }

        return hashCode.ToHashCode();
    }
}
