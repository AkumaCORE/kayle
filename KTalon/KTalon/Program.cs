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


namespace KTalon
{
    internal class Program
    {

        public const string ChampionName = "Talon";
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

        public static Spell.Active Q;
        public static Spell.Skillshot W;
        public static Spell.Targeted E;
        public static Spell.Active R;
        //public static Item tiamat, hydra, blade, yoummu, botrk;
        //public static Vector3 CursorPos = (Game.CursorPos);
        public static Slider ComboMode;
        public static bool CastedR ;
        public static readonly Item Youmuu = new Item((int)ItemId.Youmuus_Ghostblade);
        public static readonly Item Tiamat = new Item((int)ItemId.Tiamat);
        public static readonly Item Hydra = new Item((int)ItemId.Titanic_Hydra);
        public static readonly Item botrk = new Item((int)ItemId.Blade_of_the_Ruined_King);
        public static readonly Item alfange = new Item((int)ItemId.Bilgewater_Cutlass);


        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnStart;
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Gapcloser.OnGapcloser += KGapCloser;
            Orbwalker.OnPostAttack += Reset;
            
        }

        public static void Game_OnStart(EventArgs args)
        {

            try
            {
                if (ChampionName != PlayerInstance.BaseSkinName)
                {
                    return;
                }

                Bootstrap.Init(null);
                Chat.Print("KTalon Addon Loading Success", Color.Green);
                Q = new Spell.Active(SpellSlot.Q);
                W = new Spell.Skillshot(SpellSlot.W, 600, SkillShotType.Cone, 1, 2300, 75) { AllowedCollisionCount = int.MaxValue };
                E = new Spell.Targeted(SpellSlot.E, 700);
                R = new Spell.Active(SpellSlot.R);
               


               


                Menu = MainMenu.AddMenu("KTalon", "talon");
                Menu.AddSeparator();
                Menu.AddLabel("Criado por Bruno105");


                //------------//
                //-Mode Menu-//
                //-----------//

                var Enemies = EntityManager.Heroes.Enemies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);
                ModesMenu1 = Menu.AddSubMenu("Combo/Harass/KS", "Modes1Talon");
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("Combo Configs");
                ModesMenu1.AddLabel("1 = E/Q/W/R // 2 = R/E/Q/W // 3 = E/R/Q/W");
                ComboMode = ModesMenu1.Add("comboMode", new Slider("Mode", 1, 1, 3));
                ModesMenu1.Add("ComboQ", new CheckBox("Use Q on Combo", true));
                ModesMenu1.Add("ComboW", new CheckBox("Use W on Combo", true));
                ModesMenu1.Add("ComboE", new CheckBox("Use E on Combo", true));
                ModesMenu1.Add("ComboR", new CheckBox("Use R on Combo", true));
                ModesMenu1.Add("useI", new CheckBox("Use itens on Combo", true));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("Harass Configs");
                ModesMenu1.Add("ManaH", new Slider("Dont use Skills if Mana <=", 40));
                ModesMenu1.Add("HarassQ", new CheckBox("Use Q on Harass", true));
                ModesMenu1.Add("HarassW", new CheckBox("Use W on Harass", true));
                ModesMenu1.Add("HarassE", new CheckBox("Use E on Harass", false));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("Kill Steal Configs");
                ModesMenu1.Add("KQ", new CheckBox("Use Q on KillSteal", true));
                ModesMenu1.Add("KW", new CheckBox("Use Q on KillSteal", true));
                ModesMenu1.Add("KE", new CheckBox("Use E to KillSteal", true));
                ModesMenu1.Add("KR", new CheckBox("Use R to KillSteal", true));

                ModesMenu2 = Menu.AddSubMenu("Lane/Jungle/Last", "Modes2Gragas");
                ModesMenu2.AddLabel("LastHit Configs");
                ModesMenu2.Add("ManaL", new Slider("Dont use Skills if Mana <=", 40));
                ModesMenu2.Add("LastQ", new CheckBox("Use Q on LastHit", true));
                ModesMenu2.Add("LastW", new CheckBox("Use W on LastHit", true));
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
                DrawMenu = Menu.AddSubMenu("Draws", "DrawGragas");
                DrawMenu.Add("drawAA", new CheckBox("Draw do AA", true));
                DrawMenu.Add("drawW", new CheckBox(" Draw do W", true));
                DrawMenu.Add("drawE", new CheckBox(" Draw do E", true));
                DrawMenu.Add("drawR", new CheckBox(" Draw do R", true));
                //------------//
                //-Misc Menu-//
                //----------//
                //var Enemies = EntityManager.Heroes.Enemies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);
                Misc = Menu.AddSubMenu("MiscMenu", "Misc");
                Misc.Add("aarest", new CheckBox("Reset AA with Q"));
                Misc.Add("useEGapCloser", new CheckBox("E on GapCloser", true));


            }

            catch (Exception e)
            {
                Chat.Print("KTalon: Exception occured while Initializing Addon. Error: " + e.Message);

            }

        }
        public static void Game_OnDraw(EventArgs args)
        {
            if (DrawMenu["drawAA"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.White, Radius = _Player.GetAutoAttackRange(), BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Aqua, Radius = 600, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Red, Radius = 700, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Blue, Radius = 650, BorderWidth = 2f }.Draw(_Player.Position);
            }
            new Circle() { Color = Color.Blue, Radius = 1100, BorderWidth = 2f }.Draw(_Player.Position);

        }
        public static void Game_OnUpdate(EventArgs args)
        {

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (Program.ModesMenu1["useI"].Cast<CheckBox>().CurrentValue)
                {
                    Itens.UseItens();
                }
              
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
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {


                ModesManager.Flee();
            }
           

            



        }


        public static void Game_OnTick(EventArgs args)
        {

        }



        private static void Reset(AttackableUnit target, EventArgs args)
        {
            if (!Misc["aareset"].Cast<CheckBox>().CurrentValue) return;
            if (target != null && target.IsEnemy && !target.IsInvulnerable && !target.IsDead && target is AIHeroClient && target.Distance(ObjectManager.Player) <= _Player.GetAutoAttackRange())
                if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) && (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit)))) return;
            var e = target as Obj_AI_Base;
            if (!ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue || !e.IsEnemy) return;
            if (target == null) return;
            if (e.IsValidTarget() && Q.IsReady())
            {
                Q.Cast();
            }

        }
        static void KGapCloser(Obj_AI_Base sender, Gapcloser.GapcloserEventArgs args)
        {


            if (sender.IsEnemy && sender is AIHeroClient && sender.Distance(_Player) < E.Range && E.IsReady() && Misc["useEGapCloser"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(sender);
            }
        }

    }
}
