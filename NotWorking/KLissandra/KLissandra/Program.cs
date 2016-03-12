using System;
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

namespace KLissandra
{
    class Program
    {
       /* public const string ChampionName = "Lissandra";
        public static Menu Menu, Combo, Harass, Farm, KillSteal, Misc, DrawMenu;
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
        public static bool CanE2()
        {
            return Player.HasBuff("LissandraE");
        }

        public static Spell.Skillshot Q;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Targeted R;
        


        static void Main(string[] args)
        {

            Loading.OnLoadingComplete += Game_OnStart;
            Game.OnUpdate += Game_OnUpdate;
           // Game.OnUpdate += Ref.MonitorMissilePosition;
            Drawing.OnDraw += Game_OnDraw;
            //GameObject.OnCreate += Game_ObjectCreate;
            //GameObject.OnDelete += Game_OnDelete;
            //Orbwalker.OnPostAttack += Reset;
            //Game.OnTick += Game_OnTick;
            //Interrupter.OnInterruptableSpell += KInterrupter;
            //Gapcloser.OnGapcloser += KGapCloser;



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
                Chat.Print("KLissandra Addon Loading Success", Color.Blue);




                Q = new Spell.Skillshot(SpellSlot.Q, 725, SkillShotType.Linear,0,2200,75);
                W = new Spell.Active(SpellSlot.W,450);
                E = new Spell.Skillshot(SpellSlot.E, 1050, SkillShotType.Linear,0,850,125){AllowedCollisionCount = int.MaxValue};                
                R = new Spell.Targeted(SpellSlot.R, 550);



                Menu = MainMenu.AddMenu("KLissandra", "lissandra");
                Menu.AddSeparator();
                Menu.AddLabel("Criado por Bruno105");
                //-----/-------//
                //-Combo Menu-//
                //-------/---//
                Combo = Menu.AddSubMenu("Combo/Harass/KS", "Modes1Talon");
                Combo.Add("ComboQ", new CheckBox("Use Q on Combo", true));
                Combo.Add("ComboW", new CheckBox("Use W on Combo", true));
                Combo.Add("ComboE", new CheckBox("Use E on Combo", true));
                Combo.Add("ComboE2", new CheckBox("Use E2 on Combo", true));
                Combo.Add("ComboR", new CheckBox("Use R on Combo", false));

                //-----/-------//
                //-Harass Menu-//
                //-------/---//


              Harass.Add("HarassQ", new CheckBox("Use Q on Harass", true));
                Harass.Add("HarassW", new CheckBox("Use W on Harass", true));
                Harass.Add("HarassE", new CheckBox("Use E on Harass", true));

                //------------//
                //-Farm Menu-//
                //----------//
                Farm.AddLabel("Lane Clear");
                Farm.Add("LaneQ", new CheckBox("Use Q on LaneClear", true));
                Farm.Add("LaneW", new CheckBox("Use W on LaneClear", true));
                Farm.Add("LaneE", new CheckBox("Use E on LaneClear", true));
                Farm.Add("LaneE2", new CheckBox("Use E on LaneClear", true));
                Farm.AddSeparator();
                Farm.AddLabel("Last Hit");
                Farm.Add("LastQ", new CheckBox("Use Q on LastHit", true));
                Farm.Add("LastW", new CheckBox("Use W on LastHit", true));
                Farm.Add("LastE", new CheckBox("Use E on LastHit", true));
                Farm.AddSeparator();
                Farm.AddLabel("Jungle Clear");
                Farm.Add("JungQ", new CheckBox("Use Q on JungleClear", true));
                Farm.Add("JungW", new CheckBox("Use W on JungleClear", true));
                Farm.Add("JungE", new CheckBox("Use E on JungleClear", true));
                Farm.Add("JungE2", new CheckBox("Use E2 on JungleClear", true));


                //------------//
                //-Misc Menu-//
                //----------//
                Misc = Menu.AddSubMenu("MiscMenu", "Misc");
                Misc.Add("useWGapCloser", new CheckBox("W on GapCloser", true));
                Misc.Add("useRGapCloser", new CheckBox("R on GapCloser", true));
                Misc.Add("useWInterrupter", new CheckBox("use W to Interrupt", true));
                Misc.Add("useRInterrupter", new CheckBox("use R to Interrupt", true));
                
                //------------//
                //-Draw Menu-//
                //----------//
                DrawMenu = Menu.AddSubMenu("Draws", "DrawKassadin");
                DrawMenu.Add("drawAA", new CheckBox("Draw do AA", true));
                DrawMenu.Add("drawQ", new CheckBox(" Draw do Q", true));
                DrawMenu.Add("drawW", new CheckBox(" Draw do W", true));
                DrawMenu.Add("drawE", new CheckBox(" Draw do E", true));
                DrawMenu.Add("drawR", new CheckBox(" Draw do R", true));






                      }

            catch (Exception e)
            {
                Chat.Print("KLissandra: Exception occured while Initializing Addon. Error: " + e.Message);

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
                new Circle() { Color = Color.Aqua, Radius = 725, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.CadetBlue, Radius = 450, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Red, Radius = 1050, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Blue, Radius = 550, BorderWidth = 2f }.Draw(_Player.Position);
            }

        }


             static void Game_OnUpdate(EventArgs args)
             {
                

                 if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                 {
                     ModesManager.Combo();
                 }
                 if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                 {
                    // ModesManager.Harass();
                 }

                 if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                 {

                    // ModesManager.LaneClear();

                 }
                 if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                 {

                    // ModesManager.JungleClear();
                 }



                 if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                 {
                    // ModesManager.LastHit();

                 }








             }






*/

        }
    }

