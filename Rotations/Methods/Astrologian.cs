﻿using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ShinraCo.Settings;
using ShinraCo.Spells.Main;
using Resource = ff14bot.Managers.ActionResourceManager.Astrologian;

namespace ShinraCo.Rotations
{
    public sealed partial class Astrologian
    {
        private AstrologianSpells MySpells { get; } = new AstrologianSpells();

        #region Damage

        private async Task<bool> Malefic()
        {
            if (!ActionManager.HasSpell(MySpells.MaleficII.Name))
            {
                return await MySpells.Malefic.Cast();
            }
            return false;
        }

        private async Task<bool> MaleficII()
        {
            if (!ActionManager.HasSpell(MySpells.MaleficIII.Name))
            {
                return await MySpells.MaleficII.Cast();
            }
            return false;
        }

        private async Task<bool> MaleficIII()
        {
            return await MySpells.MaleficIII.Cast();
        }

        #endregion

        #region DoT

        private async Task<bool> Combust()
        {
            if (!ActionManager.HasSpell(MySpells.CombustII.Name) && !Core.Player.CurrentTarget.HasAura(MySpells.Combust.Name, true, 4000))
            {
                return await MySpells.Combust.Cast();
            }
            return false;
        }

        private async Task<bool> CombustII()
        {
            if (!Core.Player.CurrentTarget.HasAura(MySpells.CombustII.Name, true, 4000))
            {
                return await MySpells.CombustII.Cast();
            }
            return false;
        }

        #endregion

        #region AoE

        private async Task<bool> Gravity()
        {
            if (Core.Player.CurrentManaPercent > 40)
            {
                return await MySpells.Gravity.Cast();
            }
            return false;
        }

        private async Task<bool> EarthlyStar()
        {
            if (Shinra.Settings.AstrologianEarthlyStar)
            {
                var count = Shinra.Settings.CustomAoE ? Shinra.Settings.CustomAoECount : 3;

                if (Shinra.Settings.RotationMode == Modes.Multi || Helpers.EnemiesNearTarget(8) >= count)
                {
                    return await MySpells.EarthlyStar.Cast();
                }
            }
            return false;
        }

        private async Task<bool> StellarDetonation()
        {
            if (Shinra.Settings.AstrologianEarthlyStar)
            {
                return await MySpells.StellarDetonation.Cast(null, false);
            }
            return false;
        }

        #endregion

        #region Buff

        private async Task<bool> Lightspeed()
        {
            return await MySpells.Lightspeed.Cast();
        }

        #endregion

        #region Heal

