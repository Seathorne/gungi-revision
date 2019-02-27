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
        Marshal,
        Spy,
        Lieutenant,
        Major,
        General,
        Archer,
        Knight,
        Samurai,
        Cannon,
        Counsel,
        Fortress,
        Musketeer,
        Pawn
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight,
        UpUpLeft,
        UpUpRight
    }
    
    public class BoardProperties
    {
        public const int MAX_RANKS = 9, MAX_FILES = 9, MAX_TIERS = 3,
            PLACEMENT_RANKS = 3,
            MAX_MOVE_DISTANCE = 8;
    }
}
