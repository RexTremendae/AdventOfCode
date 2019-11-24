using static System.Console;

public class Day21
{
    public static void Main()
    {
        new Day21().Run();
    }

    public void Run()
    {
        long _1, _2, _3, _4;
        long? prev_2 = null;

        _1 = 0;
        while(true)
        {
            _2 = _1 | 65536;
            _1 = 6663054;
Lbl_8:
            _4 = _2 & 255;
            _1 += _4;
            _1 &= 16777215;
            _1 *= 65899;
            _1 &= 16777215;

            var _2_str = $"_1: {_1} _2: {_2}";
            if (prev_2 != null)
            {
                _2_str += $" Δ: {_2-prev_2.Value}";
            }
            //WriteLine(_2_str);
            prev_2 = _2;

            if (256 > _2) goto Lbl_28;
            _4 = 0;
Lbl_18:
            _3 = _4+1;
            _3 *= 256;
            if (_3 > _2)
            {
                _2 = _4;
                goto Lbl_8;
            }
            _4++;
            goto Lbl_18;

Lbl_28:
            WriteLine($"_1: {_1}");
            return;
        }
    }
}
