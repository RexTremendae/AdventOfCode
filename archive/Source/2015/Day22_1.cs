using System;
using System.Collections.Generic;
using System.Linq;

using static Day22.ColorWriter;

namespace Day22
{
    public class Program
    {
        private readonly Dictionary<SpellType, Spell> _spellbook;
        private bool _armourWearsOutPending;
        private bool _debug = true;

        public static void Main()
        {
            new Program().Run();
        }

        public Program()
        {
            _spellbook = new Dictionary<SpellType, Spell>();
            foreach (var spell in DefineSpells())
            {
                _spellbook.Add(spell.SpellType, spell);
            }
        }

        public void Run()
        {
            _debug = false;

            var mana = FightWithLeastManaSpent();
            WriteLine($"{mana.Item2} [{mana.Item1}]");
            return;

            var result = RunExample2();

            WriteLine();

            switch (result)
            {
                case FightResult.InvalidMove:
                    WriteLine("Invalid move!", ConsoleColor.Red);
                    break;

                case FightResult.BossDied:
                    Write("COOL! ", ConsoleColor.Green);
                    WriteLine("Great news, the boss was defeated!");
                    break;

                case FightResult.PlayerDied:
                    Write("OH NO! ", ConsoleColor.Red);
                    WriteLine("Sad times, the boss just defeated the player.");
                    break;

                case FightResult.None:
                    WriteLine("The fight is not over, but no more input was provided for the player.");
                    break;
            }
        }

        private (int, string) FightWithLeastManaSpent()
        {
            var boss = new Character(hitPoints: 55, damage: 8);
            var player = new Character(hitPoints: 50, mana: 500);

            var q = new Queue<(Character, Character, SpellType[], int)>();

            Enqueue(q, player, boss, new SpellType[]{}, 0);

            int leastMana = int.MaxValue;
            string leastManaPath = "";

            while(q.Any())
            {
                var dq = q.Dequeue();
                var result = FightOne(dq.Item1, dq.Item2, dq.Item3.Last());
                if (result == FightResult.None)
                {
                    if (dq.Item4 > leastMana) continue;
                    Enqueue(q, dq.Item1, dq.Item2, dq.Item3, dq.Item4);
                }
                else if (result == FightResult.BossDied)
                {
                    string path = "";
                    int mana = 0;
                    foreach (var spell in dq.Item3)
                    {
                        path += spell.ToString() + ", ";
                        mana += _spellbook[spell].ManaCost;
                    }
                    path = path.Substring(0, path.Length - 2);

                    if (mana < leastMana)
                    {
                        leastMana = mana;
                        leastManaPath = path;
                    }
                }
            }

            return (leastMana, leastManaPath);
        }

        private void Enqueue(Queue<(Character, Character, SpellType[], int)> q, Character player, Character boss, SpellType[] path, int manaCost)
        {
            foreach (SpellType spell in Enum.GetValues(typeof(SpellType)))
            {
                var newPath = new SpellType[path.Length+1];
                int i = 0;
                for (; i < path.Length; i++) newPath[i] = path[i];
                newPath[i] = spell;

                q.Enqueue((player.Clone(), boss.Clone(), newPath, manaCost + _spellbook[spell].ManaCost));
            }
        }

        private FightResult RunExample1()
        {
            var boss = new Character(hitPoints: 13, damage: 8);
            var player = new Character(hitPoints: 10, mana: 250);

            return Fight(player, boss,
                new[]
                {
                    SpellType.Poison,
                    SpellType.MagicMissile
                });
        }

        private FightResult RunExample2()
        {
            var boss = new Character(hitPoints: 14, damage: 8);
            var player = new Character(hitPoints: 10, mana: 250);

            return Fight(player, boss,
                new[]
                { 
                    SpellType.Recharge,
                    SpellType.Shield,
                    SpellType.Drain,
                    SpellType.Poison,
                    SpellType.MagicMissile
                });
        }

        private IEnumerable<Spell> DefineSpells()
        {
            yield return new Spell(
                SpellType.MagicMissile,
                manaCost: 53,
                immediate: new EffectEntity(Subject.Antagonist, Property.HitPoints, -4));

            yield return new Spell(
                SpellType.Drain,
                manaCost: 73,
                immediates: new[] {
                    new EffectEntity(Subject.Protagonist, Property.HitPoints, 2),
                    new EffectEntity(Subject.Antagonist, Property.HitPoints, -2)
                });

            yield return new Spell(
                SpellType.Shield,
                manaCost: 113,
                effect: new EffectEntity(Subject.Protagonist, Property.Armor, 7),
                duration: 6);

            yield return new Spell(
                SpellType.Poison,
                manaCost: 173,
                effect: new EffectEntity(Subject.Antagonist, Property.HitPoints, -3),
                duration: 6);

            yield return new Spell(
                SpellType.Recharge,
                manaCost: 229,
                effect: new EffectEntity(Subject.Protagonist, Property.Mana, 101),
                duration: 5);
        }

        private FightResult Fight(Character player, Character boss, SpellType[] playerSpells)
        {
            foreach (var spell in playerSpells)
            {
                var result = FightOne(player, boss, spell);
                if (result != FightResult.None) return result;
            }

            return FightResult.None;
        }

