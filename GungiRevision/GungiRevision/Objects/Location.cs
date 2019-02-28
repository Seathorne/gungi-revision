using GungiRevision.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GungiRevision
{
    class Location
    {
        public readonly int rank, file, tier;

        public Location(int r, int f, int t)
        {
            if (!Util.ValidLocation(r, f, t))
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
