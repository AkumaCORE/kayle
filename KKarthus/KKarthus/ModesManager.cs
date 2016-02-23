using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK.Utils;
using System.Linq;
using SharpDX;

namespace KKarthus
{
    internal class ModesManager
    {
        public static void Combo()
        {
            var Q = Program.Q;
            var W = Program.W;
            var E = Program.E;
            var useQ = Program.ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue;
            var useW = Program.ModesMenu1["ComboW"].Cast<CheckBox>().CurrentValue;
            var useE = Program.ModesMenu1["ComboE"].Cast<CheckBox>().CurrentValue;
            var AA = Program.Misc["AA"].Cast<CheckBox>().CurrentValue;
            var alvo = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (!alvo.IsValid()) return;

            if(AA)
            {
                Orbwalker.DisableAttacking = true;
            }
            if (useW && W.IsInRange(alvo) && W.IsReady())
            {
                W.Cast(alvo);
            }
            if (useQ && Q.IsReady() && Q.IsInRange(alvo))
            {
                Q.Cast(alvo);
            }
            if (useE && E.IsReady() && E.IsInRange(alvo) && Program._Player.HasBuff(""))
            { 
                E.Cast(alvo);
            }
            if (useE && E.IsReady() && E.IsInRange(alvo))
            {
                E.Cast(alvo);
            }
            

        }
        public static void Harass()
        {
            var Q = Program.Q;
            var useQ = Program.ModesMenu1["HarassQ"].Cast<CheckBox>().CurrentValue;
            var alvo = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (!alvo.IsValid()) return;
           if ((Program._Player.ManaPercent <= Program.ModesMenu1["ManaH"].Cast<Slider>().CurrentValue))
            {
                return;
            }
            if (useQ && Q.IsReady() && Q.IsInRange(alvo))
            {
                Q.Cast(alvo);
            }

        }
        public static void LaneClear()
        {
            var Q = Program.Q;
            var W = Program.W;
            var E = Program.E;
            var useQ = Program.ModesMenu2["FarmQ"].Cast<CheckBox>().CurrentValue;
            var useE = Program.ModesMenu2["FarmE"].Cast<CheckBox>().CurrentValue;
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Program.Q.Range));           
            if ((Program._Player.ManaPercent <= Program.ModesMenu2["ManaF"].Cast<Slider>().CurrentValue))
            {
                return;
            }
            if (useQ && Q.IsReady() && Q.IsInRange(minions))
            {
                Q.Cast(minions);
            }
            if (useE && E.IsReady() && E.IsInRange(alvo) && Program._Player.HasBuff(""))
            {
                E.Cast(alvo);
            }
        }
        public static void LastHit()
        { }
        public static void JungleClear()
        { }
        public static void KillSteal()
        {
            var alvo = TargetSelector.GetTarget(int.MaxValue, DamageType.Magical);
            if (!alvo.IsValid()) return;
            var R = Program.R;
            var useR = Program.Ks["KR"].Cast<CheckBox>().CurrentValue;
            if (useR && R.IsReady() && alvo.Health < DamageLib.RCalc(alvo))
            {
                R.Cast(alvo);
            }
        }
    }
}