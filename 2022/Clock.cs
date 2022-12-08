public static class Clock
{
    private static DateTime _timestamp = default;

    public static void Tick()
    {
        _timestamp = DateTime.Now;
    }

    public static TimeSpan Tock()
    {
        var now = DateTime.Now;
        var duration = now - _timestamp;
        _timestamp = now;
        return duration;
    }
}