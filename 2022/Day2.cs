using static System.Console;

public class Day2
{
    public async Task Part1()
    {
        var sum = 0;
        await foreach(var line in File.ReadLinesAsync("Day2.txt"))
        {
            if (string.IsNullOrEmpty(line)) continue;

            var split = line.Split(' ');
            var _1 = split[0][0];
            var _2 = split[1][0];

            sum += Calculate(_1, _2);
        }

        WriteLine(sum);
    }

    public async Task Part2()
    {
        var sum = 0;
        await foreach(var line in File.ReadLinesAsync("Day2.txt"))
        {
            if (string.IsNullOrEmpty(line)) continue;

            var split = line.Split(' ');
            var _1 = split[0][0];
            var _2 = split[1][0];

            _2 = _2 switch
            {
                'X' => _1 switch
                {
                    'A' => 'Z',
                    'B' => 'X',
                    'C' => 'Y',
                    _ => throw new InvalidOperationException($"{_1}")
                },
                'Y' => _1 switch
                {
                    'A' => 'X',
                    'B' => 'Y',
                    'C' => 'Z',
                    _ => throw new InvalidOperationException($"{_1}")
                },
                'Z' => _1 switch
                {
                    'A' => 'Y',
                    'B' => 'Z',
                    'C' => 'X',
                    _ => throw new InvalidOperationException($"{_1}")
                },
                _ => throw new InvalidOperationException($"{_2}")
            };

            sum += Calculate(_1, _2);
        }

        WriteLine(sum);
    }

    private int Calculate(char _1, char _2)
    {
        var score = 0;
        score += _2 switch
        {
            'X' => 1,
            'Y' => 2,
            'Z' => 3,
            _ => throw new InvalidOperationException($"{_2}")
        };

        score +=
        _1 switch
        {
            'A' => _2 switch
            {
                'X' => 3,
                'Y' => 6,
                'Z' => 0,
                _ => throw new InvalidOperationException($"{_2}")
            },
            'B' => _2 switch
            {
                'X' => 0,
                'Y' => 3,
                'Z' => 6,
                _ => throw new InvalidOperationException($"{_2}")
            },
            'C' => _2 switch
            {
                'X' => 6,
                'Y' => 0,
                'Z' => 3,
                _ => throw new InvalidOperationException($"{_2}")
            },
            _ => throw new InvalidOperationException($"{_1}")
        };

        return score;
    }
}
