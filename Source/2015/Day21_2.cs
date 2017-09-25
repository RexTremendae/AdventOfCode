using System.Collections.Generic;
using static System.Console;

namespace Day21
{
    public class Program
    {
        private int _maxCost;
        private Character _boss;

        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            var shop = new Shop();

            // RunExample();

            _boss = new Character(103, 9, 2);

            _maxCost = int.MinValue;
            int hp = 100;

            foreach (var wpn in shop.Items[ItemClass.Weapon])
            {
                Evaluate(hp, wpn);

                foreach (var armr in shop.Items[ItemClass.Armor])
                {
                    Evaluate(hp, wpn, armr);

                    var rings = shop.Items[ItemClass.Ring];
                    for (int i = 0; i < rings.Count; i++)
                    {
                        Evaluate(hp, wpn, armr, rings[i]);
                        Evaluate(hp, wpn, null, rings[i]);

                        for (int j = i+1; j < rings.Count; j++)
                        {
                            Evaluate(hp, wpn, armr, rings[i], rings[j]);
                            Evaluate(hp, wpn, null, rings[i], rings[j]);
                        }
                    }
                }
            }

            WriteLine($"Maximal cost: {(_maxCost == int.MinValue ? "-" : _maxCost.ToString())}");
        }

        private void Evaluate(int hp, Item weapon, Item armor = null, Item leftRing = null, Item rightRing = null)
        {
            var player = new Character(hp, weapon, armor, new []{ leftRing, rightRing });
            if (!Fight(player, _boss.Clone()) && player.Cost > _maxCost)
            {
                _maxCost = player.Cost;
            }
        }

        private void RunExample()
        {
            var boss = new Character(12, 7, 2);
            var player = new Character(8, 5, 5);

            Fight(player, boss, true);
        }

        private bool Fight(Character player, Character enemy, bool verbose = false)
        {
            int hp = 0;

            for(;;)
            {
                hp = enemy.TakeHitFrom(player);
                if (verbose) { WriteLine($"Enemy: {hp}"); }
                if (hp < 1)
                {
                    if (verbose) { WriteLine("Player wins!"); }
                    return true;
                }

                hp = player.TakeHitFrom(enemy);
                if (verbose) { WriteLine($"Player: {hp}"); }
                if (hp < 1)
                {
                    if (verbose) { WriteLine("Boss wins!"); }
                    return false;
                }
            }
        }
    }

    public enum ItemClass
    {
        Weapon, Armor, Ring
    }

    public class Item
    {
        public Item(ItemClass itemClass, int cost, int damage, int armor)
        {
            ItemClass = itemClass;
            Cost = cost;
            Armor = armor;
            Damage = damage;
        }

        public ItemClass ItemClass { get; }
        public int Cost { get; }
        public int Damage { get; }
        public int Armor { get; }
    }

    public class Character
    {
        public Character(int hitPoints, int damage, int armor)
        {
            HitPoints = hitPoints;
            Damage = damage;
            Armor = armor;
        }

        public Character(int hitPoints, Item weapon, Item armor, Item[] rings)
        {
            HitPoints = hitPoints;
            Damage += weapon.Damage;
            Armor += weapon.Armor;
            Cost += weapon.Cost;

            if (armor != null)
            {
                Damage += armor.Damage;
                Armor += armor.Armor;
                Cost += armor.Cost;
            }

            for (int i = 0; i < rings.Length; i++)
            {
                if (rings[i] == null) continue;

                Damage += rings[i].Damage;
                Armor += rings[i].Armor;
                Cost += rings[i].Cost;
            }
        }

        public Character Clone()
        {
            return new Character(HitPoints, Damage, Armor);
        }

        public int TakeHitFrom(Character character)
        {
            var hit = character.Damage - Armor;
            if (hit < 0) return HitPoints;

            if (hit == 0) hit = 1;

            HitPoints -= hit;
            return HitPoints;
        }

        public int HitPoints { get; private set; }
        public int Damage { get; }
        public int Armor { get; }
        public int Cost { get; }
    }

    public class Shop
    {
        public Dictionary<ItemClass, List<Item>> Items { get; }

        public Shop()
        {
            Items = new Dictionary<ItemClass, List<Item>>();

            var weapons = new List<Item>(new[]{
                new Item(ItemClass.Weapon,  8, 4, 0), // Dagger
                new Item(ItemClass.Weapon, 10, 5, 0), // Shortsword
                new Item(ItemClass.Weapon, 25, 6, 0), // Warhammer
                new Item(ItemClass.Weapon, 40, 7, 0), // Longsword
                new Item(ItemClass.Weapon, 74, 8, 0)  // Greataxe
            });

            var armor = new List<Item>(new[]{
                new Item(ItemClass.Armor,  13, 0, 1), // Leather
                new Item(ItemClass.Armor,  31, 0, 2), // Chainmail
                new Item(ItemClass.Armor,  53, 0, 3), // Splintmail
                new Item(ItemClass.Armor,  75, 0, 4), // Bandedmail
                new Item(ItemClass.Armor, 102, 0, 5)  // Platemail
            });

            var rings = new List<Item>(new[]{
                new Item(ItemClass.Ring,  25, 1, 0), // Damage +1
                new Item(ItemClass.Ring,  50, 2, 0), // Damage +2
                new Item(ItemClass.Ring, 100, 3, 0), // Damage +3
                new Item(ItemClass.Ring,  20, 0, 1), // Defense +1
                new Item(ItemClass.Ring,  40, 0, 2), // Defense +2
                new Item(ItemClass.Ring,  80, 0, 3)  // Defense +3
            });

            Items.Add(ItemClass.Weapon, weapons);
            Items.Add(ItemClass.Armor, armor);
            Items.Add(ItemClass.Ring, rings);
        }
    }
}