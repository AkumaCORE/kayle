using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using System.Linq;

namespace KJanna
{
   internal class ModesManager
    {
       
        public static void Support()
        {
            var useQ = Program.Combo["ComboQ"].Cast<CheckBox>().CurrentValue;
            var useW = Program.Combo["ComboW"].Cast<CheckBox>().CurrentValue;

            var Q = Program.Q;
            var W = Program.W;
            var E = Program.E;
            var R = Program.R;
            var alvoQ = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            var alvoW = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (!alvoQ.IsValid()) return;
            if (!alvoW.IsValid()) return;
            if (useQ && Q.IsInRange(alvoQ) && Q.IsReady() && !(Program._Player.HasBuff("Howling Gale")))
            {
                Q.Cast(alvoQ);
                if ((Program._Player.HasBuff("Holing Gale")))
                {
                    ;
                    Q.Cast(alvoQ);

               }

           }
            if (useW && W.IsInRange(alvoW) && W.IsReady())
           {
                W.Cast(alvoW);


           }








        }





    }
}
