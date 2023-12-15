using System.Text.RegularExpressions;

var input = File.ReadAllText("input.txt");

var parts = input.Split(',');
var hashes = parts.Select(Hash).ToList();

var answer1 = hashes.Sum();
Console.WriteLine($"Answer 1: {answer1}");

var lenses = parts.Select(Lens.Parse).ToList();
var hashMap = Enumerable.Range(0, 256).Select(i => new List<Lens>()).ToList();

foreach (var lens in lenses)
{
    switch (lens.Operation)
    {
        case Operation.Set:
            Set(hashMap, lens);
            break;
        case Operation.Remove:
            Remove(hashMap, lens);
            break;
    }
}

var answer2 = FocusingPower(hashMap);
Console.WriteLine($"Answer 2: {answer2}");

int FocusingPower(List<List<Lens>> hashMap)
{
    return hashMap.SelectMany((box, boxIndex) => box.Select((lens, lensIndex) => (boxIndex + 1) * (lensIndex + 1) * lens.FocalLength)).Sum();
}

void Set(List<List<Lens>> hashMap, Lens lens)
{
    var bucket = Hash(lens.Label);
    var index = hashMap[bucket].FindIndex(l => l.Label == lens.Label);

    if (index < 0)
    {
        hashMap[bucket].Add(lens);

        return;
    }

    hashMap[bucket][index] = lens;
}

void Remove(List<List<Lens>> hashMap, Lens lens)
{
    var bucket = Hash(lens.Label);
    var index = hashMap[bucket].FindIndex(l => l.Label == lens.Label);

    if (index < 0)
    {
        return;
    }

    hashMap[bucket].RemoveAt(index);
}

int Hash(string text)
{
    int result = 0;

    foreach (var c in text)
    {
        result += c;
        result *= 17;
        result %= 256;
    }

    return result;
}

enum Operation
{
    Set,
    Remove
}

class Lens
{
    public required string Label { get; set; }
    public required Operation Operation { get; set; }
    public required int FocalLength { get; set; }

    public static Lens Parse(string x)
    {
        var match = Regex.Match(x, @"(?<label>\w+)(?<operation>[-=])(?<focalLength>\d*)");

        var label = match.Groups["label"].Value;
        var operation = match.Groups["operation"].Value;
        var focalLength = match.Groups["focalLength"].Value;

        return new()
        {
            Label = label,
            Operation = operation == "=" ? Operation.Set : Operation.Remove,
            FocalLength = focalLength.Length > 0 ? int.Parse(focalLength) : 0,
        };
    }
}
