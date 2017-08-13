﻿using System.Collections.Generic;
using System.Linq;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;

namespace ShinraCo
{
    public static partial class Helpers
    {
        public static readonly HashSet<string> HealingSpells = new HashSet<string>
        {
            "Benefic", "Benefic II", "Helios", "Aspected Helios", "Ascend",
            "Cure", "Cure II", "Medica", "Medica II", "Raise",
            "Physick", "Adloquium", "Succor", "Resurrection"
        };

        public static IEnumerable<BattleCharacter> PartyMembers
        {
            get
            {
                return PartyManager.VisibleMembers.Select(pm => pm.GameObject as BattleCharacter)
                                   .Where(pm => pm != null && pm.IsTargetable && pm.InLineOfSight() &&
                                                Core.Player.UnitDistance(pm, 25, false));
            }
        }

        public static IEnumerable<BattleCharacter> HealManager
        {
            get
            {
                return GameObjectManager.GetObjectsOfType<BattleCharacter>(true, true)
                                        .Where(hm => hm.IsAlive && (PartyMembers.Contains(hm) || hm == Core.Player ||
                                                                    hm == ChocoboManager.Object)).OrderBy(HPScore);
            }
        }

        private static float HPScore(BattleCharacter c)
        {
            var score = c.CurrentHealthPercent;

            if (c.IsTank())
            {
                score -= 5f;
            }
            if (c.IsHealer())
            {
                score -= 3f;
            }
            return score;
        }

        public static bool IsTank(this Character c)
        {
            switch (c.CurrentJob)
            {
                case ClassJobType.DarkKnight:
                case ClassJobType.Gladiator:
                case ClassJobType.Marauder:
                case ClassJobType.Paladin:
                case ClassJobType.Warrior:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsHealer(this Character c)
        {
            switch (c.CurrentJob)
            {
                case ClassJobType.Arcanist:
                case ClassJobType.Astrologian:
                case ClassJobType.Conjurer:
                case ClassJobType.Scholar:
                case ClassJobType.WhiteMage:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsDPS(this Character c)
        {
            return !c.IsTank() && !c.IsHealer();
        }
    }
}