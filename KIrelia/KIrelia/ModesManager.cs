using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK.Utils;
using System.Linq;

namespace KIrelia
{
    internal class ModesManager : Program
    {
        public static void Combo()
        {
          var alvo = TargetSelector.GetTarget(1000, DamageType.Physical);
          if (!alvo.IsValid()) return;
          if (alvo == null) return;
          var ComboMode = Program.ComboMode;
          var GapMode = Program.GapMode;
          var GapMin = Program.GapMin;
          var useQ = Program.ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue;
          var useW = Program.ModesMenu1["ComboW"].Cast<CheckBox>().CurrentValue;
          var useE = Program.ModesMenu1["ComboE"].Cast<CheckBox>().CurrentValue;
         // var useR = Program.ModesMenu1["ComboR"].Cast<CheckBox>().CurrentValue;
         
            //Pro Combo
          if (ComboMode.CurrentValue == 2)
          {
              var distentrealvo = _Player.ServerPosition.Distance(alvo.ServerPosition);
              if (distentrealvo > GapMin.CurrentValue)
              {
                  if (Q.IsReady() && useQ)
                  {
                      if (distentrealvo < 650)
                      {
                          Q.Cast(alvo);
                      }
                      else
                      {
                          if (GapMode.CurrentValue == 1)
                          {
                              var gapminion = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget((Program.Q.Range)) && (DamageLib.QCalc(m) > m.Health)); ;

                              if (gapminion != null)
                              {

                                  Q.Cast(gapminion);
                              }
                          }
                      }
                  }
              }

              if (E.IsReady())
              {
                  if (_Player.HealthPercent < alvo.HealthPercent)
                  {
                      E.Cast(alvo);
                  }
                  if (alvo.HealthPercent < _Player.HealthPercent && alvo.MoveSpeed > _Player.MoveSpeed - 5 && _Player.ServerPosition.Distance(alvo.ServerPosition) < 300)
                  {
                      E.Cast(alvo);
                  }
              }
          }
            //Fim Pro Combo
            //Inicio Normal Combo
          if (ComboMode.CurrentValue == 1)
          { 
           var distentrealvo = _Player.ServerPosition.Distance(alvo.ServerPosition);
           if (distentrealvo > GapMin.CurrentValue)
           {
               if (Q.IsReady())
               {
                   if (distentrealvo < 650)
                   {
                       Q.Cast(alvo);
                   }
               }
           }
           if (E.IsReady() && E.IsInRange(alvo) && useE)
           {
               E.Cast(alvo);
           }



          
          }
            //Fim Normal Combo
        }

        public static void Harass()
        {
            var alvo = TargetSelector.GetTarget(1000, DamageType.Physical);
            if (!alvo.IsValid()) return;
            if (alvo == null) return;
            var useQ = Program.ModesMenu1["HarassQ"].Cast<CheckBox>().CurrentValue;
            var useE = Program.ModesMenu1["HarassE"].Cast<CheckBox>().CurrentValue;
            if (Q.IsReady() && Q.IsInRange(alvo) && useQ)
            {
                Q.Cast(alvo);
            }

            if (E.IsReady() && E.IsInRange(alvo) && useE)
            {
                E.Cast(alvo);
            }

        }

        public static void LaneClear()
        {
            var useQ = Program.ModesMenu2["FarmQ"].Cast<CheckBox>().CurrentValue;
            var qminions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget((Program.Q.Range)) && (DamageLib.QCalc(m) > m.Health));
            if (qminions == null) return;
            if ((Program._Player.ManaPercent <= Program.ModesMenu2["ManaF"].Cast<Slider>().CurrentValue))
            {
                return;
            }
            if (Q.IsReady() && Q.IsInRange(qminions) && useQ && qminions.Health < DamageLib.QCalc(qminions))
            {
                Q.Cast(qminions);
            }
            


        }
        public static void LastHit()
        {
            var useQ = Program.ModesMenu2["FarmQ"].Cast<CheckBox>().CurrentValue;
            var qminions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget((Program.Q.Range)) && (DamageLib.QCalc(m) > m.Health));
            if (qminions == null) return;
            if ((Program._Player.ManaPercent <= Program.ModesMenu2["ManaL"].Cast<Slider>().CurrentValue))
            {
                return;
            }
            if (Q.IsReady() && Q.IsInRange(qminions) && useQ && qminions.Health < DamageLib.QCalc(qminions))
            {
                Q.Cast(qminions);
            }



        }

        public static void JungleClear()
        {
            var useQ = Program.ModesMenu2["JungQ"].Cast<CheckBox>().CurrentValue;
            var useW = Program.ModesMenu2["JungW"].Cast<CheckBox>().CurrentValue;
            var useE = Program.ModesMenu2["JungE"].Cast<CheckBox>().CurrentValue;
            
            var jungleMonsters = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(Program.Q.Range));
            var minioon = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, Program.Q.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
            if (jungleMonsters == null) return;
            if ((Program._Player.ManaPercent <= Program.ModesMenu2["ManaJ"].Cast<Slider>().CurrentValue))
            {
                return;
            }
            if (Q.IsReady() && Q.IsInRange(jungleMonsters) && useQ)
            {
                Q.Cast(jungleMonsters);
            }
            if (W.IsReady() && _Player.Position.Distance(jungleMonsters) <= _Player.GetAutoAttackRange() && useW)
            {
                W.Cast();
            }
            if (E.IsInRange(jungleMonsters) && E.IsReady() && _Player.HealthPercent < jungleMonsters.HealthPercent && useE)
            {
                E.Cast(jungleMonsters);
            }
         
        }
        public static void KillSteal()
        {
            foreach (var enemy in EntityManager.Heroes.Enemies.Where(a => !a.IsDead && !a.IsZombie && a.Health > 0))
             {
                 if (enemy == null) return;
              
            
                 if (enemy.IsValidTarget(R.Range) && enemy.HealthPercent <= 40)
                 {

                     if (DamageLib.QCalc(enemy) + DamageLib.WCalc(enemy) + DamageLib.RCalc(enemy)>= enemy.Health)
                     {
                         if (Q.IsReady() && Q.IsInRange(enemy) && Program.ModesMenu1["KQ"].Cast<CheckBox>().CurrentValue)
                         {
                             Q.Cast(enemy);
                         }
                         if (W.IsReady() && _Player.Position.Distance(enemy) <= _Player.GetAutoAttackRange() && Program.ModesMenu1["KW"].Cast<CheckBox>().CurrentValue)
                         {
                             W.Cast();
                         }
                         if (E.IsReady() && E.IsInRange(enemy) && Program.ModesMenu1["KE"].Cast<CheckBox>().CurrentValue)
                         {
                             E.Cast(enemy);
                         }
                         /*if (R.IsReady() && R.IsInRange(enemy) && Program.ModesMenu1["KR"].Cast<CheckBox>().CurrentValue && Rp.HitChancePercent >= 90)
                         {
                             R.Cast(Rp.CastPosition);
                         }*/
                     }
                 }
             }
         }

        public static void RLogic()
        {
            var alvo = TargetSelector.GetTarget(1000, DamageType.Physical);
            if (!alvo.IsValid()) return;
            if (alvo == null) return;
            var useR = Program.ModesMenu1["ComboR"].Cast<CheckBox>().CurrentValue;
            if (_Player.HasBuff("ireliatranscendentbladesspell") && useR)
            {
                R.Cast(R.GetPrediction(alvo).CastPosition);
            }
         
        }

        }
    }

