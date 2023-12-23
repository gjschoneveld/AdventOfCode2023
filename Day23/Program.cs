using Point = (int x, int y);
using Vertex = (int start, int end, int length);
using Map = string[];

var map = File.ReadAllLines("input.txt");
Point start = (map[0].IndexOf('.'), 0);
Point end = (map[^1].IndexOf('.'), map.Length - 1);

var nodes = FindNodes(map);
nodes.Add(start);
nodes.Add(end);

var vertices = FindVertices(map, nodes, false);
var answer1 = LongestPath(vertices, [], nodes.IndexOf(start), nodes.IndexOf(end));
Console.WriteLine($"Answer 1: {answer1}");

vertices = FindVertices(map, nodes, true);
var answer2 = LongestPath(vertices, [], nodes.IndexOf(start), nodes.IndexOf(end));
Console.WriteLine($"Answer 2: {answer2}");

List<Point> FindNodes(Map map)
{
    var result = new List<Point>(); 

    for (int y = 0; y < map.Length; y++)
    {
        for (int x = 0; x < map[y].Length; x++)
        {
            if (Value(map, (x, y)) == '#')
            {
                continue;
            }

            if (Neighbours(map, (x, y), true).Count > 2)
            {
                result.Add((x, y));
            }
        }
    }

    return result;
}

Dictionary<int, List<Vertex>> FindVertices(Map map, List<Point> nodes, bool allowSlopes)
{
    var result = new List<Vertex>();

    foreach (var start in nodes)
    {
        result.AddRange(
            Neighbours(map, start, allowSlopes)
                .Select(nb => FindVertex(map, nodes, start, nb, allowSlopes))
                .Where(v => v != null)
                .Select(v => v!.Value));
    }

    return result.GroupBy(v => v.start).ToDictionary(g => g.Key, g => g.ToList());
}

Vertex? FindVertex(Map map, List<Point> nodes, Point start, Point next, bool allowSlopes)
{
    var visited = new HashSet<Point>([start]);

    var current = next;

    while (!nodes.Contains(current))
    {
        visited.Add(current);

        current = Neighbours(map, current, allowSlopes)
            .Where(nb => !visited.Contains(nb))
            .FirstOrDefault();

        if (current == default)
        {
            return null;
        }
    }

    return (nodes.IndexOf(start), nodes.IndexOf(current), visited.Count);
}

int? LongestPath(Dictionary<int, List<Vertex>> vertices, HashSet<int> visited, int current, int end)
{
    if (current == end)
    {
        return 0;
    }

    visited.Add(current);

    var innerPaths = vertices[current]
        .Where(v => !visited.Contains(v.end))
        .Select(nb => LongestPath(vertices, visited, nb.end, end) + nb.length)
        .Where(p => p != null)
        .Select(p => p!.Value)
        .ToList();

    visited.Remove(current);

    return innerPaths.Count > 0 ? innerPaths.Max() : null;
}

bool WithinBounds(Map map, Point position)
{
    return 0 <= position.y && position.y < map.Length &&
        0 <= position.x && position.x < map[position.y].Length;

}

char Value(Map map, Point position)
{
    return map[position.y][position.x];
}

List<Point> Neighbours(Map map, Point position, bool allowSlopes)
{
    var candidates = new List<(Point position, List<char> allowed)>
    {
        ((position.x - 1, position.y), allowSlopes ? ['.', '<', '>', '^', 'v'] : ['.', '<']),
        ((position.x + 1, position.y), allowSlopes ? ['.', '<', '>', '^', 'v'] : ['.', '>']),
        ((position.x, position.y - 1), allowSlopes ? ['.', '<', '>', '^', 'v'] : ['.', '^']),
        ((position.x, position.y + 1), allowSlopes ? ['.', '<', '>', '^', 'v'] : ['.', 'v']),
    };

    return candidates
        .Where(c => WithinBounds(map, c.position) && c.allowed.Contains(Value(map, c.position)))
        .Select(c => c.position)
        .ToList();
}
