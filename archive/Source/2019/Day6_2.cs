using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day6
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            var graph = BuildGraph("Day6.txt");

            WriteLine(FindPath(graph));
        }

        private long FindPath(Dictionary<string, SpaceObject> graph)
        {
            var q = new List<(string id, long distance, HashSet<string> visited)>();
            q.Add(("YOU", 0, new HashSet<string>()));

            while (q.Any())
            {
                var qItem = q.First();
                if (graph[qItem.id].Id == "SAN") return qItem.distance - 2;
                q.RemoveAt(0);
                EnQ(graph, q, qItem);
            }

            return -1;
        }

        private void EnQ(
            Dictionary<string, SpaceObject> graph,
            List<(string id, long distance, HashSet<string> visited)> q,
            (string id, long distance, HashSet<string> visited) qItem)
        {
            var current = graph[qItem.id];
            var orbits = current.Orbits;
            if (orbits == null) return;
            if (!(qItem.visited.Contains(orbits.Id)))
            {
                q.Add((orbits.Id, qItem.distance+1, qItem.visited.CloneWith(qItem.id)));
            }

            foreach (var orb in current.OrbittedBy)
            {
                if (!(qItem.visited.Contains(orb.Id)))
                {
                    q.Add((orb.Id, qItem.distance+1, qItem.visited.CloneWith(qItem.id)));
                }
            }
        }

        private Dictionary<string, SpaceObject> BuildGraph(string filename)
        {
            var lookup = new Dictionary<string, List<string>>();
            foreach ((var _1, var _2) in LoadData(filename))
            {
                if (!lookup.TryGetValue(_1, out var list))
                {
                    list = new List<string>();
                    lookup.Add(_1, list);
                }

                list.Add(_2);
            }

            var currentSo = new SpaceObject { Id = "COM" };
            var spaceObjects = new Dictionary<string, SpaceObject>();
            spaceObjects.Add(currentSo.Id, currentSo);
            var q = new List<string>(new[] {currentSo.Id});

            while(q.Any())
            {
                currentSo = spaceObjects[q.First()];
                q.RemoveAt(0);
                var id = currentSo.Id;
                lookup.TryGetValue(id, out var lookupResult);
                foreach (var orbit in lookupResult ?? Enumerable.Empty<string>())
                {
                    if (!spaceObjects.TryGetValue(orbit, out var newSo))
                    {
                        newSo = new SpaceObject { Id = orbit, Orbits = currentSo };
                        spaceObjects.Add(orbit, newSo);
                    }

                    currentSo.OrbittedBy.Add(newSo);
                    q.Add(orbit);
                }
            }

            return spaceObjects;
        }

        public List<(string, string)> LoadData(string filename)
        {
            var result = new List<(string, string)>();
            foreach (var ln in File.ReadAllLines(filename))
            {
                var line = ln.Trim();
                var parts = ln.Split(")", StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    result.Add((parts[0], parts[1]));
                }
            }

            return result;
        }
    }

    public static class Extensions
    {
        public static HashSet<string> CloneWith(this HashSet<string> source, string newEntry)
        {
            var clone = new HashSet<string>();
            foreach (var item in source)
            {
                clone.Add(item);
            }
            clone.Add(newEntry);

            return clone;
        }
    }

    public class SpaceObject
    {
        public string Id { get; set; }
        public SpaceObject Orbits { get; set; }
        public HashSet<SpaceObject> OrbittedBy { get; set; } = new HashSet<SpaceObject>();

        public override bool Equals(object obj)
        {
            var other = obj as SpaceObject;
            if (other == null) return false;

            return Id == other.Id;
        }
        
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}