        private async Task<bool> UpdateHealing()
        {
            if (Shinra.Settings.AstrologianPartyHeal || Shinra.Settings.AstrologianStyle == AstrologianStyles.Party)
            {
                if (!await Helpers.UpdateHealManager())
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> StopCasting()
        {
            if (Shinra.Settings.AstrologianInterruptOverheal && Core.Player.IsCasting)
            {
                var target = GameObjectManager.GetObjectByObjectId(Core.Player.SpellCastInfo.TargetId);
                var spellName = Core.Player.SpellCastInfo.Name;

                if (target != null)
                {
                    if (spellName == MySpells.Benefic.Name && target.CurrentHealthPercent >= Shinra.Settings.AstrologianBeneficPct + 10 ||
                        spellName == MySpells.BeneficII.Name && target.CurrentHealthPercent >= Shinra.Settings.AstrologianBeneficIIPct + 10)
                    {
                        var debugSetting = spellName == MySpells.Benefic.Name ? Shinra.Settings.AstrologianBeneficPct
                            : Shinra.Settings.AstrologianBeneficIIPct;
                        Helpers.Debug($@"Target HP: {target.CurrentHealthPercent}, Setting: {debugSetting}, Adjusted: {debugSetting + 10}");

                        Logging.Write(Colors.Yellow, $@"[Shinra] Interrupting >>> {spellName}");
                        ActionManager.StopCasting();
                        await Coroutine.Wait(500, () => !Core.Player.IsCasting);
                    }
                }
            }
            return false;
        }

        private async Task<bool> Benefic()
        {
            if (Shinra.Settings.AstrologianBenefic)
            {
                var target = Shinra.Settings.AstrologianPartyHeal
                    ? Helpers.HealManager.FirstOrDefault(hm => hm.CurrentHealthPercent < Shinra.Settings.AstrologianBeneficPct)
                    : Core.Player.CurrentHealthPercent < Shinra.Settings.AstrologianBeneficPct ? Core.Player : null;

                if (target != null)
                {
                    return await MySpells.Benefic.Cast(target);
                }
            }
            return false;
        }

        private async Task<bool> BeneficII()
        {
            if (Shinra.Settings.AstrologianBeneficII)
            {
                var target = Shinra.Settings.AstrologianPartyHeal
                    ? Helpers.HealManager.FirstOrDefault(hm => hm.CurrentHealthPercent < Shinra.Settings.AstrologianBeneficIIPct)
                    : Core.Player.CurrentHealthPercent < Shinra.Settings.AstrologianBeneficIIPct ? Core.Player : null;

                if (target != null)
                {
                    return await MySpells.BeneficII.Cast(target);
                }
            }
            return false;
        }

        private async Task<bool> EssentialDignity()
        {
            if (Shinra.Settings.AstrologianEssDignity)
            {
                var target = Shinra.Settings.AstrologianPartyHeal
                    ? Helpers.HealManager.FirstOrDefault(hm => hm.CurrentHealthPercent < Shinra.Settings.AstrologianEssDignityPct)
                    : Core.Player.CurrentHealthPercent < Shinra.Settings.AstrologianEssDignityPct ? Core.Player : null;

                if (target != null)
                {
                    return await MySpells.EssentialDignity.Cast(target, false);
                }
            }
            return false;
        }

        private async Task<bool> AspectedBenefic()
        {
            if (Shinra.Settings.AstrologianAspBenefic && SectActive)
            {
                var target = Shinra.Settings.AstrologianPartyHeal
                    ? Helpers.HealManager.FirstOrDefault(hm => hm.CurrentHealthPercent < Shinra.Settings.AstrologianAspBeneficPct &&
                                                               !hm.HasAura(MySpells.AspectedBenefic.Name, true))
                    : Core.Player.CurrentHealthPercent < Shinra.Settings.AstrologianAspBeneficPct &&
                      !Core.Player.HasAura(MySpells.AspectedBenefic.Name, true) ? Core.Player : null;

                if (target != null)
                {
                    return await MySpells.AspectedBenefic.Cast(target);
                }
            }
            return false;
        }

        private async Task<bool> Helios()
        {
            if (Shinra.Settings.AstrologianHelios && Shinra.Settings.AstrologianPartyHeal && UseAoEHeals)
            {
                var count = Helpers.FriendsNearPlayer(Shinra.Settings.AstrologianHeliosPct);

                if (count > 2)
                {
                    return await MySpells.Helios.Cast();
                }
            }
            return false;
        }

        private async Task<bool> AspectedHelios()
        {
            if (Shinra.Settings.AstrologianAspHelios && Shinra.Settings.AstrologianPartyHeal && SectActive && UseAoEHeals &&
                !Core.Player.HasAura(MySpells.AspectedHelios.Name, true))
            {
                var count = Helpers.FriendsNearPlayer(Shinra.Settings.AstrologianAspHeliosPct);

                if (count > 2)
                {
                    return await MySpells.AspectedHelios.Cast();
                }
            }
            return false;
        }

        private async Task<bool> Ascend()
        {
            if (Shinra.Settings.AstrologianAscend &&
                (Shinra.Settings.AstrologianSwiftcast && ActionManager.CanCast(MySpells.Role.Swiftcast.Name, Core.Player) ||
                 !Helpers.HealManager.Any(hm => hm.CurrentHealthPercent < Shinra.Settings.AstrologianBeneficPct)))
            {
                var target = Helpers.RessManager.FirstOrDefault(pm => !pm.HasAura("Raise"));

                if (target != null)
                {
                    if (Shinra.Settings.AstrologianSwiftcast && ActionManager.CanCast(MySpells.Ascend.Name, target))
                    {
                        if (await MySpells.Role.Swiftcast.Cast(null, false))
                        {
                            await Coroutine.Wait(3000, () => Core.Player.HasAura(MySpells.Role.Swiftcast.Name));
                        }
                    }
                    return await MySpells.Ascend.Cast(target);
                }
            }
            return false;
        }

        #endregion

        #region Card

        private async Task<bool> Draw()
        {
            if (Shinra.Settings.AstrologianDraw)
            {
                if (CardBalance || CardArrow || CardSpear)
                {
                    var target = Shinra.Settings.AstrologianStyle == AstrologianStyles.Party
                        ? Helpers.HealManager.FirstOrDefault(hm => hm.IsDPS()) : Core.Player;

                    if (target != null)
                    {
                        return await MySpells.Draw.Cast(target);
                    }
                }
                return await MySpells.Draw.Cast();
            }
            return false;
        }

        private async Task<bool> Undraw()
        {
            if (Shinra.Settings.AstrologianDraw && !ActionManager.HasSpell(MySpells.MinorArcana.Name))
            {
                if (Shinra.Settings.AstrologianStyle == AstrologianStyles.Solo)
                {
                    if (CardEwer || CardSpire)
                    {
                        return await MySpells.Undraw.Cast();
                    }
                }
                else
                {
                    if (CardBole)
                    {
                        return await MySpells.Undraw.Cast();
                    }
                }
            }
            return false;
        }

        private async Task<bool> RoyalRoad()
        {
            if (Shinra.Settings.AstrologianDraw && !HasBuff)
            {
                if (Shinra.Settings.AstrologianStyle == AstrologianStyles.Solo)
                {
                    if (CardBole)
                    {
                        return await MySpells.RoyalRoad.Cast();
                    }
                }
                else
                {
                    if (CardEwer || CardSpire)
                    {
                        return await MySpells.RoyalRoad.Cast();
                    }
                }
            }
            return false;
        }

        private async Task<bool> Spread()
        {
            if (Shinra.Settings.AstrologianDraw && HasSpread && HasBuff)
            {
                var target = Shinra.Settings.AstrologianStyle == AstrologianStyles.Party
                    ? Helpers.HealManager.FirstOrDefault(hm => hm.IsDPS()) : Core.Player;

                if (target != null)
                {
                    return await MySpells.Spread.Cast(target);
                }
                return await MySpells.Spread.Cast();
            }
            if (Shinra.Settings.AstrologianDraw && !HasSpread && !HasBuff && (CardBalance || CardArrow || CardSpear))
            {
                return await MySpells.Spread.Cast();
            }
            return false;
        }

        private async Task<bool> Redraw()
        {
            if (Shinra.Settings.AstrologianDraw && !CardBalance && !CardArrow && !CardSpear)
            {
                return await MySpells.Redraw.Cast();
            }
            return false;
        }

        private async Task<bool> MinorArcana()
        {
            if (Shinra.Settings.AstrologianDraw && !HasArcana && !CardBalance && !CardArrow && !CardSpear)
            {
                return await MySpells.MinorArcana.Cast();
            }
            return false;
        }

        private async Task<bool> LordOfCrowns()
        {
            if (Shinra.Settings.AstrologianDraw && CardLord)
            {
                return await MySpells.LordOfCrowns.Cast();
            }
            return false;
        }

        private async Task<bool> LadyOfCrowns()
        {
            if (Shinra.Settings.AstrologianDraw && CardLady)
            {
                var target = Shinra.Settings.AstrologianPartyHeal ? Helpers.HealManager.FirstOrDefault(hm => hm.CurrentHealthPercent < 80)
                    : Core.Player.CurrentHealthPercent < 80 ? Core.Player : null;

                if (target != null)
                {
                    return await MySpells.LadyOfCrowns.Cast(target);
                }
            }
            return false;
        }

        private async Task<bool> SleeveDraw()
        {
            if (Shinra.Settings.AstrologianSleeveDraw)
            {
                return await MySpells.SleeveDraw.Cast();
            }
            return false;
        }

        #endregion

        #region Sect

        private async Task<bool> DiurnalSect()
        {
            if (Shinra.Settings.AstrologianSect == AstrologianSects.Diurnal ||
                Shinra.Settings.AstrologianSect == AstrologianSects.Nocturnal && !ActionManager.HasSpell(MySpells.NocturnalSect.Name))
            {
                if (!Core.Player.HasAura(MySpells.DiurnalSect.Name))
                {
                    return await MySpells.DiurnalSect.Cast();
                }
            }
            return false;
        }

        private async Task<bool> NocturnalSect()
        {
            if (Shinra.Settings.AstrologianSect == AstrologianSects.Nocturnal && !Core.Player.HasAura(MySpells.NocturnalSect.Name))
            {
                return await MySpells.NocturnalSect.Cast();
            }
            return false;
        }

        #endregion

        #region Role

        private async Task<bool> ClericStance()
        {
            if (Shinra.Settings.AstrologianClericStance)
            {
                return await MySpells.Role.ClericStance.Cast();
            }
            return false;
        }

        private async Task<bool> Protect()
        {
            if (Shinra.Settings.AstrologianProtect)
            {
                var target = Shinra.Settings.AstrologianPartyHeal
                    ? Helpers.HealManager.FirstOrDefault(hm => !hm.HasAura(MySpells.Role.Protect.Name) && hm.Type == GameObjectType.Pc)
                    : !Core.Player.HasAura(MySpells.Role.Protect.Name) ? Core.Player : null;

                if (target != null)
                {
                    if (Shinra.Settings.AstrologianSwiftcast && ActionManager.CanCast(MySpells.Role.Protect.Name, target))
                    {
                        if (await MySpells.Role.Swiftcast.Cast(null, false))
                        {
                            await Coroutine.Wait(3000, () => Core.Player.HasAura(MySpells.Role.Swiftcast.Name));
                        }
                    }
                    return await MySpells.Role.Protect.Cast(target);
                }
            }
            return false;
        }

        private async Task<bool> Esuna()
        {
            if (Shinra.Settings.AstrologianEsuna)
            {
                var target = Shinra.Settings.AstrologianPartyHeal ? Helpers.HealManager.FirstOrDefault(hm => hm.HasDispellable())
                    : Core.Player.HasDispellable() ? Core.Player : null;

                if (target != null)
                {
                    return await MySpells.Role.Esuna.Cast(target);
                }
            }
            return false;
        }

        private async Task<bool> LucidDreaming()
        {
            if (Shinra.Settings.AstrologianLucidDreaming && Core.Player.CurrentManaPercent < Shinra.Settings.AstrologianLucidDreamingPct)
            {
                return await MySpells.Role.LucidDreaming.Cast();
            }
            return false;
        }

        private async Task<bool> Swiftcast()
        {
            if (Shinra.Settings.AstrologianSwiftcast)
            {
                return await MySpells.Role.Swiftcast.Cast();
            }
            return false;
        }

        private async Task<bool> Largesse()
        {
            return await MySpells.Role.Largesse.Cast();
        }

        #endregion

        #region Custom

        private static bool HasCard => Resource.Cards[0] != Resource.AstrologianCard.None;
        private static bool HasBuff => Resource.Buff != Resource.AstrologianCardBuff.None;
        private static bool HasSpread => Resource.Cards[1] != Resource.AstrologianCard.None;
        private static bool HasArcana => Resource.Arcana != Resource.AstrologianCard.None;

        private static bool BuffDuration => Resource.Buff == Resource.AstrologianCardBuff.Duration;
        private static bool BuffPotency => Resource.Buff == Resource.AstrologianCardBuff.Potency;
        private static bool BuffShared => Resource.Buff == Resource.AstrologianCardBuff.Shared;

        private static bool CardBalance => Resource.Cards[0] == Resource.AstrologianCard.Balance;
        private static bool CardBole => Resource.Cards[0] == Resource.AstrologianCard.Bole;
        private static bool CardArrow => Resource.Cards[0] == Resource.AstrologianCard.Arrow;
        private static bool CardSpear => Resource.Cards[0] == Resource.AstrologianCard.Spear;
        private static bool CardEwer => Resource.Cards[0] == Resource.AstrologianCard.Ewer;
        private static bool CardSpire => Resource.Cards[0] == Resource.AstrologianCard.Spire;
        private static bool CardLord => Resource.Arcana == Resource.AstrologianCard.LordofCrowns;
        private static bool CardLady => Resource.Arcana == Resource.AstrologianCard.LadyofCrowns;

        private bool UseAoEHeals => Shinra.LastSpell.Name != MySpells.Helios.Name && Shinra.LastSpell.Name != MySpells.AspectedHelios.Name;
        private bool SectActive => Core.Player.HasAura(MySpells.DiurnalSect.Name) || Core.Player.HasAura(MySpells.NocturnalSect.Name);

        #endregion
    }
}