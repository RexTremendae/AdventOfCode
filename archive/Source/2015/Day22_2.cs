using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace Day22
{
    public class Program
    {
        public static void Main()
        {
            // Shortest mana path: 226
            var sample1 = new State
            {
                PlayerMana = 250,
                PlayerHP = 10,
                BossHP = 13
            };

            // Shortest mana path: 641
            var sample2 = new State
            {
                PlayerMana = 250,
                PlayerHP = 10,
                BossHP = 14
            };

            // Shortest mana path: 953
            var challenge1 = new State
            {
                PlayerMana = 500,
                PlayerHP = 50,
                BossHP = 55
            };

            // Shortest mana path:
            var challenge2 = new State
            {
                PlayerMana = 500,
                PlayerHP = 50,
                BossHP = 55,
                Hard = true
            };

            new Program().Run(challenge2);
        }

        private DateTime _startTime;

        private string GetFormattedDuration()
        {
            var duration = DateTime.Now - _startTime;
            return $"{duration.Minutes.ToString().PadLeft(2, '0')}:{duration.Seconds.ToString().PadLeft(2, '0')}";
        }

// Magic Missile   53 mana  4 damage
// Drain           73 mana  2 damage   2 heal
// Shield [E]     113 mana  increase armor by 7 for 6 turns
// Poison [E]     173 mana  3 damage for 6 turns
// Recharge [E]   229 mana  101 mana for 5 turns

        public enum Spell
        {
            MagicMissile,
            Drain,
            Shield,
            Poison,
            Recharge
        }

        public Dictionary<Spell, int> SpellCosts = new Dictionary<Spell, int>() {
            { Spell.MagicMissile, 53 },
            { Spell.Drain, 73 },
            { Spell.Shield, 113 },
            { Spell.Poison, 173 },
            { Spell.Recharge, 229 }
        };

        public class State
        {
            public State()
            {
                Effects = new Dictionary<Spell, int>() {
                    { Spell.MagicMissile, 0 },
                    { Spell.Drain, 0 },
                    { Spell.Shield, 0 },
                    { Spell.Poison, 0 },
                    { Spell.Recharge, 0 }
                };
            }

            public int PlayerMana { get; set; }
            public int PlayerHP { get; set; }
            public int BossHP { get; set; }
            public bool Hard { get; set; }

            public readonly Dictionary<Spell, int> Effects;

            public (State, Spell)? Previous { get; set; }

            public void DecreaseEffects()
            {
                foreach (var spell in new[] { Spell.Poison, Spell.Shield, Spell.Recharge })
                {
                    Effects[spell]--;
                    if (Effects[spell] < 0) Effects[spell] = 0;
                }
            }

            public State Clone(Spell spell)
            {
                var clone = new State { PlayerMana = PlayerMana, PlayerHP = PlayerHP, BossHP = BossHP, Hard = Hard };
                foreach (var key in Effects.Keys)
                    clone.Effects[key] = Effects[key];

                clone.Previous = (this, spell);

                return clone;
            }

            public override string ToString()
            {
                return
                    $"Player mana: {PlayerMana}\n" +
                    $"Player HP:   {PlayerHP}\n" +
                    $"Boss HP:     {BossHP}";
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(PlayerMana, BossHP, Effects[Spell.Poison], Effects[Spell.Recharge], Effects[Spell.Shield]);
            }

            public override bool Equals(object? obj)
            {
                if (!(obj is State other))
                {
                    return false;
                }

                return other.PlayerMana == PlayerMana && other.BossHP == BossHP &&
                    other.Effects[Spell.Poison] == Effects[Spell.Poison] &&
                    other.Effects[Spell.Recharge] == Effects[Spell.Recharge] &&
                    other.Effects[Spell.Shield] == Effects[Spell.Shield];
            }
        }

        string _lastDuration = "";

        public void Run(State startState)
        {
            _startTime = DateTime.Now;

            var q = new List<(State state, int mana)>();
            q.Add((startState, 0));

            var minimalBossHp = int.MaxValue;

            var visited = new HashSet<State>();

            while (q.Any())
            {
                var deQ = q.First();
                q.RemoveAt(0);

                foreach (var spell in SpellCosts.OrderBy(sp => sp.Value).Select(sp => sp.Key))
                {
                    if (Next(deQ.state, spell, out var next))
                    {
                        if (visited.Contains(next)) { continue; }
                        visited.Add(next);

                        if (next.BossHP < minimalBossHp)
                        {
                            minimalBossHp = next.BossHP;
                        }

                        var newDuration = GetFormattedDuration();
                        if (newDuration != _lastDuration)
                        {
                            _lastDuration = newDuration;
                            WriteLine($"[{_lastDuration}]  {minimalBossHp}");
                        }

                        var newMana = deQ.mana + SpellCosts[spell];

                        if (next.BossHP <= 0)
                        {
                            var path = new List<(State, Spell?)>();
                            var trace = next;
                            Spell? spell_ = null;
                            while (trace.Previous != null)
                            {
                                path.Add((trace, spell_));
                                spell_ = trace.Previous.Value.Item2;
                                trace = trace.Previous.Value.Item1;
                            }
                            path.Add((trace, spell_));

                            WriteLine();
                            for (int i = path.Count - 1; i >= 0; i--)
                            {
                                WriteLine(path[i].Item1);
                                WriteLine($" -- {path[i].Item2} -- ");
                            }

                            WriteLine();
                            WriteLine(newMana);
                            WriteLine();

                            return;
                        }

                        int idx = 0;
                        for (; idx < q.Count; idx++)
                        {
                            if (q[idx].mana >= newMana) break;
                        }

                        /*
                        WriteLine(deQ.state);
                        WriteLine($" -- {spell} -- ");
                        WriteLine(next);
                        WriteLine();
                        */
                        q.Insert(idx, (next, newMana));
                    }
                }
            }

            WriteLine(GetFormattedDuration());
        }

        public bool Next(State state, Spell spell, out State nextState)
        {
            nextState = state.Clone(spell);

            if (nextState.Hard)
            {
                nextState.PlayerHP --;
                if (nextState.PlayerHP <= 0) return false;
            }

            var spellCost = SpellCosts[spell];

            if (nextState.PlayerMana < spellCost) return false;
            if (nextState.Effects[Spell.Poison] > 1 && spell == Spell.Poison) return false;
            if (nextState.Effects[Spell.Shield] > 1 && spell == Spell.Shield) return false;
            if (nextState.Effects[Spell.Recharge] > 1 && spell == Spell.Recharge) return false;

            nextState.PlayerMana -= spellCost;

            switch (spell)
            {
                case Spell.MagicMissile:
                    nextState.BossHP -= 4;
                    break;

                case Spell.Drain:
                    nextState.BossHP -= 2;
                    nextState.PlayerHP += 2;
                    break;

                case Spell.Poison:
                    nextState.Effects[Spell.Poison] = 6;
                    break;

                case Spell.Recharge:
                    nextState.Effects[Spell.Recharge] = 5;
                    break;

                case Spell.Shield:
                    nextState.Effects[Spell.Shield] = 6;
                    break;
            }

            if (nextState.Effects[Spell.Poison] > 0) nextState.BossHP -= 3;
            if (nextState.Effects[Spell.Recharge] > 0) nextState.PlayerMana += 101;

            if (nextState.BossHP <= 0) return true;

            nextState.DecreaseEffects();

            nextState.PlayerHP -= nextState.Effects[Spell.Shield] > 0 ? 1 : 8;
            if (nextState.PlayerHP <= 0) return false;

            if (nextState.Effects[Spell.Poison] > 0) nextState.BossHP -= 3;
            if (nextState.Effects[Spell.Recharge] > 0) nextState.PlayerMana += 101;

            if (nextState.BossHP <= 0) return true;

            nextState.DecreaseEffects();

            return true;
        }
    }
}