﻿using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Color = System.Drawing.Color;
using SharpDX;



namespace KSejuani
{
    internal class Program
    {



        public const string ChampionName = "Sejuani";
        public static Menu Menu, ModesMenu1, ModesMenu2, DrawMenu, Misc;
        public static AIHeroClient PlayerInstance
        {
            get { return Player.Instance; }
        }
        private static float HealthPercent()
        {
            return (PlayerInstance.Health / PlayerInstance.MaxHealth) * 100;
        }

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }





        public static Spell.Skillshot Q;
        public static Spell.Active W;
        public static Spell.Active E;
        public static Spell.Skillshot R;
        public static Spell.Targeted Ignite;



        static void Main(string[] args)
        {


            Loading.OnLoadingComplete += Game_OnStart;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Game_OnDraw;
            Interrupter.OnInterruptableSpell += KInterrupter;
        }


        static void Game_OnStart(EventArgs args)
        {

            try
            {
                if (ChampionName != PlayerInstance.BaseSkinName)
                {
                    return;
                }

                Bootstrap.Init(null);
                Chat.Print("KSejuani Addon Loading Successs", Color.Green);

                Q = new Spell.Skillshot(SpellSlot.Q, 650, SkillShotType.Linear, 0, 1600, 70);
                W = new Spell.Active(SpellSlot.W, 350);
                E = new Spell.Active(SpellSlot.E, 1000);
                R = new Spell.Skillshot(SpellSlot.R, 1175, SkillShotType.Linear, 250, 1600, 110);



                Menu = MainMenu.AddMenu("KSejuani", "sejuani");
                Menu.AddSeparator();
                Menu.AddLabel("Criado por Bruno105");


                //------------//
                //-Mode Menu-//
                //-----------//


                ModesMenu1 = Menu.AddSubMenu("Combo/Harass", "Modes1Sejuani");
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("Combo Configs");
                ModesMenu1.Add("ComboQ", new CheckBox("Use Q on Combo", true));
                ModesMenu1.Add("ComboW", new CheckBox("Use W on Combo", true));
                ModesMenu1.Add("ComboE", new CheckBox("Use E on Combo", true));
                ModesMenu1.Add("ComboR", new CheckBox("Use R on Combo", true));
                ModesMenu1.Add("MinR", new Slider("Use R if min Champs on R range:", 2, 1, 5));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("Harass Configs");
                ModesMenu1.Add("ManaH", new Slider("Dont use Skills if Mana <=", 40));
                ModesMenu1.Add("HarassQ", new CheckBox("Use Q on Harass", true));
                ModesMenu1.Add("HarassW", new CheckBox("Use W on Harass", true));
                ModesMenu1.Add("HarassE", new CheckBox("Use E on Harass", true));
                ModesMenu2 = Menu.AddSubMenu("Lane/LastHit", "Modes2Sejuani");
                ModesMenu2.AddLabel("LastHit Configs");
                ModesMenu2.Add("ManaL", new Slider("Dont use Skills if Mana <=", 40));
                ModesMenu2.Add("LastQ", new CheckBox("Use Q on LastHit", true));
                ModesMenu2.Add("LastW", new CheckBox("Use W on LastHit", true));
                ModesMenu2.Add("LastE", new CheckBox("Use E on LastHit", true));
                ModesMenu2.AddLabel("Lane Cler Config");
                ModesMenu2.Add("ManaF", new Slider("Dont use Skills if Mana <=", 40));
                ModesMenu2.Add("FarmQ", new CheckBox("Use Q on LaneClear", true));
                ModesMenu2.Add("FarmW", new CheckBox("Use W on LaneClear", true));
                ModesMenu2.Add("FarmE", new CheckBox("Use E on LaneClear", true));
                ModesMenu2.Add("MinionE", new Slider("Use E when count minions more than :", 3, 1, 5));
                //------------//
                //-Draw Menu-//
                //----------//
                DrawMenu = Menu.AddSubMenu("Draws", "DrawKassadin");
                DrawMenu.Add("drawAA", new CheckBox("Draw do AA", true));
                DrawMenu.Add("drawQ", new CheckBox(" Draw do Q", true));
                DrawMenu.Add("drawW", new CheckBox(" Draw do W", true));
                DrawMenu.Add("drawE", new CheckBox(" Draw do E", true));
                DrawMenu.Add("drawR", new CheckBox(" Draw do R", true));
                //------------//
                //-Misc Menu-//
                //----------//

                Misc = Menu.AddSubMenu("MiscMenu", "Misc");
                //Misc.Add("useQGapCloser", new CheckBox("Q on GapCloser", true));
                Misc.Add("eInterrupt", new CheckBox("use E to Interrupt", true));

            }

            catch (Exception e)
            {
                Chat.Print("KSejuani: Exception occured while Initializing Addon. Error: " + e.Message);
            }
        }


