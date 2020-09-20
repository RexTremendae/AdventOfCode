using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Console;

namespace Day19
{
    public class Program
    {
        public static Dictionary<string, int> AtomsDictionary = new Dictionary<string, int>();
        public static string[] Atoms = new string[100];

        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            BuildAtomsDictionary();
            Solve(Medicine);
            //Solve("CRnThRnPTiBFArArF"); // Solution is found in 6 steps
            for(;;)
            {
                Write("> ");
                var input = ReadLine();
                try
                {
                    Solve(input);
                }
                catch (Exception ex)
                {
                    WriteLine(ex.ToString());
                }
            }
        }

        private void BuildAtomsDictionary()
        {
            AtomsDictionary.Clear();
            Atoms = new string[100];
            int nextIndex = 0;

            foreach (var key in Transformations.Keys)
            {
                if (AtomsDictionary.ContainsKey(key)) continue;

                Atoms[nextIndex] = key;
                AtomsDictionary.Add(key, nextIndex);
                nextIndex++;
            }

            foreach (var val in Transformations.Values)
            {
                foreach (var value in val)
                {
                    foreach (var atom in value.SplitIntoAtoms())
                    {
                        if (AtomsDictionary.ContainsKey(atom)) continue;

                        Atoms[nextIndex] = atom;
                        AtomsDictionary.Add(atom, nextIndex);
                        nextIndex++;
                    }
                }
            }
        }

