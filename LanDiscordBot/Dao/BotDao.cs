using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;
using Newtonsoft.Json;

namespace LanDiscordBot.Dao
{
    public static class BotDao
    {
        public static BotSettings LoadSettings()
        {
            String data = "";

            using (FileStream stream = new FileStream(GlobalDao.ConfigPath + "bot.json", FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(stream))
            {
                data = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<BotSettings>(data, GetJsonSettings());
        }

        public static void SaveSettings(BotSettings settings)
        {
            String data = JsonConvert.SerializeObject(settings, GetJsonSettings());

            Directory.CreateDirectory(GlobalDao.ConfigPath);

            using (FileStream stream = new FileStream(GlobalDao.ConfigPath + "bot.json", FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(data);
            }

            return;
        }

        private static JsonSerializerSettings GetJsonSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();

            settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            settings.TypeNameHandling = TypeNameHandling.Auto;

            return settings;
        }
    }
}
