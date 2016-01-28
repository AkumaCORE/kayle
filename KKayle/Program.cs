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

namespace Kayle
{
    class KKayle
    {
       
        public static Menu Menu, DrawMenu, ComboMenu, HarassMenu, FarmMenu, HealMenu, UltMenu;

        public static Spell.Targeted Q;
        public static Spell.Targeted W;
        public static Spell.Active E;
        public static Spell.Targeted R;

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



        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnStart;
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnUpdate;

        }


        //---------//
        // Start Game
        // Game On Start
        static void Game_OnStart(EventArgs args)
        {
            Chat.Print("KKayle Addon");
            Q = new Spell.Targeted(SpellSlot.Q, 650);
            W = new Spell.Targeted(SpellSlot.W, 900);
            E = new Spell.Active(SpellSlot.E);
            R = new Spell.Targeted(SpellSlot.R, 900);

            Menu = MainMenu.AddMenu("KKayle", "kayle");
            Menu.AddSeparator();
            Menu.AddLabel("Criado por Bruno105");
            // Combo Menu
            ComboMenu = Menu.AddSubMenu("Combo", "ComboKayle");
            ComboMenu.Add("ComboW", new CheckBox("Usar W no Combo", true));

            // Harass Menu
            HarassMenu = Menu.AddSubMenu("Harass", "HarassKayle");
            HarassMenu.Add("HarassQ", new CheckBox("Usar Q no Harass", true));
            HarassMenu.Add("HarassW", new CheckBox("Usar W no Harass", false));
            HarassMenu.Add("HarassE", new CheckBox("Usar E no Harass", true));
            HarassMenu.Add("ManaH", new Slider("Nao usar Skill quando mana for <=", 30));

             //Farm Menu
            FarmMenu = Menu.AddSubMenu("Farm", "FarmKayle");
            FarmMenu.Add("FarmQ", new CheckBox("Usar Q para Farmar", true));
            FarmMenu.Add("FarmE", new CheckBox("Usar E para Farmar", true));
            FarmMenu.Add("ManaF", new Slider("Nao usar Skills quando mana for <=", 30));
            
            // Heal Menu
            var allies = EntityManager.Heroes.Allies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);
            HealMenu = Menu.AddSubMenu("Heal", "HealKayle");
            HealMenu.Add("autoW", new CheckBox("Usar W automaticamente", true));
            HealMenu.Add("HealSelf", new Slider("Usar W em si quando % HP", 50));
            HealMenu.Add("HealAlly", new Slider("Usar W nos aliados quando % HP", 50));
            foreach (var a in allies)
            {
                HealMenu.Add("autoHeal_" + a.BaseSkinName, new CheckBox("Usar Heal nos champs " + a.BaseSkinName));
            }
            
            //--------------//
            //---Ultmate---//
            //------------//
            UltMenu = Menu.AddSubMenu("Ultimate", "UltKayle");
            UltMenu.Add("autoR", new CheckBox("Usar Ultimate automaticamente", true));
            UltMenu.Add("UltSelf", new Slider("Usar R em si quando % HP", 20));
            UltMenu.Add("UltAlly", new Slider("Usar R em aliados quando % HP", 20));
            foreach (var a in allies)
            {
                HealMenu.Add("autoUlt_" + a.BaseSkinName, new CheckBox("Use Ult on " + a.BaseSkinName));
            }


