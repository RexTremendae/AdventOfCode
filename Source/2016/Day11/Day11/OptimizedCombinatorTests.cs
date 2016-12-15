using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Day11
{
    public class OptimizedCombinatorTests
    {
        [Fact]
        public void Test1()
        {
            State state = new State(new string[][]
            {
                new[] { "E", "AM", "AG" }
            });

            var combinations = new OptimizedCombinator().GetCombinations(state, 0);

            combinations.ShouldBe(new int[][]
            {
                new[] { 1 },
                new[] { 2 },
                new[] { 1, 2 },
            });
        }

        [Fact]
        public void Test2()
        {
            State state = new State(new string[][]
            {
                new[] { "E", "AM", "AG", "BM", "BG" }
            });

            var combinations = new OptimizedCombinator().GetCombinations(state, 0);

            combinations.ShouldBe(new int[][]
            {
                new[] { 1 },
                new[] { 2 },
                new[] { 1, 2 },
                new[] { 1, 3 },
                new[] { 2, 4 }
            });
        }

        [Fact]
        public void Test3()
        {
            State state = new State(new string[][]
            {
                new[] { "",  "",   "",   "",   "BG" },
                new[] { "E", "AM", "AG", "BM", "" }
            });

            var combinations = new OptimizedCombinator().GetCombinations(state, 1);

            combinations.ShouldBe(new int[][]
            {
                new[] { 1 },
                new[] { 1, 2 },
                new[] { 1, 3 },
                new[] { 2 }
            });
        }

        [Fact(Skip = "Why is this not working?")]
        public void Test4()
        {
            State state = new State(new string[][]
            {
                new[] { "",  "",   "AG", "",   "BG" },
                new[] { "E", "AM", "",   "BM", "" }
            });

            var combinations = new OptimizedCombinator().GetCombinations(state, 1);

            combinations.ShouldBe(new int[][]
            {
                new[] { 1 },
                new[] { 1, 3 },
            });
        }

        [Fact]
        public void Test5()
        {
            State state = new State(new string[][]
            {
                new[] { "",  "",   "AG", "",   "" },
                new[] { "",  "",   "",   "",   "BG" },
                new[] { "E", "AM", "",   "BM", "" }
            });

            var combinations = new OptimizedCombinator().GetCombinations(state, 2);

            combinations.ShouldBe(new int[][]
            {
                new[] { 1 },
                new[] { 3 },
                new[] { 1, 3 },
            });
        }

        [Fact]
        public void Test6()
        {
            State state = new State(new string[][]
            {
                new[] { "",  "AM", "",   "",   "BG" },
                new[] { "E", "",   "AG", "BM", "" }
            });

            var combinations = new OptimizedCombinator().GetCombinations(state, 1);

            combinations.ShouldBe(new int[][]
            {
                new[] { 2 },
                new[] { 3 },
                new[] { 2, 3 }
            });
        }

        [Fact]
        public void Test7()
        {
            State state = new State(new string[][]
            {
                new[] { "",  "AM", "",   "",   "" },
                new[] { "",  "",   "",   "",   "BG" },
                new[] { "E", "",   "AG", "BM", "" }
            });

            var combinations = new OptimizedCombinator().GetCombinations(state, 2);

            combinations.ShouldBe(new int[][]
            {
                new[] { 2 },
                new[] { 3 },
                new[] { 2, 3 }
            });
        }
    }

    public static class TestExtensions
    {
        public static void ShouldBe(this IEnumerable<int[]> actual, int[][] expected)
        {
            actual.Count().ShouldBe(expected.Count());

            foreach (int[] expectedIntArray in expected)
            {
                bool found = false;

                foreach (int[] actualIntArray in actual)
                {
                    if (expectedIntArray.Count() != actualIntArray.Count())
                        continue;

                    bool currentMatch = true;
                    for (int i = 0; i < expectedIntArray.Count(); i++)
                        if (actualIntArray[i] != expectedIntArray[i])
                        {
                            currentMatch = false;
                        }

                    if (currentMatch)
                        found = true;
                }

                string expectedStringMessage = string.Empty;
                foreach (var exp in expectedIntArray)
                    expectedStringMessage += exp + ", ";

                expectedStringMessage = expectedStringMessage.Substring(0, expectedStringMessage.Length - 2);
                found.ShouldBe(true, $"Expected data [{expectedStringMessage}] not found.");
            }
        }
    }
}
