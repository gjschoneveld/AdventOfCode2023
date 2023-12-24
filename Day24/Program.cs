using MathNet.Numerics.LinearAlgebra;
using Point = (double x, double y, double z);

var input = File.ReadAllLines("input.txt");
var hailstones = input.Select(FlyingObject.Parse).ToList();

var minimum = 200000000000000;
var maximum = 400000000000000;

var withinBounds = 0;

for (int i = 0; i < hailstones.Count; i++)
{
    for (int j = i + 1; j < hailstones.Count; j++)
    {
        var intersection = FindIntersection2D(hailstones[i], hailstones[j]);

        if (intersection != null && WithinBounds2D(intersection.Value, minimum, maximum))
        {
            withinBounds++;
        }
    }
}

var answer1 = withinBounds;
Console.WriteLine($"Answer 1: {answer1}");

var rock = FindRock(hailstones);
var answer2 = Math.Round(rock.Position.x + rock.Position.y + rock.Position.z);
Console.WriteLine($"Answer 2: {answer2}");

FlyingObject FindRock(List<FlyingObject> hailstones)
{
    foreach (var times in GetTimes())
    {
        Console.WriteLine((times[0], times[1]));

        for (int i = 0; i < hailstones.Count; i++)
        {
            for (int j = 0; j < hailstones.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }

                var rock = FindRockCandidate([hailstones[i], hailstones[j]], times);

                if (rock == null)
                {
                    continue;
                }

                if (Math.Abs(rock.Velocity.x) > 1000 || Math.Abs(rock.Velocity.y) > 1000 || Math.Abs(rock.Velocity.z) > 1000)
                {
                    continue;
                }

                if (IntersectsAll(hailstones, rock))
                {
                    return rock;
                }
            }
        }
    }

    throw new();
}

IEnumerable<List<int>> GetTimes()
{
    var maxTime = 0;

    while (true)
    {
        for (int i = 0; i <= maxTime; i++)
        {
            yield return [i, maxTime];
        }

        for (int i = 0; i <= maxTime - 1; i++)
        {
            yield return [maxTime, i];
        }

        maxTime++;
    }
}

bool IntersectsAll(List<FlyingObject> hailstones, FlyingObject rock)
{
    foreach (var hailstone in hailstones)
    {
        var intersection = FindIntersection3D(hailstone, rock);

        if (intersection == null ||
            !IsInteger(intersection.Value.x) ||
            !IsInteger(intersection.Value.y) ||
            !IsInteger(intersection.Value.z))
        {
            return false;
        }
    }

    return true;
}

bool WithinBounds1D(double value, double minimum, double maximum)
{
    return minimum <= value && value <= maximum; 
}

bool WithinBounds2D(Point position, double minimum, double maximum)
{
    return WithinBounds1D(position.x, minimum, maximum) &&
        WithinBounds1D(position.y, minimum, maximum);
}

Point Add(Point a, Point b)
{
    return (a.x + b.x, a.y + b.y, a.z + b.z);
}

Point Multiply(Point a, double scalar)
{
    return (scalar * a.x, scalar * a.y, scalar * a.z);
}

Point? FindIntersection2D(FlyingObject a, FlyingObject b)
{
    var matrix = Matrix<double>.Build.DenseOfArray(new double[,] {
        { a.Velocity.x, -b.Velocity.x },
        { a.Velocity.y, -b.Velocity.y }
    });

    var result = Vector<double>.Build.Dense([
        b.Position.x - a.Position.x,
        b.Position.y - a.Position.y
    ]);

    var parameters = matrix.Solve(result);

    if (parameters.Any(p => !double.IsFinite(p) || p < 0))
    {
        return null;
    }

    return Add(a.Position, Multiply(a.Velocity, parameters[0]));
}

Point? FindIntersection3D(FlyingObject a, FlyingObject b)
{
    var matrix = Matrix<double>.Build.DenseOfArray(new double[,] {
        { a.Velocity.x, -b.Velocity.x },
        { a.Velocity.y, -b.Velocity.y },
        { a.Velocity.z, -b.Velocity.z }
    });

    var result = Vector<double>.Build.Dense([
        b.Position.x - a.Position.x,
        b.Position.y - a.Position.y,
        b.Position.z - a.Position.z,
    ]);

    var parameters = matrix.Solve(result);

    if (parameters.Any(p => !double.IsFinite(p) || p < 0))
    {
        return null;
    }

    return Add(a.Position, Multiply(a.Velocity, parameters[0]));
}

bool IsInteger(double value)
{
    if (!double.IsFinite(value))
    {
        return false;
    }

    var rounded = Math.Round(value);

    return Math.Abs(value - rounded) < 1e-6;
}

FlyingObject? FindRockCandidate(List<FlyingObject> hailstones, List<int> times)
{
    var matrix = Matrix<double>.Build.DenseOfArray(new double[,] {
        { times[0], 0, 0, 1, 0, 0 },
        { 0, times[0], 0, 0, 1, 0 },
        { 0, 0, times[0], 0, 0, 1 },
        { 1, 0, 0, times[1], 0, 0 },
        { 0, 1, 0, 0, times[1], 0 },
        { 0, 0, 1, 0, 0, times[1] },
    });

    var result = Vector<double>.Build.Dense([
        hailstones[0].Position.x + times[0] * hailstones[0].Velocity.x,
        hailstones[0].Position.y + times[0] * hailstones[0].Velocity.y,
        hailstones[0].Position.z + times[0] * hailstones[0].Velocity.z,
        hailstones[1].Position.x + times[1] * hailstones[1].Velocity.x,
        hailstones[1].Position.y + times[1] * hailstones[1].Velocity.y,
        hailstones[1].Position.z + times[1] * hailstones[1].Velocity.z,
    ]);

    var parameters = matrix.Solve(result);

    if (parameters.Any(p => !IsInteger(p)))
    {
        return null;
    }

    return new()
    {
        Position = (parameters[0], parameters[1], parameters[2]),
        Velocity = (parameters[3], parameters[4], parameters[5])
    };
}

class FlyingObject
{
    public Point Position { get; set; }
    public Point Velocity { get; set; }

    public static FlyingObject Parse(string x)
    {
        var parts = x.Split(new[] { ',', ' ', '@' }, StringSplitOptions.RemoveEmptyEntries);
        var values = parts.Select(double.Parse).ToList();

        return new()
        {
            Position = (values[0], values[1], values[2]),
            Velocity = (values[3], values[4], values[5])
        };
    }
}
