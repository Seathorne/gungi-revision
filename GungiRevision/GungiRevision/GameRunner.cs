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
        private static Random random;
        private static Player black, white, curr_player, prev_player;
        
        private static Piece sel_piece;
        private static Location sel_location;

        private static GameState gamestate;
        private static Option curr_option;
        private static CheckStatus check_status;
        private static double turn_count;

        public static void Main(string[] args)
        {
            board = new Board();
            random = new Random();

            black = board.Player(PlayerColor.BLACK);
            white = board.Player(PlayerColor.WHITE);
            curr_player = black;
            prev_player = white;

            sel_piece = null;
            sel_location = null;

            gamestate = GameState.START;
            curr_option = Option.NULL;
            check_status = CheckStatus.SAFE;
            turn_count = 1.0;
            
            while (gamestate != GameState.END)
            {
                Update();
            }
        }

        private static void Update()
        {
            curr_option = Option.NULL;
            switch (gamestate)
            {
                case GameState.START:
                    gamestate = GameState.SETUP;
                    break;
                case GameState.SETUP:
                    if (!curr_player.done_setup)
                    {
                        Util.PRL("Placement Round " + turn_count + ": " +  curr_player + "'s turn.");
                        SetupTurn();
                        Util.PRL("\n");
                    }
                    SwapTurn();
                    break;
                case GameState.TURNS:
                    Util.PRL("Round " + turn_count + ": " +  curr_player + "'s turn.");
                    CheckCheck();
                    RegularTurn();
                    Util.PRL("\n");
                    SwapTurn();
                    break;
            }
        }

        private static void PrintBoardBlank()
        {
            board.PrintBoard();
        }
        private static void PrintBoard()
        {
            board.PrintBoardAndHand(curr_player);
        }
        private static void PrintBoardSelection()
        {
            board.Select(sel_piece);
            board.PrintBoardSelection(curr_player);
        }

        private static void SetupTurn()
        {
            // Drop
            if (curr_player.HasHandPiece(PieceType.MARSHAL))
            {
                PrintBoard();
                sel_piece = curr_player.GetHandPiece(PieceType.MARSHAL);
                Util.PRL("<< The marshal must be dropped first.");

                while(curr_option != Option.NEXT)
                {
                    PrintBoardSelection();
                    curr_option = SelectPrompt("Enter a location to drop this [" + sel_piece + "].", new List<Option> { Option.SELECT_DROP });

                    if (curr_option == Option.SELECT_DROP)
                    {
                        board.DropPieceTo(sel_piece, sel_location.rank, sel_location.file);
                        Util.PRL("Drop successful.");
                        curr_option = Option.NEXT;
                    }
                    else
                    {
                        Util.PRL("Invalid command.");
                    }
                }
            }
            else if (board.PlayerAllPieces(curr_player).Count < Constants.MIN_SETUP_PIECES)
            {
                // View or select, then back or drop
                while(curr_option != Option.NEXT)
                {
                    PrintBoard();
                    curr_option = SelectPrompt("Select a piece in your hand or on the board.", new List<Option> { Option.SELECT_HAND_PIECE, Option.SELECT_BOARD_PIECE });

                    if (curr_option == Option.SELECT_HAND_PIECE)
                    {
                        CheckDropPiece();
                    }
                    else if (curr_option == Option.SELECT_BOARD_PIECE)
                    {
                        PrintBoardSelection();
                        Wait();
                    }
                    else
                    {
                        Util.PRL("Invalid command.");
                    }
                }
            }
            else if (board.PlayerAllPieces(curr_player).Count < Constants.MAX_BOARD_PIECES)
            {
                // View, pass, done, or select, then back or drop
                while(curr_option != Option.NEXT)
                {
                    PrintBoard();
                    curr_option = SelectPrompt("Select a piece in your hand or on the board. [Pass] to pass, [Done] to finish placement.", new List<Option> { Option.PASS, Option.DONE, Option.SELECT_HAND_PIECE, Option.SELECT_BOARD_PIECE });

                    if (curr_option == Option.PASS)
                    {
                        Util.PRL(curr_player.color + " has passed their turn.");
                        curr_player.passed = true;
                        curr_option = Option.NEXT;
                    }
                    else if (curr_option == Option.DONE)
                    {
                        Util.PRL(curr_player.color + " has completed their placement phase.");
                        curr_player.done_setup = true;
                        curr_option = Option.NEXT;
                    }
                    else if (curr_option == Option.SELECT_HAND_PIECE)
                    {
                        CheckDropPiece();
                    }
                    else if (curr_option == Option.SELECT_BOARD_PIECE)
                    {
                        PrintBoardSelection();
                        Wait();
                    }
                    else
                    {
                        Util.PRL("Invalid command.");
                    }
                }
            }
            else
            {
                Util.PRL(curr_player.color + " has completed their placement phase.");
                curr_player.done_setup = true;
            }
        }

        private static void RegularTurn()
        {
            if (board.PlayerTopPieces(curr_player).Count < Constants.MAX_BOARD_PIECES)
            {
                // View or select, then back, drop, move, or attack
                while(curr_option != Option.NEXT)
                {
                    PrintBoard();
                    curr_option = SelectPrompt("Select a piece in your hand or on the board.", new List<Option> { Option.SELECT_HAND_PIECE, Option.SELECT_BOARD_PIECE });

                    if (curr_option == Option.SELECT_HAND_PIECE)
                    {
                        CheckDropPiece();
                    }
                    else if (curr_option == Option.SELECT_BOARD_PIECE)
                    {
                        if (board.PlayerTopPieces(curr_player).Contains(sel_piece))
                            CheckMoveOrAttackPiece();
                        else
                        {
                            PrintBoardSelection();
                            Wait();
                        }
                    }
                    else
                    {
                        Util.PRL("Invalid command.");
                    }
                }
            }
        }

        private static void CheckCheck()
        {
            check_status = board.CheckCheck(curr_player);

            if (turn_count == 1.0)
            {
                CheckStatus other_check_status = board.CheckCheck(prev_player);
                if (other_check_status == CheckStatus.CHECK || other_check_status == CheckStatus.CHECKMATE)
                {
                    Util.PRL("<< " + prev_player.color + " has been placed in check. Since it is " + curr_player.color + "'s turn, " + curr_player.color + " wins the game!");
                    gamestate = GameState.END;
                }
            }
            else if (check_status == CheckStatus.CHECK)
            {
                Util.PRL("<< " + curr_player.color + " has been placed in check.");
            }
            else if (check_status == CheckStatus.CHECKMATE)
            {
                Util.PRL("<< " + curr_player.color + " has been checkmated! " + Util.OtherPlayerColor(curr_player.color) + " wins the game!");
                gamestate = GameState.END;
            }
        }
        
        private static void SwapTurn()
        {
            sel_piece = null;
            sel_location = null;

            if (gamestate == GameState.SETUP && (prev_player.done_setup || prev_player.passed) && (curr_player.done_setup || curr_player.passed) )
            {
                EndSetup();
                return;
            }

            if (curr_player == black)
            {
                prev_player = black;
                curr_player = white;
            }
            else
            {
                prev_player = white;
                curr_player = black;
            }
            curr_player.passed = false;
            
            turn_count += 0.5;
        }

        private static void EndSetup()
        {
            Util.PRL("<< Both players have completed their placement phases.");
            Util.PRL("<< The game will now begin!\n\n");

            gamestate = GameState.TURNS;
            board.SetGameState(gamestate);

            turn_count = 1.0;
            curr_option = Option.NULL;
        }

        private static void CheckDropPiece()
        {
            PrintBoardSelection();

            while(curr_option != Option.BACK && curr_option != Option.NEXT)
            {
                curr_option = SelectPrompt("Enter a location to drop this [" + sel_piece + "]. [Back] to return.", new List<Option> { Option.BACK, Option.SELECT_DROP });

                if (curr_option == Option.SELECT_DROP)
                {
                    board.DropPieceTo(sel_piece, sel_location.rank, sel_location.file);
                    Util.PRL("Drop successful.");
                    curr_option = Option.NEXT;
                }
            }
        }

        private static void CheckMoveOrAttackPiece()
        {
            PrintBoardSelection();

            while(curr_option != Option.BACK && curr_option != Option.NEXT)
            {
                curr_option = SelectPrompt("Enter a target location for this [" + sel_piece + "]. [Back] to return.", new List<Option> { Option.BACK, Option.SELECT_MOVE, Option.SELECT_ATTACK });

                if (curr_option == Option.SELECT_MOVE)
                {
                    board.MovePieceTo(sel_piece, sel_location.rank, sel_location.file);
                    Util.PRL("Movement successful.");
                    curr_option = Option.NEXT;
                }
                else if (curr_option == Option.SELECT_ATTACK)
                {
                    board.AttackPieceTo(sel_piece, sel_location.rank, sel_location.file);
                    Util.PRL("Capture successful.");
                    curr_option = Option.NEXT;
                }
            }
        }

        private static Option SelectPrompt(String prompt, List<Option> options)
        {
            Option op = Option.NULL;

            Util.PRL(prompt);
            Util.Pr(">> ");
            String choice = Console.ReadLine().Trim().ToLower();

            if (options.Contains(Option.BACK) && choice == "back")
            {
                op = Option.BACK;
            }
            else if (options.Contains(Option.PASS) && choice == "pass")
            {
                op = Option.PASS;
            }
            else if (options.Contains(Option.DONE) && choice == "done")
            {
                op = Option.DONE;
            }
            else if (options.Contains(Option.SELECT_HAND_PIECE) && SelectHandPiece(choice))
            {
                // sel_piece set in method
                op = Option.SELECT_HAND_PIECE;
            }
            else if (options.Contains(Option.SELECT_BOARD_PIECE) && SelectLocation(choice, Option.SELECT_BOARD_PIECE))
            {
                sel_piece = board.PieceAt(sel_location);
                op = Option.SELECT_BOARD_PIECE;
            }
            else if(options.Contains(Option.SELECT_DROP) && SelectLocation(choice, Option.SELECT_DROP))
            {
                // sel_location set in method
                op = Option.SELECT_DROP;
            }
            else if(options.Contains(Option.SELECT_MOVE) && SelectLocation(choice, Option.SELECT_MOVE))
            {
                // sel_location set in method
                op = Option.SELECT_MOVE;
            }
            else if(options.Contains(Option.SELECT_ATTACK) && SelectLocation(choice, Option.SELECT_ATTACK))
            {
                // sel_location set in method
                op = Option.SELECT_ATTACK;
            }
            else
            {
                op = Option.NULL;
            }

            if (op == Option.SELECT_BOARD_PIECE || op == Option.SELECT_HAND_PIECE)
                board.Select(sel_piece);

            return op;
        }

        private static bool SelectHandPiece(string piece_char)
        {
            sel_piece = null;

            if (piece_char.ToLower() == "x")
            {
                int x = random.Next(0, curr_player.p_hand.Count());
                sel_piece = curr_player.p_hand.ElementAt(x);
            }
            else if (piece_char.Length == 1)
            {
                PieceType type = (PieceType)piece_char.ToUpper()[0];
                sel_piece = curr_player.GetHandPiece(type);
            }

            return sel_piece != null;
        }

        private static bool SelectLocation(string rft, Option location_type)
        {
            sel_location = null;
            bool valid_location_option = false;
            MatchCollection matches = Regex.Matches(rft, @"(\d)+");

            if (Regex.IsMatch(rft.ToLower(), @"x") && location_type == Option.SELECT_DROP)
            {
                int xr, xf;
                do
                {
                    xr = random.Next(1, Constants.MAX_RANKS);
                    xf = random.Next(1, Constants.MAX_FILES);
                }
                while(!sel_piece.CanDropTo(xr, xf));
                sel_location = sel_piece.GetDropAt(xr, xf);
            }
            else if (matches.Count >= 2)
            {
                int r = Convert.ToInt32(matches[0].Value);
                int f = Convert.ToInt32(matches[1].Value);

                if (matches.Count == 2 && Util.ValidLocation(r, f))
                {
                    if (location_type == Option.SELECT_DROP)
                    {
                        if (board.StackHeight(r, f) < Constants.MAX_TIERS)
                            sel_location = new Location(r, f, board.StackHeight(r, f)+1);
                    }
                    else if (location_type == Option.SELECT_BOARD_PIECE)
                    {
                        Piece top = board.TopPieceAt(r, f);
                        if (top != null)
                            sel_location = top.location;
                    }
                    else if (location_type == Option.SELECT_MOVE && Regex.IsMatch(rft, @"m"))
                    {
                        if (board.StackHeight(r, f) < Constants.MAX_TIERS)
                            sel_location = new Location(r, f, board.StackHeight(r, f)+1);
                    }
                    else if (location_type == Option.SELECT_ATTACK && Regex.IsMatch(rft, @"a"))
                    {
                        Piece top = board.TopPieceAt(r, f);
                        if (top != null)
                            sel_location = top.location;
                    }
                }
                else if (matches.Count >= 3)
                {
                    int t = Convert.ToInt32(matches[2].Value);
                    if (Util.ValidLocation(r, f, t))
                        sel_location = new Location(r, f, t);
                }
            }

            if (sel_location != null)
            {
                switch (location_type)
                {
                    case Option.SELECT_DROP:
                        valid_location_option = sel_piece.CanDropTo(sel_location);
                        if (valid_location_option && check_status == CheckStatus.CHECK)
                            if (board.CheckStatusAfterCloneDropTo(curr_player.color, sel_piece.type, sel_location.rank, sel_location.file) != CheckStatus.SAFE)
                            {
                                valid_location_option = false;
                                Util.PRL("This drop leaves " + curr_player + " in check.");
                            }
                        break;
                    case Option.SELECT_MOVE:
                        valid_location_option = sel_piece.CanMoveTo(sel_location);
                        if (valid_location_option)
                            if (board.CheckStatusAfterCloneMoveTo(curr_player.color, sel_piece.location.rank, sel_piece.location.file, sel_location.rank, sel_location.file) != CheckStatus.SAFE)
                            {
                                valid_location_option = false;
                                Util.PRL("This move leaves " + curr_player + " in check.");
                            }
                        break;
                    case Option.SELECT_ATTACK:
                        valid_location_option = sel_piece.CanAttackTo(sel_location);
                        if (valid_location_option)
                            if (board.CheckStatusAfterCloneAttackTo(curr_player.color, sel_piece.location.rank, sel_piece.location.file, sel_location.rank, sel_location.file) != CheckStatus.SAFE)
                            {
                                valid_location_option = false;
                                Util.PRL("This capture leaves " + curr_player + " in check.");
                            }
                        break;
                    case Option.SELECT_BOARD_PIECE:
                        valid_location_option = board.PieceAt(sel_location) != null;
                        break;
                    default:
                        valid_location_option = false;
                        break;
                }
            }

            if (!valid_location_option)
                sel_location = null;

            return valid_location_option;
        }

        private static void Wait()
        {
            Util.PRL("Press a key to continue.");
            Console.ReadKey(true);
            Util.PRL("");
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
