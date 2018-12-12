using Shouldly;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using static System.Console;

namespace Day12
{
    public class Program
    {
        public static void Main(string[] args)
        {
            long iterationCount = 20;
            if (args.Length > 0) long.TryParse(args[0].Replace("_", ""), out iterationCount);
            new Program().Run(iterationCount, print: false, warn: false);
        }

        public void Run(long target, bool print, bool warn)
        {
            var data = ParseInput("Day12.txt");
            var currentState = data.initialState;
            long total = SumState(currentState, 0);
            int offset = 0;
            long lastSum = 0;
            long lastOffset = offset;

            var visited = new Dictionary<string, List<long>>();
            visited.Add(currentState, new List<long> { 0 });
            long it = 1;

            long iterationCount = 200;
            //for (; it <= iterationCount; it++)
            for (; it <= iterationCount; it++)
            {
                if (print)
                {
                    WriteLine($"[{it}]");
                    WriteLine($"Surround.Input: {currentState}  [{offset}]");
                }
                var (newStateAsArray, newOffset) = SurroundWithEmpty(currentState, offset);
                currentState = newStateAsArray.CollectString();
                offset = newOffset;
                if (print) WriteLine($"Middlegrounds: {currentState}  [{offset}]");
                (currentState, offset) = CalculateNextState(currentState, offset, data.mutations);
                var sum = SumState(currentState, offset);
                if (print) WriteLine($"Calculate.Output: {currentState}  [{offset}] => {sum}");
                lastSum = sum;
                total += sum;
                if (print) WriteLine();

                int i = 0;
                while (currentState[i] == '.') { offset++; i++; }
                currentState = currentState.Trim('.');

                if (!visited.TryGetValue(currentState, out var iterationList))
                {
                    iterationList = new List<long>();
                    visited.Add(currentState, iterationList);
                }
                iterationList.Add(it);

                if (warn) AssertOffset(offset, lastOffset, it);
                lastOffset = offset;

                if (it >= iterationCount - 10)
                {
                    WriteLine($"#{it-1}   Sum: {lastSum}   Offset: {offset}");
                }
            }

            WriteLine("(etc...)");
            WriteLine();

            var repeatingState = "";

            // Seems to peek at iteration #90 different configurations,
            // (offset disregarded) where only one repeats infinitely.
            foreach (var kvp in visited.Where(x => x.Value.Count > 1))
            {
                WriteLine($"[count] {kvp.Key}: {kvp.Value.Count}");
                repeatingState = kvp.Key;
            }

            WriteLine();
            // Right answer off by one, sending in 49_999_999_999 solved the problem.
            // Strange, this number seemed to work with lower values...
            WriteLine(SumState(repeatingState, target - 87));
        }

        // Offset seems to stabilize at iteration #90
        public void AssertOffset(long offset, long lastOffset, long it)
        {
            if (it % 2 == 0)
            {
                if (offset != lastOffset + 1)
                {
                    WriteLine($"Warning! #{it} Last offset: {lastOffset}, offset: {offset}");
                }
            }
            else
            {
                if (offset != lastOffset + 1)
                {
                    WriteLine($"Warning! #{it} Last offset: {lastOffset}, offset: {offset}");
                }
            }
        }

        public long SumState(string state, long offset)
        {
            long total = 0;
            var stateAsArray = state.ToCharArray();
            for (int i = 0; i < stateAsArray.Length; i++)
            {
                if (stateAsArray[i] == '#')
                {
                    total += i + offset;
                }
            }

            return total;
        }

        public (string state, int offset) CalculateNextState(string state, int offset, Dictionary<string, char> mutations)
        {
            var newStateAsArray = state.ToList();
            var inputStateAsArray = newStateAsArray.Clone();

            for (int i = 2; i < inputStateAsArray.Count - 2; i++)
            {
                newStateAsArray[i] = mutations[inputStateAsArray.CollectSubstring(i-2, 5)];
            }

            return (newStateAsArray.CollectString(), offset);
        }

        public (List<char> state, int offset) SurroundWithEmpty(string state, int offset)
        {
            var stateAsArray = state.ToList<char>();

            for (int i = 0; i < 4; i++)
            {
                if (stateAsArray[i] == '#')
                {
                    for (int j = 0; j < 4-i; j++)
                    {
                        offset--;
                        stateAsArray.Insert(0, '.');
                    }
                    break;
                }
            }

            var len = stateAsArray.Count;
            for (int i = 0; i < 4; i++)
            {
                if (stateAsArray[len-1 - i] == '#')
                {
                    for (int j = 0; j < 4-i; j++)
                        stateAsArray.Add('.');
                    break;
                }
            }

            return (stateAsArray, offset);
        }

