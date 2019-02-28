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
        private Player[] players;
        private List<Piece>[,] pieces;

        public List<Piece>[] p_top, p_elevators;
        public Piece[] p_marshals;

        public Piece p_selected;


        public Board(Player[] pl_array)
        {
            players = pl_array;
            pieces = new List<Piece>[Constants.MAX_RANKS, Constants.MAX_FILES];

            p_top = new List<Piece>[Constants.NUM_PLAYERS];
            p_elevators = new List<Piece>[Constants.NUM_PLAYERS];
            p_marshals = new Piece[Constants.NUM_PLAYERS];

            p_selected = null;
        }

        public void Update()
        {
            // For each top piece:
                // ResetProperties
                    // Reset p.lt_sight and p.acting_tier
                // Add to p_top
                // If marshal: add to p_marshals
                // If elevator: CalcElevators
                    // For all elevators: for all 8 directions: CalcSightProcedure
                        // Set p.lt_sight and p.acting_tier
                // Update moves

            for (int r = 1; r <= Constants.MAX_RANKS; r++)
                for (int f = 1; f <= Constants.MAX_FILES; f++)
                {
                    Piece p = StackAt(r, f).Last();
                    if (p != null)
                    {
                        ResetProperties(p);
                        
                        p_top[(int)p.player.color].Add(p);

                        if (p.type == PieceType.MARSHAL)
                            p_marshals[(int)p.player.color] = p;
                        else if (p.type == PieceType.LIEUTENANT || p.type == PieceType.FORTRESS)
                            CalcElevators(p);

                        p.UpdateMoves();
                    }
                }
        }

        public void Select(Piece p)
        {
            p_selected = p;

            if (p_selected != null)
            {
                p_selected.UpdateDrops();
                //p_selected.UpdateMoves();         // Probably unnecessary
            }
        }


        public List<Piece> PlayerTopPieces(Player pl)
        {
            return p_top[(int)pl.color];
        }

        public List<Location> ValidDrops(Piece p)
        {
            List<Location> drops = new List<Location>();

            for (int r = 1; r <= Constants.MAX_RANKS; r++)
                for (int f = 1; f <= Constants.MAX_FILES; f++)
                {
                    bool has_marshal = false;
                    foreach (Piece m in p_marshals)
                        has_marshal |= StackAt(r, f).Contains(m);
                    
                    if (!has_marshal)
                    {
                        int stack_height = StackHeight(r, f);
                        if (stack_height < Constants.MAX_TIERS)
                            drops.Add(new Location(stack_height+1, r, f));
                    }
                }

            // Deal with pawn check drops

            return drops;
        }


        public void DropPiece(Piece p, Location l)
        {
            p.player.DropPiece(p);
            p.DropTo(l);
            StackAt(l).Add(p);
        }

        public void MovePiece(Location from, Location to)
        {
            Piece p = StackAt(from).Last();

            StackAt(from).Remove(p);
            p.MoveTo(to);
            StackAt(to).Add(p);
        }

        public void KillRemovePiece(Location l)
        {
            Piece p = StackAt(l).Last();

            StackAt(l).Remove(p);
            p.Kill();
        }


        private void ResetProperties(Piece p)
        {
            p.lt_sight = false;
            p.acting_tier = p.location.tier;
            p.valid_drops = null;
        }
        
        private void CalcElevators(Piece e)
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
                    Piece p = StackAt(r, f).Last();
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
            return StackAt(l).Last();
        }
        public Piece TopPieceAt(int r, int f)
        {
            return StackAt(r, f).Last();
        }

        private List<Piece> StackAt(Location l)
        {
            return pieces[l.rank, l.file];
        }
        private List<Piece> StackAt(int r, int f)
        {
            return pieces[r, f];
        }

        public int StackHeight(Location l)
        {
            return StackAt(l).Count;
        }
        public int StackHeight(int r, int f)
        {
            return pieces[r, f].Count;
        }


        override
        public string ToString()
        {
            return "Board.ToString() is not yet implemented.";
        }
    }
}
