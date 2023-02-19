using static System.Console;

public class Day24
{
    public void Part1()
    {
        var monadCode = File.ReadAllLines("Day24.txt").ToList();
        PrintCode(monadCode);

        Clock.Tick();

        for (int t = 9; t > 0; t--)
        for (int u = 9; u > 0; u--)
        for (int v = 9; v > 0; v--)
        for (int w = 9; w > 0; w--)
        for (int x = 9; x > 0; x--)
        for (int y = 9; y > 0; y--)
        for (int z = 9; z > 0; z--)
        {
            var modelNumber = $"9999{t}{u}99{v}{w}9{x}{y}{z}";
            var valid = Validate(modelNumber);
            if (valid)
            {
                WriteLine($"{modelNumber}: {valid}");
            }
        }

        WriteLine($"{Clock.Tock().TotalSeconds:0.00} s");
    }

    public bool Validate(string input)
    {
        if (input.Contains('0')) return false;

        long w = 0;
        long x = 0;
        long y = 0;
        long z = 0;

        var __4 = new[] {  1,  1,  1,  1,  26,  26,  1,  1, 26, 26,  1, 26,  26, 26 };
        var __5 = new[] { 13, 12, 12, 10, -11, -13, 15, 10, -2, -6, 14,  0, -15, -4 };
        var _15 = new[] {  8, 13,  8, 10,  12,   1, 13,  5, 10,  3,  2,  2,  12,  7 };

        for (int i = 0; i < 14; i ++)
        {
            // 0: inp w
            w = input[i] - '0';

            // 1 + 2 + 5
            x = z % 26 + __5[i];

            // 4
            z = z / __4[i];
            // / (1 | 26)

            // 6 + 7
            x = (x == w) ? 0 : 1;

            // 8 + 9 + 10 + 11 + 12
            z = (25*x + 1) * z;
            // * (1 | 26)

            // 13 + 14 + 15 + 16
            y = (w + _15[i]) * x;
            // = ([1-9] + [1-13]) * (0 | 1)

            // 17
            z += y;
        }

        return z == 0;
    }

    private void PrintCode(List<string> code)
    {
        int count = 0;
        var maxLen = 0;

        foreach (var line in code)
        {
            maxLen = Math.Max(maxLen, line.Length);
            if (line.StartsWith("inp"))
            {
                count = 0;
            }
            count++;
        }

        for (int rw = 0; rw < 18; rw++)
        {
            var last = "";
            var hdr = "";
            for (int cl = 0; cl < 14; cl++)
            {
                var current = code[18*cl+rw].PadRight(maxLen + 5);
                if (hdr == "")
                {
                    hdr = current[..3];
                    Write($"{rw.ToString().PadLeft(2)}  {hdr}  |  ");
                }
                else if (hdr != current[..3])
                {
                    throw new InvalidOperationException();
                }

                if (cl > 0 && last != current)
                {
                    ForegroundColor = ConsoleColor.Green;
                }

                Write(current[4..]);
                ResetColor();
                last = current;
            }
            WriteLine();
        }
    }
}
