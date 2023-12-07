var input = File.ReadAllLines("input.txt");
var hands = input.Select(Hand.Parse).ToList();

hands.ForEach(h => h.SetType());
var sorted = hands.OrderBy(h => h, new HandComparer()).ToList();

var answer1 = TotalWinnings(sorted);
Console.WriteLine($"Answer 1: {answer1}");


hands.ForEach(h => h.SetTypeUsingJokers());
sorted = hands.OrderBy(h => h, new HandComparer(true)).ToList();

var answer2 = TotalWinnings(sorted);
Console.WriteLine($"Answer 2: {answer2}");

int TotalWinnings(List<Hand> hands)
{
    return hands.Select((h, i) => (hand: h, rank: i + 1))
        .Aggregate(0, (acc, item) => acc += item.rank * item.hand.Bid);
}

enum Type
{
    FiveOfAKind,
    FourOfAKind,
    FullHouse,
    ThreeOfAKind,
    TwoPair,
    OnePair,
    HighCard
}

class HandComparer(bool joker = false) : IComparer<Hand>
{
    public int CardValue(char card)
    {
        return card switch
        {
            'A' => 14,
            'K' => 13,
            'Q' => 12,
            'J' when joker => 1,
            'J' => 11,
            'T' => 10,
            _ => card - '0'
        };
    }

    public int Compare(Hand? x, Hand? y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);

        if (x.Type != y.Type)
        {
            return y.Type - x.Type;
        }

        for (int i = 0; i < x.Cards.Length; i++)
        {
            if (x.Cards[i] != y.Cards[i])
            {
                return CardValue(x.Cards[i]) - CardValue(y.Cards[i]);
            }
        }

        return 0;
    }
}

class Hand
{
    public required string Cards { get; set; }
    public required int Bid { get; set; }

    public Type Type { get; set; }

    public void SetType()
    {
        var counts = Cards.GroupBy(c => c)
            .Select(g => g.Count())
            .OrderByDescending(c => c)
            .ToList();

        Type = FindType(counts);
    }

    public void SetTypeUsingJokers()
    {
        var jokers = Cards.Count(c => c == 'J');

        var counts = Cards.Where(c => c != 'J')
            .GroupBy(c => c)
            .Select(g => g.Count())
            .OrderByDescending(c => c)
            .ToList();

        if (counts.Count == 0)
        {
            // happens when we have jokers only
            counts.Add(0);
        }

        // it is always best to add jokers to the highest count
        counts[0] += jokers;

        Type = FindType(counts);
    }

    public Type FindType(List<int> counts)
    {
        return counts switch
        {
            [5] => Type.FiveOfAKind,
            [4, ..] => Type.FourOfAKind,
            [3, 2] => Type.FullHouse,
            [3, ..] => Type.ThreeOfAKind,
            [2, 2, ..] => Type.TwoPair,
            [2, ..] => Type.OnePair,
            _ => Type.HighCard,
        };
    }

    public static Hand Parse(string x)
    {
        var parts = x.Split(' ');

        var cards = parts[0];
        var bid = int.Parse(parts[1]);

        return new()
        {
            Cards = cards,
            Bid = bid
        };
    }
}
