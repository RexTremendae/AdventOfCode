using System;
using System.Linq;
using System.Collections.Generic;
using static System.Console;

namespace Day16
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            /*
    03036732577212944063491565474664 becomes 84462026.
    02935109699940807407585447034323 becomes 78725270.
    03081770884921959731165446850517 becomes 53553731.
            */
            var rawInput =
            // Puzzle input
//            "59764635797473718052486376718142408346357676818478503599633670059885748195966091103097769012608550645686932996546030476521264521211192035231303791868456877717957482002303790897587593845163033589025995509264282936119874431944634114034231860653524971772670684133884675724918425789232716494769777580613065860450960426147822968107966020797566015799032373298777368974345143861776639554900206816815180398947497976797052359051851907518938864559670396616664893641990595511306542705720282494028966984911349389079744726360038030937356245125498836945495984280140199805250151145858084911362487953389949062108285035318964376799823425466027816115616249496434133896";
                "03036732577212944063491565474664"
            ;
            // Try me!
            //rawInput = rawInput + rawInput;

            var messageOffset = int.Parse(rawInput[..7]);
            var phasesToDisplayOffset = 90;
            var phasesToDisplay = 10;

            var calculatedPhases = new List<string>();

/*
            var nextPhase = rawInput;
            for (int i = 0; i < phasesToDisplay + phasesToDisplayOffset; i++)
            {
                nextPhase = CalculatePhases(nextPhase, 1);
                calculatedPhases.Add(Collect(nextPhase.Reverse()));
            }
*/

            var input = rawInput.ToArray().Select(i => (int)(i - '0')).Reverse().ToArray();
            var result = new List<int>();
            WriteLine($"Input: {rawInput}");
            WriteLine();

            var toCalcCount = input.Length*10_000L - messageOffset - 1;

            WriteLine($"10k input length: {(input.Length*10_000L).ToString("N0").PadLeft(7)}");
            WriteLine($"Message offset:   {messageOffset.ToString("N0").PadLeft(7)}");
            WriteLine($"To calc count:    {toCalcCount.ToString("N0").PadLeft(7)}");

            result.Add(input.First());
            for (int i = 0; i < toCalcCount; i++)
            {
                result.Add(MAdd(result[i % input.Length], input[(i+1) % input.Length]));
            }

            for (var phase = 1; phase < phasesToDisplayOffset + phasesToDisplay; phase++)
            {
                result[1] = MAdd(result[1], input[0]);
                for (int i = 1; i < toCalcCount; i++)
                    result[i+1] = MAdd(result[i+1], result[i]);
                if (phase >= phasesToDisplayOffset)
                {
                    //Validate(string.Join("", result.Select(r => r.ToString())), calculatedPhases[phase]);
                }
            }

            WriteLine(Collect(result.Skip((int)(toCalcCount - 7)).Reverse()));
/*
            WriteLine();

            for (int cph = phasesToDisplayOffset; cph < calculatedPhases.Count; cph++)
            {
                if (cph < phasesToDisplayOffset + phasesToDisplay)
                    ForegroundColor = ConsoleColor.Cyan;
                Write(calculatedPhases[cph][..(result.Count)]);
                ResetColor();
                WriteLine(calculatedPhases[cph][(result.Count)..]);
            }
*/


        }

        string Collect<T>(IEnumerable<T> input)
        {
            return string.Join("", input.Select(i => i.ToString()));
        }

        void Validate(string guess, string correct)
        {
            for (int i = 0; i < guess.Length; i++)
            {
                ForegroundColor = (i >= correct.Length || guess[i] != correct[i])
                    ? ConsoleColor.Red
                    : ConsoleColor.Green;
                Write(guess[i]);
            }
            WriteLine();
            ResetColor();
        }

        int MAdd(params int[] input)
        {
            return input.Sum() % 10;
        }

        void FindRepetitions(string inputSignal)
        {
            List<int> sums = new();
            var index = new Dictionary<int, List<int>>();
            for (int i = 0; i < inputSignal.Length; i++)
            {
                var sum = 0;
                for (int j = 0; j < inputSignal.Length; j++)
                {
                    var digit = inputSignal[j];
                    var value = ((int)digit - '0') * GetPattern(i, j);
                    sums.Add(value);
                    if (!index.TryGetValue(value, out var list))
                    {
                        list = new();
                        index.Add(value, list);
                    }
                    list.Add(j);
                    sum += value;
                }

                for (int key = -2; key <= 2; key++)
                {
                    var value = index[key];
                    var prev = string.Join(", ", value.Select(l => l.ToString()));
                    WriteLine($"{key}: {prev}");
                }

                //nextSignal += sum.ToString().Last();
            }
        }

        string CalculatePhases(string inputSignal, int phaseCount)
        {
            var nextSignal = inputSignal;

            for (int i = 0; i < phaseCount; i++)
            {
                nextSignal = GenerateNext(nextSignal);
                //WriteLine(nextSignal);
            }

            return nextSignal;
        }

        int GetPattern(int i, int j)
        {
            i++;
            j++;

            var q = j / i;

            if (q % 2 == 0)
            {
                return 0;
            }

            if (((q-1) / 2) % 2 == 1)
            {
                return -1;
            }

            return 1;
        }

        string GenerateNext(string inputSignal)
        {
            var nextSignal = "";

            for (int i = 0; i < inputSignal.Length; i++)
            {
                var sum = 0;
                for (int j = 0; j < inputSignal.Length; j++)
                {
                    var digit = inputSignal[j];
                    sum += ((int)digit - '0') * GetPattern(i, j);
                }

                nextSignal += sum.ToString().Last();
            }

            return nextSignal;
        }
    }
}
