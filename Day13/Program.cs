var input = File.ReadAllLines("input.txt");
var patterns = GroupLines(input).ToList();

var lines = patterns.Select(p => FindLineOfReflection(p, 0)).ToList();
var answer1 = Summarize(lines);
Console.WriteLine($"Answer 1: {answer1}");

lines = patterns.Select(p => FindLineOfReflection(p, 1)).ToList();
var answer2 = Summarize(lines);
Console.WriteLine($"Answer 2: {answer2}");

int Summarize(List<(bool vertical, int index)> lines)
{
    return lines.Sum(line => (line.vertical ? 1 : 100) * (line.index + 1));
}

(bool vertical, int index) FindLineOfReflection(string[] pattern, int errors)
{
    for (int row = 0; row < pattern.Length - 1; row++)
    {
        if (IsLineOfReflection(pattern, row, false, errors))
        {
            return (false, row);
        }
    }

    for (int column = 0; column < pattern[0].Length - 1; column++)
    {
        if (IsLineOfReflection(pattern, column, true, errors))
        {
            return (true, column);
        }
    }

    throw new();
}

bool IsLineOfReflection(string[] pattern, int index, bool vertical, int errors)
{
    var left = index;
    var right = index + 1;

    var max = GetLine(pattern, 0, !vertical).Length;

    while (left >= 0 && right < max)
    {
        errors -= Difference(GetLine(pattern, left, vertical), GetLine(pattern, right, vertical));

        left--;
        right++;
    }

    return errors == 0;
}

int Difference(string left, string right)
{
    return Enumerable.Range(0, left.Length).Count(i => left[i] != right[i]);
}

string GetLine(string[] pattern, int index, bool vertical)
{
    if (vertical)
    {
        return new string(pattern.Select(line => line[index]).ToArray());
    }

    return pattern[index];
}

IEnumerable<string[]> GroupLines(string[] input)
{
    int start = 0;

    while (true)
    {
        var end = Array.FindIndex(input, start, string.IsNullOrEmpty);

        if (end == -1)
        {
            yield return input[start..];
            yield break;
        }

        yield return input[start..end];
        start = end + 1;
    }
}
