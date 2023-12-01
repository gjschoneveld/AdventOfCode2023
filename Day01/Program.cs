var input = File.ReadAllLines("input.txt");

var answer1 = input.Sum(CalibrationValue1);
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = input.Sum(CalibrationValue2);
Console.WriteLine($"Answer 2: {answer2}");

int CalibrationValue1(string line)
{
    var first = line.First(char.IsDigit);
    var last = line.Last(char.IsDigit);

    return int.Parse($"{first}{last}");
}

int CalibrationValue2(string line)
{
    var map = new Dictionary<string, int>
    {
        ["one"] = 1,
        ["two"] = 2,
        ["three"] = 3,
        ["four"] = 4,
        ["five"] = 5,
        ["six"] = 6,
        ["seven"] = 7,
        ["eight"] = 8,
        ["nine"] = 9,
        ["1"] = 1,
        ["2"] = 2,
        ["3"] = 3,
        ["4"] = 4,
        ["5"] = 5,
        ["6"] = 6,
        ["7"] = 7,
        ["8"] = 8,
        ["9"] = 9,
    };

    var candidates = map.Keys.Where(line.Contains).ToList();

    var first = map[candidates.MinBy(line.IndexOf)!];
    var last = map[candidates.MaxBy(line.LastIndexOf)!];

    return int.Parse($"{first}{last}");
}
