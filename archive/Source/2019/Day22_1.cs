using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using static System.Console;

namespace Day22
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run("Day22.txt");
        }

        public void Run(string filename)
        {
            var cards = GetDeck(10007);

            foreach (var item in Load(filename))
            {
                //WriteLine($"{item.technique} {item.n}");
                switch (item.technique)
                {
                    case Technique.DealIntoNew:
                        cards = cards.Reverse().ToArray();
                        break;

                    case Technique.DealWithIncrement:
                        var dealCards = new int[cards.Length];
                        int dealIdx = 0;
                        foreach (var c in cards)
                        {
                            dealCards[dealIdx] = c;
                            dealIdx += item.n;
                            if (dealIdx >= dealCards.Length) dealIdx -= dealCards.Length;
                        }
                        cards = dealCards;
                        break;

                    case Technique.Cut:
                        var cutIndex = item.n > 0 ? item.n : cards.Length + item.n;
                        var cutCards = new List<int>(cards.Length);
                        var cardList = cards.ToList();
                        cutCards.InsertRange(0, cardList.GetRange(cutIndex, cards.Length-cutIndex));
                        cutCards.AddRange(cardList.GetRange(0, cutIndex));
                        cards = cutCards.ToArray();
                        break;
                }
            }

            //foreach (var c in cards) Write($"{c} ");
            //WriteLine();

            int cardVal = 0;
            int cardPos = -1;
            while (cardVal != 2019)
            {
                cardPos++;
                cardVal = cards[cardPos];
            }

            WriteLine(cardPos);
        }

        public int[] GetDeck(int length)
        {
            var cards = new int[length];
            for (int i = 0; i < length; i ++) cards[i] = i;
            return cards;
        }

        public IEnumerable<(Technique technique, int n)> Load(string filename)
        {
            foreach (var line in File.ReadAllLines(filename))
            {
                if (line.StartsWith("deal into new stack", true, CultureInfo.InvariantCulture))
                {
                    yield return (Technique.DealIntoNew, 0);
                }
                else if (line.StartsWith("deal with increment", true, CultureInfo.InvariantCulture))
                {
                    var n = int.Parse(line.Split(' ').Last());
                    yield return (Technique.DealWithIncrement, n);
                }
                else if (line.StartsWith("cut", true, CultureInfo.InvariantCulture))
                {
                    var n = int.Parse(line.Split(' ').Last());
                    yield return (Technique.Cut, n);
                }
            }
        }
    }

    public enum Technique
    {
        DealIntoNew,
        DealWithIncrement,
        Cut
    }
}