using Position = (int x, int y);
using Direction = char;

var input = File.ReadAllLines("input.txt");
var lines = input.Select(Line.Parse).ToList();

var digged = Dig(lines);

var answer1 = digged.Count;
Console.WriteLine($"Answer 1: {answer1}");


lines = lines.Select(line => line.ParseColor()).ToList();

// step 1: find all used coordinates
var xCoordinatesSeen = new HashSet<long>();
var yCoordinatesSeen = new HashSet<long>();

var position = (0, 0);

foreach (var line in lines)
{
    var start = Step(position, line.Direction);
    var end = Step(position, line.Direction, line.Steps);
    position = end;

    if (line.Direction is 'L' or 'U')
    {
        (end, start) = (start, end);
    }

    xCoordinatesSeen.Add(start.x);
    xCoordinatesSeen.Add(end.x + 1);

    yCoordinatesSeen.Add(start.y);
    yCoordinatesSeen.Add(end.y + 1);

}

var xCoordinates = xCoordinatesSeen.OrderBy(x => x).ToList();
var yCoordinates = yCoordinatesSeen.OrderBy(y => y).ToList();

// step 2: convert commands to steps in coordinates
var newLines = new List<Line>();

position = (0, 0);

foreach (var line in lines)
{
    var start = Step(position, line.Direction);
    var end = Step(position, line.Direction, line.Steps);
    position = end;

    if (line.Direction is 'L' or 'U')
    {
        (end, start) = (start, end);
    }

    var steps = line.Direction switch
    {
        'L' or 'R' => xCoordinates.IndexOf(end.x + 1) - xCoordinates.IndexOf(start.x),
        'U' or 'D' => yCoordinates.IndexOf(end.y + 1) - yCoordinates.IndexOf(start.y),
        _ => throw new()
    };

    newLines.Add(new()
    {
        Direction = line.Direction,
        Steps = steps,
        Color = line.Color        
    });
}

// step 3: find digged using algorithm of part 1
digged = Dig(newLines);

// step 4: convert positions back to block sizes and add them all together
var offsetX = xCoordinates.IndexOf(0);
var offsetY = yCoordinates.IndexOf(0);

var area = 0L;

foreach (var (x, y) in digged)
{
    var lengthX = xCoordinates[x + offsetX + 1] - xCoordinates[x + offsetX];
    var lengthY = yCoordinates[y + offsetY + 1] - yCoordinates[y + offsetY];

    area += lengthX * lengthY;
}

var answer2 = area;
Console.WriteLine($"Answer 2: {answer2}");

HashSet<Position> Dig(List<Line> lines)
{
    Position position = (0, 0);
    HashSet<Position> border = [];
    HashSet<Position> interior = [];

    foreach (var line in lines)
    {
        interior.Add(RightOf(position, line.Direction));

        foreach (var index in Enumerable.Range(0, line.Steps))
        {
            position = Step(position, line.Direction);
            border.Add(position);
            interior.Add(RightOf(position, line.Direction));
        }
    }

    interior.ExceptWith(border);
    interior = FindEnclosed(border, interior);
    interior.UnionWith(border);

    return interior;
}

HashSet<Position> FindEnclosed(HashSet<Position> border, HashSet<Position> interior)
{
    List<Direction> directions = ['L', 'R', 'U', 'D'];

    var seen = new HashSet<Position>(interior);
    var toVisit = new Queue<Position>(interior);

    while (toVisit.Count > 0)
    {
        var position = toVisit.Dequeue();

        var emptyNeighbours = directions
            .Select(d => Step(position, d))
            .Where(nb => !border.Contains(nb))
            .ToList();

        foreach (var neighbour in emptyNeighbours)
        {
            if (seen.Contains(neighbour))
            {
                continue;
            }

            seen.Add(neighbour);
            toVisit.Enqueue(neighbour);
        }
    }

    return seen;
}

Position RightOf(Position position, char direction)
{
    return direction switch
    {
        'L' => Step(position, 'U'),
        'R' => Step(position, 'D'),
        'U' => Step(position, 'R'),
        'D' => Step(position, 'L'),
        _ => throw new()
    };
}

Position Step(Position position, char direction, int amount = 1)
{
    return direction switch
    {
        'L' => (position.x - amount, position.y),
        'R' => (position.x + amount, position.y),
        'U' => (position.x, position.y - amount),
        'D' => (position.x, position.y + amount),
        _ => throw new()
    };
}

class Line
{
    public required char Direction { get; set; }
    public required int Steps { get; set; }
    public required string Color { get; set; }

    public Line ParseColor()
    {
        var digits = Color[1..^1];
        var steps = Convert.ToInt32(digits, 16);

        var direction = Color[^1] switch
        {
            '0' => 'R',
            '1' => 'D',
            '2' => 'L',
            '3' => 'U',
            _ => throw new()
        };

        return new()
        {
            Direction = direction,
            Steps = steps,
            Color = Color
        };
    }

    public static Line Parse(string line)
    {
        var parts = line.Split(new[] { ' ', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);

        return new()
        {
            Direction = parts[0][0],
            Steps = int.Parse(parts[1]),
            Color = parts[2]
        };
    }
}
