﻿using System.Threading.Tasks;
using ShinraCo.Settings;

namespace ShinraCo.Rotations
{
    public sealed partial class Ninja : Rotation
    {
        #region Combat

        public override async Task<bool> Combat()
        {
            if (Shinra.Settings.RotationMode == Modes.Smart)
            {
                if (await TenChiJinBuff()) return true;
                if (await Huton()) return true;
                if (await Doton()) return true;
                if (await Katon()) return true;
                if (await Suiton()) return true;
                if (await Raiton()) return true;
                if (await FumaShuriken()) return true;
                if (await DeathBlossom()) return true;
                if (await ShadowFang()) return true;
                if (await ArmorCrush()) return true;
                if (await AeolianEdge()) return true;
                if (await GustSlash()) return true;
                return await SpinningEdge();
            }
            if (Shinra.Settings.RotationMode == Modes.Single)
            {
                if (await TenChiJinBuff()) return true;
                if (await Huton()) return true;
                if (await Suiton()) return true;
                if (await Raiton()) return true;
                if (await FumaShuriken()) return true;
                if (await ShadowFang()) return true;
                if (await ArmorCrush()) return true;
                if (await AeolianEdge()) return true;
                if (await GustSlash()) return true;
                return await SpinningEdge();
            }
            if (Shinra.Settings.RotationMode == Modes.Multi)
            {
                if (await TenChiJinBuff()) return true;
                if (await Huton()) return true;
                if (await Doton()) return true;
                if (await Katon()) return true;
                if (await FumaShuriken()) return true;
                if (await DeathBlossom()) return true;
                if (await ShadowFang()) return true;
                if (await ArmorCrush()) return true;
                if (await AeolianEdge()) return true;
                if (await GustSlash()) return true;
                return await SpinningEdge();
            }
            return false;
        }

        #endregion

        #region CombatBuff

        public override async Task<bool> CombatBuff()
        {
            if (await Shinra.SummonChocobo()) return true;
            if (await Shinra.ChocoboStance()) return true;
            if (await ShadeShift()) return true;
            if (await Assassinate()) return true;
            if (await Mug()) return true;
            if (await Jugulate()) return true;
            if (await Kassatsu()) return true;
            if (await TrickAttack()) return true;
            if (await DreamWithinADream()) return true;
            if (await HellfrogMedium()) return true;
            if (await Bhavacakra()) return true;
            if (await TenChiJin()) return true;
            if (await TrueNorth()) return true;
            return await Invigorate();
        }

        #endregion

        #region Heal

        public override async Task<bool> Heal()
        {
            if (await Shinra.UsePotion()) return true;
            if (await SecondWind()) return true;
            return await Bloodbath();
        }

        #endregion

        #region PreCombatBuff

        public override async Task<bool> PreCombatBuff()
        {
            if (await Shinra.SummonChocobo()) return true;
            return await Huton();
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