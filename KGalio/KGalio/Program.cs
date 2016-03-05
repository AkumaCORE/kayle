using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace KGalio
{
    internal class Program
    {

        public const string ChampionName = "Galio";
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
        public static Spell.Targeted W;
        public static Spell.Skillshot E;
        public static Spell.Active R;



        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnStart;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Game_OnDraw;
            //GameObject.OnCreate += Game_ObjectCreate;
            //GameObject.OnDelete += Game_OnDelete;
            //Orbwalker.OnPostAttack += Reset;
            Game.OnTick += Game_OnTick;
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
                Chat.Print("KGalio Addon Loading Success", Color.Green);


                Q = new Spell.Skillshot(SpellSlot.Q, 940, SkillShotType.Circular, 1, 1300, 120);
                Q.AllowedCollisionCount = int.MaxValue;
                W = new Spell.Targeted(SpellSlot.W, 830);
                E = new Spell.Skillshot(SpellSlot.E, 1180, SkillShotType.Linear, 1, 1200, 140);
                E.AllowedCollisionCount = int.MaxValue;
                R = new Spell.Active(SpellSlot.R, 560);
                /*R = new Spell.Skillshot(SpellSlot.R, 560, SkillShotType.Circular, 1, 0, 300);
                R.AllowedCollisionCount = int.MaxValue;*/



                Menu = MainMenu.AddMenu("KGalio", "galio");
                Menu.AddSeparator();
                Menu.AddLabel("Criado por Bruno105");


                //------------//
                //-Mode Menu-//
                //-----------//

                var Enemies = EntityManager.Heroes.Enemies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);
                ModesMenu1 = Menu.AddSubMenu("Combo/Harass/KS", "Modes1Galio");
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("Combo Configs");
                ModesMenu1.Add("ComboQ", new CheckBox("Use Q on Combo", true));
                ModesMenu1.Add("ComboW", new CheckBox("Use W on Combo", true));
                ModesMenu1.Add("ComboE", new CheckBox("Use E on Combo", true));
                ModesMenu1.Add("ComboR", new CheckBox("Use R on Combo", true));
                ModesMenu1.Add("MinR", new Slider("Use R if min Enemys on R range:", 3, 1, 5));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("Harass Configs");
                ModesMenu1.Add("ManaH", new Slider("Dont use Skills if Mana <=", 40));
                ModesMenu1.Add("HarassQ", new CheckBox("Use Q on Harass", true));
                ModesMenu1.Add("HarassW", new CheckBox("Use W on Harass", true));
                ModesMenu1.Add("HarassE", new CheckBox("Use E on Harass", true));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("Kill Steal Configs");
                ModesMenu1.Add("KQ", new CheckBox("Use Q on KillSteal", true));
                ModesMenu1.Add("KE", new CheckBox("Use E to KillSteal", true));
                ModesMenu1.Add("KR", new CheckBox("Use R to KillSteal", true));

                ModesMenu2 = Menu.AddSubMenu("Lane/Jungle/Last", "Modes2Galio");
                ModesMenu2.AddLabel("LastHit Configs");
                ModesMenu2.Add("ManaL", new Slider("Dont use Skills if Mana <= ", 40));
                ModesMenu2.Add("LastQ", new CheckBox("Use Q on LastHit", true));
                ModesMenu2.Add("LastE", new CheckBox("Use E on LastHit", true));
                ModesMenu2.AddLabel("Lane Clear Config");
                ModesMenu2.Add("ManaF", new Slider("Dont use Skills if Mana <=", 40));
                ModesMenu2.Add("FarmQ", new CheckBox("Use Q on LaneClear", true));
                ModesMenu2.Add("MinionQ", new Slider("Use Q when count minions more than :", 2, 1, 5));
                ModesMenu2.Add("FarmE", new CheckBox("Use E on LaneClear", true));
                ModesMenu2.Add("MinionE", new Slider("Use E when count minions more than :", 2, 1, 5));
                ModesMenu2.AddLabel("Jungle Clear Config");
                ModesMenu2.Add("ManaJ", new Slider("Dont use Skills if Mana <=", 40));
                ModesMenu2.Add("JungQ", new CheckBox("Use Q on ungle", true));
                ModesMenu2.Add("JungW", new CheckBox("Use W on Jungle", true));
                ModesMenu2.Add("JungE", new CheckBox("Use E on Jungle", true));



                //------------//
                //-Draw Menu-//
                //----------//
                DrawMenu = Menu.AddSubMenu("Draws", "DrawGalio");
                DrawMenu.Add("drawAA", new CheckBox("Draw do AA", true));
                DrawMenu.Add("drawQ", new CheckBox(" Draw do Q", true));
                DrawMenu.Add("drawE", new CheckBox(" Draw do E", true));
                DrawMenu.Add("drawR", new CheckBox(" Draw do R", true));
                //------------//
                //-Misc Menu-//
                //----------//
                /*  Misc = Menu.AddSubMenu("MiscMenu", "Misc");
                  Misc.Add("useEGapCloser", new CheckBox("E on GapCloser", true));
                  Misc.Add("useRGapCloser", new CheckBox("R on GapCloser", true));
                  Misc.Add("useEInterrupter", new CheckBox("use E to Interrupt", true));
                  Misc.Add("useRInterrupter", new CheckBox("use R to Interrupt", true));
                  Misc.Add("Key", new KeyBind("Key to insec", false, KeyBind.BindTypes.HoldActive, (uint)'A'));*/

            }

            catch (Exception e)
            {
                Chat.Print("KGalio: Exception occured while Initializing Addon. Error: " + e.Message);

            }

        }

        private static void Game_OnDraw(EventArgs args)
        {

            Circle.Draw(Color.Red, _Player.GetAutoAttackRange(), Player.Instance.Position);
            if (Q.IsReady() && Q.IsLearned)
            {
                Circle.Draw(Color.White, Q.Range, Player.Instance.Position);
            }
            if (W.IsReady() && W.IsLearned)
            {
                Circle.Draw(Color.Green, W.Range, Player.Instance.Position);
            }
            if (E.IsReady() && E.IsLearned)
            {
                Circle.Draw(Color.Aqua, E.Range, Player.Instance.Position);
            }
            if (R.IsReady() && R.IsLearned)
            {
                Circle.Draw(Color.Black, R.Range, Player.Instance.Position);
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
                ModesManager.Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {

                ModesManager.LaneClear();

            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {

                ModesManager.JungleClear();
            }



            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                ModesManager.LastHit();

            }



        }
        public static void Game_OnTick(EventArgs args)
        {
            ModesManager.KillSteal();

        }
    }
}
