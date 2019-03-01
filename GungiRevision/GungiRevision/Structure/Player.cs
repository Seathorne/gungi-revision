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
                pl.p_hand.Add(p.Clone(pl));

            return pl;
        }


        public List<Piece> TopPieces()
        {
            return board.PlayerTopPieces(this);
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
                new Piece(this, PieceType.MARSHAL),
                
                new Piece(this, PieceType.SPY),
                new Piece(this, PieceType.SPY),

                new Piece(this, PieceType.LIEUTENANT),
                new Piece(this, PieceType.LIEUTENANT),

                new Piece(this, PieceType.MAJOR),
                new Piece(this, PieceType.MAJOR),
                new Piece(this, PieceType.MAJOR),
                new Piece(this, PieceType.MAJOR),

                new Piece(this, PieceType.GENERAL),
                new Piece(this, PieceType.GENERAL),
                new Piece(this, PieceType.GENERAL),
                new Piece(this, PieceType.GENERAL),
                new Piece(this, PieceType.GENERAL),
                new Piece(this, PieceType.GENERAL),

                new Piece(this, PieceType.ARCHER),
                new Piece(this, PieceType.ARCHER),

                new Piece(this, PieceType.KNIGHT),
                new Piece(this, PieceType.KNIGHT),

                new Piece(this, PieceType.SAMURAI),
                new Piece(this, PieceType.SAMURAI),

                new Piece(this, PieceType.CANNON),
                new Piece(this, PieceType.CANNON),

                new Piece(this, PieceType.COUNSEL),
                new Piece(this, PieceType.COUNSEL),

                new Piece(this, PieceType.FORTRESS),
                new Piece(this, PieceType.FORTRESS),

                new Piece(this, PieceType.MUSKETEER),
                new Piece(this, PieceType.MUSKETEER),

                new Piece(this, PieceType.PAWN),
                new Piece(this, PieceType.PAWN),
                new Piece(this, PieceType.PAWN),
                new Piece(this, PieceType.PAWN),
                new Piece(this, PieceType.PAWN),
                new Piece(this, PieceType.PAWN),
                new Piece(this, PieceType.PAWN),
                new Piece(this, PieceType.PAWN),
                new Piece(this, PieceType.PAWN)
            };

            return list;
        }

        public Piece GetHandPiece(PieceType pt)
        {
            if (p_hand.Count > 0)
                return p_hand.Find(p => p.type == pt);
            else
                return null;
        }
        public Piece GetHandPiece(int rand)
        {
            if (p_hand.Count > 0)
                return p_hand.ElementAt(rand);
            else
                return null;
        }


        public bool IsSame(Player pl)
        {
            return pl.color == color && pl.board == board && pl.p_hand == p_hand;
        }


        override
        public string ToString()
        {
            return color.ToString();
        }

        public void PrintHand()
        {
            string str = "";
            Dictionary<string, int> dict = new Dictionary<string, int>();

            foreach (Piece p in p_hand)
                if (dict.ContainsKey(p.ToString()))
                    dict[p.ToString()] += 1;
                else
                    dict.Add(p.ToString(), 1);

            foreach (KeyValuePair<string, int> num in dict)
            {
                str += num.Key.ToString() + ":" + num.Value + "  ";
            }

            Util.PRL(str);
        }
    }
}
