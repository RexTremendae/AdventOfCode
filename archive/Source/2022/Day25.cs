public class Day25
{
    public void Part1()
    {
        var total = 0L;
        foreach (var snafu in PuzzleInput)
        {
            var dec = ToDecimal(snafu);
            var snafu2 = ToSnafu(dec);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(snafu + "  ");
            Console.ResetColor();
            Console.Write(dec + "  ");
            Console.ForegroundColor = snafu == snafu2 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(snafu2);
            Console.ResetColor();
            total += dec;
        }

        Console.WriteLine();

        Console.Write("Decimal total: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(total.ToString("### ### ### ### ##0"));
        Console.ResetColor();

        Console.Write("SNAFU total: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(ToSnafu(total));
        Console.ResetColor();

        Console.WriteLine();
    }

    private string ToSnafu(long dec)
    {
        var result = new List<char>();
        var memory = 0;

        while (dec > 0)
        {
            var current = (char)('0' + (dec % 5)) + memory;

            dec /= 5;
            if (current == '3')
            {
                result.Add('=');
                memory = 1;
            }
            else if (current == '4')
            {
                result.Add('-');
                memory = 1;
            }
            else if (current == '5')
            {
                result.Add('0');
                memory = 1;
            }
            else
            {
                result.Add((char)(current));
                memory = 0;
            }
        }

        if (memory == 1) { result.Add('1'); }
        return string.Join("", result.AsEnumerable().Reverse());
    }

    private long ToDecimal(string snafu)
    {
        var dec = 0L;

        var value = 1L;
        foreach (var ch in snafu.Reverse())
        {
            dec += value*(ch switch
            {
                '=' => -2,
                '-' => -1,
                '0' => 0,
                '1' => 1,
                '2' => 2
            });

            value *= 5;
        }

        return dec;
    }

    public static readonly string[] SnafuExamples = new [] {
        "1",             //         1
        "2",             //         2
        "1=",            //         3
        "1-",            //         4
        "10",            //         5
        "11",            //         6
        "12",            //         7
        "2=",            //         8
        "2-",            //         9
        "20",            //        10
        "21",            //        11
        "22",            //        12
        "1==",           //        13
        "1=-",           //        14
        "1=0",           //        15
        "1=1",           //        16
        "1=2",           //        17
        "1-=",           //        18
        "1--",           //        19
        "1-0",           //        20
        "1-1",           //        21
        "1-2",           //        22
        "1=11-2",        //      2022
        "1-0---0",       //     12345
        "1121-1110-1=0", // 314159265
    };

    public static readonly string[] ExampleInput = new[] {
        "1=-0-2", // 1747
        "12111",  //  906
        "2=0=",   //  198
        "21",     //   11
        "2=01",   //  201
        "111",    //   31
        "20012",  // 1257
        "112",    //   32
        "1=-1=",  //  353
        "1-12",   //  107
        "12",     //    7
        "1=",     //    3
        "122",    //   37
    };

    public static readonly string[] PuzzleInput = new[] {
        "2==-1222101=0=",
        "12200",
        "1=1",
        "1=20-212",
        "100",
        "1202=0==111--2",
        "101-0--0-1",
        "1=0--=2=1--",
        "10=0=02",
        "1102-1",
        "2110",
        "20-12-",
        "1110=-12-0=-0-01-0",
        "1-11===212=102---",
        "1=22=00201122-1",
        "10=02=11-212=-0=00",
        "2020222-211=202",
        "12=0-00",
        "1=2--2--01--=-1010",
        "1=011--0=002",
        "1-212--=-",
        "1020==01--1---0",
        "11==0==2-2=011-=",
        "20=000-1--=-00",
        "2202=21-1",
        "2--10=12--",
        "1000",
        "111022",
        "2-022",
        "1=20==01=2-=",
        "1-10=--",
        "2=2=21-=11-1--===20",
        "1==02100",
        "1-=0",
        "2=021-0122=",
        "1-0==",
        "1220-0-0=2",
        "1=0-0=2-=1",
        "10=112201=102",
        "12=0-2-112",
        "1==22102-2",
        "112-0=02=100",
        "1-020-",
        "1001-20-",
        "20-2=1=0110",
        "10=-==-",
        "1-12",
        "20-10==-",
        "1=1111022202=2101",
        "1==1=1-=",
        "10100",
        "2=21--1=11=",
        "22002-021=02",
        "22",
        "1=101-0010111---2===",
        "1==20",
        "1=2211001=-2001=-0",
        "210==-=222-1",
        "1=2-0=0",
        "1-=12-2",
        "20-1-0=---",
        "220==121=",
        "1-1110=2=-0-",
        "1012-01=0002",
        "102=121-0-002==-",
        "2200=2-120212",
        "1=1=22--2",
        "1=2=220-0-22=01",
        "2101-10-1-21",
        "2-0-1=--2==-01-2",
        "1000-=0210=--",
        "1==11=-=1=0-1",
        "1=0020-12-=-",
        "1-220--1=-10-202-",
        "1=-1===-2==02==-=21",
        "21",
        "2=2==-0-=2=-1-11=",
        "1-0==20011201",
        "1-100=",
        "21=212-022=2-",
        "2-10110=-0-2=1=-0=",
        "1012",
        "1==1==",
        "1-11=0",
        "1-20-0=-21111202==",
        "1=0=012=-=2=10-",
        "1=2=1",
        "10-0",
        "100-22==02",
        "1===121-0=0===1-2=2",
        "1-=-1-10222=12-",
        "1==-0--0=2=2-2",
        "10=2=11220021-=",
        "211",
        "202-0=11-===",
        "1==0",
        "1--0221-0",
        "1=-=000-2=101-0",
        "1-",
        "1=02=2",
        "1==",
        "1---==1--",
        "20=0",
        "22-=1-011=2",
        "12-112=",
        "102==-=-021021=-=-2",
        "1--2=2=100=20",
        "12--10=0=2==2==2",
        "1221-=2112-211",
        "1222=12-110",
        "2=--11",
        "2-02",
        "20222=-1222==",
        "20-==-",
        "2=-021",
        "111-110-1=",
        "10200=2010110",
        "21==0=",
        "101",
        "1=-=-=1=1",
        "21002010-20=-",
        "1-122=02=2=2022=1",
        "112--=221",
        "1210",
        "2=102=01102221-2",
        "1=-1102111",
        "2=-",
        "2110=0==2-11001",
        "1-00021100=-011",
        "11",
        "1=-",
        "10000=00-=2=1-0",
        "10=",
    };
}
