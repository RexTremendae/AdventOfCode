using static System.Console;

public class Day20
{
    public async Task Part1()
    {
        var numbers = new List<long>();
        foreach (var line in await System.IO.File.ReadAllLinesAsync("Day20.txt"))
        {
            if (string.IsNullOrEmpty(line)) continue;
            numbers.Add(long.Parse(line));
        }
 
        int idx;
        var decrypted = numbers.ToList();
        foreach (var n in numbers)
        {
            //WriteLine(string.Join(" ", decrypted));
            if (n == 0) 
            {
                continue;
            }
            idx = decrypted.FindIndex(_ => _ == n);
            decrypted.RemoveAt(idx);
            var nn = n;
            while (nn <= 0) nn += decrypted.Count;
            idx += (int)(nn % decrypted.Count);
            if (idx > decrypted.Count) idx -= decrypted.Count;
            decrypted.Insert(idx, n);
        }
        //WriteLine(string.Join(" ", decrypted));

        idx = decrypted.FindIndex(_ => _ == 0);
        var sum = 0L;
        sum += decrypted[((idx + 1000) % decrypted.Count)];
        sum += decrypted[((idx + 2000) % decrypted.Count)];
        sum += decrypted[((idx + 3000) % decrypted.Count)];

        // Final arrangement in example:
        // 1, 2, -3, 4, 0, 3, -2

        WriteLine(sum);
    }
}
