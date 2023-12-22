using Point = (int x, int y);

var input = File.ReadAllLines("input.txt");
var rocks = FindOfType(input, '#');
var start = FindOfType(input, 'S').First();

var toVisit = new List<Point> { start };
var visited = new HashSet<Point> { start };

var steps = 64;

for (int i = 0; i < steps; i++)
{
    toVisit = toVisit
        .SelectMany(Neighbours)
        .Distinct()
        .Where(nb => !rocks.Contains(nb))
        .Where(nb => !visited.Contains(nb))
        .ToList();

    visited.UnionWith(toVisit);
}

var answer1 = visited.Select(p => p.x + p.y).Where(v => v % 2 == steps % 2).Count();
Console.WriteLine($"Answer 1: {answer1}");

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
