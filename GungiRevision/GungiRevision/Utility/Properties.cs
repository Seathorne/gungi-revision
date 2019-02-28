using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GungiRevision
{
    public enum PlayerColor
    {
        BLACK,
        WHITE
    }

    public enum PieceType
    {
        MARSHAL = 'M',
        SPY = 'Y',
        LIEUTENANT = 'L',
        MAJOR = 'J',
        GENERAL = 'G',
        ARCHER = 'A',
        KNIGHT = 'K',
        SAMURAI = 'S',
        CANNON = 'C',
        COUNSEL = 'U',
        FORTRESS = 'F',
        MUSKETEER = 'R',
        PAWN = 'P'
    }

    public enum MoveType
    {
        BLOCKABLE,
        TELEPORTABLE,
        JUMP_ATTACK,
        PACIFIST_BLOCKABLE
    }

    public enum Status
    {
        BOARD,
        HAND,
        CAPTURED,
    }
    
    public class Constants
    {
        public const int MAX_RANKS = 9, MAX_FILES = MAX_RANKS, MAX_TIERS = 3,
            MAX_MOVES = MAX_RANKS-1,
            NUM_PLAYERS = 2, PLACEMENT_RANKS = 3;
            

        public const int HASH_PLAYER = 37783, HASH_RANK = 37277, HASH_FILE = 62017, HASH_TIER = 24109;
    }
}
