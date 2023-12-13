using static ColorWriter;

public class Day7
{
    public async Task Part1()
    {
        var hands = new List<(string hand, int bid)>();

        foreach (var line in await File.ReadAllLinesAsync("Day7.txt"))
        {
            if (string.IsNullOrWhiteSpace(line)) break;

            var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var rawHand = split[0];
            var bid = int.Parse(split[1]);

            hands.Add((rawHand, bid));
        }

        var sum = 0L;
        var rank = hands.Count;
        foreach (var rawHand in hands
            .Select(_ => (hand: _.hand, type: Evaluate(_.hand), bid: _.bid))
            .OrderByDescending(_ => _.type)
            .ThenByDescending(_ => _.hand, new HandComparer()))
        {
            sum += rank*rawHand.bid;
            WriteLine(rawHand.hand);
            WriteLine(rawHand.type);
            WriteLine($"{rank} : {rawHand.bid}");
            WriteLine("--------");
            rank--;
        }

        WriteLine(sum);
    }

    private enum HandType
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        FullHouse,
        FourOfAKind,
        FiveOfAKind
    }

    private HandType Evaluate(string hand)
    {
        var orderedHand = hand
            .ToCharArray()
            .OrderByDescending(_ => _, new CardComparer())
            .ToArray();

        var handCount = new List<int>();

        for (int i = 0; i < 5; i++)
        {
            var j = i+1;
            while (j < 5 && orderedHand[i] == orderedHand[j]) j++;
            var count = j-i;
            handCount.Add(count);
            i += (count-1);
        }
        handCount = handCount.OrderByDescending(_ => _).ToList();

        return 1 switch
        {
            _ when handCount.First() == 5 => HandType.FiveOfAKind,
            _ when handCount.First() == 4 => HandType.FourOfAKind,
            _ when handCount.First() == 3 && handCount.Skip(1).First() == 2 => HandType.FullHouse,
            _ when handCount.First() == 3 => HandType.ThreeOfAKind,
            _ when handCount.First() == 2 && handCount.Skip(1).First() == 2 => HandType.TwoPair,
            _ when handCount.First() == 2 => HandType.OnePair,
            _ => HandType.HighCard
        };
    }

    private class HandComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            var comparer = new CardComparer();
            for (int i = 0; i < 5; i++)
            {
                var cmpr = comparer.Compare(x[i], y[i]);
                if (cmpr == 0) continue;
                return cmpr;
            }

            return 0;
        }
    }

    private class CardComparer : IComparer<char>
    {
        const string CardValues = "23456789TJQKA";

        public int Compare(char x, char y)
        {
            var cardXValue = CardValues.IndexOf(x);
            var cardYValue = CardValues.IndexOf(y);

            if (cardXValue < 0 || cardYValue < 0) throw new InvalidOperationException();

            return cardXValue > cardYValue ? 1 : cardXValue == cardYValue ? 0 : -1;
        }
    }
}
