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
            var lookup = new Dictionary<string, List<string>>();
            foreach ((var _1, var _2) in LoadData("Day6.txt"))
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

            var sum = 0;
            foreach (var so in spaceObjects)
            {
                var o = so.Value;
                var length = 0;
                while (o.Id != "COM")
                {
                    o = o.Orbits;
                    length ++;
                }

                WriteLine($"{so.Key}: {length}");
                sum += length;
            }

            WriteLine($"Sum: {sum}");
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