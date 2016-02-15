using EloBuddy;
using EloBuddy.SDK;

namespace KTalon
{
    internal class DamageLib
    {

        private static readonly AIHeroClient _Player = ObjectManager.Player;
        public static float QCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 30, 55, 90, 120, 150 }[Program.Q.Level] + 1.6f * _Player.FlatPhysicalDamageMod));
        }

        public static float WCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 60, 110, 160, 210, 460 }[Program.W.Level] + 0.6f * _Player.FlatPhysicalDamageMod ));
        }



        public static float RCalc(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 450, 650, 850 }[Program.R.Level] + 0.85f * _Player.FlatPhysicalDamageMod
                    ));
        }
        public static float DmgCalc(AIHeroClient target)
        {
            var damage = 0f;
            if (Program.Q.IsReady() && target.IsValidTarget(Program.W.Range))
                damage += QCalc(target);
            if (Program.W.IsReady() && target.IsValidTarget(Program.W.Range))
                damage += WCalc(target);
            if (Program.R.IsReady() && target.IsValidTarget(Program.R.Range))
                damage += RCalc(target);
            damage += _Player.GetAutoAttackDamage(target, true) * 2;
            return damage;
        }


    }
}