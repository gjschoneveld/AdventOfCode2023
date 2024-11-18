﻿using MathNet.Numerics.LinearAlgebra;
using Point = (long x, long y, long z);

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

//var rock = FindRock(hailstones);
//var answer2 = rock.Position.x + rock.Position.y + rock.Position.z;
//Console.WriteLine($"Answer 2: {answer2}");

//var position = FindRockPosition(hailstones);
//var answer2 = position.x + position.y + position.z;
//Console.WriteLine($"Answer 2: {answer2}");

//var positionA = FindRockPosition2D(hailstones, [Axis.X, Axis.Y]);
//var positionB = FindRockPosition2D(hailstones, [Axis.X, Axis.Z]);
//var answer2 = positionA.first + positionA.second + positionB.second;
//Console.WriteLine($"Answer 2: {answer2}");

var positionA = FindRockPosition2D_V2(hailstones, [Axis.X, Axis.Y]);
var positionB = FindRockPosition2D_V2(hailstones, [Axis.X, Axis.Z]);
var answer2 = positionA.first + positionA.second + positionB.second;
Console.WriteLine($"Answer 2: {answer2}");

IEnumerable<long> GetFactors(long x)
{
    var abs = Math.Abs(x);
    var sqrt = (long)Math.Ceiling(Math.Sqrt(abs));

    for (long i = 1; i <= sqrt; i++)
    {
        if (abs % i == 0)
        {
            yield return i;
            yield return abs / i;
        }
    }
}

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

                if (IntersectsAll(hailstones, rock))
                {
                    return rock;
                }
            }
        }
    }

    throw new();
}

Point FindRockPosition(List<FlyingObject> hailstones)
{
    foreach (var velocity in GetVelocities())
    {
        var position = FindPosition(hailstones.Take(4).ToList(), velocity);

        if (position != null)
        {
            return position.Value;
        }
    }

    throw new();
}

(long first, long second) FindRockPosition2D(List<FlyingObject> hailstones, List<Axis> axes)
{
    foreach (var velocity in GetVelocities2D())
    {
        var position = FindPosition2D(hailstones.Take(4).ToList(), velocity, axes);

        if (position != null)
        {
            return position.Value;
        }
    }

    throw new();
}

(long first, long second) FindRockPosition2D_V2(List<FlyingObject> hailstones, List<Axis> axes)
{
    var withSameSpeed = hailstones.GroupBy(h => (GetValue(h.Velocity, axes[0]), GetValue(h.Velocity, axes[1]))).Select(g => g.ToList()).Where(l => l.Count > 1).First();

    var diff0 = GetValue(withSameSpeed[0].Position, axes[0]) - GetValue(withSameSpeed[1].Position, axes[0]);
    var diff1 = GetValue(withSameSpeed[0].Position, axes[1]) - GetValue(withSameSpeed[1].Position, axes[1]);

    var factors0 = GetFactors(diff0);
    var factors1 = GetFactors(diff1);

    var factors = factors0.Intersect(factors1).SelectMany(f => new List<long> { f, -f }).ToList();

    var velocities = new List<List<long>>();

    foreach (var t in factors)
    {
        var velocity0 = diff0 / t + GetValue(withSameSpeed[0].Velocity, axes[0]);
        var velocity1 = diff1 / t + GetValue(withSameSpeed[0].Velocity, axes[1]);

        velocities.Add([velocity0, velocity1]);
    }

    foreach (var velocity in velocities)
    {
        var position = FindPosition2D(hailstones.Take(4).ToList(), velocity, axes);

        if (position != null)
        {
            return position.Value;
        }
    }

    return (0, 0);
}

IEnumerable<List<int>> GetTimes()
{
    var max = 0; // 1002

    while (true)
    {
        for (int i = 0; i <= max; i++)
        {
            yield return [i, max];
        }

        for (int i = 0; i <= max - 1; i++)
        {
            yield return [max, i];
        }

        max++;
        Console.WriteLine(max);
    }
}

IEnumerable<List<long>> GetVelocities2D()
{
    var max = 0; // 10320

    while (true)
    {
        for (int i = 0; i <= max; i++)
        {
            yield return [i, -max];
            yield return [i, max];
        }

        for (int i = 0; i <= max - 1; i++)
        {
            yield return [-max, i];
            yield return [max, i];
        }

        max++;
        Console.WriteLine(max);
    }
}

