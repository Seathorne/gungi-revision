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

            Clear();
        }

        public Piece Clone(Player pl)
        {
            Piece p = new Piece(pl, this.type);

            p.location = this.location;
            p.status = this.status;
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

        public void SetValidDrops(Location[,] drops)
        {
            valid_drops = drops;
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
