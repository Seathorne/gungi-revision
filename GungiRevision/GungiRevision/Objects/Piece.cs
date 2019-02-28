using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GungiRevision.Objects
{
    class Piece
    {
        public Board board;
        public Player player;

        public PieceType type;
        public Status status;
        public Location location;
        
        public Moveset moveset;
        public List<Location> valid_moves, valid_drops;
        public bool lt_sight;
        public int acting_tier;


        public Piece(Board b, Player p, PieceType pt)
        {
            board = b;
            player = p;

            type = pt;
            status = Status.HAND;
            location = null;

            moveset = new Moveset(this);
            valid_moves = null;
            valid_drops = null;
        }

        public Piece Clone(Board b, Player pl)
        {
            Piece p = new Piece(b, pl, this.type);

            p.location = this.location;
            p.moveset = this.moveset;            // Possibly need to clone this too
            p.valid_moves = this.valid_moves;    // Possibly need to clone this too
            p.valid_drops = this.valid_drops;    // Possibly need to clone this too
            p.lt_sight = this.lt_sight;
            p.acting_tier = this.acting_tier;

            return p;
        }


        public void DropTo(Location l)
        {
            status = Status.BOARD;
            location = l;
        }

        public void MoveTo(Location l)
        {
            location = l;
        }

        public void Kill()
        {
            status = Status.CAPTURED;
            location = null;
        }


        public bool IsFriendlyWith(Piece p)
        {
            return player.Equals(p.player);
        }

        
        public void UpdateMoves()
        {
            switch (type)
            {
                case PieceType.MARSHAL:
                    moveset.AddStraightMove(0,  1,1,1,1,1, MoveType.BLOCKABLE);
                    break;
                case PieceType.SPY:
                    moveset.AddStraightMove(1,  1,0,0,1,1, MoveType.BLOCKABLE);
                    moveset.AddStraightMove(2,  2,2,2,0,1, MoveType.BLOCKABLE);
                    moveset.AddStraightMove(3,  Constants.MAX_MOVES,Constants.MAX_MOVES,Constants.MAX_MOVES,0,Constants.MAX_MOVES, MoveType.BLOCKABLE);
                    break;
                case PieceType.LIEUTENANT:
                    moveset.AddStraightMove(1,  1,1,1,0,1, MoveType.BLOCKABLE);
                    moveset.AddStraightMove(2,  1,1,1,1,1, MoveType.BLOCKABLE);
                    moveset.AddStraightMove(3,  2,1,1,1,1, MoveType.BLOCKABLE);

                    moveset.AddBentMove(3,  1,0,0,0, MoveType.BLOCKABLE);
                    break;
                case PieceType.MAJOR:
                    moveset.AddStraightMove(1,  1,1,0,1,0, MoveType.BLOCKABLE);
                    moveset.AddStraightMove(2,  1,1,1,0,1, MoveType.BLOCKABLE);
                    moveset.AddStraightMove(3,  1,1,1,1,1, MoveType.BLOCKABLE);
                    break;
                case PieceType.GENERAL:
                    moveset.AddStraightMove(1,  0,1,0,0,0, MoveType.BLOCKABLE);
                    moveset.AddStraightMove(2,  1,1,0,1,0, MoveType.BLOCKABLE);
                    moveset.AddStraightMove(3,  1,1,1,0,1, MoveType.BLOCKABLE);
                    break;
                case PieceType.ARCHER:
                    moveset.AddStraightMove(1,  1,1,1,1,1, MoveType.BLOCKABLE);
                    moveset.AddStraightMove(2,  2,2,2,2,2, MoveType.TELEPORTABLE);
                    moveset.AddStraightMove(3,  3,3,3,3,3, MoveType.TELEPORTABLE);
                    break;
                case PieceType.KNIGHT:
                    moveset.AddStraightMove(1,  1,0,1,0,0, MoveType.BLOCKABLE);
                    moveset.AddStraightMove(2,  0,1,0,1,0, MoveType.BLOCKABLE);
                    moveset.AddBentMove(1,  1,0,0,0, MoveType.TELEPORTABLE);
                    moveset.AddBentMove(2,  1,1,0,0, MoveType.TELEPORTABLE);
                    moveset.AddBentMove(3,  1,1,1,1, MoveType.TELEPORTABLE);
                    break;
                case PieceType.SAMURAI:
                    moveset.AddStraightMove(1,  0,1,0,1,0, MoveType.BLOCKABLE);
                    moveset.AddStraightMove(2,  0,2,0,2,0, MoveType.BLOCKABLE);
                    moveset.AddStraightMove(3,  0,Constants.MAX_MOVES,0,Constants.MAX_MOVES,0, MoveType.BLOCKABLE);
                    break;
                case PieceType.CANNON:
                    moveset.AddStraightMove(1,  1,0,1,0,1, MoveType.PACIFIST_BLOCKABLE);
                    moveset.AddStraightMove(2,  2,0,2,0,2, MoveType.PACIFIST_BLOCKABLE);
                    moveset.AddStraightMove(3,  Constants.MAX_MOVES,0,Constants.MAX_MOVES,0,Constants.MAX_MOVES, MoveType.PACIFIST_BLOCKABLE);
                    moveset.AddStraightMove(0,  Constants.MAX_MOVES,0,Constants.MAX_MOVES,0,Constants.MAX_MOVES, MoveType.JUMP_ATTACK);
                    break;
                case PieceType.COUNSEL:
                    moveset.AddStraightMove(0,  0,3,3,2,0, MoveType.BLOCKABLE);
                    break;
                case PieceType.FORTRESS:
                    moveset.AddStraightMove(0,  1,1,1,1,1, MoveType.BLOCKABLE);
                    break;
                case PieceType.MUSKETEER:
                    moveset.AddStraightMove(1,  0,1,0,0,1, MoveType.BLOCKABLE);
                    moveset.AddStraightMove(2,  3,0,0,0,0, MoveType.BLOCKABLE);
                    moveset.AddStraightMove(3,  Constants.MAX_MOVES,0,0,0,0, MoveType.BLOCKABLE);
                    break;
                case PieceType.PAWN:
                    moveset.AddStraightMove(1,  1,0,0,0,0, MoveType.BLOCKABLE);
                    moveset.AddStraightMove(2,  1,1,0,0,0, MoveType.BLOCKABLE);
                    moveset.AddStraightMove(3,  1,1,1,0,1, MoveType.BLOCKABLE);
                    break;
            }
        }

        public void UpdateDrops()
        {
            valid_drops = board.ValidDrops(this);
        }

        
        override
        public bool Equals(Object o)
        {
            return GetHashCode() == o.GetHashCode();
        }

        override
        public int GetHashCode()
        {
            // Unimplemented
            return -1;
        }


        override
        public string ToString()
        {
            String symbol = ( (char)type ).ToString();
            switch (player.color)
            {
                case PlayerColor.BLACK:
                    return symbol.ToLower();
                default:
                    return symbol;
            }
        }
    }
}
