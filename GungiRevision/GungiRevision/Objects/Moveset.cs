using GungiRevision.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GungiRevision.Objects
{
    class Moveset
    {
        public Piece piece;
        public Board board;
        public List<Location> moves;
        public List<Location> attacks;

        public Moveset(Piece p)
        {
            piece = p;
            board = p.board;
            moves = new List<Location>();
        }

        // Must be updated after elevators
        public void AddStraightMove(int t,  int up, int updiag, int side, int downdiag, int down, MoveType mt)
        {
            if (t == piece.acting_tier || t == 0 || piece.lt_sight)
                MakeStraightMoves(up, updiag, side, downdiag, down, mt);
        }
        public void AddBentMove(int t,  int up_updiag, int side_updiag, int side_downdiag, int down_downdiag, MoveType mt)
        {
            if (t == piece.acting_tier || t == 0 || piece.lt_sight)
                MakeBentMoves(up_updiag, side_updiag, side_downdiag, down_downdiag, mt);
        }

        private void MakeStraightMoves(int up, int updiag, int side, int downdiag, int down, MoveType mt)
        {
            Location l = piece.location;

            if (up > 0)
                StraightSightProcedure(up, l, 1, 0, mt);
            if (updiag > 0)
            {
                StraightSightProcedure(updiag, l, 1, 1, mt);
                StraightSightProcedure(updiag, l, 1, -1, mt);
            }
            if (side > 0)
            {
                StraightSightProcedure(side, l, 0, 1, mt);
                StraightSightProcedure(side, l, 0, -1, mt);
            }
            if (downdiag > 0)
            {
                StraightSightProcedure(downdiag, l, -1, 1, mt);
                StraightSightProcedure(downdiag, l, -1, -1, mt);
            }
            if (down > 0)
                StraightSightProcedure(down, l, -1, 0, mt);
        }
        private void StraightSightProcedure(int s, Location l, int rs, int fs, MoveType mt)
        {
            int r = l.rank, f = l.tier;
            if (mt == MoveType.TELEPORTABLE)
            {
                r += rs * s;
                f += fs * s;
            }
            else
            {
                r += rs;
                f += fs;
            }
            bool jumped = false;

            while (Util.ValidLocation(r, f) && s > 0)
            {
                int stack_height = board.StackHeight(r, f);

                if (stack_height < Constants.MAX_TIERS && (mt != MoveType.JUMP_ATTACK || jumped) )
                {
                    Location new_l = new Location(r, f, stack_height+1);
                    if (!moves.Contains(new_l) && mt != MoveType.JUMP_ATTACK)        // Might not think it contains it if it is a new instance. Might have to check equality.
                        moves.Add(new_l);

                    if (!piece.IsFriendlyWith(board.TopPieceAt(r, f)) && mt != MoveType.PACIFIST_BLOCKABLE)
                    {
                        new_l = new Location(r, f, stack_height);
                        if (!attacks.Contains(new_l))
                            attacks.Add(new_l);
                    }
                }

                if (mt == MoveType.TELEPORTABLE)
                    break;

                if (stack_height > 0)
                {
                    if (mt == MoveType.JUMP_ATTACK && !jumped)
                        jumped = true;
                    else
                        break;
                }

                r += rs;
                f += fs;
                s--;
            }
        }

        private void MakeBentMoves(int up_updiag, int side_updiag, int side_downdiag, int down_downdiag, MoveType mt)
        {
            if (mt == MoveType.JUMP_ATTACK || mt == MoveType.PACIFIST_BLOCKABLE)
                return;
            
            Location l = piece.location;

            if (up_updiag > 0)
            {
                BentSightProcedure(l, 2, true, 1, mt);
                BentSightProcedure(l, 2, true, -1, mt);
            }
            if (side_updiag > 0)
            {
                BentSightProcedure(l, 2, false, 1, mt);
                BentSightProcedure(l, 2, false, -1, mt);
            }
            if (side_downdiag > 0)
            {
                BentSightProcedure(l, -2, false, 1, mt);
                BentSightProcedure(l, -2, false, -1, mt);
            }
            if (down_downdiag > 0)
            {
                BentSightProcedure(l, -2,  true, 1, mt);
                BentSightProcedure(l, -2, true, -1, mt);
            }
        }
        private void BentSightProcedure(Location l, int s, bool v_first, int h_flip, MoveType mt)
        {
            int r = l.rank, f = l.file, c = Math.Sign(s);
            if (mt == MoveType.TELEPORTABLE)
            {
                r += s - (v_first ? 0 : c);
                f = (l.file + s - (v_first ? c : 0)) * h_flip;
            }
            else
            {
                if (v_first)
                    r += c;
                else
                    f += c * h_flip;
            }
            s = Math.Abs(s);
            
            while (Util.ValidLocation(r, f) && s > 0)
            {
                int stack_height = board.StackHeight(r, f);

                if (stack_height < Constants.MAX_TIERS)
                {
                    Location new_l = new Location(r, f, stack_height+1);
                    if (!moves.Contains(new_l))                 // Might not think it contains it if it is a new instance. Might have to check equality.
                        moves.Add(new_l);

                    if (!piece.IsFriendlyWith(board.TopPieceAt(r, f)))
                    {
                        new_l = new Location(r, f, stack_height);
                        if (!attacks.Contains(new_l))
                            attacks.Add(new_l);
                    }
                }

                if (stack_height > 0 || mt == MoveType.TELEPORTABLE)
                    break;

                s--;
                if (v_first)
                {
                    r += c;
                    if (s == 1)
                        f += c * h_flip;
                }
                else
                {
                    f += c * h_flip;
                    if (s == 1)
                        r += c;
                }
            }
        }
    }
}
