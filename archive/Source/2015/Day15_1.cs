using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using static System.Console;

namespace Day15
{
    public class Program
    {
        private Ingredient _butterscotch = new Ingredient("Butterscotch", -1, -2, 6, 3, 8);
        private Ingredient _cinnamon = new Ingredient("Cinnamon", 2, 3, -2, -1, 3);
        private Ingredient _sugar = new Ingredient("Sugar", 3, 0, 0, -3, 2);
        private Ingredient _sprinkles = new Ingredient("Sprinkles", -3, 3, 0, 0, 9);
        private Ingredient _candy = new Ingredient("Candy", -1, 0, 4, 0, 1);
        private Ingredient _chocolate = new Ingredient("Chocolate", 0, 0, -2, 2, 8);

        public static void Main(string[] args)
        {
            new Program().Run(args);
        }

        public void Run(string[] args)
        {
            WriteLine("---------------------------");
            WriteLine("Optimal cookie:");
            WriteLine("---------------------------");
            FindOptimalCookie(_sugar, _sprinkles, _candy, _chocolate);

            WriteLine();
            WriteLine("---------------------------");

            //CookieTestLoop();
            return;
        }

        private void CookieTestLoop()
        {
            WriteLine("======================");

            TestCookie(43, 57);
            WriteLine("======================");

            TestCookie(44, 56);
            WriteLine("======================");

            TestCookie(45, 55);
            WriteLine("======================");

            TestCookie(99, 1);
            WriteLine("======================");
        }

        private void TestCookie(long b, long c)
        {
            var cookie = new Cookie();
            cookie.AddIngredient(_butterscotch, b);
            cookie.AddIngredient(_cinnamon, c);

            WriteLine(cookie.GetRecipe());
            WriteLine("Score: " + cookie.CalculateScore().ToString("### ### ### ###"));            
        }

        private void FindOptimalCookie(params Ingredient[] ingredients)
        {
            Cookie maxCookie = new Cookie();

            for (int i = 0; i <= 100; i ++)
            {
                for (int j = 0; j <= 100 - i; j ++)
                {
                    for (int k = 0; k <= 100 - i - j; k ++)
                    {
                        var cookie = new Cookie();
                        cookie.AddIngredient(ingredients[0], i);
                        cookie.AddIngredient(ingredients[1], j);
                        cookie.AddIngredient(ingredients[2], k);
                        int l = 100 - i - j - k;
                        if (l > 0)
                            cookie.AddIngredient(ingredients[3], l);

                        //WriteLine(i + ", " + j + ", " + l);
                        long score = cookie.CalculateScore();
                        if (score > maxCookie.CalculateScore()) maxCookie = cookie;
                    }
                }
            }

            WriteLine(maxCookie.GetRecipe());
            WriteLine("Score: " + maxCookie.CalculateScore().ToString("### ### ### ###"));            
        }
    }

    public class Cookie
    {
        private List<(Ingredient ingredient, long amount)> _ingredients;
        private long? score;

        public Cookie()
        {
            _ingredients = new List<(Ingredient, long)>();
        }

        public long Capacity { get; private set; }
        public long Durability { get; private set; }
        public long Flavor { get; private set; }
        public long Texture { get; private set; }

        public long CalculateScore()
        {
            if (!score.HasValue)
            {
                if (Capacity < 0) Capacity = 0;
                if (Durability < 0) Durability = 0;
                if (Flavor < 0) Flavor = 0;
                if (Texture < 0) Texture = 0;

                score = Capacity * Durability * Flavor * Texture;
                if (score.Value < 0) score = 0;
            }

            return score.Value;
        }

        public string GetRecipe()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var ing in _ingredients)
            {
                sb.Append(ing.ingredient.Name);
                sb.Append(": ");
                sb.AppendLine(ing.amount.ToString());
            }

            return sb.ToString();
        }

        public void AddIngredient(Ingredient ingredient, long amount)
        {
            _ingredients.Add((ingredient, amount));

            Capacity += ingredient.Capacity*amount;
            Durability += ingredient.Durability*amount;
            Flavor += ingredient.Flavor*amount;
            Texture += ingredient.Texture*amount;
        }
    }

    public class Ingredient
    {
        public Ingredient(string name, long cap, long dur, long flv, long tex, long cal)
        {
            Name = name;
            Capacity = cap;
            Durability = dur;
            Flavor = flv;
            Texture = tex;
            Calories = cal;
        }

        public string Name { get; }
        public long Capacity { get; }
        public long Durability { get; }
        public long Flavor { get; }
        public long Texture { get; }
        public long Calories { get; }
    }
}