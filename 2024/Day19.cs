using static System.Console;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class Day19
{
    public async Task Part1()
    {
        var (shards, patterns) = await ReadIndata("Day19.txt");

        var count = 0;
        foreach (var ptrn in patterns)
        {
            if (Validate(ptrn, shards)) count++;
        }

        WriteLine(count);
    }

    private bool Validate(string ptrn, Dictionary<int, string[]> shards)
    {
        return ValidateInner(ptrn, shards);
    }

    private bool ValidateInner(string ptrn, Dictionary<int, string[]> shards)
    {
        foreach (var len in shards.Keys)
        {
            if (len > ptrn.Length) continue;

            foreach (var shrd in shards[len])
            {
                if (!ptrn.StartsWith(shrd)) continue;

                if (ptrn.Length == shrd.Length) return true;

                if (ValidateInner(ptrn[len..], shards))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public async Task<(Dictionary<int, string[]> shards, string[] patterns)> ReadIndata(string filename)
    {
        var lines = await File.ReadAllLinesAsync(filename);
        var shards = new Dictionary<int, List<string>>();

        var rawShards = lines[0].Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);

        foreach (var shrd in rawShards)
        {
            if (!shards.TryGetValue(shrd.Length, out var shardList))
            {
                shardList = [];
                shards.Add(shrd.Length, shardList);
            }

            shardList.Add(shrd);
        }

        foreach (var shardList in shards.Values)
        {
            shardList.Sort();
        }

        return (
            shards.ToDictionary(_ => _.Key, _ => _.Value.ToArray()),
            lines[2..]);
    }
}
