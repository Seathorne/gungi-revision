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
        public List<Location> valid_moves_list, valid_attacks_list;
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

        public Piece Clone(Player clone_pl)
        {
            Piece clone_p = new Piece(clone_pl, this.type);

            clone_p.location = this.location;
            clone_p.status = this.status;
            clone_p.acting_tier = this.acting_tier;
            clone_p.lt_sight = this.lt_sight;

            return clone_p;
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

            valid_moves_list = new List<Location>();
            valid_attacks_list = new List<Location>();
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

        public bool CanDropTo(Location l)
        {
            return valid_drops[l.rank-1, l.file-1] != null && valid_drops[l.rank-1, l.file-1].Equals(l);
        }
        public bool CanDropTo(int r, int f)
        {
            return valid_drops[r-1, f-1] != null;
        }

        public bool CanMoveTo(Location l)
        {
            return valid_moves[l.rank-1, l.file-1] != null && valid_moves[l.rank-1, l.file-1].Equals(l);
        }
        public bool CanMoveTo(int r, int f)
        {
            return valid_moves[r-1, f-1] != null;
        }

        public bool CanAttackTo(Location l)
        {
            return valid_attacks[l.rank-1, l.file-1] != null && valid_attacks[l.rank-1, l.file-1].Equals(l);
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
            valid_moves_list.Add(m);
        }

        public void AddValidAttackAt(Location a)
        {
            valid_attacks[a.rank-1, a.file-1] = a;
            valid_attacks_list.Add(a);
        }

        public Location GetMoveAt(int r, int f)
        {
            if (CanMoveTo(r, f))
                return valid_moves[r-1, f-1];
            else
                return null;
        }

        public Location GetAttackAt(int r, int f)
        {
            if (CanAttackTo(r, f))
                return valid_attacks[r-1, f-1];
            else
                return null;
        }

        public Location GetDropAt(int r, int f)
        {
            if (CanDropTo(r, f))
                return valid_drops[r-1, f-1];
            else
                return null;
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

        public string Name()
        {
            return Name(type);
        }

        public static string Name(PieceType pt)
        {
            switch (pt)
            {
                case PieceType.MARSHAL:
                    return "Marshal";
                case PieceType.SPY:
                    return "Spy";
                case PieceType.LIEUTENANT:
                    return "Lt. General";
                case PieceType.MAJOR:
                    return "Maj. General";
                case PieceType.GENERAL:
                    return "General";
                case PieceType.ARCHER:
                    return "Archer";
                case PieceType.KNIGHT:
                    return "Knight";
                case PieceType.SAMURAI:
                    return "Samurai";
                case PieceType.CANNON:
                    return "Cannon";
                case PieceType.COUNSEL:
                    return "Counsel";
                case PieceType.FORTRESS:
                    return "Fortress";
                case PieceType.MUSKETEER:
                    return "Musketeer";
                case PieceType.PAWN:
                    return "Pawn";
                default:
                    return "Invalid";
            }
        }
    }
}
