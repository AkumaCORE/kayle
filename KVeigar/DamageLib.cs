using EloBuddy;
using EloBuddy.SDK;
namespace KVeigar
{
    internal class DamageLib
    {

        private static readonly AIHeroClient _Player = ObjectManager.Player;
        public static float QCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new[] { 0, 70, 110, 150, 190, 230 }[Program.Q.Level] + 0.6f * _Player.FlatMagicDamageMod));
        }

        public static float WCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new[] { 0, 100, 150, 200, 250, 300 }[Program.W.Level] + 0.99f * _Player.FlatMagicDamageMod));
        }

        public static float RCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new[] { 0, 250, 375, 500 }[Program.R.Level] + 0.99f * _Player.FlatMagicDamageMod + 0.8f * target.FlatMagicDamageMod));




        }
        public static float DmgCalc(AIHeroClient target)
        {
            var damage = 0f;
            if (Program.Q.IsReady() && target.IsValidTarget(Program.Q.Range))
                damage += QCalc(target);
            if (Program.W.IsReady())
                damage += WCalc(target);
            if (Program.R.IsReady() && target.IsValidTarget(Program.R.Range))
                damage += RCalc(target);
            damage += _Player.GetAutoAttackDamage(target, true) * 2;
            return damage;
        }
    }
}
