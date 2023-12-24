using static ColorWriter;

public class Day24
{
    public async Task Part1()
    {
        var data = await ReadData("Day24.txt").ToArrayAsync();

        var testArea = (min: 2e14, max: 4e14);

        var sum = 0L;

        foreach (var a in 0.To(data.Length))
        foreach (var b in (a+1).To(data.Length))
        {
            var A = data[a];
            var B = data[b];

            var kA = ((double)A.vel.y) / A.vel.x;
            var kB = ((double)B.vel.y) / B.vel.x;

            var mA = A.pos.y - kA*A.pos.x;
            var mB = B.pos.y - kB*B.pos.x;

            var x = (mB - mA) / (kA - kB);
            var y = kA*x + mA;
            var tA = (y - A.pos.y) / A.vel.y;
            var tB = (y - B.pos.y) / B.vel.y;

            if (tA >= 0 && tB >= 0 && x >= testArea.min && x <= testArea.max && y >= testArea.min && y <= testArea.max)
            {
                sum++;
            }
        }

        WriteLine(sum);
    }

    public async IAsyncEnumerable<((long x, long y, long z) pos, (long x, long y, long z) vel)> ReadData(string filepath)
    {
        var splitOpts = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;

        foreach (var line in await File.ReadAllLinesAsync(filepath))
        {
            var parts = line.Split("@", splitOpts);
            var pos = parts[0].Split(",", splitOpts).Select(long.Parse).ToArray();
            var vel = parts[1].Split(",", splitOpts).Select(long.Parse).ToArray();

            yield return ((pos[0], pos[1], pos[2]), (vel[0], vel[1], vel[2]));
        }
    }
}
