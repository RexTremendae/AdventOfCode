using System.Data;
using System.Diagnostics.Metrics;
using static ColorWriter;

public class Day20
{
    public async Task Part1()
    {
        var modules = await ReadData("Day20.txt");
        var (flipFlops, conjuctions) = Setup(modules);

        var hiSum = 0L;
        var loSum = 0L;

        foreach (var _ in 0.To(1000))
        {
            var (hi, lo) = Execute(modules, flipFlops, conjuctions);
            var lastHi = hiSum;
            var lastLo = loSum;
            hiSum += hi;
            loSum += lo;
        }

        WriteLine(hiSum*loSum);
    }

    private (Dictionary<string, bool> flipFlops, Dictionary<string, Dictionary<string, bool>> conjuctions)
    Setup(Dictionary<string, (char, string[])> modules)
    {
        var flipFlops = new Dictionary<string, bool>();
        var conjuctions = new Dictionary<string, Dictionary<string, bool>>();

        var untyped = new List<string>();
        foreach (var (name, (type, mappings)) in modules)
        {
            foreach (var mp in mappings)
            {
                if (!conjuctions.TryGetValue(mp, out var cnj))
                {
                    cnj = new();
                    conjuctions.Add(mp, cnj);
                }
                cnj[name] = false;

                if (!modules.ContainsKey(mp))
                {
                    untyped.Add(mp);
                }
            }
        }

        foreach (var untp in untyped)
        {
            modules.Add(untp, (' ', Array.Empty<string>()));
        }

        return (flipFlops, conjuctions);
    }

    private (long hi, long lo) Execute(
        Dictionary<string, (char, string[])> modules,
        Dictionary<string, bool> flipFlops,
        Dictionary<string, Dictionary<string, bool>> conjuctions)
    {
        var q = new List<(string name, string from, bool pulse)> (new [] { ("broadcaster", "button", false) });
        var hiPulseCount = 0L;
        var loPulseCount = 0L;

        while (q.Any())
        {
            var (name, from, pulse) = q.First();
            q.RemoveAt(0);

            if (pulse)
            {
                hiPulseCount++;
            }
            else
            {
                loPulseCount++;
            }

            //WriteLine($"{from} -> -{(pulse ? "high" : "low")}-> {name}");

            var (type, mappings) = modules[name];
            var nextPulse = false;

            switch (type)
            {
                case 'b':
                    foreach (var next in mappings)
                    {
                        q.Add((next, name, pulse));
                    }
                    break;

                case '&':
                    if (!conjuctions.ContainsKey(name)) throw new InvalidOperationException(name);
                    var cnj = conjuctions[name];
                    cnj[from] = pulse;
                    //WriteLine($"  - {string.Join(", ", cnj)}");
                    nextPulse = !cnj.All(_ => _.Value);
                    foreach (var mp in mappings)
                    {
                        q.Add((mp, name, nextPulse));
                        //WriteLine($"  + {name} -> -{(pulse ? "high" : "low")}-> {mp}");
                    }

                    break;

                case '%':
                    if (!flipFlops.ContainsKey(name)) flipFlops.Add(name, false);
                    if (pulse) break;
                    nextPulse = !flipFlops[name];
                    flipFlops[name] = nextPulse;
                    foreach (var next in mappings)
                    {
                        q.Add((next, name, nextPulse));
                    }
                    break;
            }
        }
        //WriteLine(string.Join(", ", flipFlops));
        //WriteLine($"{hiPulseCount} {loPulseCount}");
        //WriteLine();

        return (hiPulseCount, loPulseCount);
    }

    private async Task<Dictionary<string, (char type, string[] mappings)>> ReadData(string filepath)
    {
        var modules = new Dictionary<string, (char type, string[] mappings)>();

        foreach (var line in await File.ReadAllLinesAsync(filepath))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var splitter = line.FindFirst(" -> ");
            var module = line[..splitter];
            var mappings = line[(splitter+4)..].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (module == "broadcaster")
            {
                modules.Add("broadcaster", ('b', mappings));
                continue;
            }
            else if (module.StartsWith("%") || module.StartsWith("&"))
            {
                modules.Add(module[1..], (module[0], mappings));
                continue;
            }

            throw new InvalidDataException(line);
        }

        return modules;
    }
}