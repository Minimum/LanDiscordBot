using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanDiscordBot.Dao
{
    public static class GlobalDao
    {
        public static String AppPath => Environment.CurrentDirectory + "/";
        public static String ConfigPath => AppPath + "config/";
        public static String DataPath => AppPath + "data/";

        public static void SaveData(String directory, String file, String data)
        {
            Directory.CreateDirectory(DataPath + directory + "/");

            using (FileStream stream = new FileStream(DataPath + directory + "/" + file, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(data);
            }

            return;
        }
    }
}
