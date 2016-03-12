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
namespace KEzreal
{
    internal class Program
    {
        public const string ChampionName = "Ezreal";
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
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;

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
                Chat.Print("KEzreal Addon Loading Success", Color.Green);

             
                Menu = MainMenu.AddMenu("KEzreal", "Ezreal");
                Menu.AddSeparator();
                Menu.AddLabel("Criado por Bruno105");


                //------------//
                //-Mode Menu-//
                //-----------//

                var Enemies = EntityManager.Heroes.Enemies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);
                ModesMenu1 = Menu.AddSubMenu("Combo/Harass/KS", "Modes1Ezreal");
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("Combo Configs");
                ModesMenu1.Add("ComboQ", new CheckBox("Use Q on Combo", true));
                ModesMenu1.Add("ComboW", new CheckBox("Use W on Combo", true));
                ModesMenu1.Add("ComboE", new CheckBox("Use E on Combo", true));
                ModesMenu1.Add("ComboR", new CheckBox("Use R on Combo", true));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("Use R only on:");
                foreach (var a in Enemies)
                {
                    ModesMenu1.Add("Ult_" + a.BaseSkinName, new CheckBox(a.BaseSkinName));
                }
                ModesMenu1.Add("useI", new CheckBox("Use Itens on Combo", true));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("Harass Configs");
                ModesMenu1.Add("ManaH", new Slider("Dont use Skills if Mana <=", 40));
                ModesMenu1.Add("HarassQ", new CheckBox("Use Q on Harass", true));
                ModesMenu1.Add("HarassW", new CheckBox("Use W on Harass", true));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("Kill Steal Configs");
                ModesMenu1.Add("KQ", new CheckBox("Use Q on KillSteal", true));

                ModesMenu2 = Menu.AddSubMenu("Lane/Jungle/Last", "Modes2Ezreal");
                ModesMenu2.AddLabel("LastHit Configs");
                ModesMenu2.Add("ManaL", new Slider("Dont use Skills if Mana <= ", 40));
                ModesMenu2.Add("LastQ", new CheckBox("Use Q on LastHit", true));
                ModesMenu2.AddLabel("Lane Clear Config");
                ModesMenu2.Add("ManaF", new Slider("Dont use Skills if Mana <=", 40));
                ModesMenu2.Add("FarmQ", new CheckBox("Use Q on LaneClear", true));
                ModesMenu2.Add("FarmW", new CheckBox("Use W on LaneClear", true));
                ModesMenu2.Add("MinionW", new Slider("Use W when count minions more than :", 3, 1, 5));
                ModesMenu2.AddLabel("Jungle Clear Config");
                ModesMenu2.Add("ManaJ", new Slider("Dont use Skills if Mana <=", 40));
                ModesMenu2.Add("JungQ", new CheckBox("Use Q on ungle", true));
                ModesMenu2.Add("JungW", new CheckBox("Use W on Jungle", true));



                //------------//
                //-Draw Menu-//
                //----------//
                DrawMenu = Menu.AddSubMenu("Draws", "DrawEzreal");
                DrawMenu.Add("drawAA", new CheckBox("Draw do AA", true));
                DrawMenu.Add("drawQ", new CheckBox(" Draw do Q", true));
                DrawMenu.Add("drawE", new CheckBox(" Draw do E", true));
                DrawMenu.Add("drawR", new CheckBox(" Draw do R", true));
                //------------//
                //-Misc Menu-//
                //----------//
                Misc = Menu.AddSubMenu("MiscMenu", "Misc");
                Misc.Add("useEGapCloser", new CheckBox("E on GapCloser", true));
                Misc.Add("useEInterrupter", new CheckBox("use E to Interrupt", true));
                Misc.Add("resetAA", new CheckBox("Reset AA"));
            }

            catch (Exception e)
            {
                Chat.Print("KEzreal: Exception occured while Initializing Addon. Error: " + e.Message);

            }

        }
    }
}
