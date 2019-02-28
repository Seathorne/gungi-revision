using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GungiRevision.Utility
{
    class Util
    {
        private static string input_thus_far;


        public static void Pr<T>(T o)
        {
            Console.Write(o.ToString());
        }
        public static void PrLi<T>(T o)
        {
            Console.WriteLine(o.ToString());
        }

        public static void LogInput(string input)
        {
            input_thus_far += input;
        }

        public static void PrintInputThusFar()
        {
            PrLi(input_thus_far);
        }


        public static bool ValidLocation(int r, int f, int t)
        {
            return r >= 1 && r <= Constants.MAX_RANKS
                && f >= 1 && f <= Constants.MAX_FILES
                && t >= 1 && t <= Constants.MAX_TIERS;
        }
        public static bool ValidLocation(int r, int f)
        {
            return r >= 1 && r <= Constants.MAX_RANKS
                && f >= 1 && f <= Constants.MAX_FILES;
        }


        public static PlayerColor OtherPlayerColor(PlayerColor c)
        {
            return (c == PlayerColor.BLACK) ? PlayerColor.WHITE : PlayerColor.BLACK;
        }


        public static bool HashesUnique<T>(List<T> o_list)
        {
            bool hashes_unique = true;
            
            List<int> hash_list = new List<int>();
            foreach (T o in o_list)
                hash_list.Add(o.GetHashCode());

            for (int i = 0; hashes_unique && i < hash_list.Count; i++)
                for (int j = i+1; j < hash_list.Count; j++)
                    if (hash_list[i] == hash_list[j])
                    {
                        hashes_unique = false;
                        PrLi("Hashes not unique: " + o_list[i] + " and " + o_list[j] + " are equal to " + hash_list[i] + ".");
                    }
            
            return hashes_unique;
        }
    }
}
