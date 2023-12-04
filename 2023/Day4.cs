using static ColorWriter;

public class Day4
{
    public async Task Part1()
    {
        var sum = 0;
        await foreach (var card in ReadData("Day4.txt"))
        {
            var points = 0;
            var nextPoint = 1;
            var winningNumbers = card.WinningNumbers.ToHashSet();
            foreach (var number in card.CardNumbers)
            {
                if (winningNumbers.Contains(number))
                {
                    points = nextPoint;
                    nextPoint <<= 1;
                }
            }

            WriteLine(points, ConsoleColor.DarkGray);
            sum += points;
        }

        WriteLine(sum);
    }

    public async Task Part2()
    {
        var sum = 0L;
        var cardList = await ReadData("Day4.txt").ToArrayAsync();
        var cardCount = cardList.Select(_ => 1).ToArray();

        for (int cardId = 0; cardId < cardList.Length; )
        {
            var card = cardList[cardId];
            cardId++;

            var nextId = cardId;
            var winningNumbers = card.WinningNumbers.ToHashSet();
            foreach (var number in card.CardNumbers)
            {
                if (winningNumbers.Contains(number))
                {
                    cardCount[nextId] += cardCount[cardId-1];
                    nextId++;
                }
            }
        }

        //WriteLine(string.Join(", ", cardCount));
        WriteLine(cardCount.Sum());
    }

    public async IAsyncEnumerable<Card> ReadData(string filename)
    {
        foreach (var line in await File.ReadAllLinesAsync(filename))
        {
            if (string.IsNullOrEmpty(line)) continue;

            //Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53

            var opts = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
            var commaSplit = line.Split(':', opts);
            var numberSets = commaSplit[1].Split('|', opts);
            var winningNumbers = numberSets[0].Split(' ', opts);
            var cardNumbers = numberSets[1].Split(' ', opts);

            yield return new (
                winningNumbers.Select(int.Parse).ToArray(),
                cardNumbers.Select(int.Parse).ToArray());
        }
    }
}

public readonly record struct Card(int[] WinningNumbers, int[] CardNumbers);
