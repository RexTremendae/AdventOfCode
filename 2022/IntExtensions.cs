public static class IntExtensions
{
    private static readonly string[] inclusive = new[] { "]", "<=" };
    private static readonly string[] exclusive = new[] { "[", "<" };
    private static readonly IEnumerable<string> allIntervalDefinitions = inclusive.Concat(exclusive);

    public static IEnumerable<int> To(this int start, int end, char inclusiveOrExclusive)
    {
        return To(start, end, inclusiveOrExclusive.ToString());
    }

    public static IEnumerable<int> To(this int start, int end, string inclusiveOrExclusive = "[")
    {
        if (!(allIntervalDefinitions.Contains(inclusiveOrExclusive)))
        {
            throw new ArgumentException(message: "", paramName: nameof(inclusiveOrExclusive));
        }

        if (end < start)
        {
            throw new ArgumentException(message: $"End ({end}) cannot be smaller than ({start})", paramName: nameof(end));
        }

        return Enumerable.Range(start, (new[] { "[", "<" }.Contains(inclusiveOrExclusive)

            // [  <
            ? end-start

            // ]  <=
            : end-start + 1
        ));
    }
}
