using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GungiRevision.Util
{
    class U
    {
        private static string input_thus_far;
        
        public static void Pr(String s)
        {
            Console.Write(s);
        }

        public static void PrLi(string s)
        {
            Console.WriteLine(s);
        }

        public static void LogInput(string input)
        {
            input_thus_far += input;
        }

        public static void PrintInputThusFar()
        {
            PrLi(input_thus_far);
        }
    }
}
