using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day2
{
    public class Node
    {
        public Node(string tag)
        {
            Tag = tag;
            Neighbors = new Dictionary<Node, int>();
        }

        public Dictionary<Node, int> Neighbors;
        public string Tag;

        public override int GetHashCode()
        {
            return Tag.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Node otherNode = obj as Node;
            if (otherNode == null)
                return false;
            
            return otherNode.Tag == Tag;
        }
    }

    public class Program
    {
        Dictionary<string, Node> nodes;
        List<string> longestPath;
        int longestPathLength;

        public static void Main(string[] args)
        {
            new Program().Run(args);
        }

        public void Run(string[] args)
        {
            longestPathLength = 0;
            longestPath = null;
            nodes = new Dictionary<string, Node>();
            IReader reader;

            if (args.Length > 0)
            {
                reader = new FileReader(args[0]);
            }
            else
            {
                reader = new ConsoleReader();
            }

            while (!reader.EndOfStream)
            {
                var input = reader.ReadLine();
                if (string.IsNullOrEmpty(input)) break;

                var split = input.Split(new []{"to", "="}, StringSplitOptions.None);
                Node node1, node2;
                var node1Tag = split[0].Trim();
                var node2Tag = split[1].Trim();

                if (!nodes.TryGetValue(node1Tag, out node1))
                {
                    node1 = new Node(node1Tag.Trim());
                    nodes.Add(node1Tag, node1);
                }

                if (!nodes.TryGetValue(node2Tag, out node2))
                {
                    node2 = new Node(node2Tag.Trim());
                    nodes.Add(node2Tag, node2);
                }

                node1.Neighbors.Add(node2, int.Parse(split[2]));
                node2.Neighbors.Add(node1, int.Parse(split[2]));
            }
/*
            foreach(var n in nodes.Values)
            {
                Write($"{n.Tag} | ");
                foreach(var neigh in n.Neighbors)
                {
                    Write($"{neigh.Key.Tag}: {neigh.Value} | ");
                }
                WriteLine();
            }
*/
            foreach(var n in nodes.Values)
            {
                List<string> visited = new List<string>();
                visited.Add(n.Tag);
                FindShortestPath(n, visited, 0, 0);
            }

            Write("Shortest path: ");
            foreach(var s in longestPath)
            {
                Write($"{s} -> ");
            }
            WriteLine($"\b\b\b({longestPathLength})");
        }

        public void FindShortestPath(Node node, List<string> visitedPath, int pathLength, int callId)
        {
/*
            Write($"[{callId}] {node.Tag} | {pathLength} | ");
            foreach(var s in visitedPath)
            {
                Write($"{s} -> ");
            }
            WriteLine("\b\b\b   ");
*/

            bool leaf = true;
            foreach(var neigh in node.Neighbors)
            {
                if (visitedPath.Contains(neigh.Key.Tag))
                {
                    continue;
                }

                leaf = false;
                var visitedPathClone = visitedPath.Clone();
                visitedPathClone.Add(neigh.Key.Tag);
//                WriteLine($"[{callId}] Node->Neigh | {node.Tag} | {neigh.Key.Tag} | {neigh.Value}");
                FindShortestPath(neigh.Key, visitedPathClone, pathLength + neigh.Value, callId + 1);
            }

            if (leaf)
            {
                if (longestPathLength < pathLength)
                    longestPathLength = pathLength;
                longestPath = visitedPath.Clone();
                //PrintPath(visitedPath, pathLength);
            }
        }

        public void PrintPath(List<string> visitedPath, int pathLength)
        {
            for (int i = 0; i < visitedPath.Count; i++)
            {
                Write($"{visitedPath[i]}");
                if (i < visitedPath.Count-1)
                    Write(" -> ");
            }

            WriteLine($": {pathLength}");
        }
    }

    public static class NodeListExtension
    {
        public static List<T> Clone<T>(this List<T> origin)
        {
            List<T> copy = new List<T>();

            foreach(var v in origin)
                copy.Add(v);

            return copy;
        }
    }

    interface IReader
    {
        string ReadLine();
        bool EndOfStream { get; }
    }

    public class ConsoleReader : IReader
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public bool EndOfStream => false;
    }

    public class FileReader : IReader
    {
        StreamReader _reader;
        
        public FileReader(string filename)
        {
            _reader = new StreamReader(filename);
        }

        public string ReadLine()
        {
            return _reader.ReadLine();
        }

        public bool EndOfStream 
        {
            get
            {
                return _reader.EndOfStream;
            }
        }
    }

    public class Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }

        public int Y { get; }

        public static Point Parse(string input)
        {
            var split = input.Split(',');
            int x = int.Parse(split[0]);
            int y = int.Parse(split[1]);

            return new Point(x, y);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(X, Y).GetHashCode();
        }

        public override bool Equals(object other)
        {
            Point otherPoint = other as Point;
            if (otherPoint == null)
            {
                return false;
            }

            return X == otherPoint.X && Y == otherPoint.Y;
        }
    }
}