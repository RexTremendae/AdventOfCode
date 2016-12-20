using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
 
using static System.Console;
 
namespace Day19
{
    public class Node
    {
        public uint Id;
        public Node Next;
        public Node Prev;
    }

    public class Program
    {
        Node _elveNodes;

        public static void Main(string[] args)
        {
            new Program().Run();
        }
 
        public void Run()
        {
            var elfCount = uint.Parse(ReadLine());
            _elveNodes = null;

            var currentElf = new Node { Id = 0 };
            currentElf.Next = currentElf;
            currentElf.Prev = currentElf;
            _elveNodes = currentElf;
            for (uint i = 1; i < elfCount; i++)
            {
                Node oldNext = _elveNodes.Next;

                Node node = new Node { Id = i };
                _elveNodes.Next = node;
                node.Prev = _elveNodes;
                
                node.Next = oldNext;
                oldNext.Prev = node;

                _elveNodes = node;
            }

            Node nextElf = currentElf;
            for (int i = 0; i < elfCount/2; i++)
                nextElf = nextElf.Next;
            //int nextElf = currentElf+1;
/*
            for (int i = 0; i < elfCount; i++)
            {
                WriteLine($"[{currentElf.Id}] Next:{currentElf.Next.Id} Prev:{currentElf.Prev.Id}");
                currentElf = currentElf.Next;
            }
*/
            while(true)
            {
                if (currentElf == currentElf.Next) break;

                Node oldNextElf = nextElf.Next;
                //WriteLine($"Elf {currentElf.Id+1} takes all presents from elf {nextElf.Id+1}.");
                nextElf.Prev.Next = nextElf.Next;
                nextElf.Next.Prev = nextElf.Prev;
                elfCount--;
                nextElf = oldNextElf;

                currentElf = currentElf.Next;
                if (elfCount % 2 == 0)
                    nextElf = nextElf.Next;
            }

            WriteLine();
            WriteLine("===============");
            WriteLine();

            WriteLine(currentElf.Id+1);
        }
    }
}