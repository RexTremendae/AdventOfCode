using static System.Console;

public class Day18
{
    public async Task Part1()
    {
        var cubes = new HashSet<(int x, int y, int z)>();
        await foreach (var (x, y, z) in ReadInput("Day18.txt"))
        {
            cubes.Add((x, y, z));
        }

        var sum = 0;
        foreach (var (x, y, z) in cubes)
        {
            foreach (var (xx, yy, zz) in new[]
            {
                (x+1, y  , z  ), (x-1, y  , z  ),
                (x  , y+1, z  ), (x  , y-1, z  ),
                (x  , y  , z+1), (x  , y  , z-1)
            })
            {
                if (!cubes.Contains((xx, yy, zz))) sum++;
            }
        }

        WriteLine(sum);
    }

    public async IAsyncEnumerable<(int x, int y, int z)> ReadInput(string filename)
    {
        await foreach(var line in System.IO.File.ReadLinesAsync(filename))
        {
            if (string.IsNullOrEmpty(line)) break;

            var parts = line.Split(",");
            yield return (int.Parse(parts[0]),int.Parse(parts[1]),int.Parse(parts[2]));
        }

        yield break;
    }
}