            //------------//
            //-Draw Menu-//
            //----------//
            DrawMenu = Menu.AddSubMenu("Draws", "DrawKayle");
            // DrawMenu.Add("drawDisable", new CheckBox("Desabilidatar todos os Draw", false));
            DrawMenu.Add("drawAA", new CheckBox("Desabilidatar Draw do AA", true));
            DrawMenu.Add("drawQ", new CheckBox("Desabilidatar Draw do Q", true));
            DrawMenu.Add("drawW", new CheckBox("Desabilidatar Draw do W", true));
            DrawMenu.Add("drawE", new CheckBox("Desabilidatar Draw do E", true));



        }

        // ------------//
        // Game OnDraw//
        // --------- //

        public static void Game_OnDraw(EventArgs args)
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
                new Circle() { Color = Color.Green, Radius = 900, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Red, Radius = _Player.GetAutoAttackRange() + 400, BorderWidth = 2f }.Draw(_Player.Position);
            }


        }

        //-----//
       // Heal //
      // -----//
        private static void AutoHeal()
        {
            if (!W.IsReady())
            {
                return;
            }

            var lowestHealthAlly = EntityManager.Heroes.Allies.Where(a => W.IsInRange(a) && !a.IsMe).OrderBy(a => a.Health).FirstOrDefault();

            if (HealthPercent() <= HealMenu["HealSelf"].Cast<Slider>().CurrentValue)
            {
                W.Cast(PlayerInstance);
            }

            else if (lowestHealthAlly != null)
            {
                if (!(lowestHealthAlly.Health <= HealMenu["HealAlly"].Cast<Slider>().CurrentValue))
                {
                    return;
                }
                if (HealMenu["autoHeal_" + lowestHealthAlly.BaseSkinName].Cast<CheckBox>().CurrentValue)
                {
                    W.Cast(lowestHealthAlly);
                }
            }
        }

        //-------------//
        //--Ultimate--//
        //-----------//
        private static void AutoUlt()
        {
            if (!R.IsReady())
            {
                return;
            }

            var lowestHealthAlly = EntityManager.Heroes.Allies.Where(a => W.IsInRange(a) && !a.IsMe).OrderBy(a => a.Health).FirstOrDefault();

            if (HealthPercent() <= HealMenu["UltSelf"].Cast<Slider>().CurrentValue)
            {
                R.Cast(PlayerInstance);
            }

            else if (lowestHealthAlly != null)
            {
                if (!(lowestHealthAlly.Health <= HealMenu["UltAlly"].Cast<Slider>().CurrentValue))
                {
                    return;
                }
                if (HealMenu["autoUlt_" + lowestHealthAlly.BaseSkinName].Cast<CheckBox>().CurrentValue)
                {
                    R.Cast(lowestHealthAlly);
                }
            }
        }








        // ------------//
        // Game On Update//
        // ------------//

        public static void Game_OnUpdate(EventArgs args)
        {
            AutoHeal();
            AutoUlt();
            var alvo = TargetSelector.GetTarget(1000, DamageType.Mixed);
            if (!alvo.IsValid()) return;

            //-------------//
            //----Combo----//
            //-------------//
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Combo)
            {
                if (Q.IsReady() && Q.IsInRange(alvo))
                {
                    Q.Cast(alvo);
                }
                if (W.IsReady() && W.IsInRange(alvo))
                {
                    W.Cast(Player.Instance);
                }
                if (E.IsReady() && _Player.Distance(alvo) <= _Player.GetAutoAttackRange() + 400)
                {
                    E.Cast();
                }

            }

            //-------------//
            //---Harass----//
            //-------------//
            if ((Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Harass) && _Player.ManaPercent >= HarassMenu["ManaH"].Cast<Slider>().CurrentValue)
            {
                if (Q.IsReady() && Q.IsInRange(alvo) && HarassMenu["HarassQ"].Cast<CheckBox>().CurrentValue)
                {

                    Q.Cast(alvo);
                }
                if (W.IsReady() && !HarassMenu["HarassW"].Cast<CheckBox>().CurrentValue)
                {
                    W.Cast(Player.Instance);
                }
                if (E.IsReady() && HarassMenu["HarassE"].Cast<CheckBox>().CurrentValue && _Player.Distance(alvo) <= _Player.GetAutoAttackRange() + 400)
                {
                    E.Cast();
                }

            }

            //-------------//
            //-----Farm----//
            //-------------//

          /*  if ((Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.LaneClear) && _Player.ManaPercent >= HarassMenu["ManaF"].Cast<Slider>().CurrentValue)
            {
                if (Q.IsReady() && FarmMenu["FarmQ"].Cast<CheckBox>().CurrentValue )
                {
                    var minion =
                EntityManager.MinionsAndMonsters.GetLaneMinions()
                    .OrderByDescending(m => m.Health)
                    .FirstOrDefault(m => m.IsValidTarget(Q.Range));

                    if (Q.IsReady() && minion.IsValidTarget(Q.Range))
                    {
                        Q.Cast(minion);
                    }
                    if (E.IsReady() && FarmMenu["FarmE"].Cast<CheckBox>().CurrentValue)
                    {
                        E.Cast(minion);
                    }

                }



            }
            */



        }

    }
}
