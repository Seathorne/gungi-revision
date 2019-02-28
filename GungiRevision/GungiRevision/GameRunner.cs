using GungiRevision.Objects;
using GungiRevision.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GungiRevision
{
    class GameRunner
    {
        private static Board board;
        private static Player[] players;
        public static Piece p_selected;

        public static void Main(string[] args)
        {
            players = new Player[]
            {
                new Player(board, PlayerColor.BLACK),
                new Player(board, PlayerColor.WHITE)
            };

            board = new Board(players);
            p_selected = null;

            Test();
        }

        private static void Test()
        {
            Util.PrLi(AllHashesUnique());
        }

        private static bool AllHashesUnique()
        {
            List<Object> list = new List<Object>();

            // Player hashes
            list.Add(players[(int)PlayerColor.BLACK]);
            list.Add(players[(int)PlayerColor.WHITE]);

            // Location hashes
            for (int r = 1; r <= Constants.MAX_RANKS; r++)
                for (int f = 1; f <= Constants.MAX_FILES; f++)
                    for (int t = 1; t <= Constants.MAX_TIERS; t++)
                        list.Add(new Location(r, f, t));
            
            return Util.HashesUnique<Object>(list);
        }
    }
}
