
var start = DateTime.Now;
await new Day4().Part2();
var duration = DateTime.Now-start;

ColorWriter.WriteLine("-------", ConsoleColor.White);
ColorWriter.WriteLine($" {duration.TotalSeconds:0.00}s", ConsoleColor.Magenta);
ColorWriter.WriteLine("-------", ConsoleColor.White);

public static class ColorWriter
{
    public static void Write(object? data, ConsoleColor? color = null)
    {
        if (color != null) Console.ForegroundColor = color.Value;
        Console.Write(data);
        if (color != null) Console.ResetColor();
    }

    public static void WriteLine(object? data, ConsoleColor? color = null)
    {
        if (color != null) Console.ForegroundColor = color.Value;
        Console.WriteLine(data);
        if (color != null) Console.ResetColor();
    }
}

public static class IntExtensions
{
    public static IEnumerable<int> To(this int from, int to)
    {
        if (to > from)
        {
            for (int i = from; i < to; i++)
                yield return i;
        }
        else
        {
            for (int i = from; i > to; i--)
                yield return i;
        }
    }
}

public static class StringExtensions
{
    public static int FindFirst(this string data, string toFind)
    {
        foreach (var d in 0.To(data.Length))
        {
            if (d+toFind.Length < data.Length && data[d..(d+toFind.Length)] == toFind)
            {
                return d;
            }
        }

        return -1;
    }

    public static int FindLast(this string data, string toFind)
    {
        foreach (var d in (data.Length-toFind.Length).To(-1))
        {
            if (d >= 0 && data[d..(d+toFind.Length)] == toFind)
            {
                return d;
            }
        }

        return -1;
    }
}
