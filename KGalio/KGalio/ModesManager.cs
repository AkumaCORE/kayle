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

namespace KGalio
{
    internal class ModesManager
    {
        public static void Combo()
        {
            if (Program._Player.HasBuff("GalioIdolOfDurand"))
            {
                Orbwalker.DisableMovement = true;
                
            }
            else
            {
                Orbwalker.DisableMovement = false;

            }
            var Q = Program.Q;
            var W = Program.W;
            var E = Program.E;
            var R = Program.R;
            var useQ = Program.ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue;
            var useW = Program.ModesMenu1["ComboW"].Cast<CheckBox>().CurrentValue;
            var useE = Program.ModesMenu1["ComboE"].Cast<CheckBox>().CurrentValue;
            var useR = Program.ModesMenu1["ComboR"].Cast<CheckBox>().CurrentValue;
            var alvo = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            //  var alvoR = TargetSelector.GetTarget(300, DamageType.Magical);
            var rmin = EntityManager.Heroes.Enemies.Where(t => t.IsInRange(Player.Instance.Position, R.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
            if (!alvo.IsValid()) return;
            //  if (!alvoR.IsValid()) return;
            if (useQ && Q.IsInRange(alvo) && Q.IsReady())
            {
                if (useW && W.IsReady())
                {
                    W.Cast(Player.Instance);
                    Q.Cast(alvo);
                }
                else
                {
                    Q.Cast(alvo);
                }
            }
            if (useE && E.IsReady() && E.IsInRange(alvo))
            {
                if (useW && W.IsReady())
                {
                    W.Cast(Player.Instance);
                    E.Cast(alvo);
                }
                else
                {
                    E.Cast(alvo);
                }
            }
            if (useR && R.IsReady() && rmin >= Program.ModesMenu1["MinR"].Cast<Slider>().CurrentValue && R.IsInRange(alvo))
            {
                R.Cast();
                Orbwalker.DisableMovement = true;
                if (useW && W.IsReady())
                {
                    W.Cast(Player.Instance);
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
            var alvo = TargetSelector.GetTarget(940, DamageType.Magical);
            var alvoR = TargetSelector.GetTarget(300, DamageType.Magical);
            var rmin = EntityManager.Heroes.Enemies.Where(t => t.IsInRange(Player.Instance.Position, 300) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
            if (!alvo.IsValid()) return;
            if (!alvoR.IsValid()) return;
            if ((Program._Player.ManaPercent <= Program.ModesMenu1["ManaH"].Cast<Slider>().CurrentValue))
            {
                return;
            }
            if (useQ && Q.IsInRange(alvo) && Q.IsReady())
            {
                W.Cast(Program._Player.Position);
                Q.Cast(alvo);
            }
            if (useE && E.IsReady() && E.IsInRange(alvo))
            {
                W.Cast(Program._Player.Position);
                E.Cast(alvo);
            }



        }
        public static void LaneClear()
        {

            var Q = Program.Q;
            var W = Program.W;
            var E = Program.E;
            var useQ = Program.ModesMenu2["FarmQ"].Cast<CheckBox>().CurrentValue;
            var useE = Program.ModesMenu2["FarmE"].Cast<CheckBox>().CurrentValue;
            var minionQ = Program.ModesMenu2["MinionQ"].Cast<Slider>().CurrentValue;
            var minionE = Program.ModesMenu2["MinionE"].Cast<Slider>().CurrentValue;
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Program.Q.Range));
            var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, Program.Q.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
            if (minions == null) return;
            if ((Program._Player.ManaPercent <= Program.ModesMenu2["ManaF"].Cast<Slider>().CurrentValue))
            {
                return;
            }

            if (useQ && Q.IsInRange(minions) && Q.IsReady())
            {
                Q.Cast(minions);

            }
            if (useE && E.IsInRange(minions) && E.IsReady())
            {
                E.Cast(minions);

            }


        }
        public static void JungleClear()
        {
            var Q = Program.Q;
            var W = Program.W;
            var E = Program.E;
            var useQ = Program.ModesMenu2["JungQ"].Cast<CheckBox>().CurrentValue;
            var useW = Program.ModesMenu2["JungW"].Cast<CheckBox>().CurrentValue;
            var useE = Program.ModesMenu2["JungE"].Cast<CheckBox>().CurrentValue;
            var jungleMonsters =
                       EntityManager.MinionsAndMonsters.GetJungleMonsters()
                           .OrderByDescending(j => j.Health)
                           .FirstOrDefault(j => j.IsValidTarget(Program.Q.Range));
            var minioon = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, Program.E.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
            if (jungleMonsters == null) return;
            if ((Program._Player.ManaPercent <= Program.ModesMenu2["ManaJ"].Cast<Slider>().CurrentValue))
            {
                return;
            }


            if (useQ && Q.IsInRange(jungleMonsters) && Q.IsReady())
            {
                if (useW && W.IsReady())
                {
                    W.Cast(Program._Player.Position);
                    Q.Cast(jungleMonsters);
                }
                else
                {
                    Q.Cast(jungleMonsters);
                }
            }
            if (useE && E.IsReady() && E.IsInRange(jungleMonsters))
            {
                if (useW && W.IsReady())
                {
                    W.Cast(Program._Player.Position);
                    E.Cast(jungleMonsters);
                }
                else
                {
                    E.Cast(jungleMonsters);
                }



            }
        }
        public static void LastHit()
        {
            if ((Program._Player.ManaPercent <= Program.ModesMenu2["ManaL"].Cast<Slider>().CurrentValue))
            {
                return;
            }
            var qminions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Program.Q.Range));
            var eminions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Program.E.Range));
            var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, Program.E.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
            if (qminions == null) return;
            var Q = Program.Q;
            var W = Program.W;
            var E = Program.E;
            var useQ = Program.ModesMenu2["LastQ"].Cast<CheckBox>().CurrentValue;
            var useE = Program.ModesMenu2["LastE"].Cast<CheckBox>().CurrentValue;


            if (useQ && Q.IsReady() && Q.IsInRange(qminions) && qminions.Health < DamageLib.QCalc(qminions))
            {
                Q.Cast(qminions);
            }

            if (useE && E.IsReady() && E.IsInRange(eminions) && qminions.Health < DamageLib.ECalc(eminions))
            {
                E.Cast(eminions);
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

                    if (DamageLib.QCalc(enemy) + DamageLib.ECalc(enemy) + DamageLib.RCalc(enemy) >= enemy.Health)
                    {

                        if (Program.ModesMenu1["KE"].Cast<CheckBox>().CurrentValue && (DamageLib.ECalc(enemy) >= enemy.Health) && W.IsInRange(enemy) && W.IsReady())
                        {
                            E.Cast(enemy);


                        }
                        if (Program.ModesMenu1["KQ"].Cast<CheckBox>().CurrentValue && (DamageLib.QCalc(enemy) >= enemy.Health) && E.IsInRange(enemy) && Q.IsReady())
                        { Q.Cast(enemy); }
                        if (Program.ModesMenu1["KR"].Cast<CheckBox>().CurrentValue && (DamageLib.RCalc(enemy) >= enemy.Health) && R.IsInRange(enemy) && R.IsReady())
                        { R.Cast(enemy); }
                    }


                }

            }
        }
    }
}

