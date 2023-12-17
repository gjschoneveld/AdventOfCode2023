using Point = (int x, int y);
using Map = string[];

var map = File.ReadAllLines("input.txt");

var answer1 = FindMinimalHeatLoss(map);
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = FindMinimalHeatLoss(map, true);
Console.WriteLine($"Answer 2: {answer2}");

int FindMinimalHeatLoss(Map map, bool part2 = false)
{
    var end = (map[0].Length - 1, map.Length - 1);

    var best = new Dictionary<Point, List<State>>();
    var toVisit = new PriorityQueue<State, int>([(new(), 0)]);

    while (true)
    {
        var state = toVisit.Dequeue();

        if (state.Position == end)
        {
            if (part2 && state.StepsInDirection < 4)
            {
                // this solution is not allowed
                continue;
            }

            return state.HeatLoss;
        }

        var next = Next(map, state, part2);

        foreach (var n in next)
        {
            if (!best.ContainsKey(n.Position))
            {
                best[n.Position] = [];
            }

            var bestIndex = best[n.Position].FindIndex(s => s.Direction == n.Direction && s.StepsInDirection == n.StepsInDirection);

            if (bestIndex >= 0 && best[n.Position][bestIndex].HeatLoss <= n.HeatLoss)
            {
                // we have already seen an equal or better state
                continue;
            }

            if (bestIndex == -1)
            {
                best[n.Position].Add(n);
            }
            else
            {
                best[n.Position][bestIndex] = n;
            }

            toVisit.Enqueue(n, n.HeatLoss);
        }
    }
}

bool IsValid(Map map, Point position)
{
    return 0 <= position.y && position.y < map.Length &&
        0 <= position.x && position.x < map[position.y].Length;
}

int Value(Map map, Point position)
{
    return map[position.y][position.x] - '0';
}

Point Step(Point position, Direction direction)
{
    return direction switch
    {
        Direction.Left => (position.x - 1, position.y),
        Direction.Right => (position.x + 1, position.y),
        Direction.Up => (position.x, position.y - 1),
        Direction.Down => (position.x, position.y + 1),
        _ => throw new()
    };
}

Direction Opposite(Direction direction)
{
    return direction switch
    {
        Direction.Left => Direction.Right,
        Direction.Right => Direction.Left,
        Direction.Up => Direction.Down,
        Direction.Down => Direction.Up,
        _ => throw new()
    };
}

List<State> Next(Map map, State state, bool part2)
{
    var result = new List<State>();

    List<Direction> directions = [Direction.Left, Direction.Right, Direction.Up, Direction.Down];

    foreach (var direction in directions)
    {
        if (state.Direction == Opposite(direction))
        {
            // turning back is not allowed
            continue;
        }

        var minSteps = part2 ? 4 : 0;
        var maxSteps = part2 ? 10 : 3;

        if (state.Direction != Direction.None && state.Direction != direction && state.StepsInDirection < minSteps)
        {
            // we need more steps in the current direction
            continue;
        }

        if (state.Direction == direction && state.StepsInDirection >= maxSteps)
        {
            // we are already at the max steps of this direction
            continue;
        }

        var position = Step(state.Position, direction);

        if (!IsValid(map, position))
        {
            // position is outside the map
            continue;
        }

        var steps = state.Direction == direction ? state.StepsInDirection + 1 : 1;

        result.Add(new()
        {
            Position = position,
            Direction = direction,
            StepsInDirection = steps,
            HeatLoss = state.HeatLoss + Value(map, position)
        });
    }

    return result;
}

enum Direction
{
    None,
    Left,
    Right,
    Up,
    Down
}

class State
{
    public Point Position { get; set; }
    public Direction Direction { get; set; } = Direction.None;
    public int StepsInDirection = 1;
    public int HeatLoss { get; set; }
}
