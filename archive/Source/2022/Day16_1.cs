using static System.Console;

public class Day16
{
    public async Task Part1()
    {
        var nodes = await ReadData("Day16.txt");
        var distances = CalculateDistances(nodes);

        var q = new List<(string node, long totalDist, long totalFlow, HashSet<string> nonVisited, string[] path)>
        {
            ("AA", 0, 0, nodes.Values.Where(_ => _.FlowRate > 0).Select(_ => _.Id).ToHashSet(), new[] { "AA" })
        };

        int maxSteps = 30;
        long maxFlow = 0;
        var path = Array.Empty<string>();

        while (q.Any())
        {
            var dq = q.First();
            q.RemoveAt(0);
            foreach (var next in dq.nonVisited)
            {
                var dst = dq.totalDist + distances[(dq.node, next)] + 1;

                if (dst > maxSteps)
                {
                    continue;
                }

                var flw = dq.totalFlow + (nodes[next].FlowRate * (maxSteps-dst));

                if (flw > maxFlow)
                {
                    maxFlow = flw;
                    path = dq.path.Append(next).ToArray();
                }

                var nextNonVisited = dq.nonVisited.ToHashSet();
                nextNonVisited.Remove(next);
                q.Add((next, dst, flw, nextNonVisited, dq.path.Append(next).ToArray()));
            }
        }

        WriteLine(maxFlow);

        var first = true;
        foreach (var step in path)
        {
            if (!first)
            {
                Write(" => ");
            }

            Write("[");
            WriteColorized(step, ConsoleColor.Green);
            Write("]");

            first = false;
        }
        WriteLine();
    }

    private Dictionary<(string from, string to), int> CalculateDistances(Dictionary<string, Node> nodes)
    {
        var distances = new Dictionary<(string, string), int>();

        foreach (var fromNode in nodes)
        {
            var visited = new HashSet<string>(new[] { fromNode.Key });
            var q = new List<(string, int)>(new[] { (fromNode.Key, 0) });

            while (q.Any())
            {
                var (id, len) = q.First();
                q.RemoveAt(0);

                foreach (var next in nodes[id].Connections.Where(_ => !visited.Contains(_.Id)))
                {
                    visited.Add(next.Id);
                    distances.Add((fromNode.Key, next.Id), len+1);
                    q.Add((next.Id, len+1));
                }
            }
        }

        return distances;
    }

    public async Task<Dictionary<string, Node>> ReadData(string filePath)
    {
        var nodes = new Dictionary<string, Node>();
        var connections = new Dictionary<string, string[]>();

        foreach (var line in await File.ReadAllLinesAsync(filePath))
        {
            var parts = line.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var flowRate = int.Parse(parts[0].Split('=')[1]);
            var id = parts[0].Split(' ')[1];
            var connectionString = "tunnel leads to valve ";
            var connectionIds = parts[1][connectionString.Length..].Split(',', StringSplitOptions.TrimEntries);

            nodes.Add(id, new (id, flowRate));
            connections.Add(id, connectionIds);
        }

        foreach (var (id, connectedIds) in connections)
        {
            nodes[id].Connections.AddRange(connectedIds.Select(_ => nodes[_]));
        }

        return nodes;
    }

    private static void WriteColorized(object data, ConsoleColor color)
    {
        ForegroundColor = color;
        Write(data);
        ResetColor();
    }

    private static void WriteLineColorized(object data, ConsoleColor color)
    {
        ForegroundColor = color;
        WriteLine(data);
        ResetColor();
    }
}

public class Node
{
    public Node(string id, int flowRate)
    {
        Id = id;
        FlowRate = flowRate;
    }

    public string Id { get; }
    public int FlowRate { get; }
    public List<Node> Connections { get; } = new();
}
