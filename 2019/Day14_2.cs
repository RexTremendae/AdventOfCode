using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day14
{
    public class Program
    {
        private const string Ore = "ORE";
        private const string Fuel = "FUEL";

        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            _reactions = ReadInput(@"C:\dev\AdventofCode\2019\Day14.txt");
            var unique = new HashSet<string>();
            foreach (var res in _reactions.Select(x => x.from.Chemical))
            {
                if (unique.Contains(res)) throw new Exception($"Resource {res} is not unique :(");
                unique.Add(res);
            }

            var resources = new Dictionary<string, int>();
            var oreCount = 0L;
            var visited = new Dictionary<string, (long steps, long oreCount)>();
            var totalOre = 1_000_000_000_000L;
            var steps = 0L;

            var oreUsedInOneCycle = 0L;
            var completeCycles = 0L;
            var fuelCreatedInOneCycle = 0L;
            var visitedCalculationDone = false;

            var solution = 0L;

            for (;
                oreCount < totalOre
                //steps < 15
                ;steps++)
            {
                resources = Solve(resources, Fuel);
                oreCount += -resources[Ore];
                resources.Remove(Ore);

                solution = steps;
                if (steps % 100 == 0)
                {
                WriteLine($"Steps: {steps:N0}");
                WriteLine();
                }
                var state = new List<(string, int)>();
                foreach (var r in resources.OrderBy(r => r.Key))
                {
                    //WriteLine($"{r.Key} : {r.Value}");
                    state.Add((r.Key, r.Value));
                }

                var stateKey = string.Join(" ", state.Select(s => $"[{s.Item1} {s.Item2}]"));
                if (visited.TryGetValue(stateKey, out var visitedData))
                {
                    WriteLine($"State already visited after {visitedData.steps:N0} steps, oreCount {visitedData.oreCount:N0}!");
                    if (!visitedCalculationDone)
                    {
                    fuelCreatedInOneCycle = steps - visitedData.steps;
                    oreUsedInOneCycle = oreCount - visitedData.oreCount;
                    completeCycles = totalOre / oreUsedInOneCycle;
                    oreCount = visitedData.oreCount + completeCycles*oreUsedInOneCycle;
                    steps = completeCycles*fuelCreatedInOneCycle;
                    visitedCalculationDone = true;
                    }
                }
                else
                {
                    visited.Add(stateKey, (steps, oreCount));
                }
/*
                WriteLine();
                WriteLine($"ORE count: {oreCount:N0}");
                WriteLine("--------------");
*/
            }

            WriteLine($"Ore used in one cycle: {oreUsedInOneCycle:N0}");
            WriteLine($"Complete cycles: {completeCycles:N0}");
            WriteLine($"Fuel created in one cycle: {fuelCreatedInOneCycle:N0}");

            WriteLine();
            ForegroundColor = ConsoleColor.Cyan;
            WriteLine(solution);
            ResetColor();
        }

        private List<(Part from, Part[] to)> _reactions = new();

        private Dictionary<string, int> Solve(Dictionary<string, int> resources, string goal)
        {
            var reactions = _reactions.ToDictionary(key => key.from.Chemical, value => (amount: value.from.Amount, from: value.to));
            var missingResources = new Dictionary<string, int>();
            missingResources.Add(goal, 1);
            resources.Add(Ore, 0);

            var missingQueue = new List<string> { goal };
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
                        missingQueue.Add(goal);
                    }
                    backing = true;
                    lastMissing = missing;
                    continue;
                }
                lastMissing = missing;

                /*
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
                */

                var react = reactions[missing];
                var canMake = true;

                while (canMake && missingResources.GetValueOrDefault(missing) > 0)
                {
                    foreach (var from in react.from)
                    {
                        if (from.Chemical == Ore) continue;
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
                    if (missing == goal) goalReached = true;
                }
            }

            resources.Remove(goal);
            return resources;
        }

        private void Solve2()
        {
            var start = _reactions.Single(r => r.from.Chemical == Fuel);
            var incompleteSolutions = new List<Part[]>();
            var completeSolutions = new List<Part[]>();
            incompleteSolutions.Add(start.to);

            while (incompleteSolutions.Any())
            {
                var current = incompleteSolutions[0];
                incompleteSolutions.RemoveAt(0);
                foreach (var part in current)
                {
                }
            }
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