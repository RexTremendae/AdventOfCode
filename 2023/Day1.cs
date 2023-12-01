using static System.Console;

public class Day1
{
    public static readonly string Numbers = "0123456789";
    public static readonly (string spelled, char character)[] SpelledNumbers = [
        ("zero",  '0'),
        ("one",   '1'),
        ("two",   '2'),
        ("three", '3'),
        ("four",  '4'),
        ("five",  '5'),
        ("six",   '6'),
        ("seven", '7'),
        ("eight", '8'),
        ("nine",  '9'),
    ];

    public async Task Part1()
    {
        var sum = 0;

        foreach (var data in (await ReadData("Day1.txt")).Select(_ => _.ToLower()))
        {
            var number = "";

            for (int i = 0; i < data.Length; i ++)
            {
                if (Numbers.Contains(data[i]))
                {
                    number += data[i];
                    break;
                }
            }

            for (int i = data.Length-1; i >= 0; i --)
            {
                if (Numbers.Contains(data[i]))
                {
                    number += data[i];
                    break;
                }
            }

            sum += int.Parse(number);
        }

        WriteLine(sum);
    }

    public async Task Part2()
    {
        var sum = 0;

        foreach (var data in (await ReadData("Day1.txt")).Select(_ => _.ToLower()))
        {
            var number = "";
            var candidateIdx = -1;
            var candidate = ' ';

            // ---- First ----
            for (int i = 0; i < data.Length; i ++)
            {
                if (Numbers.Contains(data[i]))
                {
                    candidateIdx = i;
                    candidate = data[i];
                    break;
                }
            }

            foreach (var (spelled, character) in SpelledNumbers)
            {
                var spelledIdx = data.FindFirst(spelled);
                if (spelledIdx >= 0 && (spelledIdx < candidateIdx || candidateIdx < 0))
                {
                    candidateIdx = spelledIdx;
                    candidate = character;
                }
            }

            number += candidate;
            candidate = ' ';
            candidateIdx = -1;

            // ---- Last ----
            for (int i = data.Length-1; i >= 0; i --)
            {
                if (Numbers.Contains(data[i]))
                {
                    candidateIdx = i;
                    candidate = data[i];
                    break;
                }
            }

            foreach (var (spelled, character) in SpelledNumbers)
            {
                var spelledIdx = data.FindLast(spelled);
                if (spelledIdx > candidateIdx)
                {
                    candidateIdx = spelledIdx;
                    candidate = character;
                }
            }

            number += (candidate == ' ' ? number : candidate);

            sum += int.Parse(number);
        }

        WriteLine(sum);
    }

    private Task<string[]> ReadData(string filename)
    {
        return File.ReadAllLinesAsync(filename);
    }
}