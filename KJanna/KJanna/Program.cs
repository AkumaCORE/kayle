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


namespace KJanna
{
    class Program
    {


        public const string ChampionName = "Janna";
        public static Menu Menu, Combo, DrawMenu, Misc;
        public static AIHeroClient PlayerInstance
        {
            get { return Player.Instance; }
        }
        public static bool HasSpell(string s)
        {
            return Player.Spells.FirstOrDefault(o => o.SData.Name.Contains(s)) != null;
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
        public static Spell.Targeted E;
        public static Spell.Active R;
        public static Vector3 Qpos;
        public static bool Qcasted = false;





        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnStart;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Game_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnCast;
            GameObject.OnCreate += this.TowerAttackOnCreate;
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
                Chat.Print("KJanna SuPorT Addon Loading Success", Color.Green);

                Q = new Spell.Skillshot(SpellSlot.Q, 850, SkillShotType.Linear, 1, 900, 120);
                Q.AllowedCollisionCount = int.MaxValue;
                W = new Spell.Targeted(SpellSlot.W, 600);
                E = new Spell.Targeted(SpellSlot.E, 800);
                R = new Spell.Active(SpellSlot.R, 875);



                Menu = MainMenu.AddMenu("KJanna", "janna");
                Menu.AddSeparator();
                Menu.AddLabel("Criado por Bruno105");

                //------------//
                //-Mode Menu-//
                //-----------//



                Combo = Menu.AddSubMenu("Combo/Harass", "ComboJanna");
                Combo.Add("ComboQ", new CheckBox("Use Q on Combo", true));
                Combo.Add("ComboW", new CheckBox("Use W on Combo", true));
                Combo.AddSeparator();
                Combo.AddLabel("Mana Manager");
                Combo.Add("ManaH", new Slider("No Skills on Harasswhen mana  <=", 40));


                //------------//
                //-Misc Menu-//
                //-----------//



                Misc = Menu.AddSubMenu("Misc", "MiscJanna");
                Misc.Add("AutoE", new CheckBox("Auto use E", true));
                Misc.Add("qInterrupt", new CheckBox("Interrupt with Q", true));
                Misc.Add("rInterrupt", new CheckBox("Interrupt with R", true));
                Misc.Add("GapQ", new CheckBox("Anti-GapCloser with Q", true));
                Misc.Add("GapR", new CheckBox("Anti-GapCloser with R", true));
                //------------//
                //-Draw Menu-//
                //----------//
                DrawMenu = Menu.AddSubMenu("Draws", "DrawJanna");
                DrawMenu.Add("drawAA", new CheckBox("Desable Draw do AA", true));
                DrawMenu.Add("drawQ", new CheckBox("Desable Draw do Q", true));
                DrawMenu.Add("drawW", new CheckBox("Desable Draw do W", true));
                DrawMenu.Add("drawE", new CheckBox("Desabile Draw do E", true));
                DrawMenu.Add("drawR", new CheckBox("Desabile Draw do R", true));



            }
            catch (Exception e)
            {
                Chat.Print("KKayle: Exception occured while Initializing Addon. Error: " + e.Message);
            }

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
                new Circle() { Color = Color.Aqua, Radius = 850, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Green, Radius = 600, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Red, Radius = 800, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Azure, Radius = 875, BorderWidth = 2f }.Draw(_Player.Position);
            }


        }


        public static void Game_OnUpdate(EventArgs args)
        {
            if (_Player.IsDead) return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                ModesManager.Support();
            }


            if ((Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)))
            {
                if (!(Player.Instance.ManaPercent > Program.Combo["ManaH"].Cast<Slider>().CurrentValue))
                {
                    return;
                }
                Orbwalker.DisableAttacking = true;
                ModesManager.Support();

            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                // ModesManager.LaneClear();

            }





        }

        public static void OnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (_Player.IsDead || !sender.IsEnemy || !(sender is AIHeroClient)) return;

            if (sender.IsAlly && sender is Obj_AI_Base && args.Target.IsAlly && E.IsReady() && args.Target.Position.Distance(Player.Instance.Position) <= E.Range)
            {
                // List<AIHeroClient> Allies = new List<AIHeroClient>();

                if (args.Target != null)
                {
                    if (args.Target.IsAlly || args.Target.IsMe)
                    {
                        var target = EntityManager.Heroes.Allies.FirstOrDefault(x => x.ServerPosition.Distance(args.Target.Position) < 5);

                        if (target != null) E.Cast(target);
                        return;


                    }




                }
            }

        }


        public static void TowerAttackOnCreate(GameObject sender, EventArgs args)
        {

            if (!E.IsReady())
            {
                return;
            }

           
                var missile = (MissileClient)sender;
            var turrent = Obj_AI_Turret.m

        }


    }
}

        
