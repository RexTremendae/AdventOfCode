using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using static System.Console;

namespace Day24
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            var allGroups = ParseInput("Day24.txt").ToList();

            int boost = 1;
            var winner = GroupType.Infection;
            bool immuneSystemWins = false;

            while(!immuneSystemWins)
            {
                WriteLine("Boost: " + boost);

                var clonedGroups = new List<Group>();
                foreach (var g in allGroups)
                {
                    var clone = g.Clone();
                    if (clone.GroupType == GroupType.ImmuneSystem) clone.DamageHitPoints += boost;
                    clonedGroups.Add(clone);
                }

                var result = Fight(clonedGroups, boost);
                var immuneGroupCount = result.Count(x => x.GroupType == GroupType.ImmuneSystem);
                var infectionGroupCount = result.Count(x => x.GroupType == GroupType.Infection);
                immuneSystemWins = infectionGroupCount == 0;

                if (immuneSystemWins) WriteLine("Immune system WINS!");
                else WriteLine("Immune system LOSES!");
                WriteLine(result.Sum(x => x.UnitCount));
                WriteLine();
                boost ++;
            }
        }

        public IEnumerable<Group> Fight(List<Group> allGroups, int boost)
        {
            long totalUnitLoss = 0;

            while(allGroups.Any(x => x.GroupType == GroupType.Infection) && allGroups.Any(x => x.GroupType == GroupType.ImmuneSystem))
            {
                var immuneSystemGroups = allGroups.Where(x => x.GroupType == GroupType.ImmuneSystem).ToList();
                var infectionGroups = allGroups.Where(x => x.GroupType == GroupType.Infection).ToList();
                var selectingOrder = allGroups.OrderByDescending(x => x.EffectivePower).ThenByDescending(x => x.Initiative);

                var fights = new List<(Group attacker, Group defender)>();

                foreach (var attacker in selectingOrder)
                {
                    var potentialDefenders = attacker.GroupType == GroupType.Infection ? immuneSystemGroups : infectionGroups;

                    Group defender = null;

                    var defenders = potentialDefenders.Where(x => x.WeakTo.Contains(attacker.DamageType));
                    if (!defenders.Any())
                    {
                        defenders = potentialDefenders.Where(x => !x.ImmuneTo.Contains(attacker.DamageType));
                    }

                    if (defenders != null)
                    {
                        defender = defenders
                            .OrderByDescending(x => x.EffectivePower)
                            .ThenByDescending(x => x.Initiative)
                            .FirstOrDefault();
                    }

                    if (defender != null)
                    {
                        fights.Add((attacker, defender));
                        potentialDefenders.Remove(defender);
                    }
                }

                totalUnitLoss = 0;
                checked {
                foreach (var f in fights.OrderByDescending(x => x.attacker.Initiative))
                {
                    var damage = f.attacker.EffectivePower;
                    if (damage < 1) continue;

                    if (f.defender.WeakTo.Contains(f.attacker.DamageType)) damage *= 2;
                    var unitLoss = damage / f.defender.HitPoints;

                    //WriteLine($"{f.attacker.GroupType} {f.attacker.Id} deals {damage} damage (kills {unitLoss} units) to {f.defender.GroupType} {f.defender.Id}");
                    f.defender.UnitCount -= unitLoss;
                    if (f.defender.UnitCount <= 0)
                    {
                        allGroups.Remove(f.defender);
                    }

                    totalUnitLoss += unitLoss;
                }
                }

                if (totalUnitLoss == 0) break;
            }

            //WriteLine(allGroups.Sum(x => x.UnitCount));
            return allGroups;
        }

        public IEnumerable<Group> ParseInput(string filename)
        {
            var immuneSystemGroups = new List<Group>();
            var infectionGroups = new List<Group>();

            bool immuneSystem = false;
            var regex = new Regex(
                @"^(?<unitCount>\d+) units each with " +
                @"(?<hitPoints>\d+) hit points \(?" +
                @"(immune to (?<immuneTo>[a-z\s,]+))?" + @"(;\s)?" +
                @"(weak to (?<weakTo>[a-z\s,]+))?" +
                @"\)?\s?with an attack that does (?<damagePoints>\d+) (?<damageType>[a-z]+) damage " +
                @"at initiative (?<initiative>\d+)$");

            int immuneId = 1;
            int infectionId = 1;

            foreach (var line in File.ReadAllLines(filename).Select(x => x.Trim()))
            {
                if (string.IsNullOrEmpty(line)) continue;

                if (line == "Immune System:")
                {
                    immuneSystem = true;
                    continue;
                }

                if (line == "Infection:")
                {
                    immuneSystem = false;
                    continue;
                }

                var match = regex.Matches(line).FirstOrDefault();
                if (match == null) throw new Exception("No match!");
                var groups = match.Groups;

                var unitCount = long.Parse(groups["unitCount"].Value);
                var hitPoints = long.Parse(groups["hitPoints"].Value);
                var initiative = long.Parse(groups["initiative"].Value);
                var damageHitPoints = long.Parse(groups["damagePoints"].Value);
                var damageType = groups["damageType"].Value;
                var immuneTo = groups["immuneTo"].Value?.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
                var weakTo = groups["weakTo"].Value?.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
/*
                Write($"{unitCount} units with {hitPoints} HP, ");
                if (immuneTo.Any())
                {
                    var immuneText = string.Join(",", immuneTo);
                    Write($"immune to {immuneText}; ");
                }
                if (weakTo.Any())
                {
                    var weakText = string.Join(",", weakTo);
                    Write($"weak to {weakText}; ");
                }
                WriteLine($"doing {damageHitPoints} damage of type {damageType}.");
*/
                var group = new Group
                {
                    UnitCount = unitCount,
                    HitPoints = hitPoints,
                    Initiative = initiative,
                    DamageType = damageType,
                    DamageHitPoints = damageHitPoints,
                    ImmuneTo = immuneTo,
                    WeakTo = weakTo,
                    GroupType = immuneSystem ? GroupType.ImmuneSystem : GroupType.Infection,
                    Id = immuneSystem ? immuneId++ : infectionId++
                };

                yield return group;
            }
        }
    }

    public enum GroupType
    {
        ImmuneSystem,
        Infection
    }

    public class Group
    {
        public long UnitCount { get; set; }
        public long HitPoints { get; set; }
        public long Initiative { get; set; }
        public string DamageType { get; set; }
        public long DamageHitPoints { get; set; }

        public List<string> ImmuneTo { get; set; }
        public List<string> WeakTo { get; set; }

        public GroupType GroupType { get; set; }

        public int Id { get; set; }

        public long EffectivePower => UnitCount * DamageHitPoints;

        public Group Clone()
        {
            return new Group {
                UnitCount = this.UnitCount,
                HitPoints = this.HitPoints,
                Initiative = this.Initiative,
                DamageType = this.DamageType,
                DamageHitPoints = this.DamageHitPoints,

                ImmuneTo = this.ImmuneTo,
                WeakTo = this.WeakTo,

                GroupType = this.GroupType,

                Id = this.Id
            };
        }
    }
}
