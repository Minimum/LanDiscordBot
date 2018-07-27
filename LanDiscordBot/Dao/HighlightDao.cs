using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Highlights;
using Newtonsoft.Json;

namespace LanDiscordBot.Dao
{
    public static class HighlightDao
    {
        public static Dictionary<UInt64, HighlightServer> LoadServers()
        {
            String data = "";

            using (var file = new FileStream(GlobalDao.DataPath + "highlights/servers.json", FileMode.Open))
            using (StreamReader reader = new StreamReader(file))
            {
                data = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<Dictionary<UInt64, HighlightServer>>(data, GetJsonSettings());
        }

        public static void SaveServers(Dictionary<UInt64, HighlightServer> servers)
        {
            String data = JsonConvert.SerializeObject(servers, GetJsonSettings());

            GlobalDao.SaveData("highlights", "servers.json", data);

            return;
        }

        private static JsonSerializerSettings GetJsonSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();

            //settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            settings.TypeNameHandling = TypeNameHandling.Auto;

            return settings;
        }
    }
}
