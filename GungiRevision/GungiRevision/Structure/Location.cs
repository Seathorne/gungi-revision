using System;

namespace GungiRevision
{
    class Location
    {
        public readonly int rank, file, tier;

        public Location(int r, int f, int t)
        {
            if (!Valid(r, f, t))
            {
                throw new OutsideBoardException(r, f, t);
            }
            else
            {
                rank = r;
                file = f;
                tier = t;
            }
        }

        public static bool Valid(int r, int f, int t)
        {
            return r >= 1 && r <= Constants.MAX_RANKS
                && f >= 1 && f <= Constants.MAX_FILES
                && t >= 1 && t <= Constants.MAX_TIERS;
        }
        public static bool Valid(int r, int f)
        {
            return r >= 1 && r <= Constants.MAX_RANKS
                && f >= 1 && f <= Constants.MAX_FILES;
        }

        override
        public bool Equals(Object o)
        {
            return GetHashCode() == o.GetHashCode();
        }

        override
        public int GetHashCode()
        {
            return (Constants.HASH_RANK * rank) + (Constants.HASH_FILE * file) + (Constants.HASH_TIER * tier);
        }

        override
        public string ToString()
        {
            return String.Format("{0:d}-{1:d}-{2:d}", rank, file, tier);
        }
    }
}