        private FightResult FightOne(Character player, Character boss, SpellType spell)
        {
            MoveResult moveResult = MoveResult.None;
            FightResult fightResult = FightResult.None;

            PrintTurn(player, boss, "Player", ConsoleColor.Green);

            ApplyEffectsFor(player, "Player");
            ApplyEffectsFor(boss, "Boss");

            fightResult = EvaluateStatus(player, boss);
            if (fightResult != FightResult.None) return fightResult;

            moveResult = PlayerMove(player, boss, spell);
            if (moveResult == MoveResult.Invalid) return FightResult.InvalidMove;

            fightResult = EvaluateStatus(player, boss);
            if (fightResult != FightResult.None) return fightResult;

            PrintTurn(player, boss, "Boss", ConsoleColor.Red);

            ApplyEffectsFor(player, "Player");
            ApplyEffectsFor(boss, "Boss");

            fightResult = EvaluateStatus(player, boss);
            if (fightResult != FightResult.None) return fightResult;

            moveResult = BossMove(player, boss);

            fightResult = EvaluateStatus(player, boss);
            return fightResult;
        }

        private void ApplyEffectsFor(Character character, string characterDescription = "")
        {
            var indicesToRemove = new List<int>();
            if (_armourWearsOutPending)
            {
                _armourWearsOutPending = false;
                character.Properties[Property.Armor] = 0;
            }

            for (int i = 0; i < character.Effects.Count; i++)
            {
                var effect = character.Effects[i].Item1;
                var effectCount = character.Effects[i].Item2;

                if (_debug)
                {
                    Write($"{characterDescription} ");
                    Write("got effect ");
                    Write($"{effect.Property} ", ConsoleColor.Yellow);
                    Write($"{effect.Magnitude} ", effect.Magnitude < 0 ? ConsoleColor.Red : ConsoleColor.Green);
                    WriteLine("applied.");
                }

                if (effect.Property != Property.Armor || character.Properties[effect.Property] == 0)
                    character.Properties[effect.Property] += effect.Magnitude;

                if (effectCount > 1)
                {
                    character.Effects[i] = (effect, effectCount-1);
                }
                else
                {
                    indicesToRemove.Add(i);
                }
            }

            for (int i = indicesToRemove.Count - 1; i >= 0; i--)
            {
                var effect = character.Effects[indicesToRemove[i]].Item1;
                if (_debug)
                {
                    Write("Effect ");
                    Write($"{effect.Property} {effect.Magnitude}", ConsoleColor.Yellow);
                    WriteLine(" has worn out.");
                }
                character.Effects.RemoveAt(indicesToRemove[i]);

                if (effect.Property == Property.Armor) _armourWearsOutPending = true;
            }
        }

        private FightResult EvaluateStatus(Character player, Character boss)
        {
            if (player.Properties[Property.HitPoints] <= 0) return FightResult.PlayerDied;
            if (boss.Properties[Property.HitPoints] <= 0) return FightResult.BossDied;

            return FightResult.None;
        }

        private void PrintTurn(Character player, Character boss, string character, ConsoleColor color)
        {
            if (!_debug) return;

            WriteLine();

            Write("-- ");
            Write($"{character} turn", color);
            Write(" --");
            WriteLine();

            PrintStatistics("Player", player);
            PrintStatistics("Boss", boss);
        }

        private void PrintStatistics(string characterType, Character character)
        {
            Write($"{characterType} has ");
            Write(character.Properties[Property.HitPoints].ToString(), ConsoleColor.Cyan);
            Write(" hit points, ");
            Write(character.Properties[Property.Armor].ToString(), ConsoleColor.Yellow);
            Write(" armor, ");
            Write(character.Properties[Property.Mana].ToString(), ConsoleColor.DarkMagenta);
            WriteLine(" mana");
        }

        private MoveResult BossMove(Character player, Character boss)
        {
            var damage = boss.Properties[Property.Damage] - player.Properties[Property.Armor];
            if (damage < 1) damage = 1;
            player.Properties[Property.HitPoints] -= damage;

            if (_debug)
            {
                Write("Boss deals ");
                Write(damage.ToString(), ConsoleColor.Red);
                WriteLine(" damage.");
            }

            return MoveResult.None;
        }

