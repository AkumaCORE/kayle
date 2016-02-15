using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK.Utils;
using System.Linq;

namespace KTalon
{
    internal class ModesManager
    {


        public static void Combo()
        {
            var Q = Program.Q;
            var W = Program.W;
            var E = Program.E;
            var R = Program.R;
            var ComboMode = Program.ComboMode;
            var alvo = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            //var predPosW = Prediction.Position.PredictLinearMissile(alvo, W.Range, W.Width, W.CastDelay, W.Speed, int.MaxValue, null, false);
            if (!alvo.IsValid()) return;
            var useQ = Program.ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue;
            var useW = Program.ModesMenu1["ComboW"].Cast<CheckBox>().CurrentValue;
            var useE = Program.ModesMenu1["ComboE"].Cast<CheckBox>().CurrentValue;
            //var useE1 = Program.ModesMenu1["ActiveE"].Cast<CheckBox>().CurrentValue;
            var useR = Program.ModesMenu1["ComboR"].Cast<CheckBox>().CurrentValue;
            if (Program.ModesMenu1["useI"].Cast<CheckBox>().CurrentValue)
            {
                Itens.UseItens();
            }

            if (ComboMode.CurrentValue == 1)
            {
                if (useE && E.IsReady() && E.IsInRange(alvo) && (Q.IsReady() || E.IsReady()))
                {
                    E.Cast(alvo);
                }
                if (useQ && Q.IsReady())
                {
                    Q.Cast();

                }

                if (useW && W.IsReady() && W.IsInRange(alvo))
                {
                    W.Cast(alvo);
                }
                if (useR && R.IsReady() && R.IsInRange(alvo) && (DamageLib.RCalc(alvo) >= alvo.Health))
                {
                    R.Cast();
                }

            }


            /////Caso 2
            if (ComboMode.CurrentValue == 2)
            {
                var Rtarget = TargetSelector.GetTarget(1100, DamageType.Physical);
                var CastedR = Program.CastedR;
                
                if (!Rtarget.IsValid()) return;
                if (R.IsReady() && !(Program._Player.HasBuff("TalonDisappear")) && (Program._Player.Distance(alvo) <= E.Range + 300))
                {
                    R.Cast();
                    
                }
                if (useE && E.IsReady() && E.IsInRange(alvo) && (Q.IsReady() || E.IsReady()))
                {
                    E.Cast(alvo);
                }
                if (useQ && Q.IsReady())
                {
                    Q.Cast();
                }
                if (useW && W.IsReady() && W.IsInRange(alvo))
                {
                    W.Cast(alvo);
                }
            }

            //Caso 3
            if (ComboMode.CurrentValue == 3)
            {
                if (useE && E.IsReady() && E.IsInRange(alvo) && (Q.IsReady() || E.IsReady()))
                {
                    E.Cast(alvo);
                }

                if (R.IsReady())
                {
                    R.Cast();
                }
                if (useQ && Q.IsReady())
                {
                    Q.Cast();
                }
                if (useW && W.IsReady() && W.IsInRange(alvo))
                {
                    W.Cast(alvo);
                }

            }
            //Caso 4
            if (ComboMode.CurrentValue == 4)
            {
                if (useE && E.IsReady() && E.IsInRange(alvo) && (Q.IsReady() || E.IsReady()))
                {
                    E.Cast(alvo);
                }

                if (Q.IsReady())
                {
                    Q.Cast();
                }
                if (useR && Q.IsReady())
                {
                    R.Cast();
                }
                if (useW && W.IsReady() && W.IsInRange(alvo))
                {
                    W.Cast(alvo);
                }

            }

        }
        public static void Harass()
        {
            var Q = Program.Q;
            var W = Program.W;
            var E = Program.E;
            var R = Program.R;
            var useQ = Program.ModesMenu1["HarassQ"].Cast<CheckBox>().CurrentValue;
            var useW = Program.ModesMenu1["HarassW"].Cast<CheckBox>().CurrentValue;
            var useE = Program.ModesMenu1["HarassE"].Cast<CheckBox>().CurrentValue;
            var alvo = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            var predPosW = Prediction.Position.PredictLinearMissile(alvo, W.Range, W.Width, W.CastDelay, W.Speed, int.MaxValue, null, false);
            if (!alvo.IsValid()) return;
            if ((Program._Player.ManaPercent <= Program.ModesMenu1["ManaH"].Cast<Slider>().CurrentValue))
            {
                return;
            }
            if ((useE) && W.IsReady() && E.IsInRange(alvo))
            {
                E.Cast(alvo);
            }

            if (useQ && Q.IsReady() && Program._Player.Distance(alvo) <= Program._Player.GetAutoAttackRange() + 30)
            {
                Q.Cast();
            }
            if (useW && W.IsReady() && W.IsInRange(alvo))
            {
                W.Cast(alvo);
            }

        }
        public static void LaneClear()
        {
            var Q = Program.Q;
            var W = Program.W;
            var useQ = Program.ModesMenu2["FarmQ"].Cast<CheckBox>().CurrentValue;
            var useW = Program.ModesMenu2["FarmW"].Cast<CheckBox>().CurrentValue;
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(W.Range));
            var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, W.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
            if (minions == null) return;
            if ((Program._Player.ManaPercent <= Program.ModesMenu2["ManaF"].Cast<Slider>().CurrentValue))
            {
                return;
            }
            if (useQ && Q.IsReady() && W.IsInRange(minions))
            {
                Q.Cast();
            }
            if (useW && W.IsReady() && W.IsInRange(minions) && (minion >= Program.ModesMenu2["Minionw"].Cast<Slider>().CurrentValue))
            {
                W.Cast(minions);
            }


        }
        public static void JungleClear()
        {
            var Q = Program.Q;
            var W = Program.W;
            var useQ = Program.ModesMenu2["JungQ"].Cast<CheckBox>().CurrentValue;
            var useW = Program.ModesMenu2["JungW"].Cast<CheckBox>().CurrentValue;
            var jungleMonsters = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(j => j.Health).FirstOrDefault(j => j.IsValidTarget(Program.W.Range));
            var minioon = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, Program.W.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
            if ((Program._Player.ManaPercent <= Program.ModesMenu2["ManaJ"].Cast<Slider>().CurrentValue))
            {
                return;
            }
            if (jungleMonsters == null) return;
            if (useQ && Q.IsReady() && W.IsInRange(jungleMonsters))
            {
                Q.Cast();
            }
            if (useW && W.IsReady() && W.IsInRange(jungleMonsters))
            {
                W.Cast(jungleMonsters);
            }
        }
        public static void LastHit()
        {

            var Q = Program.Q;
            var W = Program.W;
            var useQ = Program.ModesMenu2["LastQ"].Cast<CheckBox>().CurrentValue;
            var useW = Program.ModesMenu2["LastW"].Cast<CheckBox>().CurrentValue;
            var qminions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget((Program.W.Range)) && (DamageLib.QCalc(m) > m.Health));
            var wminions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Program.W.Range) && (DamageLib.WCalc(m) > m.Health));
            //var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, Program.E.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
            if (qminions == null) return;
            if (wminions == null) return;
            if ((Program._Player.ManaPercent <= Program.ModesMenu2["ManaL"].Cast<Slider>().CurrentValue))
            {
                return;
            }
            var prediction = W.GetPrediction(wminions);
            if (W.IsReady() && W.IsInRange(qminions) && useW && wminions.Health < DamageLib.WCalc(wminions))

                W.Cast(wminions);

            if (Q.IsReady() && (Program._Player.Distance(qminions) <= Program._Player.GetAutoAttackRange()) && useQ && qminions.Health < DamageLib.QCalc(qminions))
            {
                Q.Cast();
            }


        }
        public static void Flee()
        {
            var E = Program.E;
            var R = Program.R;

            if (E.IsReady() && !(Program._Player.HasBuff("TalonDisappear")))
            {
                R.Cast();


            }

            if (E.IsReady())
            {
                var FleeAlvo = EntityManager.MinionsAndMonsters.CombinedAttackable.FirstOrDefault(it => it.IsValidTarget(E.Range) && it.Distance(Game.CursorPos) <= 300 && !(Program._Player.HasBuff("TalonDisappear")));

                if (FleeAlvo != null) E.Cast(FleeAlvo);

                else
                {
                    FleeAlvo = EntityManager.MinionsAndMonsters.CombinedAttackable.FirstOrDefault(it => it.IsValidTarget(E.Range) && it.Distance(Game.CursorPos) <= 300 && !(Program._Player.HasBuff("TalonDisappear")));

                    if (FleeAlvo != null) E.Cast(FleeAlvo);
                }
            }
        }
        public static void KillSteal()
        {
            var Q = Program.Q;
            var W = Program.W;
            var E = Program.E;
            var R = Program.R;

            foreach (var enemy in EntityManager.Heroes.Enemies.Where(a => !a.IsDead && !a.IsZombie && a.Health > 0))
            {
                if (enemy.IsValidTarget(R.Range) && enemy.HealthPercent <= 40)
                {

                    if (DamageLib.QCalc(enemy) + DamageLib.WCalc(enemy) + DamageLib.RCalc(enemy) >= enemy.Health)
                    {
                        if (E.IsReady() && E.IsInRange(enemy) && Program.ModesMenu1["KE"].Cast<CheckBox>().CurrentValue)
                        {
                            E.Cast(enemy);


                        }
                        if (Program.ModesMenu1["KW"].Cast<CheckBox>().CurrentValue && (DamageLib.WCalc(enemy) >= enemy.Health) && W.IsInRange(enemy) && W.IsReady())
                        {
                            W.Cast(enemy);


                        }
                        if (Program.ModesMenu1["KQ"].Cast<CheckBox>().CurrentValue && (DamageLib.QCalc(enemy) >= enemy.Health) && E.IsInRange(enemy) && Q.IsReady())
                        { Q.Cast(enemy); }
                        if (Program.ModesMenu1["KR"].Cast<CheckBox>().CurrentValue && (DamageLib.RCalc(enemy) >= enemy.Health) && R.IsInRange(enemy) && R.IsReady())
                        { R.Cast(); }
                    }

                }
            }

        }









    }


    }
