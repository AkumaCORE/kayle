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

namespace KGragas
{
    internal class Program
    {
        public const string ChampionName = "Gragas";
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
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        public static Spell.Targeted Ignite;
        public static bool CastedQ;
        public static Vector3 insecpos, eqpos, movingawaypos;
        public static Vector3 teste;



        static void Main(string[] args)
        {

            Loading.OnLoadingComplete += Game_OnStart;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Game_OnDraw;
            GameObject.OnCreate += Game_ObjectCreate;
            //GameObject.OnDelete += Game_OnDelete;
            //Orbwalker.OnPostAttack += Reset;
            Game.OnTick += Game_OnTick;
            Interrupter.OnInterruptableSpell += KInterrupter;
            Gapcloser.OnGapcloser += KGapCloser;
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
                Chat.Print("KGragas Addon Loading Success", Color.Green);




                Q = new Spell.Skillshot(SpellSlot.Q, 775, SkillShotType.Circular, 1, 1000, 110);
                Q.AllowedCollisionCount = int.MaxValue;
                W = new Spell.Active(SpellSlot.W);
                E = new Spell.Skillshot(SpellSlot.E, 675, SkillShotType.Linear, 0, 1000, 50);
                R = new Spell.Skillshot(SpellSlot.R, 1100, SkillShotType.Circular, 1, 1000, 700);
                R.AllowedCollisionCount = int.MaxValue;




                Menu = MainMenu.AddMenu("KGragas", "gragas");
                Menu.AddSeparator();
                Menu.AddLabel("Criado por Bruno105");


                //------------//
                //-Mode Menu-//
                //-----------//

                var Enemies = EntityManager.Heroes.Enemies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);
                ModesMenu1 = Menu.AddSubMenu("Combo/Harass/KS", "Modes1Gragas");
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("Combo Configs");
                ModesMenu1.Add("ComboQ", new CheckBox("Use Q on Combo", true));
                ModesMenu1.Add("ComboW", new CheckBox("Use W on Combo", true));
                ModesMenu1.Add("ComboE", new CheckBox("Use E on Combo", true));
                ModesMenu1.Add("ComboR", new CheckBox("Use R on Combo", true));
                ModesMenu1.AddLabel("Use R only on:");
                foreach (var a in Enemies)
                {
                    ModesMenu1.Add("Ult_" + a.BaseSkinName, new CheckBox(a.BaseSkinName));
                }
                // ModesMenu1.Add("MinR", new Slider("Use R if min Champs on R range:", 2, 1, 5));
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

                ModesMenu2 = Menu.AddSubMenu("Lane/Jungle/Last", "Modes2Gragas");
                ModesMenu2.AddLabel("LastHit Configs");
                ModesMenu2.Add("ManaL", new Slider("Dont use Skills if Mana <=", 40));
                ModesMenu2.Add("LastQ", new CheckBox("Use Q on LastHit", true));
                ModesMenu2.Add("LastW", new CheckBox("Use W on LastHit", true));
                ModesMenu2.Add("LastE", new CheckBox("Use E on LastHit", true));
                ModesMenu2.AddLabel("Lane Clear Config");
                ModesMenu2.Add("ManaF", new Slider("Dont use Skills if Mana <=", 40));
                ModesMenu2.Add("FarmQ", new CheckBox("Use Q on LaneClear", true));
                ModesMenu2.Add("FarmW", new CheckBox("Use W on LaneClear", true));
                ModesMenu2.Add("FarmE", new CheckBox("Use E on LaneClear", true));
                ModesMenu2.Add("MinionQ", new Slider("Use Q when count minions more than :", 3, 1, 5));
                ModesMenu2.AddLabel("Jungle Clear Config");
                ModesMenu2.Add("ManaJ", new Slider("Dont use Skills if Mana <=", 40));
                ModesMenu2.Add("JungQ", new CheckBox("Use Q on ungle", true));
                ModesMenu2.Add("JungW", new CheckBox("Use W on Jungle", true));
                ModesMenu2.Add("JungE", new CheckBox("Use E on Jungle", true));



                //------------//
                //-Draw Menu-//
                //----------//
                DrawMenu = Menu.AddSubMenu("Draws", "DrawKassadin");
                DrawMenu.Add("drawAA", new CheckBox("Draw do AA", true));
                DrawMenu.Add("drawQ", new CheckBox(" Draw do Q", true));
                DrawMenu.Add("drawE", new CheckBox(" Draw do E", true));
                DrawMenu.Add("drawR", new CheckBox(" Draw do R", true));
                //------------//
                //-Misc Menu-//
                //----------//
                //var Enemies = EntityManager.Heroes.Enemies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);
                Misc = Menu.AddSubMenu("MiscMenu", "Misc");
                //Misc.Add("aarest", new CheckBox("Reset AA with w"));
                Misc.Add("useEGapCloser", new CheckBox("E on GapCloser", true));
                Misc.Add("useEGapCloser", new CheckBox("R on GapCloser", true));
                Misc.Add("useEInterrupter", new CheckBox("use E to Interrupt", true));
                Misc.Add("useRInterrupter", new CheckBox("use R to Interrupt", true));
                Misc.Add("Key", new KeyBind("Key to insec", false,KeyBind.BindTypes.HoldActive, (uint) 'A'));

            }

            catch (Exception e)
            {
                Chat.Print("KGragas: Exception occured while Initializing Addon. Error: " + e.Message);

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
                new Circle() { Color = Color.Aqua, Radius = 775, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Red, Radius = 675, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Blue, Radius = 1100, BorderWidth = 2f }.Draw(_Player.Position);
            }

        }

        static void Game_OnUpdate(EventArgs args)
        {
            InsecLogic.Insec();

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

        static void Game_OnTick(EventArgs args)
        {

            ModesManager.KillSteal();

        }



        private static void Game_ObjectCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == ("Gragas_Base_Q_Ally.Troy"))
            {
                if (Player.Instance.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 2 || Player.Instance.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 1)
                {
                    Program.Q.Cast(Player.Instance);
                    CastedQ = false;
                }
                else
                {
                    CastedQ = false;
                }


            }


        }
        static void KInterrupter(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
        {

            if (args.DangerLevel == DangerLevel.High && sender.IsEnemy && sender is AIHeroClient && sender.Distance(_Player) < E.Range && E.IsReady() && Misc["useEInterrupter"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(sender);
            }
            if (args.DangerLevel == DangerLevel.High && sender.IsEnemy && sender is AIHeroClient && sender.Distance(_Player) < R.Range && R.IsReady() && Misc["useRInterrupter"].Cast<CheckBox>().CurrentValue)
            {
                R.Cast(sender);
            }

        }
        static void KGapCloser(Obj_AI_Base sender, Gapcloser.GapcloserEventArgs args)
        {


            if (sender.IsEnemy && sender is AIHeroClient && sender.Distance(_Player) < E.Range && E.IsReady() && Misc["useEGapCloser"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(sender);
            }
            if (sender.IsEnemy && sender is AIHeroClient && sender.Distance(_Player) < R.Range && R.IsReady() && Misc["useRGapCloser"].Cast<CheckBox>().CurrentValue)
            {
                R.Cast(sender);

            }
        }




        private static readonly KeyBind Key;

        public static bool KeyI
        {
            get { return Key.CurrentValue; }
        }


 




        }
    }

