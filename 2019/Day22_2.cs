using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;

public enum Operation
{
    Cut,
    DealIntoNewStack,
    DealWithIncrement
};

public class VisitedData
{
    long[] _cards;
    long _start;
    int _dir;
    //Operation _operation = Operation.Cut;
    object _arg = new();

    public VisitedData(long[] cards, long start, int dir)//, Operation operation, object arg)
    {
        _cards = cards;
        _start = start;
        _dir = dir;
/*
        _arg = arg;
        _operation = operation;
*/
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(_start);
        hashCode.Add(_dir);
/*
        hashCode.Add(_arg);
        hashCode.Add(_operation);
*/
        foreach (var c in _cards)
        {
            hashCode.Add(c);
        }

        return hashCode.ToHashCode();
    }
/*
    public override bool Equals(object obj)
    {
        var other = (VisitedData)obj;
        if (other._arg != _arg) return false;
        if (other._dir != _dir) return false;
        if (other._operation != _operation) return false;
        if (other._start != _start) return false;

        for (long i = 0; i < _cards.Length; i++)
            if (other._cards[i] != _cards[i]) return false;

        return true;
    }
    */
}

class Program
{
    /*
    static void Main()
    {
        var primes = new List<long>();

        var l = 3L;
        for (; l <= 11931571; l+=2)
        {
            bool isPrime = true;
            foreach (var p in primes)
            {
                if (l % p == 0)
                {
                    isPrime = false;
                    break;
                }
            }

            if (isPrime) primes.Add(l);
        }

        foreach (var p in primes)
        {
            Write($"{p} ");
        }
        WriteLine();
        //new Program().Run();
    }
    */

    long[] _cards = Array.Empty<long>();
    long _start;
    int _dir;

    void Run()
    {
        var visited = new HashSet<VisitedData>();

        var operations = Load().ToList();
        //_cards = new long[10];
        _cards = new long[10007];
        for (long i = 0; i < _cards.Length; i++)
            _cards[i] = i;

        _start = 0;
        _dir = 1;
        var visitedData = 
            new VisitedData(_cards, _start, _dir);//, operation, arg));
        visited.Add(visitedData);
/*
        operations = new[]
        {
            (Operation.DealWithIncrement, 3),
            (Operation.Cut, -1),
            (Operation.DealIntoNewStack, 0),
            (Operation.Cut, 4),
        }.ToList();
*/
        //PrintDeck();

        //var visited = HashSet<>();

        //for (int ii = 0; ii < 10000; ii++)
        foreach ((var op, var arg) in operations)
        {
            switch (op)
            {
                case Operation.Cut:
                    _start += _dir * arg;
                    FixWrap();
                    break;
                case Operation.DealIntoNewStack:
                    _start -= _dir;
                    _dir *= -1;
                    FixWrap();
                    break;
                case Operation.DealWithIncrement:
                    var origPos = _start;
                    var nextPos = _start;
                    var next = new long[_cards.Length];
                    for (long i = 0; i < _cards.Length; i++)
                    {
                        next[nextPos] = _cards[origPos];
                        nextPos += _dir*arg;
                        origPos += _dir;

                        if (nextPos < 0) nextPos += _cards.Length;
                        else if(nextPos >= _cards.Length) nextPos -= _cards.Length;
                        if (origPos < 0) origPos = _cards.Length-1;
                        if (origPos >= _cards.Length) origPos = 0;
                    }
                    _cards = next;
                    break;
            }
            //WriteLine();
            //WriteLine($"Executing {op} {arg}:");
            //PrintDeck();
            visitedData = new VisitedData(_cards, _start, _dir);//, operation, arg));
            if (visited.Contains(visitedData))
            {
                WriteLine("Revisited");
            }
            visited.Add(visitedData);
        }

        WriteLine(Find(2019));
        WriteLine();
    }

    void FixWrap()
    {
        if (_start < 0) _start += _cards.Length;
        else if (_start >= _cards.Length) _start -= _cards.Length;
    }

    void PrintDeck(long count = long.MaxValue)
    {
        bool first = true;
        for (long i = _start;;i += _dir, count--)
        {
            if (i < 0) i = _cards.Length - 1;
            else if (i >= _cards.Length) i = 0;
            if (!first && i == _start || count == 0) break;

            Write($"{_cards[i]} ");
            first = false;
        }

        WriteLine();
    }

    long Find(long value)
    {
        long pos = 0;
        for (long i = _start;;i += _dir, pos ++)
        {
            if (i < 0) i = _cards.Length - 1;
            else if (i >= _cards.Length) i = 0;
            if (_cards[i] == value) return pos;
        }
    }

    IEnumerable<(Operation, int)> Load()
    {
        foreach (var line in File.ReadAllLines("Day22.txt"))
        {
            var split = 0;
            for (; split < line.Length; split++)
            {
                var ch = line[split];
                if (ch == '-' || (ch >= '0' && ch <= '9'))
                {
                    break;
                }
            }
            var opPart = line[..split].Replace(" ", "");
            if (Enum.TryParse<Operation>(opPart, true, out var op))
            {
                var arg = split < line.Length ? int.Parse(line[split..]) : 0;
                yield return (op, arg);
            }
            else
            {
                throw new InvalidOperationException(opPart);
            }
        }
    }
}