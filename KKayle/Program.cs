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

namespace Kayle
{
    class KKayle
    {

        public static Menu Menu, DrawMenu, ComboMenu, HarassMenu, FarmMenu, HealMenu;
        public static Spell.Targeted Q;
        public static Spell.Targeted W;
        public static Spell.Active E;
        public static Spell.Targeted R;
        public const string ChampionName = "Kayle";

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
            if (ChampionName != PlayerInstance.BaseSkinName)
            {
                return;
            }
            Bootstrap.Init(null);

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

            /* Farm Menu
            FarmMenu = Menu.AddSubMenu("Farm", "FarmKayle");
            FarmMenu.Add("FarmQ", new CheckBox("Usar Q para Farmar", true));
            FarmMenu.Add("FarmE", new CheckBox("Usar E para Farmar", true));
            */
            // Heal Menu
            var allies = EntityManager.Heroes.Allies.Where(a => !a.IsMe).ToArray();
            HealMenu = Menu.AddSubMenu("Heal", "FarmKayle");
            HealMenu.Add("autoW", new CheckBox("Usar W automaticamente", true));
            HealMenu.Add("HealSelf", new Slider("Usar W em si quando % HP", 50));
            HealMenu.Add("HealAlly", new Slider("Usar W nos aliados quando % HP", 50));
            foreach (var a in allies)
            {
                HealMenu.Add("autoHeal_" + a.BaseSkinName, new CheckBox("Usar Heal nos champs " + a.BaseSkinName));
            }



            // Draw Menu
            DrawMenu = Menu.AddSubMenu("Drwans", "DrawnKayle");
            // DrawMenu.Add("drawDisable", new CheckBox("Desabilidatar todos os Draw", false));
            DrawMenu.Add("drawAA", new CheckBox("Desabilidatar Draw do AA", true));
            DrawMenu.Add("drawQ", new CheckBox("Desabilidatar Draw do Q", true));
            DrawMenu.Add("drawW", new CheckBox("Desabilidatar Draw do W", true));
            DrawMenu.Add("drawE", new CheckBox("Desabilidatar Draw do E", true));



        }

        // <Draw Game//
        // Game OnDraw//
        // ------------- //

        public static void Game_OnDraw(EventArgs args)
        {
            if (DrawMenu["drawAA"].Cast<CheckBox>().CurrentValue)
            {
                new Circle() { Color = Color.White, Radius = ObjectManager.Player.GetAutoAttackRange(), BorderWidth = 2f }.Draw(_Player.Position);
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


        /*private static void AutoHeal()
        {
           
            var autoWSelf = HealMenu["HealSelf"].Cast<Slider>().CurrentValue;
            var autoWAlly = HealMenu["HealAlly"].Cast<Slider>().CurrentValue;

            if (HealMenu["autoW"].Cast<CheckBox>().CurrentValue && W.IsReady())
            {
                var lowestHealthAlly = EntityManager.Heroes.Allies.OrderBy(a => a.Health).FirstOrDefault(a => W.IsInRange(a) && !a.IsMe);

                if (lowestHealthAlly != null)
                {
                    if (lowestHealthAlly.HealthPercent <= autoWAlly && PlayerInstance.HealthPercent >= autoWSelf)
                    {
                        if (HealMenu["autoHeal_" + lowestHealthAlly.BaseSkinName].Cast<CheckBox>().CurrentValue)
                        {
                            W.Cast(lowestHealthAlly);
                        }
                    }
                }
            }
        }*/
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
                if (!(lowestHealthAlly.Health <= HealMenu["autoW"].Cast<Slider>().CurrentValue))
                {
                    return;
                }
                if (HealMenu["autoHeal_" + lowestHealthAlly.BaseSkinName].Cast<CheckBox>().CurrentValue)
                {
                    W.Cast(lowestHealthAlly);
                }
            }
        }








        // ------------//
        // Game On Update
        // ------------//

        public static void Game_OnUpdate(EventArgs args)
        {
            var alvo = TargetSelector.GetTarget(1000, DamageType.Mixed);
            if (!alvo.IsValid()) return;

            //-------------//
            //----Combo----//
            //-------------//
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Combo)
            {
                if (Q.IsReady())
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
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Harass)
            {
                if (Q.IsReady() && HarassMenu["HarassQ"].Cast<CheckBox>().CurrentValue)
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




        }

    }
}
