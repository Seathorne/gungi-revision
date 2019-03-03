using GungiRevision.Objects;
using GungiRevision.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GungiRevision
{
    class IllegalActionException : Exception
    {
        public IllegalActionException(Piece p, int r, int f, Option op)
        {
            Util.L(String.Format("IllegalActionException: Piece [{0}] cannot {3} to {1:d}-{2:d}.", p, r, f, op) );
        }
    }
}
