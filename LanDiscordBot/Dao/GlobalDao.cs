using System;
using System.Collections.Generic;
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
    }
}
