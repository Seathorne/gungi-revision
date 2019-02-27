using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GungiRevision
{
    class Location
    {
        readonly int rank, file, tier;

        public Location(int r, int f, int t)
        {
            if ( r < 1 || r > BoardProperties.MAX_RANKS
                || f < 1 || f > BoardProperties.MAX_FILES
                || t < 1 || t > BoardProperties.MAX_TIERS )
            {
                throw new OutsideBoardException("location " + r + "-" + f + "-" + t + " out of range");
            }
            else
            {
                rank = r;
                file = f;
                tier = t;
            }
            
        }

        public Location Shift(int forward, int right, int up, bool invert)
        {
            if ( invert )
            {
                forward = -forward;
                right = -right;
                up = -up;
            }
            
            int r = rank + forward,
                f = file + right,
                t = tier + up;

            if ( r < 1 || r > BoardProperties.MAX_RANKS
                || f < 1 || f > BoardProperties.MAX_FILES
                || t < 1 || t > BoardProperties.MAX_TIERS )
            {
                throw new OutsideBoardException("location " + r + "-" + f + "-" + t + " out of range");
            }
            else
            {
                return new Location(r, f, t);
            }
        }

        override
        public bool Equals(Object o)
        {
            if ( o == null || !(o is Location) )
                return false;
            else if ( o == this )
                return true;
            else
            {
                Location location_o = (Location) o;
                return rank == location_o.rank && file == location_o.file && tier == location_o.tier;
            }
        }

        override
        public int GetHashCode()
        {
            var hashCode = 224543195;
            hashCode = hashCode * -1258091289 + rank.GetHashCode();
            hashCode = hashCode * -2145716287 + file.GetHashCode();
            hashCode = hashCode * -1250972151 + tier.GetHashCode();
            return hashCode;
        }

        override
        public string ToString()
        {
            return String.Format("{0:d}-{1:d}-{2:d}", rank, file, tier);
        }
    }
}
