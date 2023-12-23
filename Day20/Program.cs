using System.Data;
using System.Numerics;
using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");
var modules = input.Select(Module.Parse).ToDictionary(m => m.Name);

foreach (var (name, module) in modules)
{
    module.Initialize(modules);
}

var history = new Dictionary<bool, long>
{
    [false] = 0,
    [true] = 0,
};

var buttonPresses = 1000;

for (int i = 0; i < buttonPresses; i++)
{
    Simulate(modules, history, new());
}

var answer1 = history[false] * history[true];
Console.WriteLine($"Answer 1: {answer1}");

// rx is low when all these are low
// these are low periodic, so rx will be low
// if we take the least common multiple
var targetModule = (Conjunction)modules.First(kv => kv.Value.Destinations.Contains("rx")).Value;
var targets = targetModule.Inputs.Select(kv => kv.Key).ToList();

var values = new Dictionary<string, int>();

while (true)
{
    buttonPresses++;
    var seen = Simulate(modules, history, targets);

    foreach (var target in seen)
    {
        if (!values.ContainsKey(target))
        {
            values[target] = buttonPresses;
        }
    }

    if (values.Count == targets.Count)
    {
        // we have found all targets
        break;
    }
}

var answer2 = values.Values.Aggregate((BigInteger)1, (acc, value) => LeastCommonMultiple(acc, value));
Console.WriteLine($"Answer 2: {answer2}");

BigInteger LeastCommonMultiple(BigInteger a, BigInteger b)
{
    return a / BigInteger.GreatestCommonDivisor(a, b) * b;
}

HashSet<string> Simulate(Dictionary<string, Module> modules, Dictionary<bool, long> history, List<string> targets)
{
    var pulses = new Queue<Pulse>();

    pulses.Enqueue(new()
    {
        Source = "button",
        Destination = "broadcaster",
        Value = false
    });

    var seen = new HashSet<string>();

    while (pulses.Count > 0)
    {
        var pulse = pulses.Dequeue();
        history[pulse.Value]++;

        if (targets.Contains(pulse.Destination) && !pulse.Value)
        {
            seen.Add(pulse.Destination);
        }

        if (!modules.ContainsKey(pulse.Destination))
        {
            continue;
        }

        var nextPulses = modules[pulse.Destination].Process(pulse);

        foreach (var next in nextPulses)
        {
            pulses.Enqueue(next);
        }
    }

    return seen;
}

class Pulse
{
    public required string Source { get; set; }
    public required string Destination { get; set; }
    public bool Value { get; set; }
}

abstract class Module
{
    public string Name { get; set; } = "";
    public List<string> Destinations { get; set; } = [];

    public virtual void Initialize(Dictionary<string, Module> modules)
    {
    }

    public abstract List<Pulse> Process(Pulse pulse);

    protected List<Pulse> Send(bool value)
    {
        return Destinations.Select(dest => new Pulse
        {
            Source = Name,
            Destination = dest,
            Value = value
        }).ToList();
    }

    public static Module Parse(string line)
    {
        var match = Regex.Match(line, @"^(?<type>[%&])?(?<name>\w+) -> ((?<dest>\w+)(, )?)+$");

        var type = match.Groups["type"].Value;
        var name = match.Groups["name"].Value;
        var destinations = match.Groups["dest"].Captures.Select(c => c.Value).ToList();

        Module module = type switch
        {
            "%" => new FlipFlop(),
            "&" => new Conjunction(),
            _ => new Broadcast()
        };

        module.Name = name;
        module.Destinations = destinations;

        return module;
    }
}

class Broadcast : Module
{
    public override List<Pulse> Process(Pulse pulse)
    {
        return Send(pulse.Value);
    }
}

class FlipFlop : Module
{
    private bool state;

    public override List<Pulse> Process(Pulse pulse)
    {
        if (pulse.Value)
        {
            return [];
        }

        state = !state;

        return Send(state);
    }
}

class Conjunction : Module
{
    public Dictionary<string, bool> Inputs { get; set; } = [];

    public override void Initialize(Dictionary<string, Module> modules)
    {
        foreach (var (name, module) in modules)
        {
            if (module.Destinations.Contains(Name))
            {
                Inputs.Add(name, false);
            }
        }
    }

    public override List<Pulse> Process(Pulse pulse)
    {
        Inputs[pulse.Source] = pulse.Value;

        var result = !Inputs.Values.All(v => v);

        return Send(result);
    }
}
