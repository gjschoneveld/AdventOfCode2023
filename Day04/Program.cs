using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");
var cards = input.Select(Card.Parse).ToList();

var answer1 = cards.Select(c => c.MatchingNumbers.Count).Sum(Points);
Console.WriteLine($"Answer 1: {answer1}");


for (int index = 0; index < cards.Count; index++)
{
    var profit = cards[index].MatchingNumbers.Count;

    foreach (var card in cards.GetRange(index + 1, profit))
    {
        card.CopiesOwned += cards[index].CopiesOwned;
    }
}

var answer2 = cards.Sum(c => c.CopiesOwned);
Console.WriteLine($"Answer 2: {answer2}");

int Points(int count)
{
    if (count == 0)
    {
        return 0;
    }

    return 1 << (count - 1);
}

class Card
{
    public int Id { get; set; }

    public List<int> WinningNumbers { get; set; } = [];
    public List<int> PossessedNumbers { get; set; } = [];

    public List<int> MatchingNumbers => WinningNumbers.Intersect(PossessedNumbers).ToList();

    public int CopiesOwned { get; set; } = 1;

    public static Card Parse(string x)
    {
        var match = Regex.Match(x, @"^Card\s*(?<id>\d+):((\s*)(?<winning>\d+))+\s\|((\s*)(?<possessed>\d+))+$");

        var id = match.Groups["id"].Value;
        var winning = match.Groups["winning"].Captures.Select(c => c.Value).ToList();
        var possessed = match.Groups["possessed"].Captures.Select(c => c.Value).ToList();

        return new()
        {
            Id = int.Parse(id),
            WinningNumbers = winning.Select(int.Parse).ToList(),
            PossessedNumbers = possessed.Select(int.Parse).ToList(),
        };
    }
}
