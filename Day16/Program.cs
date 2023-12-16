using Point = (int x, int y);

var input = File.ReadAllLines("input.txt");
var lengthX = input[0].Length;
var lengthY = input.Length;

var answer1 = EnergizeTiles(input, new() { Position = (0, 0), Direction = Direction.Right });
Console.WriteLine($"Answer 1: {answer1}");

List<Step> starts = [
    .. Enumerable.Range(0, lengthX).Select(x => new Step { Position = (x, 0), Direction = Direction.Down }),
    .. Enumerable.Range(0, lengthX).Select(x => new Step { Position = (x, lengthY - 1), Direction = Direction.Up }),
    .. Enumerable.Range(0, lengthY).Select(y => new Step { Position = (0, y), Direction = Direction.Right }),
    .. Enumerable.Range(0, lengthY).Select(y => new Step { Position = (lengthX - 1, y), Direction = Direction.Left })
];

var answer2 = starts.Max(s => EnergizeTiles(input, s));
Console.WriteLine($"Answer 2: {answer2}");

int EnergizeTiles(string[] input, Step start)
{
    var visited = new HashSet<Step>();
    var toVisit = new Queue<Step>([start]);

    while (toVisit.Count > 0)
    {
        var step = toVisit.Dequeue();
        visited.Add(step);

        var next = Turn(input, step)
            .Select(s => s.Next())
            .Where(s => IsValid(input, s.Position))
            .Where(s => !visited.Contains(s))
            .ToList();

        foreach (var s in next)
        {
            toVisit.Enqueue(s);
        }
    }

    return visited.Select(s => s.Position).Distinct().Count();
}

List<Step> Turn(string[] input, Step step)
{
    return input[step.Position.y][step.Position.x] switch
    {
        '/' when step.Direction is Direction.Left => [step with { Direction = Direction.Down }],
        '/' when step.Direction is Direction.Right => [step with { Direction = Direction.Up }],
        '/' when step.Direction is Direction.Up => [step with { Direction = Direction.Right }],
        '/' when step.Direction is Direction.Down => [step with { Direction = Direction.Left }],
        '\\' when step.Direction is Direction.Left => [step with { Direction = Direction.Up }],
        '\\' when step.Direction is Direction.Right => [step with { Direction = Direction.Down }],
        '\\' when step.Direction is Direction.Up => [step with { Direction = Direction.Left }],
        '\\' when step.Direction is Direction.Down => [step with { Direction = Direction.Right }],
        '|' when step.Direction is Direction.Left or Direction.Right =>
            [
                step with { Direction = Direction.Up },
                step with { Direction = Direction.Down }
            ],
        '-' when step.Direction is Direction.Up or Direction.Down =>
            [
                step with { Direction = Direction.Left },
                step with { Direction = Direction.Right }
            ],
        _ => [step]
    };
}

bool IsValid(string[] input, Point position)
{
    return 0 <= position.y && position.y < input.Length
        && 0 <= position.x && position.x < input[position.y].Length;
}

enum Direction
{
    Left,
    Right,
    Up,
    Down
}

record Step
{
    public Point Position { get; set; }
    public Direction Direction { get; set; }

    public Step Next()
    {
        return Direction switch
        {
            Direction.Left => this with { Position = (Position.x - 1, Position.y) },
            Direction.Right => this with { Position = (Position.x + 1, Position.y) },
            Direction.Up => this with { Position = (Position.x, Position.y - 1) },
            Direction.Down => this with { Position = (Position.x, Position.y + 1) },
            _ => throw new()
        };
    }
}
