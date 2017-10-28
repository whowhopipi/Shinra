﻿using System.Threading.Tasks;

namespace ShinraCo.Rotations
{
    public sealed partial class Bard : Rotation
    {
        #region Combat

        public override async Task<bool> Combat()
        {
            if (await Opener()) return true;
            if (await IronJaws()) return true;
            if (await RefulgentArrow()) return true;
            if (await StraightShotBuff()) return true;
            if (await Windbite()) return true;
            if (await VenomousBite()) return true;
            if (await QuickNock()) return true;
            if (await StraightShot()) return true;
            return await HeavyShot();
        }

        #endregion
        
        #region CombatBuff

        public override async Task<bool> CombatBuff()
        {
            if (await Shinra.SummonChocobo()) return true;
            if (await Shinra.ChocoboStance()) return true;
            if (await Opener()) return true;
            // Songs
            if (await WanderersMinuet()) return true;
            if (await MagesBallad()) return true;
            if (await ArmysPaeon()) return true;
            // Buffs
            if (await BattleVoice()) return true;
            if (await RagingStrikes()) return true;
            if (await Barrage()) return true;
            // Off-GCDs
            if (await PitchPerfect()) return true;
            if (await MiserysEnd()) return true;
            if (await RainOfDeath()) return true;
            if (await Bloodletter()) return true;
            if (await EmpyrealArrow()) return true;
            if (await Sidewinder()) return true;
            // Role
            await Helpers.UpdateParty();
            if (await Palisade()) return true;
            if (await Refresh()) return true;
            if (await Tactician()) return true;
            return await Invigorate();
        }

        #endregion

        #region Heal

        public override async Task<bool> Heal()
        {
            return await SecondWind();
        }

        #endregion

        #region PreCombatBuff

        public override async Task<bool> PreCombatBuff()
        {
            if (await Shinra.SummonChocobo()) return true;
            return await Peloton();
        }

        #endregion

        #region Pull

        public override async Task<bool> Pull()
        {
            return await Combat();
        }

        #endregion
    }
}