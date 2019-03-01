using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GungiRevision.Objects
{
    class Piece
    {
        public readonly Board board;
        public readonly Player player;

        public readonly PieceType type;
        public Status status;
        public Location location;
        
        private Moveset moveset;
        private Location[,] valid_moves, valid_attacks, valid_drops;
        public bool lt_sight;
        public int acting_tier;


        public Piece(Player pl, PieceType pt)
        {
            player = pl;
            board = player.board;

            type = pt;
            status = Status.HAND;
            location = null;

            moveset = new Moveset(this);
            Clear();
        }

        public Piece Clone(Player pl)
        {
            Piece p = new Piece(pl, this.type);

            p.location = this.location;
            p.moveset = this.moveset;            // Possibly need to clone this too
            p.valid_moves = this.valid_moves;    // Possibly need to clone this too
            p.valid_attacks = this.valid_attacks;    // Possibly need to clone this too
            p.valid_drops = this.valid_drops;    // Possibly need to clone this too
            p.lt_sight = this.lt_sight;
            p.acting_tier = this.acting_tier;

            return p;
        }


        public void Clear()
        {
            lt_sight = false;
            if (location != null)
                acting_tier = location.tier;
            else
                acting_tier = 0;
            
            valid_moves = new Location[Constants.MAX_RANKS, Constants.MAX_FILES];
            valid_attacks = new Location[Constants.MAX_RANKS, Constants.MAX_FILES];
            valid_drops = new Location[Constants.MAX_RANKS, Constants.MAX_FILES];
        }

        // Called only by Board
        public void DropTo(Location l)
        {
            status = Status.BOARD;
            location = l;
        }

        // Called only by Board
        public void MoveTo(Location l)
        {
            location = l;
        }

        // Called only by Board
        public void Kill()
        {
            status = Status.CAPTURED;
            location = null;
        }

        public bool CanDropTo(int r, int f)
        {
            return valid_drops[r-1, f-1] != null;
        }

        public bool CanMoveTo(int r, int f)
        {
            return valid_moves[r-1, f-1] != null;
        }

        public bool CanAttackTo(int r, int f)
        {
            return valid_attacks[r-1, f-1] != null;
        }

        public void AddValidDropAt(Location d)
        {
            valid_drops[d.rank-1, d.file-1] = d;
        }

        public void AddValidMoveAt(Location m)
        {
            valid_moves[m.rank-1, m.file-1] = m;
        }

        public void AddValidAttackAt(Location a)
        {
            valid_attacks[a.rank-1, a.file-1] = a;
        }


        public bool IsFriendlyWith(Piece p)
        {
            return player.Equals(p.player);
        }

        
        public void UpdateMovesAndAttacks()
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

                    moveset.AddBentMove(3,  2,0,0,0, MoveType.BLOCKABLE);
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
                    moveset.AddBentMove(1,  2,0,0,0, MoveType.TELEPORTABLE);
                    moveset.AddBentMove(2,  2,2,0,0, MoveType.TELEPORTABLE);
                    moveset.AddBentMove(3,  2,2,2,2, MoveType.TELEPORTABLE);
                    break;
                case PieceType.SAMURAI:
                    moveset.AddStraightMove(1,  0,1,0,1,0, MoveType.BLOCKABLE);
                    moveset.AddStraightMove(2,  0,2,0,2,0, MoveType.BLOCKABLE);
                    moveset.AddStraightMove(3,  0,Constants.MAX_MOVES,0,Constants.MAX_MOVES,0, MoveType.BLOCKABLE);
                    break;
                case PieceType.CANNON:
                    moveset.AddStraightMove(1,  1,0,1,0,1, MoveType.BLOCKABLE_PACIFIST);
                    moveset.AddStraightMove(2,  2,0,2,0,2, MoveType.BLOCKABLE_PACIFIST);
                    moveset.AddStraightMove(3,  Constants.MAX_MOVES,0,Constants.MAX_MOVES,0,Constants.MAX_MOVES, MoveType.BLOCKABLE_PACIFIST);
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

        override
        public string ToString()
        {
            String symbol = ( (char)type ).ToString();
            if (player.color == PlayerColor.BLACK)
                symbol = symbol.ToLower();
            return symbol;
        }
    }
}
