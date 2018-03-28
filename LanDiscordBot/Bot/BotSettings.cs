using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanDiscordBot.Bot
{
    public class BotSettings
    {
        // Bot auth
        public String AuthToken { get; set; }

        // Bot settings
        public String PlayStatus { get; set; }

        // Chat settings
        public String ChatCommandPrefix { get; set; }

        // Dad settings
        public bool DadTalkEnabled { get; set; }
        public String DadTalkName { get; set; }
        public HashSet<UInt64> DadTalkServers { get; set; }

        public BotSettings()
        {
            AuthToken = "";

            PlayStatus = "";

            ChatCommandPrefix = ".";

            DadTalkEnabled = false;
            DadTalkName = "Dad";
            DadTalkServers = new HashSet<UInt64>();
        }
    }
}
