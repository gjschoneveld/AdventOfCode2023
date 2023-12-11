using Map = string[];
using Position = (int x, int y);

var map = File.ReadAllLines("input.txt");
var galaxies = FindGalaxies(map);

var emptyRows = Enumerable.Range(0, map.Length).Where(y => !galaxies.Any(g => g.y == y)).ToList();
var emptyColumns = Enumerable.Range(0, map[0].Length).Where(x => !galaxies.Any(g => g.x == x)).ToList();

var answer1 = DistanceSum(galaxies, emptyRows, emptyColumns);
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = DistanceSum(galaxies, emptyRows, emptyColumns, 1_000_000);
Console.WriteLine($"Answer 2: {answer2}");

List<Position> FindGalaxies(Map map)
{
    var result = new List<Position>();

    for (int y = 0; y < map.Length; y++)
    {
        for (int x = 0; x < map[y].Length; x++)
        {
            if (map[y][x] == '#')
            {
                result.Add((x, y));
            }
        }
    }

    return result;
}

long LineDistance(int start, int end, List<int> empty, int factor)
{
    if (start > end)
    {
        // swap to make sure start is the lower value
        (end, start) = (start, end);
    }

    long count = empty.Count(v => start < v && v < end);

    return end - start + (factor - 1) * count;
}

long Distance(Position start, Position end, List<int> emptyRows, List<int> emptyColumns, int factor)
{
    return LineDistance(start.x, end.x, emptyColumns, factor) + LineDistance(start.y, end.y, emptyRows, factor);
}

long DistanceSum(List<Position> galaxies, List<int> emptyRows, List<int> emptyColumns, int factor = 2)
{
    long distanceSum = 0;

    for (int i = 0; i < galaxies.Count; i++)
    {
        for (int j = i + 1; j < galaxies.Count; j++)
        {
            distanceSum += Distance(galaxies[i], galaxies[j], emptyRows, emptyColumns, factor);
        }
    }

    return distanceSum;
}
