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

        public String PlayStatus { get; set; }

        // Chat settings
        public String ChatCommandPrefix { get; set; }

        public BotSettings()
        {
            AuthToken = "";

            PlayStatus = "";

            ChatCommandPrefix = ".";
        }
    }
}
