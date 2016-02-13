using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace KLissandra
{
    internal class Ref
    {



        public static void EToMouse(Vector3 Position)
        {
            if (Program.E.IsReady() && LissEMissile == null )
            {
              Program.E.Cast(Position);
                jumping = true;
            }
        }

        public  static Vector2 MissilePosition;
        public static MissileClient LissEMissile;
        public static bool jumping;
        private static readonly AIHeroClient _Player = ObjectManager.Player;

        public static void MonitorMissilePosition(EventArgs args)
        {
            
            var E = Program.E;

            if (LissEMissile == null || _Player.IsDead)
            {
                return;
            }
            MissilePosition = LissEMissile.Position.To2D();
            if (jumping)
            {
                if ((Vector2.Distance(MissilePosition, LissEMissile.EndPosition.To2D()) < 40))
                {
                    E.Cast();
                    jumping = false;
                }
              
            }
        }

       public static void SecondEChecker(Obj_AI_Base target)
        {
              var E = Program.E;

            if ( LissEMissile != null && E.IsReady())
            {
                if (Vector2.Distance(MissilePosition, target.ServerPosition.To2D()) < Vector3.Distance(Program.PlayerInstance.ServerPosition, target.ServerPosition) &&  Vector3.Distance(target.ServerPosition, LissEMissile.EndPosition) > Vector3.Distance(Program.PlayerInstance.ServerPosition, target.ServerPosition))
                {
                   E.Cast(Program.PlayerInstance.Position);
                    return;
                }           
            }
        }
    
    
    
    }

}