        public void Solve(string input)
        {
            var rawSplitAtoms = input.SplitIntoAtoms();
            var atomsArray = new IntArray(rawSplitAtoms.Select(a => AtomsDictionary[a]));

            var transTree = BuildReversedTransformationTree();
            var inputAtoms = new IntArray(input.SplitIntoAtoms().Select(a => AtomsDictionary[a]));
            var q = new List<(int moves, IntArray atoms)>();
            var visisted = new HashSet<IntArray>();
            var lastMovesCount = 0;
            var lastQueueSize = 0;
            var startTime = DateTime.Now;

            q.Add((0, inputAtoms));
            for(;;)
            {
                var deQ = q.First();
                q.RemoveAt(0);
                var atoms = deQ.atoms;

                bool update = false;
                if (deQ.moves > lastMovesCount)
                {
                    lastMovesCount = deQ.moves;
                    update = true;
                }

                if (q.Count > 10_000 + lastQueueSize)
                {
                    lastQueueSize += 10_000;
                    update = true;
                }

                if (update)
                {
                    var elapsedTime = (DateTime.Now - startTime);
                    WriteLine($"[{elapsedTime.Minutes.ToString("00")}:{elapsedTime.Seconds.ToString("00")}]   Max moves: {lastMovesCount.ToString("##0").PadRight(3, ' ')}   Max Q size: {lastQueueSize:### ### ##0}");
                }

                for (int i = 0; i < atoms.Length; i++)
                {
                    int len = 1;
                    bool anyMatch = true;
                    while (i + len <= atoms.Length && anyMatch)
                    {
                        var key = atoms[i..(i+len)];
                        var matches = transTree.Match(key).ToArray();

                        if (matches.Any())
                        {
                            var firstMatch = matches.First();
                            if (firstMatch.key.Equals(key))
                            {
                                if (firstMatch.value.Equals(new int[] { AtomsDictionary["e"] }))
                                {
                                    WriteLine($"Solution: {deQ.moves+1}");
                                    return;
                                }

                                var newAtoms = new IntArray();
                                newAtoms.Add(atoms[0..i]);
                                newAtoms.Add(firstMatch.value);
                                newAtoms.Add(atoms[(i+len)..]);
                                if (!visisted.Contains(newAtoms))
                                {
                                    //WriteLine($"  Enqueued {newAtoms.ToString()}");
                                    int idx = 0;
                                    var alen = newAtoms.Length;
                                    while (idx < q.Count && q[idx].atoms.Length < alen) idx++;

                                    q.Insert(idx, (deQ.moves+1, newAtoms));
                                    visisted.Add(newAtoms);
                                }
                            }
                        }
                        else
                        {
                            anyMatch = false;
                        }

                        len++;
                    }
                }
            }
        }

        public class IntArray
        {
            private List<int> _items = new List<int>();
            private StringBuilder _hashBuilder = new StringBuilder();

            public int Length => _items.Count;

            public static implicit operator IntArray(int[] array) => new IntArray(array);
            public static implicit operator int[](IntArray array) => array.ToArray();

            public int[] this[Range range]
            {
                get
                {
                    var (offset, length) = range.GetOffsetAndLength(_items.Count);

                    return new Span<int>(_items.ToArray())
                        .Slice(offset, length)
                        .ToArray();
                }
            }

            public IntArray() {}

            public IntArray(IEnumerable<int> data)
            {
                _items = new List<int>(data);
            }

            public bool IsEmpty => _items.Count == 0;
            public int First => _items[0];

            public IntArray CloneAndAppend(int value)
            {
                return CloneAndAppend(new int[] {value});
            }

            public IntArray CloneAndAppend(IntArray toAppend)
            {
                var clone = this.Clone();
                clone.Add(toAppend);
                return clone;
            }

            public int[] ToArray()
            {
                var clone = new int[_items.Count];
                for (int i = 0; i < _items.Count; i++)
                    clone[i] = _items[i];

                return clone;
            }

            public IntArray Clone()
            {
                return new IntArray(_items);
            }

            public void Add(params int[] data)
            {
                _items.AddRange(data);
                _hashBuilder.Append(string.Join("", data));
            }

            public override string ToString()
            {
                return string.Join(", ", _items);
            }

            public override bool Equals(object? o)
            {
                if (o is IntArray otherIntArray)
                {
                    if (otherIntArray._items.Count != _items.Count)
                        return false;

                    for (int i = 0; i < _items.Count; i ++)
                        if (otherIntArray._items[i] != _items[i])
                            return false;

                    return true;
                }
                else if (o is int[] otherArray)
                {
                    if (otherArray.Length != _items.Count)
                        return false;

                    for (int i = 0; i < _items.Count; i ++)
                        if (otherArray[i] != _items[i])
                            return false;

                    return true;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return _hashBuilder.ToString().GetHashCode();
            }
        }

        public void RunInteractive()
        {
            var transTree = BuildReversedTransformationTree();

            for(;;)
            {
                try
                {
                    Write("> ");
                    var input = ReadLine();
                    var atoms = input.SplitIntoAtoms().Select(a => AtomsDictionary[a]).ToArray();

                    for (int i = 0; i < atoms.Length; i++)
                    {
                        int len = 1;
                        bool anyMatch = true;
                        while (i + len <= atoms.Length && anyMatch)
                        {
                            var key = atoms[i..(i+len)];
                            var matches = transTree.Match(key).ToArray();

                            if (matches.Any())
                            {
                                var firstMatch = matches.FirstOrDefault();
                                if (firstMatch.key.Equals(key))
                                {
                                    var range = len > 1 ? $" - {i+len-1}" : "";
                                    WriteLine($"[{i}{range}]: {firstMatch.key} => {firstMatch.value}");
                                }
                            }
                            else
                            {
                                anyMatch = false;
                            }

                            len++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLine(ex.Message);
                }
            }
        }

        private RevTransTree BuildReversedTransformationTree()
        {
            var tree = new RevTransTree();

            foreach (var kvp in Transformations)
            {
                foreach (var val in kvp.Value)
                {
                    tree.AddTransformation(kvp.Key, val);
                }
            }

            return tree;
        }

        public class KeyNode
        {
            private Dictionary<int, KeyNode> _nodes;

            public KeyNode()
            {
                Value = new IntArray();
                _nodes = new Dictionary<int, KeyNode>();
            }

            public IReadOnlyDictionary<int, KeyNode> Nodes => _nodes;
            public IntArray Value { get; private set; }

            public void AddTransformation(IntArray key, string[] valueAtoms)
            {
                if (valueAtoms.Length == 0)
                {
                    if (!Value.IsEmpty)
                    {
                        throw new InvalidOperationException("Node already has a value.");
                    }
                    Value = key.Clone();
                    return;
                }

                var atom = Program.AtomsDictionary[valueAtoms[0]];
                if (!_nodes.TryGetValue(atom, out var node))
                {
                    node = new KeyNode();
                    _nodes.Add(atom, node);
                }

                node.AddTransformation(key, valueAtoms[1..]);
            }
        }

        public class RevTransTree
        {
            private KeyNode _root;

            public RevTransTree()
            {
                _root = new KeyNode();
            }

            public void AddTransformation(string key, string value)
            {
                var keyAtoms = key.SplitIntoAtoms().Select(a => Program.AtomsDictionary[a]).ToArray();
                _root.AddTransformation(keyAtoms, value.SplitIntoAtoms().ToArray());
            }

            public IntArray ExactMatch(IntArray input)
            {
                KeyNode found = _root;
                var inputString = string.Empty;
                while (!input.IsEmpty)
                {
                    if (!found.Nodes.TryGetValue(input.First, out var node))
                    {
                        return new IntArray();
                    }

                    found = node;
                    inputString += input.First;
                    input = input[1..];
                }

                return found.Value;
            }

            public IEnumerable<(IntArray key, IntArray value)> Match(IEnumerable<int> input)
            {
                KeyNode found = _root;
                var inputString = new IntArray();
                foreach (var atom in input)
                {
                    if (!found.Nodes.TryGetValue(atom, out var node))
                    {
                        return new (IntArray, IntArray)[] {};
                    }

                    found = node;
                    inputString.Add(atom);
                }

                var results = new List<(IntArray, IntArray)>();
                GenerateMatchAnswers(inputString, found, results);

                return results;
            }

            private void GenerateMatchAnswers(IntArray input, KeyNode found, List<(IntArray, IntArray)> results)
            {
                if (!found.Value.IsEmpty)
                {
                    results.Add((input, found.Value));
                }

                foreach (var kvp in found.Nodes)
                {
                    GenerateMatchAnswers(input.CloneAndAppend(kvp.Key), kvp.Value, results);
                }
            }
        }

        public static readonly Dictionary<string, string[]> Transformations = new Dictionary<string, string[]>
        {
            { "e",  new [] { "HF", "NAl", "OMg" } },
            { "Al", new [] { "ThF", "ThRnFAr" } },
            { "B",  new [] { "BCa", "TiB", "TiRnFAr" } },
            { "Ca", new [] { "CaCa", "PB", "PRnFAr", "SiRnFYFAr", "SiRnMgAr", "SiTh" } },
            { "F",  new [] { "CaF", "PMg", "SiAl" } },
            { "H",  new [] { "CRnAlAr", "CRnFYFYFAr", "CRnFYMgAr", "CRnMgYFAr", "HCa", "NRnFYFAr", "NRnMgAr", "NTh", "OB", "ORnFAr" } },
            { "Mg", new [] { "BF", "TiMg" } },
            { "N",  new [] { "CRnFAr", "HSi" } },
            { "O",  new [] { "CRnFYFAr", "CRnMgAr", "HP", "NRnFAr", "OTi" } },
            { "P",  new [] { "CaP", "PTi", "SiRnFAr" } },
            { "Si", new [] { "CaSi" } },
            { "Th", new [] { "ThCa" } },
            { "Ti", new [] { "BP", "TiTi" } }
        };

        public const string Medicine =
            "CRnCaCaCaSiRnBPTiMgArSiRnSiRnMgArSiRnCaFArTiTiBSiThFYCaFArCaCaSiThCaPBSiThSiThCaCa" +
            "PTiRnPBSiThRnFArArCaCaSiThCaSiThSiRnMgArCaPTiBPRnFArSiThCaSiRnFArBCaSiRnCaPRnFArPMgY" +
            "CaFArCaPTiTiTiBPBSiThCaPTiBPBSiRnFArBPBSiRnCaFArBPRnSiRnFArRnSiRnBFArCaFArCaCaCaSiTh" +
            "SiThCaCaPBPTiTiRnFArCaPTiBSiAlArPBCaCaCaCaCaSiRnMgArCaSiThFArThCaSiThCaSiRnCaFYCaSi" +
            "RnFYFArFArCaSiRnFYFArCaSiRnBPMgArSiThPRnFArCaSiRnFArTiRnSiRnFYFArCaSiRnBFArCaSiRnTiMg" +
            "ArSiThCaSiThCaFArPRnFArSiRnFArTiTiTiTiBCaCaSiRnCaCaFYFArSiThCaPTiBPTiBCaSiThSiRnMgArCaF";
    }

    public static class Extensions
    {
        public static IEnumerable<string> SplitIntoAtoms(this string input)
        {
            int index = 0;
            while (index < input.Length)
            {
                var current = GetAtom(input, index);
                yield return current;
                index += current.Length;
            }
        }

        public static string GetInitialAtom(this string input)
        {
            return GetAtom(input, 0);
        }

        public static string GetAtom(this string input, int startIndex)
        {
            if (input == "e") return input;

            if (startIndex >= input.Length)
            {
                return string.Empty;
            }

            var first = input[startIndex];
            if (!char.IsUpper(first))
            {
                throw new InvalidOperationException($"No atom at position {startIndex} in '{input}'.");
            }

            string atom = first.ToString();

            var nextIndex = startIndex + 1;
            while (nextIndex < input.Length)
            {
                var next = input[nextIndex];
                if (char.IsUpper(next))
                {
                    return atom;
                }

                atom += next;
                nextIndex++;
            }

            return atom;
        }
    }
}