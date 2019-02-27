using GungiRevision.Objects;
using GungiRevision.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GungiRevision
{
    class GameRunner
    {
        public static Player black_player, white_player;

        static void Main(string[] args)
        {
            black_player = new Player(PlayerColor.BLACK);
            white_player = new Player(PlayerColor.WHITE);

            U.PrLi( black_player.GetHashCode() + "");
            U.PrLi( white_player.GetHashCode() + "");
        }
    }
}