        private MoveResult PlayerMove(Character player, Character boss, SpellType spellType)
        {
            if (_debug)
            {
                Write("Player casts ");
                WriteLine(spellType, ConsoleColor.Cyan);
            }

            var spell = _spellbook[spellType].Clone();
            if (spell.ManaCost > player.Properties[Property.Mana])
            {
                return MoveResult.Invalid;
            }

            var playerEffects = new List<(EffectEntity, int)>();
            var bossEffects = new List<(EffectEntity, int)>();

            foreach (var effect in spell.Effects)
            {
                if (effect.Subject == Subject.Antagonist)
                {
                    if (boss.Effects.Any(x => x.Item1.Property == effect.Property)) return MoveResult.Invalid;
                    bossEffects.Add((effect, spell.Duration));
                }
                else if (effect.Subject == Subject.Protagonist)
                {
                    if (player.Effects.Any(x => x.Item1.Property == effect.Property)) return MoveResult.Invalid;
                    playerEffects.Add((effect, spell.Duration));
                }
                else
                {
                    return MoveResult.Invalid;
                }
            }

            foreach (var bossEffect in bossEffects) boss.Effects.Add(bossEffect);
            foreach (var playerEffect in playerEffects) player.Effects.Add(playerEffect);

            player.Properties[Property.Mana] -= spell.ManaCost;

            foreach (var effect in spell.Immediates)
            {
                if (effect.Subject == Subject.Antagonist)
                {
                    boss.Properties[effect.Property] += effect.Magnitude;
                }
                else if (effect.Subject == Subject.Protagonist)
                {
                    player.Properties[effect.Property] += effect.Magnitude;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            return MoveResult.None;
        }

        private enum MoveResult
        {
            Invalid = -1,
            None = 0
        }

        private enum FightResult
        {
            InvalidMove = -1,
            None = 0,
            BossDied,
            PlayerDied
        }
    }

    public enum SpellType
    {
        MagicMissile,
        Drain,
        Shield,
        Poison,
        Recharge
    }

    public enum Subject
    {
        None,
        Protagonist,
        Antagonist
    }

    public enum Property
    {
        None,
        HitPoints,
        Armor,
        Mana,
        Damage
    }

    public class EffectEntity
    {
        public Subject Subject { get; }
        public Property Property { get; }
        public int Magnitude { get; }

        public EffectEntity(Subject subject, Property property, int magnitude)
        {
            Subject = subject;
            Property = property;
            Magnitude = magnitude;
        }

        public EffectEntity Clone()
        {
            return new EffectEntity(Subject, Property, Magnitude);
        }
    }

    public class Spell
    {
        public Spell(SpellType spellType, int manaCost, EffectEntity immediate = null, EffectEntity effect = null, int duration = 0)
            : this(spellType,
                   manaCost,
                   immediates: immediate != null ? new EffectEntity[] { immediate } : new EffectEntity[] {},
                   effects: effect != null ? new EffectEntity[] { effect } : new EffectEntity[] {},
                   duration: duration)
        {
        }

        public Spell(SpellType spellType, int manaCost, IEnumerable<EffectEntity> immediates = null, IEnumerable<EffectEntity> effects = null, int duration = 0)
        {
            SpellType = spellType;
            ManaCost = manaCost;
            Immediates = immediates ?? new EffectEntity[] {};
            Effects = effects ?? new EffectEntity[] {};
            Duration = duration;
        }

        public SpellType SpellType { get; }
        public int ManaCost { get; }
        public int Duration { get; }
        public IEnumerable<EffectEntity> Immediates { get; }
        public IEnumerable<EffectEntity> Effects { get; }

        public Spell Clone()
        {
            var immediatesClone = new List<EffectEntity>();
            foreach (var effect in Immediates)
            {
                immediatesClone.Add(effect.Clone());
            }

            var effectsClone = new List<EffectEntity>();
            foreach (var effect in Effects)
            {
                effectsClone.Add(effect.Clone());
            }

            return new Spell(
                SpellType,
                ManaCost,
                immediates: immediatesClone,
                effects: effectsClone,
                duration: Duration);
        }
    }

    public class Character
    {
        public Dictionary<Property, int> Properties { get; set; }
        public List<(EffectEntity, int)> Effects { get; set; }

        public Character(int hitPoints, int damage = 0, int mana = 0, int armor = 0)
        {
            Properties = new Dictionary<Property, int>();

            Properties[Property.Mana] = mana;
            Properties[Property.HitPoints] = hitPoints;
            Properties[Property.Armor] = armor;
            Properties[Property.Damage] = damage;

            Effects = new List<(EffectEntity, int)>();
        }

        public Character Clone()
        {
            var mana = Properties[Property.Mana];
            var hp = Properties[Property.HitPoints];
            var damage = Properties[Property.Damage];
            var armor = Properties[Property.Armor];

            var clone = new Character(hp, damage, mana, armor);

            foreach (var effect in Effects)
            {
                clone.Effects.Add((effect.Item1.Clone(), effect.Item2));
            }

            return clone;
        }
    }

    public static class ColorWriter
    {
        public static void Write(object obj, ConsoleColor color)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(obj);
            Console.ForegroundColor = oldColor;
        }

        public static void Write(object obj)
        {
            Console.Write(obj);
        }

        public static void WriteLine(object obj, ConsoleColor color)
        {
            Write(obj.ToString(), color);
            Console.WriteLine();
        }

        public static void WriteLine(object obj)
        {
            Console.WriteLine(obj);
        }

        public static void WriteLine()
        {
            Console.WriteLine();
        }

        public static void PrintParts(IEnumerable<(string, bool)> parts)
        {
            foreach (var p in parts)
            {
                var color = p.Item2 ? ConsoleColor.Green : ConsoleColor.Red;
                ColorWriter.Write($"[{p.Item1}] ", color);
            }
            Console.WriteLine();
        }
    }
}