using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;
using LanDiscordBot.Chat;

namespace LanDiscordBot.Dad.Commands
{
    public class RemoveDadServerCommand : ChatCommand
    {
        public RemoveDadServerCommand(BotService service) 
            : base(service)
        {
            AccessFlag = "DadManageServer";
            Description = "Removes the current server from the Dad Mode whitelist.";
        }

        public override void Execute(ChatMessageArgs message, string args)
        {
            if (!Service.Accounts.CheckAccess(message.User.Id, AccessFlag))
            {
                Service.Chat.SendDirectMessage(message.User, "You do not have permission to remove servers from the Dad Mode whitelist!");

                return;
            }

            var server = message.Server;

            if (server == null)
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " This is not a valid server!");

                return;
            }

            if (Service.Settings.DadTalkServers.Remove(server.Id))
            {
                Service.SaveChanges();

                Service.Chat.SendMessage(message.Channel, message.User.Mention + " This server has been removed from the Dad Mode whitelist!");
            }
            else
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " This server was not found in the Dad Mode whitelist!");
            }

            return;
        }
    }
}
