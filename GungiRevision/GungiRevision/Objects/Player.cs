using System;
using GungiRevision.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GungiRevision.Objects
{
    class Player
    {
        public readonly Board board;
        public readonly PlayerColor color;
        public List<Piece> p_hand;


        // Creates a new player object, defined by its color, with reference to its board.
        public Player(Board b, PlayerColor c)
        {
            board = b;
            color = c;

            p_hand = StartingHand();
        }

        public Player Clone(Board b)
        {
            Player pl = new Player(b, this.color);

            foreach (Piece p in this.p_hand)
                pl.p_hand.Add(p.Clone(b, pl));

            return pl;
        }


        // Called only by Board
        public void DropPiece(Piece p)
        {
            p_hand.Remove(p);
        }


        // Returns a new list of all pieces that are given to a player upon starting the game.
        public List<Piece> StartingHand()
        {
            List<Piece> list = new List<Piece>
            {
                new Piece(board, this, PieceType.MARSHAL),
                
                new Piece(board, this, PieceType.SPY),
                new Piece(board, this, PieceType.SPY),

                new Piece(board, this, PieceType.LIEUTENANT),
                new Piece(board, this, PieceType.LIEUTENANT),

                new Piece(board, this, PieceType.MAJOR),
                new Piece(board, this, PieceType.MAJOR),
                new Piece(board, this, PieceType.MAJOR),
                new Piece(board, this, PieceType.MAJOR),

                new Piece(board, this, PieceType.GENERAL),
                new Piece(board, this, PieceType.GENERAL),
                new Piece(board, this, PieceType.GENERAL),
                new Piece(board, this, PieceType.GENERAL),
                new Piece(board, this, PieceType.GENERAL),
                new Piece(board, this, PieceType.GENERAL),

                new Piece(board, this, PieceType.ARCHER),
                new Piece(board, this, PieceType.ARCHER),

                new Piece(board, this, PieceType.KNIGHT),
                new Piece(board, this, PieceType.KNIGHT),

                new Piece(board, this, PieceType.SAMURAI),
                new Piece(board, this, PieceType.SAMURAI),

                new Piece(board, this, PieceType.CANNON),
                new Piece(board, this, PieceType.CANNON),

                new Piece(board, this, PieceType.COUNSEL),
                new Piece(board, this, PieceType.COUNSEL),

                new Piece(board, this, PieceType.FORTRESS),
                new Piece(board, this, PieceType.FORTRESS),

                new Piece(board, this, PieceType.MUSKETEER),
                new Piece(board, this, PieceType.MUSKETEER),

                new Piece(board, this, PieceType.PAWN),
                new Piece(board, this, PieceType.PAWN),
                new Piece(board, this, PieceType.PAWN),
                new Piece(board, this, PieceType.PAWN),
                new Piece(board, this, PieceType.PAWN),
                new Piece(board, this, PieceType.PAWN),
                new Piece(board, this, PieceType.PAWN),
                new Piece(board, this, PieceType.PAWN),
                new Piece(board, this, PieceType.PAWN)
            };

            return list;
        }


        override
        public bool Equals(Object o)
        {
            return GetHashCode() == o.GetHashCode();
        }

        override
        public int GetHashCode()
        {
            return Constants.HASH_PLAYER * ((int)color + 1);
        }


        override
        public string ToString()
        {
            return color.ToString();
        }
    }
}
