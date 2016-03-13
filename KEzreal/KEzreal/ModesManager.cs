using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK.Utils;
using System.Linq;

namespace KEzreal
{
    internal class ModesManager : Program
    {
        public static void Combo()
        {
            // var Elogic = aa;

            var alvo = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (alvo == null) return;
            var useQ = ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue;
            var useW = ModesMenu1["ComboW"].Cast<CheckBox>().CurrentValue;
            var useE = ModesMenu1["ComboE"].Cast<CheckBox>().CurrentValue;
            var useR = ModesMenu1["ComboR"].Cast<CheckBox>().CurrentValue;
            var Qp = Q.GetPrediction(alvo);
            var Wp = W.GetPrediction(alvo);
            var Ep = E.GetPrediction(alvo);
            var Rp = R.GetPrediction(alvo);
            if (!alvo.IsValid()) return;
            if (ModesMenu1["useI"].Cast<CheckBox>().CurrentValue)
            {
                //Itens.UseItens();
            }

           
                if (Q.IsInRange(alvo) && Q.IsReady() && useQ && Qp.HitChance >= HitChance.High)
                {
                    Q.Cast(Qp.CastPosition);
                }
                if (W.IsInRange(alvo) && W.IsReady() && useW && Wp.HitChance >= HitChance.High)
                {
                    W.Cast(Wp.CastPosition);

                }
                if ((_Player.Distance(alvo) <= 1100) && E.IsReady() && useE)
                {
                    E.Cast(Game.CursorPos);
                }
                if (R.IsInRange(alvo) && R.IsReady() && useR)
                {
                    R.Cast(Rp.CastPosition);
                }
            }
        
        public static void Harass()
        {
            //Harass

            var alvo = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (alvo == null) return;
            var alvoR = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            var useQ = ModesMenu1["HarassQ"].Cast<CheckBox>().CurrentValue;
            var useW = ModesMenu1["HarassW"].Cast<CheckBox>().CurrentValue;
            var useE = ModesMenu1["HarassE"].Cast<CheckBox>().CurrentValue;
            var useR = ModesMenu1["HarassR"].Cast<CheckBox>().CurrentValue;
            var Qp = Q.GetPrediction(alvo);
            var Wp = W.GetPrediction(alvo);
            var Ep = E.GetPrediction(alvo);
            var Rp = R.GetPrediction(alvoR);
            if (!alvo.IsValid() && alvo == null) return;
            if (ModesMenu1["useI"].Cast<CheckBox>().CurrentValue)
            {
                Itens.UseItens();
            }


            if (Q.IsInRange(alvo) && Q.IsReady() && useQ && Qp.HitChance >= HitChance.High)
            {
                Q.Cast(Qp.CastPosition);
            }
            if (W.IsInRange(alvo) && W.IsReady() && useW && Wp.HitChance >= HitChance.High)
            {
                W.Cast(Wp.CastPosition);

            }
            if ((_Player.Distance(alvo) <= 1100) && E.IsReady() && useE && Ep.HitChance >= HitChance.High)
            {
                E.Cast(Ep.CastPosition);
            }
            if (R.IsInRange(alvo) && R.IsReady() && useR && Rp.HitChance >= HitChance.High)
             {
                 R.Cast(Rp.CastPosition);
             }
        }
         public static void LaneClear()
        {
            var useQ = ModesMenu2["FarmQ"].Cast<CheckBox>().CurrentValue;
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Q.Range));
            var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, W.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
            if (minions == null) return;
            if ((_Player.ManaPercent <= Program.ModesMenu2["ManaF"].Cast<Slider>().CurrentValue))
            {
                return;
            }

            if (useQ && Q.IsReady() && Q.IsInRange(minions))
            {
                Q.Cast(minions);
            }

         }
         public static void JungleClear()
         {

             var useQ = ModesMenu2["JungQ"].Cast<CheckBox>().CurrentValue;
             var jungleMonsters = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(Program.Q.Range));
             var minioon = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, Program.Q.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
             if (jungleMonsters == null) return; 
             if ((Program._Player.ManaPercent <= Program.ModesMenu2["ManaJ"].Cast<Slider>().CurrentValue))
             {
                 return;
             }
             var Qp = Q.GetPrediction(jungleMonsters);
             if (jungleMonsters == null) return;
             if (useQ && Q.IsReady() && Q.IsInRange(jungleMonsters))
             {
                 Q.Cast(Qp.CastPosition);
             }
    
         }
         public static void LastHit()
         {

             var useQ = Program.ModesMenu2["LastQ"].Cast<CheckBox>().CurrentValue;
             var qminions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget((Program.Q.Range)) && (DamageLib.QCalc(m) > m.Health));
             if (qminions == null) return;
             if ((Program._Player.ManaPercent <= Program.ModesMenu2["ManaL"].Cast<Slider>().CurrentValue))
             {
                 return;
             }

             if (Q.IsReady() && (Program._Player.Distance(qminions) <= Program._Player.GetAutoAttackRange()) && useQ && qminions.Health < DamageLib.QCalc(qminions))
             {
                 Q.Cast(qminions);
             }

         }
         public static void KillSteal()
         {

            
             foreach (var enemy in EntityManager.Heroes.Enemies.Where(a => !a.IsDead && !a.IsZombie && a.Health > 0))
             {
                 if (enemy == null) return;
                 var Qp = Q.GetPrediction(enemy);
                 var Wp = W.GetPrediction(enemy);
                 var Ep = E.GetPrediction(enemy);
                 var Rp = R.GetPrediction(enemy);
            
                 if (enemy.IsValidTarget(R.Range) && enemy.HealthPercent <= 40)
                 {

                     if (DamageLib.QCalc(enemy) + DamageLib.WCalc(enemy) + DamageLib.RCalc(enemy)>= enemy.Health)
                     {
                         if (Q.IsReady() && Q.IsInRange(enemy) && Program.ModesMenu1["KQ"].Cast<CheckBox>().CurrentValue)
                         {
                             Q.Cast(Qp.CastPosition);
                         }
                         if (W.IsReady() && W.IsInRange(enemy) && Program.ModesMenu1["KW"].Cast<CheckBox>().CurrentValue)
                         {
                             W.Cast(Wp.CastPosition);
                         }
                         if (E.IsReady() && E.IsInRange(enemy) && Program.ModesMenu1["KE"].Cast<CheckBox>().CurrentValue)
                         {
                             E.Cast(Ep.CastPosition);
                         }
                         if (R.IsReady() && R.IsInRange(enemy) && Program.ModesMenu1["KR"].Cast<CheckBox>().CurrentValue)
                         {
                             R.Cast(Rp.CastPosition);
                         }
                     }
                 }
             }
         }
         public static void AutoQ()
         {
             var alvo = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
             if (alvo == null) return;
             var useQ = ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue;
             var useW = ModesMenu1["ComboW"].Cast<CheckBox>().CurrentValue;
             var useE = ModesMenu1["ComboE"].Cast<CheckBox>().CurrentValue;
             var useR = ModesMenu1["ComboR"].Cast<CheckBox>().CurrentValue;
             var Qp = Q.GetPrediction(alvo);
             var Wp = W.GetPrediction(alvo);
             var Ep = E.GetPrediction(alvo);
             var Rp = R.GetPrediction(alvo);
             if (!alvo.IsValid()) return;
             if (Q.IsInRange(alvo) && Q.IsReady() && useQ && Qp.HitChance >= HitChance.High)
             {
                 Q.Cast(Qp.CastPosition);
             }
         }
    }
}
