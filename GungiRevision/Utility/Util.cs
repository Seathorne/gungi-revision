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
        private static readonly string file_name = "Logs" + Path.DirectorySeparatorChar + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff") + ".txt";
        private static readonly string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file_name);

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
            FileStream f_log = new FileStream(path, FileMode.Append, FileAccess.Write);
            StreamWriter log_writer = new StreamWriter(f_log);

            log_writer.WriteLine(input.ToString());

            log_writer.Close();
        }
    }
}
