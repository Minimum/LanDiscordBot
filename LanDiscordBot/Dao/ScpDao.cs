using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Scp;
using Newtonsoft.Json;

namespace LanDiscordBot.Dao
{
    public static class ScpDao
    {
        public static Dictionary<int, ScpObject> LoadScps()
        {
            String data = "";

            using (var file = new FileStream(GlobalDao.DataPath + "scp/scps.json", FileMode.Open))
            using (StreamReader reader = new StreamReader(file))
            {
                data = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<Dictionary<int, ScpObject>>(data, GetJsonSettings());
        }

        public static void SaveScps(Dictionary<int, ScpObject> scps)
        {
            String data = JsonConvert.SerializeObject(scps, GetJsonSettings());

            Directory.CreateDirectory(GlobalDao.DataPath + "scp/");

            using (FileStream stream = new FileStream(GlobalDao.DataPath + "scp/scps.json", FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(data);
            }

            return;
        }

        public static List<String> LoadInvalidResponses()
        {
            String data = "";

            using (var file = new FileStream(GlobalDao.DataPath + "scp/invalidResponses.json", FileMode.Open))
            using (StreamReader reader = new StreamReader(file))
            {
                data = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<List<String>>(data, GetJsonSettings());
        }

        public static void SaveInvalidResponses(List<String> responses)
        {
            String data = JsonConvert.SerializeObject(responses, GetJsonSettings());

            Directory.CreateDirectory(GlobalDao.DataPath + "scp/");

            using (FileStream stream = new FileStream(GlobalDao.DataPath + "scp/invalidResponses.json", FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(data);
            }

            return;
        }

        public static List<String> LoadUnknownResponses()
        {
            String data = "";

            using (var file = new FileStream(GlobalDao.DataPath + "scp/unknownResponses.json", FileMode.Open))
            using (StreamReader reader = new StreamReader(file))
            {
                data = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<List<String>>(data, GetJsonSettings());
        }

        public static void SaveUnknownResponses(List<String> responses)
        {
            String data = JsonConvert.SerializeObject(responses, GetJsonSettings());

            Directory.CreateDirectory(GlobalDao.DataPath + "scp/");

            using (FileStream stream = new FileStream(GlobalDao.DataPath + "scp/unknownResponses.json", FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(data);
            }

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
