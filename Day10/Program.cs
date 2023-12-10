using Map = string[];
using Position = (int x, int y);

var pipes = new Dictionary<char, Dictionary<Direction, (Direction direction, int angle)>>
{
    ['|'] = new()
    {
        [Direction.North] = (Direction.South, 0),
        [Direction.South] = (Direction.North, 0)
    },
    ['-'] = new()
    {
        [Direction.East] = (Direction.West, 0),
        [Direction.West] = (Direction.East, 0)
    },
    ['L'] = new()
    {
        [Direction.North] = (Direction.East, -90),
        [Direction.East] = (Direction.North, 90)
    },
    ['J'] = new()
    {
        [Direction.North] = (Direction.West, 90),
        [Direction.West] = (Direction.North, -90)
    },
    ['7'] = new()
    {
        [Direction.South] = (Direction.West, -90),
        [Direction.West] = (Direction.South, 90)
    },
    ['F'] = new()
    {
        [Direction.South] = (Direction.East, 90),
        [Direction.East] = (Direction.South, -90)
    },
    ['.'] = new()
};

var map = File.ReadAllLines("input.txt");

var visited = Visit(map);

var answer1 = visited.Count / 2;
Console.WriteLine($"Answer 1: {answer1}");

map = RemoveUnvisited(map, visited);
var inside = Visit(map, false);

var answer2 = inside.Count;
Console.WriteLine($"Answer 2: {answer2}");

Map RemoveUnvisited(Map map, HashSet<Position> visited)
{
    return map.Select((line, y) => new string(line.Select((cell, x) => visited.Contains((x, y)) ? cell : '.').ToArray())).ToArray();
}

HashSet<Position> Visit(Map map, bool returnVisited = true)
{
    var startPosition = FindStart(map);
    var startType = DetermineType(map, startPosition);
    pipes['S'] = pipes[startType];

    var visited = new HashSet<Position>();

    var left = new HashSet<Position>();
    var right = new HashSet<Position>();

    var position = startPosition;
    var direction = pipes['S'].Keys.First();

    var angle = 0;

    while (!visited.Contains(position))
    {
        visited.Add(position);

        CheckEmpty(map, Step(position, Left(direction)), left);
        CheckEmpty(map, Step(position, Right(direction)), right);

        position = Step(position, direction);

        CheckEmpty(map, Step(position, Left(direction)), left);
        CheckEmpty(map, Step(position, Right(direction)), right);

        var pipe = GetPipe(map, position);
        var info = pipes[pipe][Opposite(direction)];
        direction = info.direction;
        angle += info.angle;
    }

    if (returnVisited)
    {
        return visited;
    }

    // angle of 360 means we have clockwise loop, -360 is counterclockwise
    var inside = angle switch
    {
        -360 => left,
        360 => right,
        _ => throw new()
    };

    return FindEnclosedEmpty(map, inside);
}

HashSet<Position> FindEnclosedEmpty(Map map, HashSet<Position> positions)
{
    List<Direction> directions = [
        Direction.North,
        Direction.South,
        Direction.East,
        Direction.West];

    var visited = new HashSet<Position>();
    var toVisit = new Queue<Position>(positions);

    while (toVisit.Count > 0)
    {
        var position = toVisit.Dequeue();
        visited.Add(position);

        var emptyNeighbours = directions
            .Select(d => Step(position, d))
            .Where(nb => GetPipe(map, nb) == '.')
            .ToList();

        foreach (var neighbour in emptyNeighbours)
        {
            if (visited.Contains(neighbour))
            {
                continue;
            }

            toVisit.Enqueue(neighbour);
        }
    }

    return visited;
}


void CheckEmpty(Map map, Position position, HashSet<Position> result)
{
    if (GetPipe(map, position) == '.')
    {
        result.Add(position);
    }
}

Position FindStart(Map map)
{
    for (int y = 0; y < map.Length; y++)
    {
        for (int x = 0; x < map[y].Length; x++)
        {
            if (map[y][x] == 'S')
            {
                return (x, y);
            }
        }
    }

    throw new();
}

Position Step(Position position, Direction direction)
{
    return direction switch
    {
        Direction.North => (position.x, position.y - 1),
        Direction.South => (position.x, position.y + 1),
        Direction.East => (position.x + 1, position.y),
        Direction.West => (position.x - 1, position.y),
        _ => throw new()
    };
}

char GetPipe(Map map, Position position)
{
    if (position.y < 0 || position.y >= map.Length ||
        position.x < 0 || position.x >= map[position.y].Length)
    {
        return '.';
    }

    return map[position.y][position.x];
}

Direction Opposite(Direction direction)
{
    return (Direction)(((int)direction + 2) % 4);
}

Direction Right(Direction direction)
{
    return (Direction)(((int)direction + 1) % 4);
}

Direction Left(Direction direction)
{
    return (Direction)(((int)direction + 3) % 4);
}

char DetermineType(Map map, Position position)
{
    List<Direction> directions = [
        Direction.North,
        Direction.South,
        Direction.East,
        Direction.West];

    var connected = directions.Where(d => pipes[GetPipe(map, Step(position, d))].ContainsKey(Opposite(d))).ToList();

    return pipes.FirstOrDefault(p => p.Value.ContainsKey(connected[0]) && p.Value[connected[0]].direction == connected[1]).Key;
}

enum Direction
{
    North,
    East,
    South,
    West
}