        public (string initialState, Dictionary<string, char> mutations) ParseInput(string filename)
        {
            var allLines = File.ReadAllLines(filename);
            var mutations = new Dictionary<string, char>();

            for (int i = 0; i < 32; i++)
            {
                var key = "";
                for (int bit = 1; bit <= 16; bit <<= 1)
                {
                    key += (bit & i) == 0 ? '.' : '#';
                }
                mutations.Add(key, '.');
            }

            var initialState = allLines.First().Split(':')[1].Trim();

            foreach (var line in allLines.Skip(2))
            {
                var split = line.Split(' ').Select(x => x.Trim()).ToList();
                mutations[split[0]] = split[2][0];
            }

            return (initialState, mutations);
        }
    }

    public static class Extensions
    {
        public static T SecondLast<T>(this IEnumerable<T> source)
        {
            var asList = source.ToList<T>();
            return asList[asList.Count - 2];
        }

        public static T Second<T>(this IEnumerable<T> source)
        {
            return source.Skip(1).First();
        }

        public static string CollectString(this List<char> source)
        {
            return source.CollectSubstring(0, source.Count);
        }

        public static string CollectSubstring(this List<char> source, int start, int length)
        {
            var substring = "";

            for (int i = start; i < start + length; i++)
            {
                substring += source[i];
            }

            return substring;
        }

        public static List<char> Clone(this List<char> source)
        {
            var newList = new List<char>();
            foreach (var c in source) newList.Add(c);

            return newList;
        }
    }

    public class Tests
    {
        [Theory]
        [InlineData(new char[] {}, "")]
        [InlineData(new[] {'.', '.'}, "..")]
        [InlineData(new[] {'#', '.', '#'}, "#.#")]
        public void CollectString(char[] input, string expectedOutput)
        {
            input.ToList().CollectString().ShouldBe(expectedOutput);
        }

        [Theory]
        [InlineData(new[] {'.', '.', '#', '#'}, 1, 2, ".#")]
        [InlineData(new[] {'#', '.', '#'}, 0, 1, "#")]
        [InlineData(new[] {'#', '.', '#'}, 0, 3, "#.#")]
        [InlineData(new[] {'.', '.', '#'}, 2, 1, "#")]
        public void CollectSubstring(char[] input, int start, int length, string expectedOutput)
        {
            input.ToList().CollectSubstring(start, length).ShouldBe(expectedOutput);
        }

        [Theory]
        [InlineData("#..#.#..##......###...###", 0, "....#..#.#..##......###...###....", -4)]
        [InlineData("....#...#....#.....#..#..#..#....", -4, "....#...#....#.....#..#..#..#....", -4)]
        public void SurroundsWith(string inputState, int inputOffset, string expectedState, int expectedOffset)
        {
            var result = new Program().SurroundWithEmpty(inputState, inputOffset);
            result.state.CollectString().ShouldBe(expectedState);
            result.offset.ShouldBe(expectedOffset);
        }

        [Theory]
        [InlineData("....#..#.#..##......###...###....", -4, "....#...#....#.....#..#..#..#....", -4)]
        [InlineData("....#...#....#.....#..#..#..#....", -4, "....##..##...##....#..#..#..##...", -4)]
        public void CalculateNextStateSmall(string inputState, int inputOffset, string expectedState, int expectedOffset)
        {
            var mutations = new Dictionary<string, char>()
            {
                {"...##", '#'},
                {"..#..", '#'},
                {".#...", '#'},
                {".#.#.", '#'},
                {".#.##", '#'},
                {".##..", '#'},
                {".####", '#'},
                {"#.#.#", '#'},
                {"#.###", '#'},
                {"##.#.", '#'},
                {"##.##", '#'},
                {"###..", '#'},
                {"###.#", '#'},
                {"####.", '#'},

                {"#..#.", '.'},
                {"##...", '.'},
                {"#....", '.'},
                {"#...#", '.'},
                {"...#.", '.'},
                {".#..#", '.'},
                {".....", '.'},
                {"#.##.", '.'},
                {"#.#..", '.'},
                {"##..#", '.'},
                {"..#.#", '.'},
                {"....#", '.'},
                {"#..##", '.'},
                {"..###", '.'},
                {"#####", '.'},
                {".##.#", '.'},
                {".###.", '.'},
                {"..##.", '.'},
            };

            var result = new Program().CalculateNextState(inputState, inputOffset, mutations);
            result.offset.ShouldBe(expectedOffset);
            result.state.ShouldBe(expectedState);
        }

        [Theory]
        [InlineData("....#...#....#.....#..#..#..#....", -4, 91)]
        [InlineData("....##..##...##....#..#..#..##...", -4, 132)]
        public void SumState(string inputState, int inputOffset, int expectedSum)
        {
            var result = new Program().SumState(inputState, inputOffset);
            result.ShouldBe(expectedSum);
        }
    }
}
