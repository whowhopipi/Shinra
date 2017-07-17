﻿using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ShinraCo.Spells.Main;

namespace ShinraCo.Rotations
{
    public sealed partial class RedMage
    {
        private RedMageSpells MySpells { get; } = new RedMageSpells();

        #region Damage

        private async Task<bool> Riposte()
        {
            if (!ActionManager.HasSpell(MySpells.Jolt.Name))
            {
                return await MySpells.Riposte.Cast();
            }
            return false;
        }

        private async Task<bool> Jolt()
        {
            if (!ActionManager.HasSpell(MySpells.JoltII.Name))
            {
                return await MySpells.Jolt.Cast();
            }
            return false;
        }

        private async Task<bool> JoltII()
        {
            return await MySpells.JoltII.Cast();
        }

        private async Task<bool> Impact()
        {
            return await MySpells.Impact.Cast();
        }

        private async Task<bool> Verthunder()
        {
            if (Core.Player.HasAura("Dualcast") || Core.Player.HasAura("Swiftcast"))
            {
                return await MySpells.Verthunder.Cast();
            }
            return false;
        }

        private async Task<bool> Veraero()
        {
            if ((Core.Player.HasAura("Dualcast") || Core.Player.HasAura("Swiftcast")) && BlackMana > WhiteMana)
            {
                return await MySpells.Veraero.Cast();
            }
            return false;
        }

        private async Task<bool> Verfire()
        {
            return await MySpells.Verfire.Cast();
        }

        private async Task<bool> Verstone()
        {
            return await MySpells.Verstone.Cast();
        }

        private async Task<bool> EnchantedRiposte()
        {
            if (WhiteMana >= 80 && BlackMana >= 80 && Core.Player.TargetDistance(5, false))
            {
                return await MySpells.EnchantedRiposte.Cast();
            }
            return false;
        }

        private async Task<bool> EnchantedZwerchhau()
        {
            if (ActionManager.LastSpell.Name == MySpells.Riposte.Name && Core.Player.TargetDistance(5, false))
            {
                return await MySpells.EnchantedZwerchhau.Cast();
            }
            return false;
        }

        private async Task<bool> EnchantedRedoublement()
        {
            if (ActionManager.LastSpell.Name == MySpells.Zwerchhau.Name && Core.Player.TargetDistance(5, false))
            {
                return await MySpells.EnchantedRedoublement.Cast();
            }
            return false;
        }

        private async Task<bool> Verflare()
        {
            if (WhiteMana >= BlackMana)
            {
                return await MySpells.Verflare.Cast();
            }
            return false;
        }

        private async Task<bool> Verholy()
        {
            if (BlackMana >= WhiteMana)
            {
                return await MySpells.Verholy.Cast();
            }
            return false;
        }

        #endregion

        #region AoE

        private async Task<bool> Scatter()
        {
            if (ActionManager.HasSpell(MySpells.EnchantedMoulinet.Name))
            {
                return await MySpells.Scatter.Cast();
            }
            return false;
        }

        private async Task<bool> EnchantedMoulinet()
        {
            if (WhiteMana >= 30 && BlackMana >= 30)
            {
                return await MySpells.EnchantedMoulinet.Cast();
            }
            return false;
        }

        #endregion

        #region Cooldown

        private async Task<bool> CorpsACorps()
        {
            if (Shinra.Settings.RedMageCorpsACorps)
            {
                if (!MovementManager.IsMoving && WhiteMana >= 80 && BlackMana >= 80 && Core.Player.TargetDistance(5))
                {
                    return await MySpells.CorpsACorps.Cast();
                }
            }
            return false;
        }

        private async Task<bool> Fleche()
        {
            if (UseOffGCD)
            {
                return await MySpells.Fleche.Cast();
            }
            return false;
        }

        private async Task<bool> ContreSixte()
        {
            if (UseOffGCD)
            {
                return await MySpells.ContreSixte.Cast();
            }
            return false;
        }

        #endregion

        #region Buff

        private async Task<bool> Acceleration()
        {
            if (UseOffGCD)
            {
                return await MySpells.Acceleration.Cast();
            }
            return false;
        }

        private async Task<bool> Embolden()
        {
            if (Shinra.Settings.RedMageEmbolden && Shinra.LastSpell.Name == MySpells.EnchantedRiposte.Name)
            {
                return await MySpells.Embolden.Cast(null, false);
            }
            return false;
        }

        private async Task<bool> Manafication()
        {
            if (Shinra.Settings.RedMageManafication)
            {
                if (UseOffGCD && WhiteMana >= 40 && WhiteMana < 60 && BlackMana >= 40 && BlackMana < 60)
                {
                    return await MySpells.Manafication.Cast();
                }
            }
            return false;
        }

        #endregion

        #region Heal

        private async Task<bool> Vercure()
        {
            if (Shinra.Settings.RedMageVercure && Core.Player.CurrentHealthPercent < Shinra.Settings.RedMageVercurePct)
            {
                var target = Core.Player;

                if (target != null)
                {
                    return await MySpells.Vercure.Cast(target);
                }
            }
            return false;
        }

        #endregion

        #region Role

        private async Task<bool> Drain()
        {
            if (Shinra.Settings.RedMageDrain && Core.Player.CurrentHealthPercent < Shinra.Settings.RedMageDrainPct)
            {
                return await MySpells.Role.Drain.Cast();
            }
            return false;
        }

        private async Task<bool> LucidDreaming()
        {
            if (Shinra.Settings.RedMageLucidDreaming && Core.Player.CurrentManaPercent < Shinra.Settings.RedMageLucidDreamingPct)
            {
                return await MySpells.Role.LucidDreaming.Cast();
            }
            return false;
        }

        private async Task<bool> Swiftcast()
        {
            if (Shinra.Settings.RedMageSwiftcast && UseOffGCD && !Core.Player.HasAura("Verfire Ready") &&
                !Core.Player.HasAura("Verstone Ready"))
            {
                return await MySpells.Role.Swiftcast.Cast();
            }
            return false;
        }

        #endregion

        #region Custom

        private static int WhiteMana => ActionResourceManager.RedMage.WhiteMana;
        private static int BlackMana => ActionResourceManager.RedMage.BlackMana;

        private static bool UseOffGCD => ActionManager.LastSpell.Name == "Veraero" || ActionManager.LastSpell.Name == "Verthunder" ||
                                         ActionManager.LastSpell.Name == "Scatter";

        #endregion
    }
}