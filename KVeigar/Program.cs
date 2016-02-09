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

namespace KVeigar
{
    internal class Program
    {
        public const string ChampionName = "Veigar";
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
        public static Spell.Targeted R;
        public static Spell.Targeted Ignite;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnStart;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Game_OnDraw;
            
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
                Chat.Print("KVeigar Addon Loading Successs", Color.Green);
              
                Q = new Spell.Skillshot(SpellSlot.Q, 950, SkillShotType.Linear, 250, 2000, 65);
                Q.AllowedCollisionCount = 1;
                W = new Spell.Skillshot(SpellSlot.W,900,SkillShotType.Circular,1350,0,210);
                E = new Spell.Skillshot(SpellSlot.E,690,SkillShotType.Circular,300,0,375);
                R = new Spell.Targeted(SpellSlot.R,650);
                var Enemies = EntityManager.Heroes.Enemies.Where(a => !a.IsMe).OrderBy(a => a.BaseSkinName);
                Menu = MainMenu.AddMenu("KVeigar", "veigar");
                Menu.AddSeparator();
                Menu.AddLabel("Criado por Bruno105");
                ModesMenu1 = Menu.AddSubMenu("Combo/Harass", "Modes1Veigar");
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
                ModesMenu1.Add("UseIgnite", new CheckBox("Use Ignite on Combo", true));
                ModesMenu1.AddSeparator();
                ModesMenu1.AddLabel("Harass Configs");
                ModesMenu1.Add("ManaH", new Slider("Dont use Skills if Mana <=", 40));
                ModesMenu1.Add("HarassQ", new CheckBox("Use Q on Harass", true));
                ModesMenu1.Add("HarassW", new CheckBox("Use W on Harass", true));
                ModesMenu2 = Menu.AddSubMenu("Lane/LastHit", "Modes2Veigar");
                ModesMenu2.AddLabel("LastHit Configs");
                ModesMenu2.Add("ManaL", new Slider("Dont use Skills if Mana <=", 40));
                ModesMenu2.Add("LastQ", new CheckBox("Use Q on LastHit", true));
                ModesMenu2.Add("LastW", new CheckBox("Use W on LastHit", true));
                ModesMenu2.AddLabel("Lane Cler Config");
                ModesMenu2.Add("ManaF", new Slider("Dont use Skills if Mana <=", 40));
                ModesMenu2.Add("FarmQ", new CheckBox("Use Q on LaneClear", true));
                ModesMenu2.Add("FarmW", new CheckBox("Use W on LaneClear", true));
                ModesMenu2.AddSeparator();
                ModesMenu2.AddLabel("W Count Config");
                ModesMenu2.Add("MinionW", new Slider("Use W when count minions more than :", 3, 1, 5));
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
                Chat.Print("KVeigar: Exception occured while Initializing Addon. Error: " + e.Message);
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
                new Circle() { Color = Color.Aqua, Radius = 950, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Green, Radius = 900, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Red, Radius = 700, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Blue, Radius = 650, BorderWidth = 2f }.Draw(_Player.Position);
            }

        }


        static void Game_OnUpdate(EventArgs args)
        {
          
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var alvo = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (!alvo.IsValid()) return;
            
            if (Q.IsReady() && Q.IsInRange(alvo) && ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue)
                {
                    Q.Cast(alvo);
                }
                if (W.IsReady() && W.IsInRange(alvo) && ModesMenu1["ComboW"].Cast<CheckBox>().CurrentValue)
                {
                    W.Cast(alvo);

                }
                if (E.IsReady() && E.IsInRange(alvo) && ModesMenu1["ComboE"].Cast<CheckBox>().CurrentValue)
                {
                    E.Cast(alvo);

                }
                if (R.IsReady() && R.IsInRange(alvo) && ModesMenu1["ComboR"].Cast<CheckBox>().CurrentValue && ModesMenu1["Ult_" + alvo.BaseSkinName].Cast<CheckBox>().CurrentValue)
                {
                    R.Cast(alvo);

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
                    E.Cast(alvo);

                }
                if (R.IsReady() && R.IsInRange(alvo) && ModesMenu1["HarassR"].Cast<CheckBox>().CurrentValue)
                {
                    R.Cast(alvo);

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
                
                if (W.IsReady() && W.IsInRange(minions) && ModesMenu2["FarmW"].Cast<CheckBox>().CurrentValue &&  (minion >= ModesMenu2["MinionW"].Cast<Slider>().CurrentValue))
                {
                    W.Cast(minions);

                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                var qminions =EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(Q.Range) && (DamageLib.QCalc(m) > m.Health));
                var wminions = EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(m => m.IsValidTarget(W.Range) && (DamageLib.WCalc(m) > m.Health));
                var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, E.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
                if (qminions == null) return;
                var prediction = Q.GetPrediction(qminions);
                if (Q.IsReady() && Q.IsInRange(qminions) && ModesMenu2["LastQ"].Cast<CheckBox>().CurrentValue && qminions.Health < DamageLib.QCalc(qminions))
                {
                    Q.Cast(qminions);
                }
                if (W.IsReady() && W.IsInRange(wminions) && ModesMenu2["LastW"].Cast<CheckBox>().CurrentValue && wminions.Health < DamageLib.WCalc(wminions) && !(Q.IsReady()) && (minion >= ModesMenu2["MinionW"].Cast<Slider>().CurrentValue))
                {
                    W.Cast(wminions);
                }

        }







        }


    }
}
