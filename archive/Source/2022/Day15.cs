using static System.Console;

public class Day15
{
    public async Task Part1()
    {
        var y = 2_000_000;
        var xMarks = new HashSet<int>();

        var input = ReadInput("Day15.txt").ToBlockingEnumerable().ToList();
        foreach (var (sensor, beacon) in input)
        {
            var dist = Math.Abs(sensor.x - beacon.x) + Math.Abs(sensor.y - beacon.y);
            var xDist = dist - Math.Abs(sensor.y - y);
            if (xDist < 0) continue;

            foreach (var x in (sensor.x - xDist).To(sensor.x + xDist + 1))
            {
                xMarks.Add(x);
            }
            //WriteLine($"S: {sensor}   B: {beacon}");
        }

        foreach (var (sensor, beacon) in input.Where(_ => _.beacon.y == y))
        {
            xMarks.Remove(beacon.x);
        }

        WriteLine(xMarks.Count);
    }

    public async IAsyncEnumerable<((int x, int y) sensor, (int x, int y) beacon)> ReadInput(string filename)
    {
        var data = new HashSet<(int, int)>();

        await foreach(var line in System.IO.File.ReadLinesAsync(filename))
        {
            var parts = line.Split(":");
            var sensor = ParseXY(parts[0]);
            var beacon = ParseXY(parts[1]);
            yield return (sensor, beacon);
        }
    }

    private (int x, int y) ParseXY(string input)
    {
        var idx = input.IndexOf(" at ") + 4;
        var xy = input[idx..].Split(",", StringSplitOptions.TrimEntries);
        return (int.Parse(xy[0][2..]), int.Parse(xy[1][2..]));
    }
}
