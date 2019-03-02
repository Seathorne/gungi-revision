using GungiRevision.Objects;
using GungiRevision.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GungiRevision
{
    class GameRunner
    {
        private static Board board;
        private static Player black, white, curr_player;
        
        private static Piece sel_piece;
        private static Location sel_location;
        private static GameState gamestate;
        private static double turn_count;

        private static Random random;

        public static void Main(string[] args)
        {
            board = new Board();
            random = new Random();

            black = board.Player(PlayerColor.BLACK);
            white = board.Player(PlayerColor.WHITE);
            
            curr_player = black;
            sel_piece = null;
            sel_location = null;
            gamestate = GameState.START;
            turn_count = 0.0;
            
            while (gamestate != GameState.END)
            {
                Update();
            }
        }

        private static void Update()
        {
            switch (gamestate)
            {
                case GameState.START:
                    gamestate = GameState.SETUP;
                    break;
                case GameState.SETUP:
                    board.PrintBoard();
                    SetupTurn();
                    SwapTurn();
                    break;
            }
        }

        private static void SetupTurn()
        {
            if (curr_player.HasHandPiece(PieceType.MARSHAL))
            {
                sel_piece = curr_player.GetHandPiece(PieceType.MARSHAL);
                board.Select(sel_piece);
                board.PrintBoardSelection();

                while(!LocationPrompt("Enter a location to drop this [" + sel_piece + "]."));
                if (board.PieceAt(sel_location) == null && sel_piece.CanDropTo(sel_location.rank, sel_location.file))
                    board.DropPieceTo(sel_piece, sel_location.rank, sel_location.file);
            }
            else
            {
                String choice = Console.ReadLine().Trim();
            }
        }

        private static void SwapTurn()
        {
            if (curr_player == black)
                curr_player = white;
            else
                curr_player = black;
            turn_count += 0.5;
        }

        private static void SelectPrompt(String prompt)
        {
            Util.PRL(prompt);
            sel_piece = null;

            while (sel_piece == null)
            {
                String choice = Console.ReadLine().Trim();
                if (choice == "pass")
                {
                    curr_player.passed = true;
                    return;
                }
                else if (choice == "done")
                {
                    curr_player.done_setup = true;
                    return;
                }
                else if (choice.Length == 1)
                {
                    PieceType type = (PieceType)choice[0];
                    if (curr_player.HasHandPiece(type))
                        sel_piece = curr_player.GetHandPiece(type);
                    else
                        Util.PRL("Invalid piece type.");
                }
                else
                {
                    Location loc = ConvertToLocation(choice);
                    Util.PRL(loc);
                    if (loc != null)
                        sel_piece = board.PieceAt(loc);
                }
            }

            board.Select(sel_piece);
        }

        private static bool LocationPrompt(String prompt)
        {
            Util.PRL(prompt);
            sel_location = null;

            while (sel_location == null)
            {
                String choice = Console.ReadLine().Trim();

                if (choice == "back")
                    return false;
                else
                {
                    ConvertToLocation(choice);
                }
            }

            return true;
        }

        private static Location ConvertToLocation(String rft)
        {
            MatchCollection matches = Regex.Matches(rft, @"(\d)+");
            if (matches.Count >= 2)
            {
                int r = Convert.ToInt32(matches[0].Value);
                int f = Convert.ToInt32(matches[1].Value);

                if (matches.Count == 2 && Util.ValidLocation(r, f))
                {
                    Piece top = board.TopPieceAt(r, f);
                    if (top == null)
                    {
                        sel_location = new Location(r, f, board.StackHeight(r, f)+1);
                    }
                }
                else if (matches.Count >= 3)
                {
                    int t = Convert.ToInt32(matches[2].Value);
                    if (Util.ValidLocation(r, f, t))
                        sel_location = new Location(r, f, t);
                }

                if (sel_location == null)
                    Util.PRL("Invalid location.");
            }
            else
            {
                Util.PRL("Invalid command.");
            }

            return sel_location;
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
