var input = File.ReadAllLines("input.txt");
var lines = input.Select(Line.Parse).ToList();

var answer1 = lines.Sum(l => l.Ways());
Console.WriteLine($"Answer 1: {answer1}");

var unfolded = lines.Select(l => l.Unfold()).ToList();
var answer2 = unfolded.Sum(l => l.Ways());
Console.WriteLine($"Answer 2: {answer2}");

class Line
{
    public required string Record { get; set; }
    public required List<int> Groups { get; set; }

    public Line Unfold()
    {
        return new()
        {
            Record = $"{Record}?{Record}?{Record}?{Record}?{Record}",
            Groups = [.. Groups, .. Groups, .. Groups, .. Groups, .. Groups]
        };
    }

    private bool IsFilledAllowed(int recordIndex, int count)
    {
        return Record.Skip(recordIndex).Take(count).All(c => c == '#' || c == '?');
    }

    private bool IsEmptyAllowed(int recordIndex, int count)
    {
        return Record.Skip(recordIndex).Take(count).All(c => c == '.' || c == '?');
    }

    private readonly Dictionary<(int record, int group), long> cache = [];

    private long Ways(int recordIndex, int groupIndex)
    {
        if (cache.ContainsKey((recordIndex, groupIndex)))
        {
            return cache[(recordIndex, groupIndex)];
        }

        var group = Groups[groupIndex];

        if (!IsFilledAllowed(recordIndex, group))
        {
            return cache[(recordIndex, groupIndex)] = 0;
        }

        if (groupIndex == Groups.Count - 1)
        {
            if (!IsEmptyAllowed(recordIndex + group, Record.Length - recordIndex - group))
            {
                return cache[(recordIndex, groupIndex)] = 0;
            }

            return cache[(recordIndex, groupIndex)] = 1;
        }

        long ways = 0;

        var remainderGroups = Groups[(groupIndex + 1)..];
        var remainder = remainderGroups.Sum() + remainderGroups.Count - 1;

        var maxSpace = Record.Length - recordIndex - group - remainder;
        var space = 1;

        while (space <= maxSpace)
        {
            if (!IsEmptyAllowed(recordIndex + group, space))
            {
                return cache[(recordIndex, groupIndex)] = ways;
            }

            ways += Ways(recordIndex + group + space, groupIndex + 1);
            space++;
        }

        return cache[(recordIndex, groupIndex)] = ways;
    }

    public long Ways()
    {
        long ways = 0;

        var remainder = Groups.Sum() + Groups.Count - 1;

        var maxSpace = Record.Length - remainder;
        var space = 0;

        while (space <= maxSpace)
        {
            if (!IsEmptyAllowed(0, space))
            {
                return ways;
            }

            ways += Ways(space, 0);
            space++;
        }

        return ways;
    }

    public static Line Parse(string x)
    {
        var parts = x.Split(' ', ',');

        return new()
        {
            Record = parts[0],
            Groups = parts[1..].Select(int.Parse).ToList()
        };
    }
}
