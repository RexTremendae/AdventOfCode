using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day14
{
    public class Program
    {
        private const string Start = "ORE";
        private const string Goal = "FUEL";

        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            _reactions = ReadInput("Day14_small1.txt");
            var unique = new HashSet<string>();
            foreach (var res in _reactions.Select(x => x.from.Chemical))
            {
                if (unique.Contains(res)) throw new Exception($"Resource {res} is not unique :(");
                unique.Add(res);
            }
            var trace = FindRoute().ToList();
            Backtrace(trace);
        }

        private List<(Part from, Part[] to)> _reactions;
        private Dictionary<string, int> _resources;
        private void Backtrace(List<(Part to, Part[] fromList)[]> trace)
        {
            _resources = new Dictionary<string, int>();
            _resources.Add(Start, 0);

            for (int i = 0; i < trace.Count; i++)
            {
                WriteLine($"Trace step {i}");
                foreach (var step in trace[i])
                {
                    {
                        var from = string.Join("; ", step.fromList);
                        WriteLine($"  {from} => {step.to}");
                    }

                    _resources.TryGetValue(step.to.Chemical, out var amount);
                    amount += step.to.Amount;
                    _resources[step.to.Chemical] = amount;

                    foreach (var from in step.fromList) _resources[from.Chemical] -= from.Amount;
                }
                foreach(var res in _resources.Where(x => x.Value != 0)) WriteLine($"{res.Key}: {res.Value}");
                WriteLine();
            }

            WriteLine();
            foreach(var res in _resources.Where(x => x.Value != 0)) WriteLine($"{res.Key}: {res.Value}");
        }

        private IEnumerable<(Part to, Part[] fromList)[]> FindRoute()
        {
            var queue = new List<
                (string[] unresolved,
                 Dictionary<string, int> data,
                 List<
                    (Part from, Part[] to)[]
                 > trace)
            >();
            var data = new Dictionary<string, int> {{ Goal, 1 }};
            var trace = new List<(Part from, Part[] to)[]>();
            queue.Add((new[] { Goal }, data, trace));

            var unresolved = new List<string>();
            var lastTrace = new List<(Part from, Part[] toList)[]>();

            do
            {
                var qItem = queue.First();
                queue.RemoveAt(0);
                data = qItem.data;
                unresolved = new List<string>();

                var traceItem = new List<(Part from, Part[] to)>();
                foreach (var u in qItem.unresolved)
                {
                    (var from, var toList) = _reactions.Single(x => x.from.Chemical == u);
                    var newFromAmount = data[from.Chemical] - from.Amount;
                    if (data[from.Chemical] == 0) data.Remove(from.Chemical);

                    foreach (var to in toList)
                    {
                        if (to.Chemical != Start)
                            unresolved.Add(to.Chemical);
                        data.TryGetValue(to.Chemical, out var toAmount);
                        data[to.Chemical] = to.Amount + toAmount;
                    }

                    traceItem.Add((from, toList));
                }
                lastTrace = qItem.trace.CloneAndAdd(traceItem.ToArray());
                queue.Add((unresolved.ToArray(), data.Clone(), lastTrace));
            }
            while (unresolved.Any());

            return lastTrace.AsEnumerable().Reverse();
        }

        private List<(Part from, Part[] to)> ReadInput(string filename)
        {
            var reactions = new List<(Part from, Part[] to)>();

            foreach (var line in File.ReadAllLines(filename))
            {
                if (string.IsNullOrEmpty(line)) continue;
                var parts = line.Split("=>", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
                var leftPart = parts.First();
                var rightParts = parts.Skip(1).First().Split(' ');

                Part from = new Part(int.Parse(rightParts[0]), rightParts[1].Trim());
                var to = new List<Part>();

                foreach(var subpart in leftPart.Split(',').Select(x => x.Trim()))
                {
                    var split = subpart.Split(' ').Select(x => x.Trim());
                    to.Add(new Part(int.Parse(split.First()), split.Skip(1).First()));
                }
                reactions.Add((from, to.ToArray()));
            }

            return reactions;
        }
    }

    public static class Extensions
    {
        public static List<T> CloneAndAdd<T>(this List<T> source, T addition)
        {
            var result = new List<T>();
            foreach (var item in source)
            {
                result.Add(item);
            }

            result.Add(addition);
            return result;
        }

        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> source)
        {
            var result = new Dictionary<TKey, TValue>();
            foreach (var kvp in source) result.Add(kvp.Key, kvp.Value);

            return result;
        }
    }

    public struct Part
    {
        public Part(int amount, string chemical)
        {
            Amount = amount;
            Chemical = chemical;
        }

        public int Amount { get; set; }
        public string Chemical { get; set; }

        public override string ToString()
        {
            return $"{Amount.ToString()} {Chemical}";
        }
    }
}