using Point = (int x, int y);
using Schematic = System.Collections.Generic.Dictionary<(int x, int y), Item>;

var input = File.ReadAllLines("input.txt");
var schematic = Parse(input);

FindAdjacentPartNumbers(schematic);

var answer1 = schematic.Values.OfType<Symbol>().SelectMany(s => s.AdjacentPartNumbers).Sum(n => n.Value);
Console.WriteLine($"Answer 1: {answer1}");

var gears = schematic.Values.OfType<Symbol>().Where(s => s.Value == '*' && s.AdjacentPartNumbers.Count == 2).ToList();

var answer2 = gears.Sum(g => g.AdjacentPartNumbers.Select(n => n.Value).Aggregate((a, b) => a * b));
Console.WriteLine($"Answer 2: {answer2}");

List<Point> Neighbours(Point position)
{
    return
    [
        (position.x - 1, position.y - 1),
        (position.x, position.y - 1),
        (position.x + 1, position.y - 1),
        (position.x - 1, position.y),
        (position.x + 1, position.y),
        (position.x - 1, position.y + 1),
        (position.x, position.y + 1),
        (position.x + 1, position.y + 1)
    ];
}

void FindAdjacentPartNumbers(Schematic schematic)
{
    var symbols = schematic.Values.OfType<Symbol>().ToList();

    foreach (var symbol in symbols)
    {
        symbol.AdjacentPartNumbers = Neighbours(symbol.Position)
            .Where(schematic.ContainsKey)
            .Select(nb => schematic[nb])
            .OfType<PartNumber>()
            .ToHashSet();
    }
}

Schematic Parse(string[] input)
{
    var result = new Schematic();

    for (int y = 0; y < input.Length; y++)
    {
        for (int x = 0; x < input[y].Length; x++)
        {
            if (input[y][x] == '.')
            {
                continue;
            }

            var position = (x, y);

            if (!char.IsDigit(input[y][x]))
            {
                result[position] = new Symbol
                {
                    Position = position,
                    Value = input[y][x]
                };

                continue;
            }

            var number = new PartNumber
            {
                Position = position,
                Value = input[y][x] - '0',
            };

            result[position] = number;

            while (x + 1 < input[y].Length && char.IsDigit(input[y][x + 1]))
            {
                x++;
                number.Value = number.Value * 10 + input[y][x] - '0';
                result[(x, y)] = number;
            }
        }
    }

    return result;
}

abstract class Item
{
    public Point Position { get; set; }
}

class Symbol : Item
{
    public char Value { get; set; }
    public HashSet<PartNumber> AdjacentPartNumbers { get; set; } = []; 
}

class PartNumber : Item
{
    public int Value { get; set; }
}
