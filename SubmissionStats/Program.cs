using static System.Console;

var timestamps = new List<(int year, int day, int part, DateTime timestamp)>();

var ending = "json";

// Input data files are content received from (one file per year, named "yyyy.json"):
// https://adventofcode.com/{year}/leaderboard/private/view/{userId}.json
foreach (var filename in Directory.GetFiles(".", $"*.{ending}"))
{
    var fullPathFilename = Path.GetFullPath(filename);
    var currentYear = int.Parse(
        fullPathFilename[..^$".{ending}".Length]
        .Split(Path.DirectorySeparatorChar)
        .Last()
    );

    var lines = File
        .ReadAllLines($"{currentYear}.json")
        .Select(l => l.Trim())
        .ToArray();

    var insideCompletionDay = false;
    int? currentDay = null;
    int? currentPart = null;

    for (int i = 0; i < lines.Length; i++)
    {
        if (lines[i].StartsWith("\"completion_day_level\":"))
        {
            insideCompletionDay = true;
            continue;
        }

        if (!insideCompletionDay) continue;

        var curr = lines[i];

        if (curr.StartsWith("}"))
        {
            if (currentPart != null) currentPart = null;
            else if (currentDay != null) currentDay = null;
            else insideCompletionDay = false;
            continue;
        }

        if (curr.EndsWith('{'))
        {
            var secondCitationIdx = curr[1..].IndexOf('\"');
            var data = curr[1..(secondCitationIdx+1)];
            if (currentDay == null) currentDay = int.Parse(data);
            else if (currentPart == null) currentPart = int.Parse(data);
        }
        else if (curr.Contains("get_star_ts"))
        {
            var unixTimestamp = long.Parse(curr.Split(":")[1]);
            var timestamp =
                new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                + TimeSpan.FromSeconds(unixTimestamp);
            timestamps.Add((currentYear, currentDay!.Value, currentPart!.Value, timestamp));
        }
    }
}

var orderedTimestamps = timestamps
    .OrderBy(t => t.year)
    .ThenBy(t => t.day)
    .ThenBy(t => t.part);

foreach (var (year, day, part, timestamp) in orderedTimestamps)
{
    WriteLine(
        $"{year} {day.ToString().PadLeft(2)}.{part}:  " +
        timestamp.ToSwedishLocalString());
}
