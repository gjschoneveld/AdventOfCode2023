var input = File.ReadAllLines("input.txt");
var lines = input.Select(line => line.Split(' ').Select(int.Parse).ToList()).ToList();

var answer1 = lines.Sum(Next);
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = lines.Sum(Previous);
Console.WriteLine($"Answer 2: {answer2}");

List<List<int>> FindDifferences(List<int> line)
{
    List<List<int>> values = [line];

    while (values[^1].Any(v => v != 0))
    {
        values.Add(values[^1].Skip(1).Zip(values[^1], (right, left) => right - left).ToList());
    }

    return values;
}

int Next(List<int> line)
{
    var values = FindDifferences(line);

    return values.Sum(v => v[^1]);
}

int Previous(List<int> line)
{
    var values = FindDifferences(line);

    return values.Select(v => v[0]).Reverse().Aggregate((acc, v) => v - acc);
}
