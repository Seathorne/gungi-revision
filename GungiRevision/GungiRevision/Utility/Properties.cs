using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GungiRevision
{
    public enum PlayerColor
    {
        BLACK = 0,
        WHITE = 1
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
        BLOCKABLE_PACIFIST
    }

    public enum Status
    {
        BOARD,
        HAND,
        CAPTURED
    }

    public enum CheckStatus
    {
        SAFE,
        CHECK,
        CHECKMATE
    }

    public enum GameState
    {
        START,
        SETUP,
        TURNS,
        END
    }
    
    public class Constants
    {
        public const int MAX_RANKS = 9, MAX_FILES = MAX_RANKS, MAX_TIERS = 3,
            MAX_MOVES = MAX_RANKS-1,
            NUM_PLAYERS = 2, PLACEMENT_RANKS = 3;

        public const int HASH_PLAYER = 37783, HASH_RANK = 37277, HASH_FILE = 62017, HASH_TIER = 24109;

        public const string CHAR_FRIENDLY = "%", CHAR_ENEMY = "#", CHAR_MOVE = "o", CHAR_ATTACK = "#", CHAR_DROP = "o",
            CHAR_V_SEPARATOR = "|", CHAR_H_SEPARATOR = "-", CHAR_I_SEPARATOR = "·", CHAR_EMPTY = " ";

        public static readonly string[] LEGEND =
        {
            "Mm = Marshal",
            "Yy = Spy",
            "Ll = Lt. General",
            "Jj = Maj. General",
            "Gg = General",
            "Aa = Archer",
            "Kk = Knight",
            "Ss = Samurai",
            "Cc = Cannon",
            "Uu = Counsel",
            "Ff = Fortress",
            "Rr = Musketeer",
            "Pp = Pawn"
        };
    }
}
