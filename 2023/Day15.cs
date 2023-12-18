using static ColorWriter;

public class Day15
{
    public async Task Part1()
    {
        var sum = 0L;
        foreach (var data in await ReadData("Day15.txt"))
        {
            sum += Hash(data);
        }

        WriteLine(sum);
    }

    private int Hash(string data)
    {
        var hash = 0;
        foreach (var ch in data)
        {
            hash += ch;
            hash *= 17;
            hash %= 256;
        }

        return hash;
    }

    private async Task<string[]> ReadData(string filepath)
    {
        return (await File.ReadAllTextAsync(filepath)).Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}