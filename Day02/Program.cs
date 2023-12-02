using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");
var games = input.Select(Game.Parse).ToList();

var answer1 = games.Where(g => g.Rounds.All(r => r.Red <= 12 && r.Green <= 13 && r.Blue <= 14)).Sum(g => g.Id);
Console.WriteLine($"Answer 1: {answer1}");

var answer2 = games.Sum(g => g.MinimalSet.Power);
Console.WriteLine($"Answer 2: {answer2}");

class Game
{
    public int Id { get; set; }
    public List<Set> Rounds { get; set; } = [];

    public Set MinimalSet
    {
        get => new()
        {
            Red = Rounds.Max(r => r.Red),
            Green = Rounds.Max(r => r.Green),
            Blue = Rounds.Max(r => r.Blue)
        };
    }

    public static Game Parse(string x)
    {
        var match = Regex.Match(x, @"Game (?<id>\d+):((?<round>[^;]+);?)+");

        var id = match.Groups["id"].Value;
        var rounds = match.Groups["round"].Captures.Select(c => c.Value).ToList();

        return new()
        {
            Id = int.Parse(id),
            Rounds = rounds.Select(Set.Parse).ToList(),
        };
    }
}

class Set
{
    public int Red { get; set; }
    public int Green { get; set; }
    public int Blue { get; set; }

    public int Power => Red * Green * Blue;

    public static Set Parse(string x)
    {
        int red = 0;
        int green = 0;
        int blue = 0;

        var parts = x.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < parts.Length; i += 2)
        {
            var count = int.Parse(parts[i]);
            var type = parts[i + 1];

            switch (type)
            {
                case "red":
                    red = count;
                    break;
                case "green":
                    green = count;
                    break;
                case "blue":
                    blue = count;
                    break;
            }
        }

        return new()
        {
            Red = red,
            Green = green,
            Blue = blue
        };
    }
}
