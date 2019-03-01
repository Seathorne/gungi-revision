using GungiRevision.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GungiRevision
{
    class OutsideBoardException : Exception
    {
        public OutsideBoardException(int r, int f, int t)
        {
            Util.PRL(String.Format("OutsideBoardException: location {0:d}-{1:d}-{2:d} out of range.", r, f, t) );
        }
    }
}
