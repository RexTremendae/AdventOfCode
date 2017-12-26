using System;

namespace Day23
{
    public class Program
    {
        private Int64 h = 0;

        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            Improved4();
        }

        public void IncreaseH()
        {
            h++;
            Console.WriteLine($"H: {h}");
        }

        public void Improved4()
        {
            Int64 b = 7_900 + 100_000;

            for (int i = 0; i <= 1000; i ++, b += 17)
            {
                for (Int64 d = 2; d < b; d++)
                {
                    if (b % d == 0)
                    {
                        IncreaseH();
                        break;
                    }
                }
            }
        }

        public void Improved3()
        {
            Int64 b = 7_900 + 100_000;

            for (int i = 0; i < 1000; i++)
            {
                bool f = false;

                for (Int64 d = 2; d < b; d++)
                {
                    for (Int64 e = 2; e < b; e++)
                    {
                        if (d*e == b)
                        {
                            IncreaseH();
                            f = true;
                            break;
                        }
                    }

                    if (f) break;
                }

                b += 17;
            }
        }

        public void Improved2()
        {
            Int64 b = 7_900 + 100_000;

            for (int i = 0; i < 1000; i++)
            {
                bool f = false;
                Int64 d = 2;

                while (true)
                {
                    Int64 e = 2;

                    while (true)
                    {
                        if (d*e-b == 0)
                        {
                            IncreaseH();
                            f = true;
                            break;
                        }

                        e++;
                        if (e-b == 0) break;
                    }
                    if (f) break;

                    d++;
                    if (d-b == 0) break;
                }

                b += 17;
            }
        }

        public Int64 Improved1()
        {
            Int64 h = 0;
            Int64 b = 7_900 + 100_000;
            Int64 c = b + 17_000;

            while (true)
            {
                Int64 f = 1;
                Int64 d = 2;

                while (true)
                {
                    Int64 e = 2;

                    while (true)
                    {
                        Int64 g = d*e-b;

                        if (g == 0) f = 0;

                        e++;
                        if (e-b == 0) break;
                    }

                    d++;
                    if (d-b == 0) break;
                }

                if (f == 0) h++;
                if (b-c == 0) return h;

                b += 17;
            }
        }

        public Int64 Original()
        {
            Int64 h = 0;
            Int64 b = 79;
            Int64 c = b;
            b *= 100;
            b -= -100_000;
            c = b;
            c -= -17_000;
C:
            Int64 f = 1;
            Int64 d = 2;
B:
            Int64 e = 2;
A:
            Int64 g = d;
            g *= e;
            g -= b;

            if (g == 0) f = 0;

            e -= -1;
            g = e;
            g -= b;
            if (g != 0) goto A;

            d -= -1;
            g = d;
            g -= b;

            if (g != 0) goto B;
            if (f == 0) h -= -1;

            g = b;
            g -= c;
            if (g == 0) return h;

            b -= -17;
            goto C;
        }
    }
}
