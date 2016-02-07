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

namespace Kassadin
{
    class KKassadin
    {
        public const string ChampionName = "Kassadin";

        public static Menu Menu, ModesMenu, DrawMenu;

        public static Spell.Targeted Q;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        public static Spell.Targeted Ignite;

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
        private static int EBuffCount
        {
            get { return _Player.GetBuffCount("forcepulsecounter"); }
        }
        public static float RMana
        {
            get { return _Player.Spellbook.GetSpell(SpellSlot.R).SData.Mana; }
        }

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnStart;
            Drawing.OnDraw += Game_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
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
                Chat.Print("KKassadin Addon Loading Success", Color.Aqua);
                Q = new Spell.Targeted(SpellSlot.Q, 650);
                W = new Spell.Active(SpellSlot.W);
                E = new Spell.Skillshot(SpellSlot.E, 700, SkillShotType.Cone, (int)0.5f, int.MaxValue, 10);
                R = new Spell.Skillshot(SpellSlot.R, 500, SkillShotType.Circular, (int)0.5f, int.MaxValue, 150);
                if (_Player.GetSpellSlotFromName("summonerdot") != SpellSlot.Unknown)
                    Ignite = new Spell.Targeted(_Player.GetSpellSlotFromName("summonerdot"), 550);

                Menu = MainMenu.AddMenu("KKassadin", "kassadin");
                Menu.AddSeparator();
                Menu.AddLabel("Criado por Bruno105");
                //------------//
                //-Mode Menu-//
                //-----------//
                ModesMenu = Menu.AddSubMenu("Modes", "ModesKassadin");
                ModesMenu.AddSeparator();
                ModesMenu.AddLabel("Combo Configs");
                ModesMenu.Add("ComboQ", new CheckBox("Use Q on Combo", true));
                ModesMenu.Add("ComboW", new CheckBox("Use W on Combo", true));
                ModesMenu.Add("ComboE", new CheckBox("Use E on Combo", true));
                ModesMenu.Add("ComboR", new CheckBox("Use R on Combo", true));
                ModesMenu.Add("MaxR", new Slider("Don't use R if more than Champs on range :", 2, 1, 5));
                ModesMenu.AddSeparator();
                ModesMenu.AddLabel("Harass Configs");
                ModesMenu.Add("ManaH", new Slider("Dont use Skills if Mana <=", 40));
                ModesMenu.Add("HarassQ", new CheckBox("Use Q on Harass", true));
                ModesMenu.Add("HarassW", new CheckBox("Use W on Harass", true));
                ModesMenu.Add("HarassE", new CheckBox("Use E on Harass", true));
                ModesMenu.Add("HarassR", new CheckBox("Use R on Harass", true));
                ModesMenu.AddSeparator();
                ModesMenu.AddLabel("Farm Configs");
                ModesMenu.Add("ManaF", new Slider("Dont use Skills if Mana <=", 40));
                ModesMenu.Add("FarmQ", new CheckBox("Use Q on Farm", true));
                ModesMenu.Add("FarmW", new CheckBox("Use W on Farm", true));
                ModesMenu.Add("FarmE", new CheckBox("Use E on Farm", true));
                ModesMenu.Add("MinionE", new Slider("Use E when count minions more than :",3,1,5));
                ModesMenu.Add("FarmR", new CheckBox("Use R on Farm", true));

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
                Chat.Print("KKassadin: Exception occured while Initializing Addon. Error: " + e.Message);
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
                new Circle() { Color = Color.Green, Radius = (_Player.GetAutoAttackRange() + 50), BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Red, Radius = 600, BorderWidth = 2f }.Draw(_Player.Position);
            }
            if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.Blue, Radius = 500, BorderWidth = 2f }.Draw(_Player.Position);
            }

        }

        static void Game_OnUpdate(EventArgs args)
        {


            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                var alvo = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                var rmax = EntityManager.Heroes.Enemies.Where(t => t.IsInRange(Player.Instance.Position, R.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
                if (!alvo.IsValid()) return;
                if (Q.IsReady() && Q.IsInRange(alvo) && ModesMenu["ComboQ"].Cast<CheckBox>().CurrentValue)
                {
                    Q.Cast(alvo);
                }
                if (W.IsReady() && _Player.Distance(alvo) <= _Player.GetAutoAttackRange() + 50 && ModesMenu["ComboW"].Cast<CheckBox>().CurrentValue)
                {
                    W.Cast();

                }
                if (E.IsReady() && E.IsInRange(alvo) && ModesMenu["ComboE"].Cast<CheckBox>().CurrentValue)
                {
                    E.Cast(alvo);

                }
                if (R.IsReady() && R.IsInRange(alvo) && ModesMenu["ComboE"].Cast<CheckBox>().CurrentValue && !(rmax >= ModesMenu["MaxR"].Cast<Slider>().CurrentValue))
                {
                    R.Cast(alvo);

                }


            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                var alvo = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (!alvo.IsValid()) return;



                if ((_Player.ManaPercent <= ModesMenu["ManaH"].Cast<Slider>().CurrentValue))
                {
                    return;
                }
                if (Q.IsReady() && Q.IsInRange(alvo) && ModesMenu["HarassQ"].Cast<CheckBox>().CurrentValue)
                {
                    Q.Cast(alvo);
                }
                if (W.IsReady() && _Player.Distance(alvo) <= _Player.GetAutoAttackRange() + 50 && ModesMenu["HarassW"].Cast<CheckBox>().CurrentValue)
                {
                    W.Cast();

                }
                if (E.IsReady() && E.IsInRange(alvo) && ModesMenu["HarassE"].Cast<CheckBox>().CurrentValue)
                {
                    E.Cast(alvo);

                }
                if (R.IsReady() && R.IsInRange(alvo) && ModesMenu["HarassR"].Cast<CheckBox>().CurrentValue)
                {
                    R.Cast(alvo);

                }


            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {

                
                
                var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _Player.Position, Q.Range).OrderByDescending(x => x.MaxHealth).FirstOrDefault();

                var minion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(t => t.IsInRange(Player.Instance.Position, E.Range) && !t.IsDead && t.IsValid && !t.IsInvulnerable).Count();
                
                if ((_Player.ManaPercent <= ModesMenu["ManaF"].Cast<Slider>().CurrentValue))
                {
                    return;
                }
                if (Q.IsReady() && Q.IsInRange(minions) && ModesMenu["FarmQ"].Cast<CheckBox>().CurrentValue)
                {
                    Q.Cast(minions);
                }
                if (W.IsReady() && _Player.Distance(minions) <= _Player.GetAutoAttackRange() + 50 && ModesMenu["FarmW"].Cast<CheckBox>().CurrentValue)
                {
                    W.Cast();

                }
                if (E.IsReady() && E.IsInRange(minions) && ModesMenu["FarmE"].Cast<CheckBox>().CurrentValue && (minion >= ModesMenu["MinionE"].Cast<Slider>().CurrentValue))
                {

                    E.Cast(minions);

                }
                if (R.IsReady() && R.IsInRange(minions) && ModesMenu["FarmR"].Cast<CheckBox>().CurrentValue)
                {
                    R.Cast(minions);

                }
            }
        }
    }
}

