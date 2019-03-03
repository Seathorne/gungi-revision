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
        private static bool log_output;
        
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
            log_output = true;
            
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
                        CheckCheck();
                        Util.L("Placement Round " + turn_count + ": " +  curr_player + "'s turn.");
                        SetupTurn();
                        Util.L("");
                    }
                    SwapTurn();
                    break;
                case GameState.TURNS:
                    Util.L("Round " + turn_count + ": " +  curr_player + "'s turn.");
                    if (CheckCheck() == CheckStatus.CHECKMATE)
                        return;
                    RegularTurn();
                    Util.L("");
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
                CheckDropMarshal();
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
                        Util.L("Invalid command.");
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
                        Util.L(curr_player.color + " has passed their turn.");
                        curr_player.passed = true;
                        curr_option = Option.NEXT;
                    }
                    else if (curr_option == Option.DONE)
                    {
                        Util.L(curr_player.color + " has completed their placement phase.");
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
                        Util.L("Invalid command.");
                    }
                }
            }
            else
            {
                Util.L(curr_player.color + " has completed their placement phase.");
                curr_player.done_setup = true;
            }
        }

        private static void RegularTurn()
        {
            if (board.PlayerTopPieces(curr_player).Count < Constants.MAX_BOARD_PIECES && curr_player.p_hand.Count > 0)
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
                        Util.L("Invalid command.");
                    }
                }
            }
            else
            {
                // View or select, then back, move, or attack
                while(curr_option != Option.NEXT)
                {
                    PrintBoard();
                    curr_option = SelectPrompt("Select a piece on the board.", new List<Option> { Option.SELECT_BOARD_PIECE });

                    if (curr_option == Option.SELECT_BOARD_PIECE)
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
                        Util.L("Invalid command.");
                    }
                }
            }
        }

        private static CheckStatus CheckCheck()
        {
            check_status = board.GetCheck(curr_player);

            if (gamestate == GameState.SETUP)
            {
                if (check_status == CheckStatus.CHECK || check_status == CheckStatus.CHECKMATE)
                {
                    Util.L("<< " + curr_player.color + " has been placed in CHECK.");
                }

                CheckStatus prev_check_status = board.GetCheck(prev_player);
                if (prev_check_status == CheckStatus.CHECK || prev_check_status == CheckStatus.CHECKMATE)
                {
                    Util.L("<< " + prev_player.color + " has been placed in CHECK.");
                }
            }
            else
            {
                if (turn_count == 1.0)
                {
                    CheckStatus prev_check_status = board.GetCheck(prev_player);
                    if (prev_check_status == CheckStatus.CHECK || prev_check_status == CheckStatus.CHECKMATE)
                    {
                        Util.L("<< " + prev_player.color + " has been placed in " + prev_check_status + ". Since it is " + curr_player.color + "'s turn, " + curr_player.color + " wins the game!");
                        gamestate = GameState.END;
                    }
                }

                if (check_status == CheckStatus.CHECK)
                {
                    Util.L("<< " + curr_player.color + " has been placed in CHECK.");
                }
                else if (check_status == CheckStatus.CHECKMATE)
                {
                    Util.L("<< " + curr_player.color + " has been CHECKMATED! " + prev_player.color + " wins the game!");
                    gamestate = GameState.END;
                }
            }

            if (gamestate == GameState.END)
                return CheckStatus.CHECKMATE;
            else
                return check_status;
        }
        
        private static void SwapTurn()
        {
            sel_piece = null;
            sel_location = null;

            if (gamestate == GameState.SETUP && (turn_count >= 27 || ((prev_player.done_setup || prev_player.passed) && (curr_player.done_setup || curr_player.passed))) )
            {
                EndSetup();
                return;
            }

            prev_player = curr_player == black ? black : white;
            curr_player = curr_player == black ? white : black;
            
            curr_player.passed = false;
            turn_count += 0.5;
        }

        private static void EndSetup()
        {
            Util.L(" Both players have completed their placement phases.");
            Util.L("<< The game will now begin!\n\n");

            gamestate = GameState.TURNS;
            board.SetGameState(gamestate);

            prev_player = black;
            curr_player = white;
            
            turn_count = 1.0;
            curr_option = Option.NULL;
        }

        private static void CheckDropMarshal()
        {
            PrintBoard();
            sel_piece = curr_player.GetHandPiece(PieceType.MARSHAL);
            Util.L("<< The Marshal must be dropped first.");
            
            while(curr_option != Option.NEXT)
            {
                PrintBoardSelection();
                curr_option = SelectPrompt("Enter a location to drop this [" + sel_piece + "].", new List<Option> { Option.BACK, Option.SELECT_DROP });

                if (curr_option == Option.BACK)
                {
                    log_output = !log_output;
                }
                else if (curr_option == Option.SELECT_DROP)
                {
                    board.DropPieceTo(sel_piece, sel_location.rank, sel_location.file);
                    curr_option = Option.NEXT;
                }
                else
                {
                    Util.L("Invalid command.");
                }
            }
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
                    curr_option = Option.NEXT;
                }
                else if (curr_option == Option.SELECT_ATTACK)
                {
                    board.AttackPieceTo(sel_piece, sel_location.rank, sel_location.file);
                    curr_option = Option.NEXT;
                }
            }
        }

        private static Option SelectPrompt(String prompt, List<Option> options)
        {
            Option op = Option.NULL;

            Util.L(prompt);
            Util.P(">> ");
            String choice = Console.ReadLine().Trim().ToLower();

            if (options.Contains(Option.BACK) && (Regex.IsMatch(choice, @"back") || Regex.IsMatch(choice, @"\.")) )
            {
                op = Option.BACK;
            }
            else if (options.Contains(Option.PASS) && Regex.IsMatch(choice, @"pass"))
            {
                op = Option.PASS;
                LogDrop(Option.PASS);
            }
            else if (options.Contains(Option.DONE) && Regex.IsMatch(choice, @"done"))
            {
                op = Option.DONE;
                LogDrop(Option.DONE);
            }
            else if (options.Contains(Option.SELECT_HAND_PIECE) && SelectHandPiece(choice))
            {
                // sel_piece set in method
                op = Option.SELECT_HAND_PIECE;
            }
            else if (options.Contains(Option.SELECT_BOARD_PIECE) && SelectLocation(choice, Option.SELECT_BOARD_PIECE))
            {
                // sel_location set in method
                sel_piece = board.PieceAt(sel_location);
                op = Option.SELECT_BOARD_PIECE;
            }
            else if(options.Contains(Option.SELECT_DROP) && SelectLocation(choice, Option.SELECT_DROP))
            {
                // sel_location set in method
                op = Option.SELECT_DROP;
                LogDrop(Option.SELECT_DROP);
            }
            else if(options.Contains(Option.SELECT_MOVE) && SelectLocation(choice, Option.SELECT_MOVE))
            {
                // sel_location set in method
                op = Option.SELECT_MOVE;
                LogDrop(Option.SELECT_MOVE);
            }
            else if(options.Contains(Option.SELECT_ATTACK) && SelectLocation(choice, Option.SELECT_ATTACK))
            {
                // sel_location set in method
                op = Option.SELECT_ATTACK;
                LogDrop(Option.SELECT_ATTACK);
            }
            else
            {
                op = Option.NULL;
            }

            if (op == Option.SELECT_BOARD_PIECE || op == Option.SELECT_HAND_PIECE)
                board.Select(sel_piece);

            return op;
        }

        private static bool SelectHandPiece(string choice)
        {
            sel_piece = null;

            if (Regex.IsMatch(choice.ToLower(), @"^x"))
            {
                int x = random.Next(0, curr_player.p_hand.Count());
                sel_piece = curr_player.p_hand.ElementAt(x);
            }
            else if (Regex.IsMatch(choice.ToLower(), @"^\w"))
            {
                PieceType type = (PieceType)choice.ToUpper()[0];
                sel_piece = curr_player.GetHandPiece(type);
            }

            return sel_piece != null;
        }

        private static bool SelectLocation(string rft, Option location_type)
        {
            sel_location = null;
            MatchCollection matches = Regex.Matches(rft, @"(\d)+");

            if (Regex.IsMatch(rft.ToLower(), @"^x") && location_type == Option.SELECT_DROP)
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

                if (matches.Count >= 3)
                {
                    int t = Convert.ToInt32(matches[2].Value);
                    if (Location.Valid(r, f, t))
                    {
                        Location loc = new Location(r, f, t);

                        if ( (location_type == Option.SELECT_BOARD_PIECE && board.PieceAt(loc) != null)
                            || (location_type == Option.SELECT_DROP && sel_piece.CanDropTo(loc))
                            || (location_type == Option.SELECT_MOVE && sel_piece.CanMoveTo(loc))
                            || (location_type == Option.SELECT_ATTACK && sel_piece.CanAttackTo(loc)) )
                        {
                            sel_location = loc;
                        }
                    }
                }
                else if (Location.Valid(r, f))
                {
                    if (location_type == Option.SELECT_BOARD_PIECE)
                    {
                        Piece top = board.TopPieceAt(r, f);
                        if (top != null)
                            sel_location = top.location;
                    }
                    else if (location_type == Option.SELECT_DROP && sel_piece.CanDropTo(r, f))
                    {
                        sel_location = sel_piece.GetDropAt(r, f);
                    }
                    else if (location_type == Option.SELECT_MOVE && sel_piece.CanMoveTo(r, f) && (!sel_piece.CanAttackTo(r, f) || Regex.IsMatch(rft, @"m")) )
                    {
                        sel_location = sel_piece.GetMoveAt(r, f);
                    }
                    else if (location_type == Option.SELECT_ATTACK && sel_piece.CanAttackTo(r, f) && (!sel_piece.CanMoveTo(r, f) || Regex.IsMatch(rft, @"a")) )
                    {
                        sel_location = sel_piece.GetAttackAt(r, f);
                    }
                }
            }

            if (sel_location != null && gamestate != GameState.SETUP)
            {
                switch (location_type)
                {
                    case Option.SELECT_DROP:
                        if (board.CheckStatusAfterCloneDropTo(curr_player.color, sel_piece.type, sel_location.rank, sel_location.file) != CheckStatus.SAFE)
                        {
                            sel_location = null;
                            Util.L("Invalid drop: leaves " + curr_player + " in check.\n");
                        }
                        break;
                    case Option.SELECT_MOVE:
                        if (board.CheckStatusAfterCloneMoveTo(curr_player.color, sel_piece.location.rank, sel_piece.location.file, sel_location.rank, sel_location.file) != CheckStatus.SAFE)
                        {
                            sel_location = null;
                            Util.L("Invalid move: leaves " + curr_player + " in check.\n");
                        }
                        break;
                    case Option.SELECT_ATTACK:
                        if (board.CheckStatusAfterCloneAttackTo(curr_player.color, sel_piece.location.rank, sel_piece.location.file, sel_location.rank, sel_location.file) != CheckStatus.SAFE)
                        {
                            sel_location = null;
                            Util.L("Invalid capture: leaves " + curr_player + " in check.\n");
                        }
                        break;
                }
            }

            return sel_location != null;
        }

        private static void LogDrop(Option op)
        {
            if (log_output)
            {
                string turn = "";
                if (turn_count == 1 || turn_count%5 == 0)
                    turn = " #" + (int)turn_count;

                switch (op)
                {
                    case Option.PASS:
                        Util.Log("pass" + turn);
                        break;
                    case Option.DONE:
                        Util.Log("done" + turn);
                        break;
                    case Option.SELECT_DROP:
                        Util.Log(sel_piece + turn);
                        Util.Log(sel_location);
                        break;
                    case Option.SELECT_MOVE:
                    case Option.SELECT_ATTACK:
                        Util.Log(sel_piece.location + turn);
                        Util.Log(sel_location);
                        break;
                }
            }
        }

        private static void Wait()
        {
            Util.L("Press a key to continue.");
            Console.ReadKey(true);
            Util.L("");
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
