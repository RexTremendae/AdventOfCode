using static System.Console;

namespace Day25
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            CalculateIndex(2947, 3029);
        }

        public long CalculateIndex(int row, int col)
        {
            var index = 0;
            for (int x = 1; x <= col; x++)
                index += x;
            for (int y = 1; y < row; y++)
                index += y + col - 1;

            var code = 20_151_125L;
            for (int i = 0; i < index - 1; i++)
                code = CalculateNextCode(code);

            WriteLine($"{row}, {col}: {index}  =>  {code}");
            return index;
        }

        public long CalculateNextCode (long current)
        {
            return (current * 252_533) % 33_554_393;
        }
    }
}

/*

   | 1   2   3   4   5   6   7   8   9  10  11  12  13
---+---+---+---+---+---+---+---+---+---+---+---+---+---+
 1 |  1   3   6  10  15  21  28  36  45  55  66  78  91
 2 |  2   5   9  14  20  27  35  44  54  65  77  90
 3 |  4   8  13  19  26  34  43  53  64  76  89
 4 |  7  12  18  25  33  42  52  63  75  88
 5 | 11  17  24  32  41  51  62  74  87
 6 | 16  23  31  40  50  61  73  86
 7 | 22  30  39  49  60  72  85  99
 8 | 29  38  48  59  71  84  98
 9 | 37  47  58  70  83  97
10 | 46  57  69  82  96
11 | 56  68  81  95
12 | 67  80  94
13 | 79  93
14 | 92
*/