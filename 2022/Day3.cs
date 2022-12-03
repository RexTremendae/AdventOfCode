using static System.Console;

public class Day3
{
    const string alphabet = "abcdefghijklmnopqrstuvwxyz";
    static readonly string prio = $" {alphabet}{alphabet.ToUpper()}";

    public async Task Part1()
    {
        var sum = 0L;
        await foreach(var line in File.ReadLinesAsync("Day3.txt"))
        {
            if (string.IsNullOrEmpty(line)) continue;

            var l2 = line.Length/2;
            var first = line[..l2].ToCharArray().ToHashSet();
            foreach (var chr in line[l2..])
            {
                if (first.Contains(chr))
                {
                    var p = 0;
                    while (prio[p] != chr) p++;
                    sum += p;
                    break;
                }
            }
        }

        WriteLine(sum);
    }

    public async Task Part2()
    {
        var sum = 0L;
        int lc = 0;

        HashSet<char>[] hash = new HashSet<char>[3];

        await foreach(var line in File.ReadLinesAsync("Day3.txt"))
        {
            if (string.IsNullOrEmpty(line)) break;

            var idx = lc%3;
            hash[idx] = line.ToHashSet();
            lc ++;

            if (idx < 2) continue;

            var toRemove = new List<char>();
            foreach (var chr in hash[0])
            {
                if (!hash[1].Contains(chr)) toRemove.Add(chr);
                if (!hash[2].Contains(chr)) toRemove.Add(chr);
            }

            foreach (var chr in toRemove)
            {
                hash[0].Remove(chr);
            }

            var ch = hash[0].Single();

            var p = 0;
            while (prio[p] != ch) p++;

            sum += p;
        }

        WriteLine(sum);
    }
}