using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Accounts;
using LanDiscordBot.Bot;
using Newtonsoft.Json;

namespace LanDiscordBot.Dao
{
    public static class AccountDao
    {
        public static AccountService LoadAccounts()
        {
            String data = "";

            using (FileStream stream = new FileStream(GlobalDao.DataPath + "accounts/accounts.json", FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(stream))
            {
                data = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<AccountService>(data, GetJsonSettings());
        }

        public static void SaveAccounts(AccountService accounts)
        {
            String data = JsonConvert.SerializeObject(accounts, GetJsonSettings());

            Directory.CreateDirectory(GlobalDao.DataPath + "accounts/");

            using (FileStream stream = new FileStream(GlobalDao.DataPath + "accounts/accounts.json", FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(data);
            }

            return;
        }

        private static JsonSerializerSettings GetJsonSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.Auto
            };

            return settings;
        }
    }
}
