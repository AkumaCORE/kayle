using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK.Utils;
using System.Linq;

namespace KTalon
{
    internal class RLogic
    {
     
        public static void RLogic1()
        {
            var Rtarget = TargetSelector.GetTarget(1000, DamageType.Physical);
            var CastedR = Program.CastedR;  
            var R = Program.R;
            if(!Rtarget.IsValid()) return;
            if (R.IsReady() && (Program.CastedR = true) && Program._Player.Distance(Rtarget) <= 1000)
            {
                R.Cast();
                Program.CastedR = false;
            }







            }
        
    }
}
