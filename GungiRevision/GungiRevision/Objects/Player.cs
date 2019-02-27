using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GungiRevision.Objects
{
    class Player
    {
        public readonly PlayerColor player_color;

        public Player(PlayerColor c)
        {
            player_color = c;
        }

        override
        public bool Equals(Object o)
        {
            if ( o == null || !(o is Player) )
                return false;
            else if ( o == this )
                return true;
            else
            {
                Player player_o = (Player) o;
                return player_color == player_o.player_color;
            }
        }

        override
        public int GetHashCode()
        {
            var hashCode = 121871523;
            hashCode = hashCode * -1258091289 + player_color.GetHashCode();
            return hashCode;
        }

        override
        public string ToString()
        {
            string ret = "INVALID";

            switch ( player_color )
            {
                case PlayerColor.BLACK:
                    ret = "BLACK";
                    break;
                case PlayerColor.WHITE:
                    ret = "WHITE";
                    break;
            }

            return ret;
        }
    }
}
