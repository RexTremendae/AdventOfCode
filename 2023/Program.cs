
var start = DateTime.Now;
await new Day1().Part2();
var duration = DateTime.Now-start;
Console.WriteLine($"{duration.TotalSeconds:0.00}s");


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
