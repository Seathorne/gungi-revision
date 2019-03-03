using GungiRevision.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GungiRevision.Objects
{
    class Board
    {
        private readonly Player[] players;
        private readonly List<Piece>[,] board;

        private List<Piece>[] p_all, p_top, p_checked_by;
        private List<Piece> p_elevators;
        private Piece[] p_marshals;
        private Piece p_selected;
        private CheckStatus[] check_status;
        private GameState gamestate;

        private Location[][,] valid_drops, valid_pawn_drops;
        public bool IS_CLONE;


        public Board()
        {
            IS_CLONE = false;
            p_selected = null;
            gamestate = GameState.SETUP;
            
            players = new Player[]
            {
                new Player(this, PlayerColor.BLACK),
                new Player(this, PlayerColor.WHITE)
            };

            board = new List<Piece>[Constants.MAX_RANKS, Constants.MAX_FILES];
            for (int r = 1; r <= Constants.MAX_RANKS; r++)
                for (int f = 1; f <= Constants.MAX_FILES; f++)
                    board[r-1, f-1] = new List<Piece>();
            
            Update();
        }

        public Board Clone()
        {
            Board clone_b = new Board();
            clone_b.IS_CLONE = true;

            foreach (Player pl in players)
                clone_b.players[(int)pl.color] = pl.Clone(clone_b);

            for (int r = 1; r <= Constants.MAX_RANKS; r++)
                for (int f = 1; f <= Constants.MAX_FILES; f++)
                {
                    List<Piece> stack = this.StackAt(r, f);
                    for (int t = 1; t <= stack.Count; t++)
                    {
                        Piece p = stack.ElementAt(t-1);
                        Player clone_pl = clone_b.players[(int)p.player.color];
                        Piece clone_p = p.Clone(clone_pl);

                        clone_b.board[r-1, f-1].Add(clone_p);
                    }
                }
      
            clone_b.Update();

            return clone_b;
        }

        private void Clear()
        {
            p_all = new List<Piece>[] {new List<Piece>(), new List<Piece>()};
            p_top = new List<Piece>[] {new List<Piece>(), new List<Piece>()};
            p_checked_by = new List<Piece>[] {new List<Piece>(), new List<Piece>()};
            p_elevators = new List<Piece>();
            p_marshals = new Piece[] {null, null};
            check_status = new CheckStatus[] { CheckStatus.SAFE, CheckStatus.SAFE };

            valid_drops = new Location[][,] { new Location[Constants.MAX_RANKS, Constants.MAX_FILES], new Location[Constants.MAX_RANKS, Constants.MAX_FILES] };
            valid_pawn_drops = new Location[][,] { new Location[Constants.MAX_RANKS, Constants.MAX_FILES], new Location[Constants.MAX_RANKS, Constants.MAX_FILES] };
        }

        private void Update()
        {
            // For each top piece:
                // Add to p_top
                // Clear
                    // Reset p.lt_sight and p.acting_tier
                // If marshal: add to p_marshals
                // If elevator: add  to p_elevators
            // For all elevators: for all 8 directions: CalcSightProcedure
                // Set p.lt_sight and p.acting_tier
            //For all top pieces
                // Update moves
            // For all hand pieces
                // Update drops

            Clear();

            for (int r = 1; r <= Constants.MAX_RANKS; r++)
                for (int f = 1; f <= Constants.MAX_FILES; f++)
                {
                    List<Piece> stack = StackAt(r, f);
                    foreach (Piece all_piece in stack)
                    {
                        all_piece.Clear();
                        p_all[(int)all_piece.player.color].Add(all_piece);
                    }
                    
                    Piece top = TopPieceAt(r, f);
                    if (top != null)
                    {
                        p_top[(int)top.player.color].Add(top);

                        if (top.type == PieceType.MARSHAL)
                            p_marshals[(int)top.player.color] = top;
                        else if (top.type == PieceType.LIEUTENANT || top.type == PieceType.FORTRESS)
                            p_elevators.Add(top);
                    }
                }

            foreach (Piece elev in p_elevators)
                ApplyElevators(elev);
            
            UpdateOverallDrops();

            foreach (Player pl in players)
            {
                foreach (Piece p in p_top[(int)pl.color])
                    Moveset.UpdateMovesAndAttacksFor(p);

                foreach (Piece h in pl.p_hand)
                    UpdateDropsFor(h);
            }

            if (gamestate != GameState.SETUP)
                foreach (Piece m in p_marshals)
                    UpdateInCheck(m);
        }


        public void Select(Piece p)
        {
            p_selected = p;
        }

        public void Deselect()
        {
            p_selected = null;
        }

        public void SetGameState(GameState gs)
        {
            gamestate = gs;
            Update();
        }


        public Player Player(PlayerColor c)
        {
            return players[(int)c];
        }

        public List<Piece> PlayerAllPieces(Player pl)
        {
            return p_all[(int)pl.color];
        }
        
        public List<Piece> PlayerTopPieces(Player pl)
        {
            return p_top[(int)pl.color];
        }


        private void UpdateOverallDrops()
        {
            for (int r = 1; r <= Constants.MAX_RANKS; r++)
                for (int f = 1; f <= Constants.MAX_FILES; f++)
                {
                    if (!ContainsMarshal(r, f))
                    {
                        int stack_height = StackHeight(r, f);
                        if (stack_height < Constants.MAX_TIERS)
                        {
                            if (gamestate == GameState.SETUP)
                            {
                                if (r <= Constants.NUM_SETUP_RANKS)
                                    valid_drops[(int)PlayerColor.WHITE][r-1, f-1] = new Location(r, f, stack_height+1);
                                else if (r > Constants.MAX_RANKS - Constants.NUM_SETUP_RANKS)
                                    valid_drops[(int)PlayerColor.BLACK][r-1, f-1] = new Location(r, f, stack_height+1);
                            }
                            else
                            {
                                foreach (Player pl in players)
                                {
                                    valid_drops[(int)pl.color][r-1, f-1] = new Location(r, f, stack_height+1);
                                    valid_pawn_drops[(int)pl.color][r-1, f-1] = new Location(r, f, stack_height+1);
                                
                                    if (r >= p_marshals[(int)pl.color].location.rank-1 && r <= p_marshals[(int)pl.color].location.rank+1
                                        && f >= p_marshals[(int)pl.color].location.file-1 && f <= p_marshals[(int)pl.color].location.file + 1)
                                    {
                                        if (CheckedByPawnDrop(pl.color, r, f))
                                        {
                                            valid_pawn_drops[(int)Util.OtherPlayerColor(pl.color)][r-1, f-1] = null;
                                            Util.PRL("null pawn drop at " + r + " " + f);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
        }
        
        public void UpdateDropsFor(Piece p)
        {
            if (gamestate == GameState.SETUP)
                p.SetValidDrops(valid_drops[(int)p.player.color]);
            else if (p.type == PieceType.PAWN)
                p.SetValidDrops(valid_pawn_drops[(int)p.player.color]);
            else
                p.SetValidDrops(valid_drops[(int)p.player.color]);
        }

        public bool ContainsMarshal(int r, int f)
        {
            foreach (Piece m in p_marshals)
                if(StackAt(r, f).Contains(m))
                    return true;
            return false;
        }


        public bool DropPieceTo(Piece p, int r, int f)
        {
            if (!p.CanDropTo(r, f))
                return false;
            
            p.player.DropPiece(p);

            StackAt(r, f).Add(p);
            p.DropTo(new Location(r, f, StackHeight(r, f)));

            Update();
            return true;
        }

        public bool MovePieceTo(Piece p, int r, int f)
        {
            if (!p.CanMoveTo(r, f))
                return false;
            
            StackAt(p.location.rank, p.location.file).Remove(p);
            
            StackAt(r, f).Add(p);
            p.MoveTo(new Location(r, f, StackHeight(r, f)));

            Update();
            return true;
        }

        public bool AttackPieceTo(Piece p, int r, int f)
        {
            if (!p.CanAttackTo(r, f))
                return false;
            
            Piece e = StackAt(r, f).Last();
            StackAt(r, f).Remove(e);
            e.Kill();

            StackAt(p.location.rank, p.location.file).Remove(p);

            StackAt(r, f).Add(p);
            p.MoveTo(new Location(r, f, StackHeight(r, f)));

            Update();
            return true;
        }

        
        private void ApplyElevators(Piece e)
        {
            int s = (e.type == PieceType.LIEUTENANT) ? Constants.MAX_MOVES : 1;
            for (int r = -1; r <= 1; r++)
                for (int f = -1; f <= 1; f++)
                    if (r != 0 || f != 0)
                        CalcSightProcedure(e, s, r, f);
        }
        private void CalcSightProcedure(Piece e, int s, int rs, int fs)
        {       // !(rs == 0 && fs == 0)
            int r = e.location.rank + rs,
                f = e.location.file + fs;

            while (Util.ValidLocation(r, f) && s > 0)
            {
                if (StackHeight(r, f) > 0)
                {
                    Piece p = TopPieceAt(r, f);
                    if (e.IsFriendlyWith(p))
                    {
                        if (e.type == PieceType.LIEUTENANT)
                            p.lt_sight = true;
                        else
                            p.acting_tier = Math.Min(Constants.MAX_TIERS, Math.Max(p.location.tier, e.location.tier+1));
                    }
                    return;
                }

                r += rs;
                f += fs;
                s--;
            }
        }


        public Piece PieceAt(Location l)
        {
            if (StackHeight(l) >= l.tier)
                return StackAt(l)[l.tier-1];
            else
                return null;
        }
        public Piece TopPieceAt(int r, int f)
        {
            if (StackHeight(r, f) > 0)
                return StackAt(r, f).Last();
            else
                return null;
        }

        private List<Piece> StackAt(Location l)
        {
            return board[l.rank-1, l.file-1];
        }
        private List<Piece> StackAt(int r, int f)
        {
            return board[r-1, f-1];
        }

        public int StackHeight(Location l)
        {
            return StackAt(l).Count;
        }
        public int StackHeight(int r, int f)
        {
            return board[r-1, f-1].Count;
        }


        private void UpdateInCheck(Piece m)
        {
            if (m == null)
                return;
            
            int player_color = (int)m.player.color;
            int enemy_color = (int)Util.OtherPlayerColor(m.player.color);
            CheckStatus current = CheckStatus.SAFE;
            
            foreach (Piece p in p_top[enemy_color])
                if (p.CanAttackTo(m.location.rank, m.location.file))
                    p_checked_by[player_color].Add(p);

            if (p_checked_by[player_color].Count > 0)
                current = CheckStatus.CHECKMATE;
            

            if (!IS_CLONE && current == CheckStatus.CHECKMATE)
            {
                Piece drop_piece = m.player.GetNonPawnHandPiece();
                if (drop_piece == null)
                    drop_piece = m.player.GetHandPiece(PieceType.PAWN);
                if (drop_piece != null)
                    for (int r = 1; r <= Constants.MAX_RANKS; r++)
                        for (int f = 1; f <= Constants.MAX_FILES; f++)
                            if (drop_piece.CanDropTo(r, f))
                                if (CheckStatusAfterCloneDropTo(m.player.color, drop_piece.type, r, f) == CheckStatus.SAFE)
                                {
                                    Util.PRL(m.player + " can escape check by dropping a piece to " + r + "-" + f + ".");
                                    current = CheckStatus.CHECK;
                                }

                foreach (Piece p in p_top[player_color])
                {
                    foreach (Location l in p.valid_moves_list)
                        if (CheckStatusAfterCloneMoveTo(m.player.color, p.location.rank, p.location.file, l.rank, l.file) == CheckStatus.SAFE)
                        {
                            Util.PRL(m.player + " can escape check by moving [" + p + "] from " + p.location + " to " + l + ".");
                            current = CheckStatus.CHECK;
                        }
                    foreach (Location l in p.valid_attacks_list)
                        if (CheckStatusAfterCloneMoveTo(m.player.color, p.location.rank, p.location.file, l.rank, l.file) == CheckStatus.SAFE)
                        {
                            Util.PRL(m.player + " can escape check by attacking [" + p + "] from " + p.location + " to " + l + ".");
                            current = CheckStatus.CHECK;
                        }
                }
            }

            check_status[player_color] = current;
        }

        public CheckStatus CheckCheck(Player pl)
        {
            return check_status[(int)pl.color];
        }

        public CheckStatus CheckStatusAfterCloneDropTo(PlayerColor c, PieceType pt, int r, int f)
        {
            Board clone_board = this.Clone();
            Player clone_player = clone_board.players[(int)c];
            Piece clone_piece = clone_player.GetHandPiece(pt);

            clone_board.DropPieceTo(clone_piece, r, f);
            CheckStatus status = clone_board.CheckCheck(clone_player);

            clone_board = null;
            return status;
        }

        public CheckStatus CheckStatusAfterCloneMoveTo(PlayerColor c, int from_r, int from_f, int to_r, int to_f)
        {
            Board clone_board = this.Clone();
            Player clone_player = clone_board.players[(int)c];
            Piece clone_piece = clone_board.TopPieceAt(from_r, from_f);

            clone_board.MovePieceTo(clone_piece, to_r, to_f);
            CheckStatus status = clone_board.CheckCheck(clone_player);

            clone_board = null;
            return status;
        }

        public CheckStatus CheckStatusAfterCloneAttackTo(PlayerColor c, int from_r, int from_f, int to_r, int to_f)
        {
            Board clone_board = this.Clone();
            Player clone_player = clone_board.players[(int)c];
            Piece clone_piece = clone_board.TopPieceAt(from_r, from_f);

            clone_board.AttackPieceTo(clone_piece, to_r, to_f);
            CheckStatus status = clone_board.CheckCheck(clone_player);

            clone_board = null;
            return status;
        }

        private bool CheckedByPawnDrop(PlayerColor c, int r, int f)
        {
            Board clone_board = this.Clone();
            Player clone_player = clone_board.players[(int)c];
            Piece clone_piece = clone_player.GetHandPiece(PieceType.PAWN);

            bool checked_by_pawn_drop = false;
            Piece clone_m = clone_board.p_marshals[(int)c];
            
            int player_color = (int)clone_m.player.color;
            int enemy_color = (int)Util.OtherPlayerColor(clone_m.player.color);

            clone_board.DropPieceTo(clone_piece, r, f);

            if (clone_piece.CanAttackTo(clone_m.location.rank, clone_m.location.file))
                checked_by_pawn_drop = true;

            clone_board = null;
            return checked_by_pawn_drop;
        }


        public void PrintBoard()
        {
            PrintBoard(0, Constants.LEGEND, false, false, false);
        }
        public void PrintBoardAndHand(Player pl)
        {
            Util.PRL("");
            PrintBoard(0, pl.HandToLegend(), false, false, false);
        }
        public void PrintBoardSelection(Player pl)
        {
            if (p_selected.status == Status.HAND)
            {
                Util.PRL("\nValid drop locations for [" + p_selected + "]:");
                PrintBoard(0, Constants.LEGEND, false, false, true);
            }
            else if (p_selected.status == Status.BOARD)
            {
                Util.PRL("\nValid moves and attacks for [" + p_selected + "]:");
                PrintBoard(0, Constants.LEGEND, true, true, false);
            }
        }

        public void PrintBoard(PlayerColor c, string[] legend, bool moves, bool attacks, bool drops)
        {
            int legend_i = 0;
            string str = "";

            for (int f = 1; f <= (Constants.MAX_TIERS/2)+3; f++)
                str += " ";
            for (int f = 1; f <= Constants.MAX_FILES; f++)
            {
                str += f%10;
                for (int t = 1; t <= Constants.MAX_TIERS; t++)
                    str += " ";
            }
            str += "\n";

            for (int r = Constants.MAX_RANKS; r >= 1; r--)
            {
                str += "  " + Constants.CHAR_I_SEPARATOR;
                for (int f = 1; f <= Constants.MAX_FILES; f++)
                {
                    string str_stack = "";

                    if (drops && p_selected.CanDropTo(r, f))
                    {
                        for (int t = 1; t <= StackHeight(r, f); t++)
                            str_stack += Constants.CHAR_H_SEPARATOR;

                        str_stack += Constants.CHAR_DROP;
                    } 
                    else if (moves || attacks)
                    {
                        if (p_selected != null && r == p_selected.location.rank && f == p_selected.location.file)
                        {
                            for (int t = 1; t < p_selected.location.tier; t++)
                                str_stack += Constants.CHAR_H_SEPARATOR;
                            str_stack += Constants.CHAR_SELECTED;
                        }
                        else
                        {
                            for (int t = 1; t < StackHeight(r, f); t++)
                                str_stack += Constants.CHAR_H_SEPARATOR;

                            if (attacks && p_selected.CanAttackTo(r, f))
                                str_stack += Constants.CHAR_ATTACK;
                            
                            for (int t = str_stack.Length; t < StackHeight(r, f); t++)
                                str_stack += Constants.CHAR_H_SEPARATOR;

                            if (moves && p_selected.CanMoveTo(r, f))
                                str_stack += Constants.CHAR_MOVE;
                        }
                    }

                    for (int t = str_stack.Length+1; t <= Constants.MAX_TIERS; t++)
                        str_stack += Constants.CHAR_H_SEPARATOR;

                    str += str_stack + Constants.CHAR_I_SEPARATOR;
                }
                
                if (r < Constants.MAX_RANKS && legend_i < legend.Length)
                    str += "  " + legend[legend_i++];

                str += "\n";

                str += r%10 + " " + Constants.CHAR_V_SEPARATOR;
                for (int f = 1; f <= Constants.MAX_FILES; f++)
                {
                    string str_stack = "";

                    if (StackHeight(r, f) > 0)
                        foreach (Piece p in StackAt(r, f))
                            str_stack += p.ToString();

                    for (int t = str_stack.Length+1; t <= Constants.MAX_TIERS; t++)
                        str_stack += Constants.CHAR_EMPTY;

                    str += str_stack + Constants.CHAR_V_SEPARATOR;
                }

                if (r < Constants.MAX_RANKS && legend_i < legend.Length)
                    str += "  " + legend[legend_i++];

                str += "\n";
            }

            str += "  " + Constants.CHAR_I_SEPARATOR;
            for (int f = 1; f <= Constants.MAX_FILES; f++)
            {
                for (int t = 1; t <= Constants.MAX_TIERS; t++)
                    str += Constants.CHAR_H_SEPARATOR;
                str += Constants.CHAR_I_SEPARATOR;
            }

            str += "\n";
            Util.PRL(str);
        }

    }
}
