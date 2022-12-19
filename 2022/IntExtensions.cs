public static class IntExtensions
{
    public static IEnumerable<int> To(this int start, int end)
    {
        var increment = 1;

        if (end < start)
        {
            increment = -1;
        }

        for (int i = start; i != end; i += increment) yield return i;
    }
}
