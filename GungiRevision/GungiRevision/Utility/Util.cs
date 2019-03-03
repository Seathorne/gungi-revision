using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GungiRevision.Utility
{
    class Util
    {
        private static readonly string f_log_name = "W:/Documents/Programming/gungi-revision/Logs/" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff") + ".txt";

        public static void P<T>(T o)
        {
            Console.Write(o.ToString());
        }
        public static void L<T>(T o)
        {
            Console.WriteLine(o.ToString());
        }

        public static void Log<T>(T input)
        {
            FileStream f_log = new FileStream(f_log_name, FileMode.Append, FileAccess.Write);
            StreamWriter log_writer = new StreamWriter(f_log);

            log_writer.WriteLine(input.ToString());

            log_writer.Close();
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
                        L("Hashes not unique: " + o_list[i] + " and " + o_list[j] + " are equal to " + hash_list[i] + ".");
                    }
            
            return hashes_unique;
        }
    }
}
