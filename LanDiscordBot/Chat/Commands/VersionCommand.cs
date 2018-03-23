using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;

namespace LanDiscordBot.Chat.Commands
{
    public class VersionCommand : ChatCommand
    {
        public VersionCommand(BotService service) 
            : base(service)
        {
            Description = "Shows the current version info.";
        }

        public override void Execute(ChatMessageArgs message, string args)
        {
            Service.Chat.SendMessage(message.Channel, "LanDiscordBot " + BotService.Version + "\n---\nGitHub: https://github.com/Minimum/LanDiscordBot");

            return;
        }
    }
}
