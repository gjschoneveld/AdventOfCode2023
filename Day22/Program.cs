using Point = (int x, int y, int z);

var input = File.ReadAllLines("input.txt");

var withoutMoving = 0;
var totalMoved = 0;

for (int i = 0; i < input.Length; i++)
{
    var moved = Simulate(input, i);
    withoutMoving += moved == 0 ? 1 : 0;
    totalMoved += moved;
}

var answer1 = withoutMoving;
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = totalMoved;
Console.WriteLine($"Answer 2: {answer2}");

int Simulate(string[] input, int toRemove)
{
    var bricks = input.Select(Brick.Parse).ToList();

    var grid = new Dictionary<Point, Brick>();

    foreach (var brick in bricks)
    {
        brick.Add(grid);
    }

    MoveBricks(grid);

    bricks[toRemove].Remove(grid);

    return MoveBricks(grid);
}

int MoveBricks(Dictionary<Point, Brick> grid)
{
    var moved = 0;

    foreach (var brick in grid.Values.Distinct().OrderBy(b => b.Positions.Min(p => p.z)))
    {
        if (brick.MoveDown(grid))
        {
            moved++;
        }
    }

    return moved;
}

class Brick
{
    public required List<Point> Positions { get; set; }

    public Point Below(Point position)
    {
        return (position.x, position.y, position.z - 1);
    }

    public bool HasBrickBelow(Dictionary<Point, Brick> grid)
    {
        return Positions
            .Select(Below)
            .Any(p => grid.ContainsKey(p) && grid[p] != this);
    }

    public bool CanMoveDown(Dictionary<Point, Brick> grid)
    {
        return Positions.All(p => p.z > 1) && !HasBrickBelow(grid);
    }

    public bool MoveDown(Dictionary<Point, Brick> grid)
    {
        if (!CanMoveDown(grid))
        {
            return false;
        }

        Remove(grid);

        while (CanMoveDown(grid))
        {
            Positions = Positions.Select(Below).ToList();
        }

        Add(grid);

        return true;
    }

    public void Add(Dictionary<Point, Brick> grid)
    {
        foreach (var position in Positions)
        {
            grid.Add(position, this);
        }
    }

    public void Remove(Dictionary<Point, Brick> grid)
    {
        foreach (var position in Positions)
        {
            grid.Remove(position);
        }
    }

    public static List<Point> FindPositions(Point start, Point end)
    {
        Point delta = (Math.Sign(end.x - start.x), Math.Sign(end.y - start.y), Math.Sign(end.z - start.z));

        var position = start;
        var result = new List<Point> {  start };

        while (position != end)
        {
            position = (position.x + delta.x, position.y + delta.y, position.z + delta.z);
            result.Add(position);
        }

        return result;
    }

    public static Brick Parse(string x)
    {
        var parts = x.Split(',', '~');
        var values = parts.Select(int.Parse).ToList();

        var start = (values[0], values[1], values[2]);
        var end = (values[3], values[4], values[5]);

        return new()
        {
            Positions = FindPositions(start, end)
        };
    }
}
