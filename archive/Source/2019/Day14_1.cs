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
            _reactions = ReadInput("Day14.txt");
            var unique = new HashSet<string>();
            foreach (var res in _reactions.Select(x => x.from.Chemical))
            {
                if (unique.Contains(res)) throw new Exception($"Resource {res} is not unique :(");
                unique.Add(res);
            }
            Solve(debug: false);
        }

        private List<(Part from, Part[] to)> _reactions;
        private void Solve(bool debug)
        {
            var reactions = _reactions.ToDictionary(key => key.from.Chemical, value => (amount: value.from.Amount, from: value.to));
            var missingResources = new Dictionary<string, int>();
            missingResources.Add(Goal, 1);
            var resources = new Dictionary<string, int>();
            resources.Add(Start, 0);

            var missingQueue = new List<string>() { Goal };
            var lastMissing = "";
            bool backing = false;
            bool goalReached = false;

            while (!goalReached)
            {
                var missing = missingQueue.Last();

                if (missing == lastMissing)
                {
                    missingQueue.Remove(missing);
                    if (missingQueue.Count == 0)
                    {
                        missingQueue.Add(Goal);
                    }
                    backing = true;
                    lastMissing = missing;
                    continue;
                }
                lastMissing = missing;

                if (debug)
                {
                    WriteLine($"Trying to make {missingResources[missing]} {missing}");
                    WriteLine("  Resources:");
                    foreach (var r in resources.OrderBy(x => x.Key)) WriteLine($"    {r.Key} : {r.Value}");
                    WriteLine("  Missing Resources:");
                    foreach (var r in missingResources.OrderBy(x => x.Key)) WriteLine($"    {r.Key} : {r.Value}");
                    WriteLine("  Missing Resource Queue:");
                    foreach (var r in missingQueue) WriteLine($"    {r}");
                    WriteLine();
                }

                var react = reactions[missing];
                var canMake = true;

                while (canMake && missingResources.GetValueOrDefault(missing) > 0)
                {
                    foreach (var from in react.from)
                    {
                        if (from.Chemical == Start) continue;
                        var available = resources.GetValueOrDefault(from.Chemical);
                        if (available < from.Amount)
                        {
                            canMake = false;
                            if (!missingResources.ContainsKey(from.Chemical)) backing = false;
                            var fromMissing = missingResources.GetValueOrDefault(from.Chemical);
                            missingResources[from.Chemical] = Math.Max(from.Amount-available, fromMissing);
                            if (!missingQueue.Contains(from.Chemical)) missingQueue.Add(from.Chemical);
                        }
                    }

                    if (!canMake)
                    {
                        if (backing)
                        {
                            missingQueue.Remove(missing);
                        }
                        continue;
                    }

                    backing = false;
                    foreach (var from in react.from)
                    {
                        resources[from.Chemical] -= from.Amount;
                        if (resources[from.Chemical] == 0) resources.Remove(from.Chemical);
                    }
                    var toAmount = resources.GetValueOrDefault(missing);
                    resources[missing] = toAmount + react.amount;
                    missingQueue.Remove(missing);
                    missingResources[missing] -= react.amount;
                    if (missingResources[missing] <= 0) missingResources.Remove(missing);
                    if (missing == Goal) goalReached = true;
                }
            }

            foreach (var r in resources) WriteLine($"{r.Key} : {r.Value}");
            WriteLine();
        }

        private List<(Part from, Part[] to)> ReadInput(string filename)
        {
            var reactions = new List<(Part from, Part[] to)>();
            var uniqueTo = new HashSet<string>();

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

                if (uniqueTo.Contains(from.Chemical)) throw new InvalidOperationException($"More than one reaction produces {from.Chemical}");
                uniqueTo.Add(from.Chemical);
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