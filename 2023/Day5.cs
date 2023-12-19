using static ColorWriter;

public class Day5
{
    public async Task Part1()
    {
        var (seeds, maps) = await ReadData("Day5.txt");

        var min = long.MaxValue;
        foreach (var seed in seeds)
        {
            var currentCategory = "seed";
            var currentValue = seed;

            for(;;)
            {
                var mapCandidates = maps.Where(_ => _.src == currentCategory).ToArray();
                if (mapCandidates.Length != 1) break;

                var map = mapCandidates[0];

                var nextValue = -1L;
                foreach (var range in map.ranges)
                {
                    if (currentValue >= range.SrcStart && currentValue < range.SrcStart + range.Length)
                    {
                        nextValue = range.DstStart + (currentValue - range.SrcStart);
                        break;
                    }
                }
                if (nextValue == -1) nextValue = currentValue;

                currentCategory = map.dst;
                currentValue = nextValue;
            }

            min = long.Min(min, currentValue);
        }

        WriteLine(min);
    }

    public async Task<(long[] seeds, (string src, string dst, SeedRange[] ranges)[] maps)> ReadData(string filename)
    {
        var seeds = Array.Empty<long>();
        var opts = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
        var currentMap = (src: string.Empty, dst: string.Empty);

        var maps = new List<(string, string, SeedRange[])>();
        var ranges = new List<SeedRange>();

        foreach (var line in await File.ReadAllLinesAsync(filename))
        {
            if (!seeds.Any())
            {
                if (!line.StartsWith("seeds:")) throw new InvalidDataException();
                seeds = line[7..].Split(' ', opts).Select(long.Parse).ToArray();
                continue;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                if (!string.IsNullOrWhiteSpace(currentMap.src))
                {
                    maps.Add((currentMap.src, currentMap.dst, ranges.ToArray()));
                    currentMap = (string.Empty, string.Empty);
                }

                continue;
            }

            if (string.IsNullOrEmpty(currentMap.src) || string.IsNullOrEmpty(currentMap.dst))
            {
                ranges.Clear();
                var mapIndex = line.FindLast(" map:");
                var mapParts = line[..mapIndex].Split('-', opts);
                currentMap = (mapParts[0], mapParts[2]);
            }
            else
            {
                var data = line.Split(' ', opts);
                var range = new SeedRange(long.Parse(data[0]), long.Parse(data[1]), long.Parse(data[2]));
                ranges.Add(range);
            }
        }

        maps.Add((currentMap.src, currentMap.dst, ranges.ToArray()));

        return (seeds.ToArray(), maps.ToArray());
    }

    public readonly record struct SeedRange
    (
        long DstStart,
        long SrcStart,
        long Length
    );
}
