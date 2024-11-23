using Edge = (string Start, string End);

var input = File.ReadAllLines("input.txt");
var rules = input.Select(Parse).ToList();

var graph = new Dictionary<string, List<string>>();

foreach (var (source, destinations) in rules)
{
    foreach (var destination in destinations)
    {
        AddEdge(graph, (source, destination));
    }
}    

var edges = graph
    .SelectMany(kv => kv.Value.Select(end => (Start: kv.Key, End: end)))
    .Where(e => e.Start.CompareTo(e.End) < 0)
    .ToList();

var steps = edges.Select(e => FindAlternativeRoute(graph, e)).OrderByDescending(s => s.Steps).ToList();

foreach (var step in steps.Take(3))
{
    RemoveEdge(graph, step.Edge);
}

var sizeA = GetSize(graph, graph.Keys.First());
var sizeB = graph.Count - sizeA;

var answer = sizeA * sizeB;
Console.WriteLine($"Answer: {answer}");

(string Source, List<string> Destinations) Parse(string line)
{
    var parts = line.Split([' ', ':'], StringSplitOptions.RemoveEmptyEntries);

    return (parts[0], parts[1..].ToList());
}

void AddEdge(Dictionary<string, List<string>> graph, Edge edge)
{
    if (!graph.ContainsKey(edge.Start))
    {
        graph[edge.Start] = [];
    }

    if (!graph.ContainsKey(edge.End))
    {
        graph[edge.End] = [];
    }

    graph[edge.Start].Add(edge.End);
    graph[edge.End].Add(edge.Start);
}

void RemoveEdge(Dictionary<string, List<string>> graph, Edge edge)
{
    graph[edge.Start].Remove(edge.End);
    graph[edge.End].Remove(edge.Start);
}

(Edge Edge, int Steps) FindAlternativeRoute(Dictionary<string, List<string>> graph, Edge edge)
{
    var steps = 0;

    HashSet<string> toVisit = [edge.Start];
    HashSet<string> visited = [];

    while (!toVisit.Contains(edge.End))
    {
        visited.UnionWith(toVisit);
        toVisit = toVisit.SelectMany(v => graph[v]).Distinct().Where(v => !visited.Contains(v)).ToHashSet();
        steps++;

        if (steps == 1)
        {
            toVisit.Remove(edge.End);
        }
    }

    return (edge, steps);
}

int GetSize(Dictionary<string, List<string>> graph, string start)
{
    HashSet<string> toVisit = [start];
    HashSet<string> visited = [];

    while(toVisit.Count > 0)
    {
        visited.UnionWith(toVisit);
        toVisit = toVisit.SelectMany(v => graph[v]).Distinct().Where(v => !visited.Contains(v)).ToHashSet();
    }

    return visited.Count;
}
