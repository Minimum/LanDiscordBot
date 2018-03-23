using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using LanDiscordBot.Bot;

namespace LanDiscordBot.Chat.Commands
{
    public class SetNickCommand : ChatCommand
    {
        public SetNickCommand(BotService service)
            : base(service)
        {
            Description = "Sets the bot's global nickname.";

            AccessFlag = "AccountSetNickname";
        }

        public override void Execute(ChatMessageArgs message, String arguments)
        {
            if (Service.Accounts.CheckAccess(message.User.Id, "AccountSetNickname"))
            {
                ChangeName(arguments);

                Service.Chat.SendDirectMessage(message.User, "I have changed my global nickname to \"" + arguments + "\".");
            }
            else
            {
                Service.Chat.SendDirectMessage(message.User, "You currently lack the access to change my global nickname.");

                // TODO: log
            }

            return;
        }

        private async void ChangeName(String name)
        {
            foreach (var guild in Service.Client.Guilds)
            {
                await guild.GetUser(Service.Client.CurrentUser.Id).ModifyAsync(x => { x.Nickname = name; });
            }

            return;
        }

        
    }
}
