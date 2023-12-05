using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");
var seeds = ParseSeeds(input[0]);
var maps = GroupLines([.. input[2..]]).Select(Map.Parse).ToList();

var values = seeds.ToList();

foreach (var map in maps)
{
    values = values.Select(map.MapValue).ToList();
}

var answer1 = values.Min();
Console.WriteLine($"Answer 1: {answer1}");


var ranges = seeds.Chunk(2).Select(pair => new Range { Start = pair[0], Length = pair[1] }).ToList();

foreach (var map in maps)
{
    ranges = ranges.SelectMany(map.MapRange).ToList();
}

var answer2 = ranges.Min(r => r.Start);
Console.WriteLine($"Answer 2: {answer2}");

IEnumerable<List<string>> GroupLines(List<string> input)
{
    int start = 0;

    while (true)
    {
        var end = input.FindIndex(start, string.IsNullOrEmpty);

        if (end == -1)
        {
            yield return input[start..];
            yield break;
        }

        yield return input[start..end];
        start = end + 1;
    }
}

List<long> ParseSeeds(string line)
{
    return line.Split(' ').Skip(1).Select(long.Parse).ToList();
}

class Range
{
    public long Start { get; set; }
    public long Length { get; set; }

    public long End => Start + Length;
}

class Map
{
    public required string Source { get; set; }
    public required string Destination { get; set; }
    public required List<Line> Lines { get; set; }

    public long MapValue(long value)
    {
        var line = Lines.FirstOrDefault(l => l.SourceStart <= value && value < l.SourceEnd);

        if (line == null)
        {
            return value;
        }

        return value - line.SourceStart + line.DestinationStart;
    }

    public Line? NextLine(long value)
    {
        return Lines.Where(l => value < l.SourceEnd).MinBy(l => l.SourceStart);
    }

    public IEnumerable<Range> MapRange(Range range)
    {
        var start = range.Start;

        while (start < range.End)
        {
            var line = NextLine(start);

            if (line == null)
            {
                // no line found; return the remainder
                yield return new Range
                {
                    Start = start,
                    Length = range.End - start
                };

                yield break;
            }

            if (start < line.SourceStart)
            {
                // found some stuff before the next line
                yield return new Range
                {
                    Start = start,
                    Length = Math.Min(range.End, line.SourceStart) - start
                };
            }

            if (line.SourceStart < range.End)
            {
                // return mapped stuff
                var mappedStart = Math.Max(start, line.SourceStart);
                var mappedEnd = Math.Min(range.End, line.SourceEnd);

                yield return new Range
                {
                    Start = mappedStart - line.SourceStart + line.DestinationStart,
                    Length = mappedEnd - mappedStart
                };
            }

            start = line.SourceEnd;
        }
    }

    public static Map Parse(List<string> lines)
    {
        var match = Regex.Match(lines[0], @"(?<source>\w+)-to-(?<destination>\w+)");

        return new()
        {
            Source = match.Groups["source"].Value,
            Destination = match.Groups["destination"].Value,
            Lines = lines[1..].Select(Line.Parse).ToList()
        };
    }
}

class Line
{
    public long SourceStart { get; set; }
    public long DestinationStart { get; set; }
    public long Length { get; set; }

    public long SourceEnd => SourceStart + Length;

    public static Line Parse(string line)
    {
        var numbers = line.Split(' ').Select(long.Parse).ToList();

        return new()
        { 
            DestinationStart = numbers[0],
            SourceStart = numbers[1],
            Length = numbers[2]
        };
    }
}
