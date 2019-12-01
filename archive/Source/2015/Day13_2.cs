using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day2
{
    public class Person
    {
        public Person(string name)
        {
            Name = name;
            HapinessModifier = new Dictionary<string, int>();
        }

        public string Name { get; }
        public Dictionary<string, int> HapinessModifier { get; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            new Program().Run(args);
        }

        public void Run(string[] args)
        {
            IReader reader;
            if (args.Length > 0)
            {
                reader = new FileReader(args[0]);
            }
            else
            {
                reader = new ConsoleReader();
            }

            Dictionary<string, Person> people = new Dictionary<string, Person>();

            while (!reader.EndOfStream)
            {
                var input = reader.ReadLine();
                if (string.IsNullOrEmpty(input)) break;

                var split = input.Split(' ');
                string p1 = split[0];
                string gainLose = split[2];
                string happinessStr = split[3];
                string p2 = split[10];
                p2 = p2.Substring(0, p2.Length-1);
                int happiness = int.Parse(happinessStr);
                if (gainLose == "lose")
                    happiness *= -1;
                Person person;

                if (!people.TryGetValue(p1, out person))
                {
                    person = new Person(p1);
                    people.Add(p1, person);
                }

                person.HapinessModifier.Add(p2, happiness);
                //WriteLine($"{p1} = {p2}: {(happiness > 0 ? '+' : ' ')}{happiness}");
            }

            string me = "Me";
            Person mePerson = new Person(me);

            foreach (var p in people.Values)
            {
                mePerson.HapinessModifier.Add(p.Name, 0);
                p.HapinessModifier.Add(me, 0);
            }
            people.Add(mePerson.Name, mePerson);

            Person pStart = people.First().Value;
            List<string> visited = new List<string>();
            visited.Add(pStart.Name);
            //PrintPeople(people);

            Permutate(people, visited);
            
            WriteLine(maxHappiness);
            PrintNeighborList(maxHappinessOrder);
        }

        int maxHappiness = int.MinValue;
        List<string> maxHappinessOrder;

        private void Permutate(Dictionary<string, Person> people, List<string> visited)
        {
            bool isLeaf = true;
            foreach (var p in people.Keys)
            {
                if (visited.Contains(p))
                {
                    continue;
                }

                isLeaf = false;
                List<string> visitedCopy = new List<string>();
                visitedCopy.AddRange(visited);
                visitedCopy.Add(p);
                Permutate(people, visitedCopy);
            }

            if (isLeaf)
            {
                int happiness = 0;

                for (int idx = 0; idx < visited.Count; idx++)
                {
                    int nextIdx = idx+1;
                    if (nextIdx >= visited.Count)
                        nextIdx = 0;
                    
                    happiness += people[visited[idx]].HapinessModifier[visited[nextIdx]];
                    happiness += people[visited[nextIdx]].HapinessModifier[visited[idx]];
                }

                if (maxHappiness < happiness)
                {
                    maxHappiness = happiness;
                    maxHappinessOrder = new List<string>();
                    maxHappinessOrder.AddRange(visited);
                }

                //PrintNeighborList(visited);
            }
        }

        private void PrintNeighborList(List<string> list)
        {
            foreach (var v in list)
                Write($"{v} -> ");
                
            WriteLine("\b\b\b   ");
        }
        
        private void PrintPeople(Dictionary<string, Person> people)
        {
            foreach (var p in people)
            {
                WriteLine($"{p.Key} ({p.Value.Name})");
                for(int i = 0; i < p.Key.Length*2 + 3; i++)
                    Write("=");
                WriteLine();

                foreach(var pp in p.Value.HapinessModifier)
                {
                    WriteLine($" {pp.Key} {pp.Value}");
                }

                WriteLine();
            }
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