        static void Game_OnDraw(EventArgs args)
        {



            if (DrawMenu["drawAA"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.White, Radius = _Player.GetAutoAttackRange(), BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Aqua, Radius = 650, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Green, Radius = 350, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Red, Radius = 1000, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Blue, Radius = 1175, BorderWidth = 2f }.Draw(_Player.Position);
            }

        }
        static void KInterrupter(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {

            if (args.DangerLevel == DangerLevel.High && sender.IsEnemy && sender is AIHeroClient && sender.Distance(_Player) < Q.Range && Q.IsReady() && Misc["useQGapCloser"].Cast<CheckBox>().CurrentValue)
            {
                Q.Cast(sender);
            }

        }




        static void Game_OnUpdate(EventArgs args)
        {


            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var alvo = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                var rmax = EntityManager.Heroes.Enemies.Where(t => t.IsInRange(Player.Instance.Position, R.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
                if (!alvo.IsValid()) return;
                var predPos = Prediction.Position.PredictLinearMissile(alvo, R.Range, R.Width, R.CastDelay, R.Speed,int.MaxValue, null, false);
                
                
                if (Q.IsReady() && Q.IsInRange(alvo) && ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue)
                {
                    Q.Cast(alvo);
                }
                if (W.IsReady() && W.IsInRange(alvo) && ModesMenu1["ComboW"].Cast<CheckBox>().CurrentValue)
                {
                    W.Cast();

                }
                if (E.IsReady() && E.IsInRange(alvo) && ModesMenu1["ComboE"].Cast<CheckBox>().CurrentValue)
                {
                    E.Cast();

                }
                if (R.IsReady() && R.IsInRange(alvo) && ModesMenu1["ComboR"].Cast<CheckBox>().CurrentValue && (rmax >= ModesMenu1["MinR"].Cast<Slider>().CurrentValue) && predPos.HitChance > HitChance.High    )
                {
                     R.Cast(predPos.CastPosition);

                }


            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                var alvo = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (!alvo.IsValid()) return;



                if ((_Player.ManaPercent <= ModesMenu1["ManaH"].Cast<Slider>().CurrentValue))
                {
                    return;
                }
                if (Q.IsReady() && Q.IsInRange(alvo) && ModesMenu1["HarassQ"].Cast<CheckBox>().CurrentValue)
                {
                    Q.Cast(alvo);
                }
                if (W.IsReady() && W.IsInRange(alvo) && ModesMenu1["HarassW"].Cast<CheckBox>().CurrentValue)
                {
                    W.Cast();

                }
                if (E.IsReady() && E.IsInRange(alvo) && ModesMenu1["HarassE"].Cast<CheckBox>().CurrentValue)
                {
                    E.Cast();

                }


            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {


                var minions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Q.Range) && (DamageLib.QCalc(m) > m.Health));
                var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, E.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
                if (minions == null) return;
                if ((_Player.ManaPercent <= ModesMenu2["ManaF"].Cast<Slider>().CurrentValue))
                {
                    return;
                }
                if (Q.IsReady() && Q.IsInRange(minions) && ModesMenu2["FarmQ"].Cast<CheckBox>().CurrentValue && minions.Health < DamageLib.QCalc(minions))
                {
                    Q.Cast(minions);
                }

                if (W.IsReady() && W.IsInRange(minions) && ModesMenu2["FarmW"].Cast<CheckBox>().CurrentValue )
                {
                    W.Cast();

                }
                if (E.IsReady() && E.IsInRange(minions) && ModesMenu1["FarmE"].Cast<CheckBox>().CurrentValue && (minion >= ModesMenu2["MinionE"].Cast<Slider>().CurrentValue))
                {
                    E.Cast();

                }
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                var qminions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Q.Range) && (DamageLib.QCalc(m) > m.Health));
                var wminions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(W.Range) && (DamageLib.WCalc(m) > m.Health));
                var eminions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(E.Range) && (DamageLib.ECalc(m) > m.Health));
                var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, E.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
                if (qminions == null) return;
                var prediction = Q.GetPrediction(qminions);
                if (Q.IsReady() && Q.IsInRange(qminions) && ModesMenu2["LastQ"].Cast<CheckBox>().CurrentValue && qminions.Health < DamageLib.QCalc(qminions))
                {
                    Q.Cast(qminions);
                }
                if (W.IsReady() && W.IsInRange(wminions) && ModesMenu2["LastW"].Cast<CheckBox>().CurrentValue && wminions.Health < DamageLib.WCalc(wminions) && (minion >= ModesMenu2["MinionW"].Cast<Slider>().CurrentValue))
                {
                    W.Cast(wminions);
                }
                if (W.IsReady() && E.IsInRange(wminions) && ModesMenu2["LastE"].Cast<CheckBox>().CurrentValue && eminions.Health < DamageLib.ECalc(wminions) && (minion >= ModesMenu2["MinionW"].Cast<Slider>().CurrentValue))
                {
                    W.Cast(wminions);
                }

            }







        }
    }
}
