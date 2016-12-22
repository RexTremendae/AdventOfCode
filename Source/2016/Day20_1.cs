using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using static System.Console;

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
                InitialState = new Interval[] {},
                IntervalToAdd = new Interval { Min = 1, Max = 2 },
                ExpectedResult = new [] { new Interval {Min = 1, Max = 2 }}
            });

            _testCases.Add(new IntervalTestCase
            {
                Id = 2,
                InitialState = new Interval[]
                {
                    new Interval { Min = 5, Max = 6 }
                },
                IntervalToAdd = new Interval { Min = 1, Max = 2 },
                ExpectedResult = new []
                {
                    new Interval { Min = 1, Max = 2 },
                    new Interval { Min = 5, Max = 6 }
                }
            });

            _testCases.Add(new IntervalTestCase
            {
                Id = 3,
                InitialState = new Interval[]
                {
                    new Interval { Min = 1, Max = 2 },
                    new Interval { Min = 8, Max = 9 }
                },
                IntervalToAdd = new Interval { Min = 4, Max = 5 },
                ExpectedResult = new []
                {
                    new Interval { Min = 1, Max = 2 },
                    new Interval { Min = 4, Max = 5 },
                    new Interval { Min = 8, Max = 9 }
                }
            });

            _testCases.Add(new IntervalTestCase
            {
                Id = 4,
                InitialState = new Interval[]
                {
                    new Interval { Min = 1, Max = 4 },
                },
                IntervalToAdd = new Interval { Min = 4, Max = 5 },
                ExpectedResult = new []
                {
                    new Interval { Min = 1, Max = 5 }
                }
            });

            _testCases.Add(new IntervalTestCase
            {
                Id = 5,
                InitialState = new Interval[]
                {
                    new Interval { Min = 7, Max = 10 },
                },
                IntervalToAdd = new Interval { Min = 6, Max = 7 },
                ExpectedResult = new []
                {
                    new Interval { Min = 6, Max = 10 }
                }
            });

            _testCases.Add(new IntervalTestCase
            {
                Id = 6,
                InitialState = new Interval[]
                {
                    new Interval { Min = 5, Max = 10 },
                },
                IntervalToAdd = new Interval { Min = 6, Max = 7 },
                ExpectedResult = new []
                {
                    new Interval { Min = 5, Max = 10 }
                }
            });

            _testCases.Add(new IntervalTestCase
            {
                Id = 7,
                InitialState = new Interval[]
                {
                    new Interval { Min = 6, Max = 8 },
                },
                IntervalToAdd = new Interval { Min = 5, Max = 9 },
                ExpectedResult = new []
                {
                    new Interval { Min = 5, Max = 9 }
                }
            });

            _testCases.Add(new IntervalTestCase
            {
                Id = 8,
                InitialState = new Interval[]
                {
                    new Interval { Min = 2, Max = 4 },
                    new Interval { Min = 6, Max = 8 },
                },
                IntervalToAdd = new Interval { Min = 4, Max = 6 },
                ExpectedResult = new []
                {
                    new Interval { Min = 2, Max = 4 }
                }
            });
        }

        public static void Run()
        {
            Setup();

            bool success = true;
            foreach (var testcase in _testCases)
            {
                IntervalMerger merger = new IntervalMerger(testcase.InitialState);
                merger.Add(testcase.IntervalToAdd);
                
                if (merger.Intervals.Count() != testcase.ExpectedResult.Count())
                {
                    var color = ForegroundColor;
                    ForegroundColor = ConsoleColor.Red;
                    Write($"Testcase #{testcase.Id} failed! ");
                    ForegroundColor = color;
                    WriteLine($"Number of intervals mismatched, actually: {merger.Intervals.Count()}, expected: {testcase.ExpectedResult.Count()}");
                    success = false;
                }

                for (int i = 0; i < testcase.ExpectedResult.Length; i++)
                {
                    var expected = testcase.ExpectedResult.ToArray();
                    var actual = merger.Intervals.ToArray();

                    if (expected[i].Min != actual[i].Min || expected[i].Max != actual[i].Max)
                    {
                        var color = ForegroundColor;
                        ForegroundColor = ConsoleColor.Red;
                        Write($"Testcase #{testcase.Id} failed! ");
                        ForegroundColor = color;
                        WriteLine($"Interval {i} mismatched, expected: [{expected[i].Min} - {expected[i].Max}], actual: [{actual[i].Min} - {actual[i].Max}]");
                        success = false;
                    }
                }
            }

            if (success)
            {
                var color = ForegroundColor;
                ForegroundColor = ConsoleColor.Green;
                WriteLine("All test cases succeeded!");
                ForegroundColor = color;
            }
        }

        private class IntervalTestCase
        {
            public int Id;
            public Interval[] InitialState;
            public Interval[] ExpectedResult;
            public Interval IntervalToAdd;
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