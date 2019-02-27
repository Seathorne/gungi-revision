using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GungiRevision
{
    class OutsideBoardException : Exception
    {
        public OutsideBoardException()
        {

        }

        public OutsideBoardException(string message)
        {
            Console.WriteLine("OutsideBoardException: " + message + ".");
        }
    }
}
