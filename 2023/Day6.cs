using static ColorWriter;

public class Day6
{
    public Task Part1()
    {
        // Example
        //var time     = new[] { 7, 15,  30 };
        //var distance = new[] { 9, 40, 200 };

        // Puzzle input
        var time     = new[] {  55,   99,   97,   93 };
        var distance = new[] { 401, 1485, 2274, 1405 };

        var sum = 1L;

        foreach (var i in 0.To(time.Length))
        {
            var min = 1;
            for (; ; min++)
            {
                var d = (time[i] - min) * min;
                if (d > distance[i]) break;
            }
            Write($"{i}: {min} - ");

            var max = time[i] - 1;
            for (; ; max--)
            {
                var d = (time[i] - max) * max;
                if (d > distance[i]) break;
            }
            WriteLine(max);

            sum *= (max-min+1);
        }

        WriteLine(sum);

        return Task.CompletedTask;
    }
}
