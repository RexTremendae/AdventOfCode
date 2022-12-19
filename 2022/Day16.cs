using static System.Console;

public class Day16
{
    public class State
    {
        private string _pos;
        private string _opened;

        public State(string pos, HashSet<string> opened)
        {
            _pos = pos;
            _opened = string.Join("_", opened.OrderBy(_ => _));
        }

        public override bool Equals(object? obj)
        {
            if (obj is not State other) return false;
            return _pos == other._pos && _opened == other._opened;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_pos, _opened);
        }
    }

    public async Task Part1()
    {
        var rates = new Dictionary<string, int>();
        var graph = new Dictionary<string, string[]>();

        await foreach (var (from, to, rate) in ReadInput("Day16.txt"))
        {
            rates.Add(from, rate);
            graph.Add(from, to);
        }

        var start_pos = "AA";
        var start_total = 0;
        var start_minutes = 30;

        var q = new List<(string pos, int total, int minutes, HashSet<string> opened, HashSet<State> visited)>
        {
            (start_pos, start_total, start_minutes, new HashSet<string>(), new HashSet<State>())
        };

        WriteLine("!!!");

        var max = 0;
        while (q.Any())
        {
            var dq = q.First();
            q.RemoveAt(0);

            if (dq.minutes == 0)
            {
                max = Math.Max(dq.total, max);
                continue;
            }

            var pos = dq.pos;
            if (rates[pos] > 0 && !dq.opened.Contains(pos))
            {
                var opened = dq.opened.ToHashSet();
                opened.Add(pos);
                var visited = dq.visited.ToHashSet();
                visited.Add(new State(pos, dq.opened));
                q.Add((pos, dq.total + rates[pos]*(dq.minutes-1), dq.minutes-1, opened, visited));
            }

            foreach (var newPos in graph[pos])
            {
                var state = new State(newPos, dq.opened);
                if (dq.visited.Contains(state)) continue;
                var visited = dq.visited.ToHashSet();
                visited.Add(state);
                q.Add((newPos, dq.total, dq.minutes-1, dq.opened, visited));
            }
        }

        WriteLine(max);
    }

    private async IAsyncEnumerable<(string from, string[] to, int rate)> ReadInput(string filename)
    {
        const string fromLabelText = "Valve ";
        const string fromRateText = "Valve AA has flow rate=";
        const string toText = "tunnels lead to valve";

        await foreach(var line in System.IO.File.ReadLinesAsync(filename))
        {
            if (string.IsNullOrEmpty(line)) yield break;

            var parts = line.Split(";", StringSplitOptions.TrimEntries);
            var from = parts[0][(fromLabelText.Length)..(fromLabelText.Length+2)];
            var rate = int.Parse(parts[0][(fromRateText.Length)..]);
            var toData = parts[1][(toText.Length)..];
            if (toData[0] == 's') toData = toData[1..];
            var to = toData.Split(",", StringSplitOptions.TrimEntries);

            yield return (from, to, rate);
        }
    }
}
