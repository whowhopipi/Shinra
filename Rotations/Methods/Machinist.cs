﻿using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ShinraCo.Settings;
using ShinraCo.Spells.Main;
using Resource = ff14bot.Managers.ActionResourceManager.Machinist;

namespace ShinraCo.Rotations
{
    public sealed partial class Machinist
    {
        private MachinistSpells MySpells { get; } = new MachinistSpells();

        #region Damage

        private async Task<bool> SplitShot()
        {
            return await MySpells.SplitShot.Cast();
        }

        private async Task<bool> SlugShot()
        {
            if (Core.Player.HasAura("Enhanced Slug Shot"))
            {
                return await MySpells.SlugShot.Cast();
            }
            return false;
        }

        private async Task<bool> CleanShot()
        {
            if (Core.Player.HasAura("Cleaner Shot"))
            {
                return await MySpells.CleanShot.Cast();
            }
            return false;
        }

        private async Task<bool> HotShot()
        {
            if (!Core.Player.HasAura(MySpells.HotShot.Name, true, 6000))
            {
                return await MySpells.HotShot.Cast();
            }
            return false;
        }

        #endregion

        #region AoE

        private async Task<bool> SpreadShot()
        {
            if (Core.Player.CurrentTPPercent > 40)
            {
                if (Shinra.Settings.RotationMode == Modes.Multi || Shinra.Settings.RotationMode == Modes.Smart &&
                    Helpers.EnemiesNearTarget(5) > 2)
                {
                    return await MySpells.SpreadShot.Cast();
                }
            }
            return false;
        }

        #endregion

        #region Cooldown

        private async Task<bool> Heartbreak()
        {
            return await MySpells.Heartbreak.Cast();
        }

        private async Task<bool> Wildfire()
        {
            if (Shinra.Settings.MachinistWildfire && (Core.Player.CurrentTarget.IsBoss() ||
                                                      Core.Player.CurrentTarget.CurrentHealth > Shinra.Settings.MachinistWildfireHP))
            {
                return await MySpells.Wildfire.Cast();
            }
            return false;
        }

        private async Task<bool> GaussRound()
        {
            return await MySpells.GaussRound.Cast();
        }

        private async Task<bool> Ricochet()
        {
            if (Shinra.Settings.MachinistRicochet)
            {
                return await MySpells.Ricochet.Cast();
            }
            return false;
        }

        #endregion

        #region Buff

        private async Task<bool> Reload()
        {
            if (Shinra.Settings.MachinistReload)
            {
                if (Resource.Ammo == 0 && !Core.Player.HasAura("Enhanced Slug Shot") && !Core.Player.HasAura("Cleaner Shot") &&
                    Core.Player.HasAura(MySpells.HotShot.Name, true, 10000))
                {
                    return await MySpells.Reload.Cast();
                }
            }
            return false;
        }

        private async Task<bool> Reassemble()
        {
            if (Shinra.Settings.MachinistReassemble)
            {
                if (Core.Player.HasAura("Cleaner Shot") || !ActionManager.HasSpell(MySpells.CleanShot.Name) &&
                    Core.Player.HasAura("Enhanced Slug Shot"))
                {
                    return await MySpells.Reassemble.Cast();
                }
            }
            return false;
        }

        private async Task<bool> QuickReload()
        {
            if (Resource.Ammo < 2)
            {
                return await MySpells.QuickReload.Cast();
            }
            return false;
        }

        private async Task<bool> RapidFire()
        {
            if (Shinra.Settings.MachinistRapidFire)
            {
                return await MySpells.RapidFire.Cast();
            }
            return false;
        }

        private async Task<bool> GaussBarrel()
        {
            if (Shinra.Settings.MachinistGaussBarrel && !Resource.GaussBarrel)
            {
                return await MySpells.GaussBarrel.Cast(null, false);
            }
            return false;
        }

        private async Task<bool> Hypercharge()
        {
            if (Shinra.Settings.MachinistHypercharge && TurretExists)
            {
                return await MySpells.Hypercharge.Cast();
            }
            return false;
        }

        #endregion

        #region Turret

        private async Task<bool> RookAutoturret()
        {
            if (Shinra.Settings.MachinistTurret == MachinistTurrets.Rook || Shinra.Settings.MachinistTurret == MachinistTurrets.Bishop &&
                !ActionManager.HasSpell(MySpells.BishopAutoturret.Name))
            {
                if (PetManager.ActivePetType != PetType.Rook_Autoturret || TurretDistance > 20)
                {
                    return await MySpells.RookAutoturret.Cast();
                }
            }
            return false;
        }

        private async Task<bool> BishopAutoturret()
        {
            if (Shinra.Settings.MachinistTurret == MachinistTurrets.Bishop)
            {
                if (PetManager.ActivePetType != PetType.Bishop_Autoturret || TurretDistance > 20)
                {
                    return await MySpells.BishopAutoturret.Cast();
                }
            }
            return false;
        }

        #endregion

        #region Role

        private async Task<bool> SecondWind()
        {
            if (Shinra.Settings.MachinistSecondWind && Core.Player.CurrentHealthPercent < Shinra.Settings.MachinistSecondWindPct)
            {
                return await MySpells.Role.SecondWind.Cast();
            }
            return false;
        }

        private async Task<bool> Peloton()
        {
            if (Shinra.Settings.MachinistPeloton && !Core.Player.HasAura(MySpells.Role.Peloton.Name) && !Core.Player.HasTarget &&
                MovementManager.IsMoving)
            {
                return await MySpells.Role.Peloton.Cast(null, false);
            }
            return false;
        }

        private async Task<bool> Invigorate()
        {
            if (Shinra.Settings.MachinistInvigorate && Core.Player.CurrentTPPercent < Shinra.Settings.MachinistInvigoratePct)
            {
                return await MySpells.Role.Invigorate.Cast();
            }
            return false;
        }

        private async Task<bool> Tactician()
        {
            if (Shinra.Settings.MachinistTactician && Core.Player.CurrentTPPercent < Shinra.Settings.MachinistTacticianPct)
            {
                return await MySpells.Role.Tactician.Cast();
            }
            return false;
        }

        #endregion

        #region Custom

        private static bool TurretExists => Core.Player.Pet != null;
        private static float TurretDistance => TurretExists && Core.Player.HasTarget && Core.Player.CurrentTarget.CanAttack
            ? Core.Player.Pet.Distance2D(Core.Player.CurrentTarget) - Core.Player.CurrentTarget.CombatReach : 0;

        #endregion
    }
}