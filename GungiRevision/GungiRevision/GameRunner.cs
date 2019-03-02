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
        public static Piece p_selected;
        public static Player black, white;
        private static Random random;

        public static void Main(string[] args)
        {
            board = new Board();
            random = new Random();
            p_selected = null;

            black = board.Player(PlayerColor.BLACK);
            white = board.Player(PlayerColor.WHITE);

            //Test();
            Update();
        }

        private static void Test()
        {
            black.PrintHand();
            white.PrintHand();
            //Util.PrLi(AllHashesUnique());
        }

        private static void Update()
        {
            board.PrintBoard();
            





            Piece p1 = white.GetHandPiece(PieceType.MARSHAL);
            board.Select(p1);

            board.PrintBoardSelection();
        }

        private static bool AllHashesUnique()
        {
            List<Object> list = new List<Object>();

            // Player hashes
            list.Add(board.Player(PlayerColor.BLACK));
            list.Add(board.Player(PlayerColor.WHITE));

            // Location hashes
            for (int r = 1; r <= Constants.MAX_RANKS; r++)
                for (int f = 1; f <= Constants.MAX_FILES; f++)
                    for (int t = 1; t <= Constants.MAX_TIERS; t++)
                        list.Add(new Location(r, f, t));
            
            return Util.HashesUnique<Object>(list);
        }
    }
}
