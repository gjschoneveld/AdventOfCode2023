var input = File.ReadAllLines("input.txt");

var races = Parse1(input);
var waysToWin = races.Select(r => r.WaysToWin()).ToList();
var answer1 = waysToWin.Aggregate((a, b) => a * b);
Console.WriteLine($"Answer 1: {answer1}");

var race = Parse2(input);
var answer2 = race.WaysToWin();
Console.WriteLine($"Answer 2: {answer2}");

List<long> GetNumbers(string line)
{
    return line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Skip(1)
        .Select(long.Parse)
        .ToList();
}

List<Race> Parse1(string[] input)
{
    var times = GetNumbers(input[0]);
    var distances = GetNumbers(input[1]);

    return times.Zip(distances, (t, d) => new Race { Time = t, Distance = d }).ToList();
}

long GetNumber(string line)
{
    return long.Parse(new string(line.Where(char.IsDigit).ToArray()));
}

Race Parse2(string[] input)
{
    var time = GetNumber(input[0]);
    var distance = GetNumber(input[1]);

    return new()
    {
        Time = time,
        Distance = distance
    };
}

class Race
{
    public long Time { get; set; }
    public long Distance { get; set; }

    public long WaysToWin()
    {
        var root = Math.Sqrt(Time * Time - 4 * Distance);

        var lowerBound = (long)Math.Floor((Time - root) / 2) + 1;
        var upperBound = (long)Math.Ceiling((Time + root) / 2) - 1;

        return upperBound - lowerBound + 1;
    }
}
