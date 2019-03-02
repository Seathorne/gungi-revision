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
        static Piece piece;
        
        public static void UpdateMovesAndAttacksFor(Piece p)
        {
            piece = p;
            
            switch (piece.type)
            {
                case PieceType.MARSHAL:
                    AddStraightMove(0,  1,1,1,1,1, MoveType.BLOCKABLE);
                    break;
                case PieceType.SPY:
                    AddStraightMove(1,  1,0,0,1,1, MoveType.BLOCKABLE);
                    AddStraightMove(2,  2,2,2,0,1, MoveType.BLOCKABLE);
                    AddStraightMove(3,  Constants.MAX_MOVES,Constants.MAX_MOVES,Constants.MAX_MOVES,0,Constants.MAX_MOVES, MoveType.BLOCKABLE);
                    break;
                case PieceType.LIEUTENANT:
                    AddStraightMove(1,  1,1,1,0,1, MoveType.BLOCKABLE);
                    AddStraightMove(2,  1,1,1,1,1, MoveType.BLOCKABLE);
                    AddStraightMove(3,  2,1,1,1,1, MoveType.BLOCKABLE);
                    AddBentMove(3,  2,0,0,0, MoveType.BLOCKABLE);
                    break;
                case PieceType.MAJOR:
                    AddStraightMove(1,  1,1,0,1,0, MoveType.BLOCKABLE);
                    AddStraightMove(2,  1,1,1,0,1, MoveType.BLOCKABLE);
                    AddStraightMove(3,  1,1,1,1,1, MoveType.BLOCKABLE);
                    break;
                case PieceType.GENERAL:
                    AddStraightMove(1,  0,1,0,0,0, MoveType.BLOCKABLE);
                    AddStraightMove(2,  1,1,0,1,0, MoveType.BLOCKABLE);
                    AddStraightMove(3,  1,1,1,0,1, MoveType.BLOCKABLE);
                    break;
                case PieceType.ARCHER:
                    AddStraightMove(1,  1,1,1,1,1, MoveType.BLOCKABLE);
                    AddStraightMove(2,  2,2,2,2,2, MoveType.TELEPORTABLE);
                    AddStraightMove(3,  3,3,3,3,3, MoveType.TELEPORTABLE);
                    break;
                case PieceType.KNIGHT:
                    AddStraightMove(1,  1,0,1,0,0, MoveType.BLOCKABLE);
                    AddStraightMove(2,  0,1,0,1,0, MoveType.BLOCKABLE);
                    AddBentMove(1,  2,0,0,0, MoveType.TELEPORTABLE);
                    AddBentMove(2,  2,2,0,0, MoveType.TELEPORTABLE);
                    AddBentMove(3,  2,2,2,2, MoveType.TELEPORTABLE);
                    break;
                case PieceType.SAMURAI:
                    AddStraightMove(1,  0,1,0,1,0, MoveType.TELEPORTABLE);
                    AddStraightMove(2,  0,2,0,2,0, MoveType.TELEPORTABLE);
                    AddStraightMove(3,  0,Constants.MAX_MOVES,0,Constants.MAX_MOVES,0, MoveType.BLOCKABLE);
                    break;
                case PieceType.CANNON:
                    AddStraightMove(1,  1,0,1,0,1, MoveType.BLOCKABLE_PACIFIST);
                    AddStraightMove(2,  2,0,2,0,2, MoveType.BLOCKABLE_PACIFIST);
                    AddStraightMove(3,  Constants.MAX_MOVES,0,Constants.MAX_MOVES,0,Constants.MAX_MOVES, MoveType.BLOCKABLE_PACIFIST);
                    AddStraightMove(0,  Constants.MAX_MOVES,0,Constants.MAX_MOVES,0,Constants.MAX_MOVES, MoveType.JUMP_ATTACK);
                    break;
                case PieceType.COUNSEL:
                    AddStraightMove(0,  0,3,3,2,0, MoveType.BLOCKABLE);
                    break;
                case PieceType.FORTRESS:
                    AddStraightMove(0,  1,1,1,1,1, MoveType.BLOCKABLE);
                    break;
                case PieceType.MUSKETEER:
                    AddStraightMove(1,  0,1,0,0,1, MoveType.BLOCKABLE);
                    AddStraightMove(2,  3,0,0,0,0, MoveType.BLOCKABLE);
                    AddStraightMove(3,  Constants.MAX_MOVES,0,0,0,0, MoveType.BLOCKABLE);
                    break;
                case PieceType.PAWN:
                    AddStraightMove(1,  1,0,0,0,0, MoveType.BLOCKABLE);
                    AddStraightMove(2,  1,1,0,0,0, MoveType.BLOCKABLE);
                    AddStraightMove(3,  1,1,1,0,1, MoveType.BLOCKABLE);
                    break;
            }
        }

        // Must be updated after elevators
        private static void AddStraightMove(int t,  int up, int updiag, int side, int downdiag, int down, MoveType mt)
        {
            if (piece.player.color == PlayerColor.BLACK)
            {
                int temp;
                temp = up; up = down; down = temp;
                temp = updiag; updiag = downdiag; downdiag = temp;
            }

            if ((t == piece.acting_tier) || (t == 0) || piece.lt_sight)
                MakeStraightMoves(up, updiag, side, downdiag, down, mt);
        }

        private static void AddBentMove(int t,  int up_updiag, int side_updiag, int side_downdiag, int down_downdiag, MoveType mt)
        {
            if (piece.player.color == PlayerColor.BLACK)
            {
                int temp;
                temp = up_updiag; up_updiag = down_downdiag; down_downdiag = temp;
                temp = side_updiag; side_updiag = side_downdiag; side_downdiag = temp;
            }
            
            if (t == piece.acting_tier || t == 0 || piece.lt_sight)
                MakeBentMoves(up_updiag, side_updiag, side_downdiag, down_downdiag, mt);
        }

        private static void MakeStraightMoves(int up, int updiag, int side, int downdiag, int down, MoveType mt)
        {
            Location location = piece.location;

            if (up > 0)
                SightProcedure(up, 0, location, 1, 0, mt);
            if (updiag > 0)
            {
                SightProcedure(updiag, updiag, location, 1, 1, mt);
                SightProcedure(updiag, updiag, location, 1, -1, mt);
            }
            if (side > 0)
            {
                SightProcedure(0, side, location, 0, 1, mt);
                SightProcedure(0, side, location, 0, -1, mt);
            }
            if (downdiag > 0)
            {
                SightProcedure(downdiag, downdiag, location, -1, 1, mt);
                SightProcedure(downdiag, downdiag, location, -1, -1, mt);
            }
            if (down > 0)
                SightProcedure(down, 0, location, -1, 0, mt);
        }

        private static void MakeBentMoves(int up_updiag, int side_updiag, int side_downdiag, int down_downdiag, MoveType mt)
        {
            Location location = piece.location;

            if (up_updiag > 1)
            {
                SightProcedure(up_updiag, up_updiag-1, location, 1, 1, mt);
                SightProcedure(up_updiag, up_updiag-1, location, 1, -1, mt);
            }
            if (side_updiag > 1)
            {
                SightProcedure(side_updiag-1, side_updiag, location, 1, 1, mt);
                SightProcedure(side_updiag-1, side_updiag, location, 1, -1, mt);
            }
            if (side_downdiag > 1)
            {
                SightProcedure(side_downdiag-1, side_downdiag, location, -1, 1, mt);
                SightProcedure(side_downdiag-1, side_downdiag, location, -1, -1, mt);
            }
            if (down_downdiag > 1)
            {
                SightProcedure(down_downdiag, down_downdiag-1, location, -1, 1, mt);
                SightProcedure(down_downdiag, down_downdiag-1, location, -1, -1, mt);
            }
        }

        private static void SightProcedure(int steps_r, int steps_f, Location l, int rs, int fs, MoveType mt)
        {
            int r = l.rank, f = l.file, steps;
            if (mt == MoveType.TELEPORTABLE)
            {
                steps = 1;
                r += steps_r * rs;
                f += steps_f * fs;
            }
            else
            {
                steps = Math.Max(steps_r, steps_f);
                r += rs;
                f += fs;
            }
            bool jumped = false;

            while (Util.ValidLocation(r, f) && steps > 0)
            {
                int stack_height = piece.board.StackHeight(r, f);

                if (stack_height < Constants.MAX_TIERS && (mt != MoveType.JUMP_ATTACK || jumped) )
                {
                    Location l_move = new Location(r, f, stack_height+1);
                    if (mt != MoveType.JUMP_ATTACK)
                        piece.AddValidMoveAt(l_move);
                }

                if (stack_height > 0)
                {
                    if (!piece.IsFriendlyWith(piece.board.TopPieceAt(r, f)) && mt != MoveType.BLOCKABLE_PACIFIST)
                    {
                        Location l_attack = new Location(r, f, stack_height);
                        piece.AddValidAttackAt(l_attack);
                    }
                    
                    if (mt == MoveType.JUMP_ATTACK && !jumped)
                        jumped = true;
                    else
                        break;
                }

                steps -= 1;
                if (steps == 1)
                {
                    r = l.rank + (steps_r * rs);
                    f = l.file + (steps_f * fs);
                }
                else
                {
                    r += rs;
                    f += fs;
                }
            }
        }

    }
}
