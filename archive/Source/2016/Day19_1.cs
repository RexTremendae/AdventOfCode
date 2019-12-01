using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
 
using static System.Console;
 
namespace Day19
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Program().Run();
        }
 
        public void Run()
        {
            var elfCount = int.Parse(ReadLine());
 
            int[] elves = new int[elfCount];
            for (int i = 0; i < elfCount; i++)
            {
                elves[i] = 1;
            }
 
            int currentElf = 0;
            while(true)
            {
                while (elves[currentElf] == 0)
                {
                    currentElf++;
                    if (currentElf >= elfCount) currentElf = 0;
                }
 
                int nextElf = currentElf+1;
                if (nextElf >= elfCount) nextElf = 0;
 
                while (elves[nextElf] == 0)
                {
                    nextElf++;
                    if (nextElf >= elfCount) nextElf = 0;
                }
 
                if (nextElf == currentElf) break;
 
                elves[currentElf] += elves[nextElf];
                //WriteLine($"Elf {currentElf+1} takes {elves[nextElf]} presents from elf {nextElf+1} and have now {elves[currentElf]} presents.");
                elves[nextElf] = 0;
 
                currentElf++;
                if (currentElf >= elfCount) currentElf = 0;
            }
 
            WriteLine(currentElf+1);
        }
    }
}