IEnumerable<Point> GetVelocities()
{
    var max = 0; // 565

    while (true)
    {
        for (int x = -max; x <= max; x++)
        {
            for (int y = -max; y <= max; y++)
            {
                yield return (x, y, -max);
                yield return (x, y, max);
            }
        }

        for (int x = -max; x <= max; x++)
        {
            for (int z = -max; z <= max; z++)
            {
                yield return (x, -max, z);
                yield return (x, max, z);
            }
        }

        for (int y = -max; y <= max; y++)
        {
            for (int z = -max; z <= max; z++)
            {
                yield return (-max, y, z);
                yield return (max, y, z);
            }
        }

        max++;
        Console.WriteLine(max);
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

Point Multiply(Point a, long scalar)
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

    return Add(a.Position, Multiply(a.Velocity, (long)Math.Round(parameters[0])));
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

    return Add(a.Position, Multiply(a.Velocity, (long)Math.Round(parameters[0])));
}

bool IsInteger(double value)
{
    if (!double.IsFinite(value))
    {
        return false;
    }

    var rounded = Math.Round(value);

    return Math.Abs(value - rounded) < 1e-3;
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
        Position = ((long)Math.Round(parameters[0]), (long)Math.Round(parameters[1]), (long)Math.Round(parameters[2])),
        Velocity = ((long)Math.Round(parameters[3]), (long)Math.Round(parameters[4]), (long)Math.Round(parameters[5]))
    };
}

Point? FindPosition(List<FlyingObject> hailstones, Point velocity)
{
    var matrixData = new double[hailstones.Count * 3, hailstones.Count + 3];
    var resultData = new double[hailstones.Count * 3];

    for (var i = 0; i < hailstones.Count; i++)
    {
        matrixData[i * 3, 0] = 1;
        matrixData[i * 3 + 1, 1] = 1;
        matrixData[i * 3 + 2, 2] = 1;

        matrixData[i * 3, 3 + i] = velocity.x - hailstones[i].Velocity.x;
        matrixData[i * 3 + 1, 3 + i] = velocity.y - hailstones[i].Velocity.y;
        matrixData[i * 3 + 2, 3 + i] = velocity.z - hailstones[i].Velocity.z;

        resultData[i * 3] = hailstones[i].Position.x;
        resultData[i * 3 + 1] = hailstones[i].Position.y;
        resultData[i * 3 + 2] = hailstones[i].Position.z;
    }

    var matrix = Matrix<double>.Build.DenseOfArray(matrixData);
    var result = Vector<double>.Build.Dense(resultData);

    var parameters = matrix.Solve(result);

    if (parameters.Any(p => !IsInteger(p)))
    {
        return null;
    }

    return ((long)Math.Round(parameters[0]), (long)Math.Round(parameters[1]), (long)Math.Round(parameters[2]));
}

(long first, long second)? FindPosition2D(List<FlyingObject> hailstones, List<long> velocities, List<Axis> axes)
{
    var matrixData = new double[hailstones.Count * 2, hailstones.Count + 2];
    var resultData = new double[hailstones.Count * 2];

    for (var i = 0; i < hailstones.Count; i++)
    {
        matrixData[i * 2, 0] = 1;
        matrixData[i * 2 + 1, 1] = 1;

        matrixData[i * 2, 2 + i] = velocities[0] - GetValue(hailstones[i].Velocity, axes[0]);
        matrixData[i * 2 + 1, 2 + i] = velocities[1] - GetValue(hailstones[i].Velocity, axes[1]);

        resultData[i * 2] = GetValue(hailstones[i].Position, axes[0]);
        resultData[i * 2 + 1] = GetValue(hailstones[i].Position, axes[1]);
    }

    var matrix = Matrix<double>.Build.DenseOfArray(matrixData);
    var result = Vector<double>.Build.Dense(resultData);

    var parameters = matrix.Solve(result);

    if (parameters.Any(p => !IsInteger(p)))
    {
        return null;
    }

    return ((long)Math.Round(parameters[0]), (long)Math.Round(parameters[1]));
}

long GetValue(Point position, Axis axis)
{
    return axis switch
    {
        Axis.X => position.x,
        Axis.Y => position.y,
        Axis.Z => position.z,
        _ => throw new()
    };
}

enum Axis
{
    X,
    Y,
    Z
}

class FlyingObject
{
    public Point Position { get; set; }
    public Point Velocity { get; set; }

    public static FlyingObject Parse(string x)
    {
        var parts = x.Split(new[] { ',', ' ', '@' }, StringSplitOptions.RemoveEmptyEntries);
        var values = parts.Select(long.Parse).ToList();

        return new()
        {
            Position = (values[0], values[1], values[2]),
            Velocity = (values[3], values[4], values[5])
        };
    }
}
