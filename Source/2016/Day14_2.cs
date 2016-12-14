using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using static System.Console;

namespace Day14
{
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

            string salt = ReadLine();
            int keyToFind = int.Parse(ReadLine());

            Solve(salt, keyToFind);
        }

        private class Multiples
        {
            public string Triplets = "";
            public string Quintlets = "";
        }

        private void Solve(string salt, int keyToFind)
        {
            var calculated = new Dictionary<long, Multiples>();
            long l = 0;
            var keys = new List<long>(); 

            long lastUpdate = 0;
            int updateSteps = 500;

            while(keys.Count < keyToFind)
            {
                Multiples multiples;

                if (calculated.Count >= lastUpdate + updateSteps)
                {
                    lastUpdate += updateSteps;
                    WriteLine($"{lastUpdate}...");
                }

                if (calculated.ContainsKey(l))
                {
                    multiples = calculated[l];
                }
                else
                {
                    multiples = CalculateMultiples(l, salt);
                    calculated.Add(l, multiples);
/*
                    if (!string.IsNullOrEmpty(multiples.Triplets) || !string.IsNullOrEmpty(multiples.Quintlets))
                    {
                        WriteLine($"[{l}] {md5}");
                        WriteLine($"  x3: {multiples.Triplets}");
                        WriteLine($"  x5: {multiples.Quintlets}");
                    }
*/
                }

                if (multiples.Triplets != "")
                {
                    bool keyFound = false;
                    long lnext = l + 1000;
                    Multiples m2;

                    for (long i = l+1; i <= lnext; i++)
                    {
                        if (calculated.ContainsKey(i))
                        {
                            m2 = calculated[i];
                        }
                        else
                        {
                            m2 = CalculateMultiples(i, salt);
                            calculated.Add(i, m2);
                        }

                        foreach(var c in multiples.Triplets)
                            if (m2.Quintlets.Contains(c))
                            {
                                keyFound = true;
                            }

                        if (keyFound) break;
                    }

                    if (keyFound) keys.Add(l);
                }

                l++;
            }

            WriteLine(keys[keyToFind-1]);
        }

        private Multiples CalculateMultiples(long l, string salt)
        {
            var md5 = MD5.Calculate($"{salt}{l}").ToLower();
            for (int i = 0; i < 2016; i++)
                md5 = MD5.Calculate(md5).ToLower();

            var multiples = new Multiples();

            for (int i = 0; i < md5.Length; i++)
            {
                int j = 1;

                for (; j < 5; j++)
                {
                    if (i+j >= md5.Length) break;
                    if (md5[i] != md5[i+j]) break;
                }

                if (j >= 3 && multiples.Triplets == "") multiples.Triplets = md5[i].ToString();
                if (j >= 5) multiples.Quintlets += md5[i];
            }

            return multiples;
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

    public static class MD5
    {
        private static System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

        public static string Calculate(string input)
        {
            // step 1, calculate MD5 hash from input
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}