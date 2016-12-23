using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using static System.Console;
using static System.ConsoleColor;
using static Day20.ColorWriter;

namespace Day20
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

            IntervalMergerTests.Run();
            return;
            while (!reader.EndOfStream)
            {
                var input = reader.ReadLine();
                if (string.IsNullOrEmpty(input)) break;
            }
        }
    }

    public class IntervalMerger
    {
        private List<Interval> _intervals;
        public IEnumerable<Interval> Intervals { get { return _intervals; } }

        public IntervalMerger(IEnumerable<Interval> intervals = null)
        {
            _intervals = new List<Interval>();
            _intervals.AddRange(intervals);
        }

        public void Add(Interval newInterval)
        {
            int i = 0;

            bool overlap = false;
            Interval existing = null;

            for (i = 0; i < _intervals.Count; i ++)
            {
                existing = _intervals[i];

                if (existing.Min > newInterval.Max)
                {
                    break;
                }

                if (existing.Max >= newInterval.Min && newInterval.Max >= existing.Min)
                {
                    overlap = true;
                    break;
                }
            }

            if (i >= _intervals.Count)
                _intervals.Add(newInterval);
            else if(overlap)
            {
                if (existing.Min > newInterval.Min)
                    existing.Min = newInterval.Min;

                if (existing.Max < newInterval.Max)
                    existing.Max = newInterval.Max;
            }
            else
                _intervals.Insert(i, newInterval);
        }
    }

    public class Interval
    {
        public int Min;
        public int Max;
    }

    public static class IntervalMergerTests
    {
        static List<IntervalTestCase> _testCases;

        private static void Setup()
        {
            _testCases = new List<IntervalTestCase>();

            _testCases.Add(new IntervalTestCase
            {
                Id = 1,
                InitialState =   "",
                IntervalToAdd =  " []",
                ExpectedResult = " []"
            });

            _testCases.Add(new IntervalTestCase
            {
                Id = 2,
                InitialState =   "     [-]",
                IntervalToAdd =  " [-]",
                ExpectedResult = " [-] [-]"
            });

            _testCases.Add(new IntervalTestCase
            {
                Id = 3,
                InitialState =   " [-]     [-]",
                IntervalToAdd =  "     [-]",
                ExpectedResult = " [-] [-] [-]"
            });

            _testCases.Add(new IntervalTestCase
            {
                Id = 4,
                InitialState =   " [--]",
                IntervalToAdd =  "    [-]",
                ExpectedResult = " [----]"
            });

            _testCases.Add(new IntervalTestCase
            {
                Id = 5,
                InitialState =   "   [--]",
                IntervalToAdd =  " [-]",
                ExpectedResult = " [----]"
            });

            _testCases.Add(new IntervalTestCase
            {
                Id = 6,
                InitialState =   " [----]",
                IntervalToAdd =  "  [-]",
                ExpectedResult = " [----]"
            });

            _testCases.Add(new IntervalTestCase
            {
                Id = 7,
                InitialState =   "  [-]",
                IntervalToAdd =  " [---]",
                ExpectedResult = " [---]"
            });

            _testCases.Add(new IntervalTestCase
            {
                Id = 8,
                InitialState =   " [-] [-]",
                IntervalToAdd =  "   [-]",
                ExpectedResult = " [-----]"
            });

            _testCases.Add(new IntervalTestCase
            {
                Id = 9,
                InitialState =   " [-] [-] [-]",
                IntervalToAdd =  "  [-------]",
                ExpectedResult = " [---------]"
            });
        }

        public static void Run()
        {
            Setup();

            foreach (var testcase in _testCases)
            {
                Write($"Running testcase #{testcase.Id}... ", DarkYellow);

                bool success = true;
                bool firstFail = true;

                IntervalMerger merger = new IntervalMerger(testcase.InitialStateAsIntervals);
                merger.Add(testcase.IntervalToAddAsInterval);
                
                var expected = testcase.ExpectedResultAsIntervals.ToArray();
                var actual = merger.Intervals.ToArray();

                if (actual.Count() != expected.Count())
                {
                    if (firstFail)
                    {
                        firstFail = false;
                        WriteLine();
                    }

                    Write($"Testcase #{testcase.Id} failed! ", Red);
                    WriteLine($"Number of intervals mismatched, expected: {expected.Count()}, actual: {actual.Count()}");
                    success = false;
                }

                for (int i = 0; i < testcase.ExpectedResultAsIntervals.Length; i++)
                {
                    if (expected[i].Min != actual[i].Min || expected[i].Max != actual[i].Max)
                    {
                        if (firstFail)
                        {
                            firstFail = false;
                            WriteLine();
                        }

                        Write($"Testcase #{testcase.Id} failed! ", Red);
                        WriteLine($"Interval {i} mismatched, expected: [{expected[i].Min} - {expected[i].Max}], actual: [{actual[i].Min} - {actual[i].Max}]");
                        success = false;
                    }
                }

                if (success)
                {
                    WriteLine($"Test case #{testcase.Id} succeeded!", Green);
                }
            }
        }

        private class IntervalTestCase
        {
            public int Id;
            private string _initalState;
            private string _expectedResult;
            private string _intervalToAdd;

            public string InitialState
            {
                get { return _initalState; }
                set
                {
                    _initalState = value;
                    InitialStateAsIntervals = ToIntervals(_initalState);
                }
            }

            public string ExpectedResult
            {
                get { return _expectedResult; }
                set
                {
                    _expectedResult = value;
                    ExpectedResultAsIntervals = ToIntervals(_expectedResult);
                }
            }

            public string IntervalToAdd
            {
                get { return _intervalToAdd; }
                set
                {
                    _intervalToAdd = value;
                    IntervalToAddAsInterval = ToIntervals(_intervalToAdd).Single();
                }
            }

            private Interval[] ToIntervals(string input)
            {
                var intervals = new List<Interval>();
                int openIndex = -1;

                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i] == '[')
                    {
                        openIndex = i;
                    }
                    else if (input[i] == ']')
                    {
                        if (openIndex < 0)
                            throw new InvalidOperationException("Invalid inteval!");

                        intervals.Add(new Interval { Min = openIndex, Max = i });
                        openIndex = -1;
                    }
                }

                if (openIndex >= 0)
                    throw new InvalidOperationException("Invalid inteval!");

                return intervals.ToArray();
            }

            public Interval[] InitialStateAsIntervals;
            public Interval[] ExpectedResultAsIntervals;
            public Interval IntervalToAddAsInterval;
        }
    }

    public static class ColorWriter
    {
        public static void Write(string text, ConsoleColor color)
        {
            var oldColor = ForegroundColor;
            ForegroundColor = color;
            Console.Write(text);
            ForegroundColor = oldColor;
        }

        public static void WriteLine(string text, ConsoleColor color)
        {
            Write(text, color);
            Console.WriteLine();
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
}