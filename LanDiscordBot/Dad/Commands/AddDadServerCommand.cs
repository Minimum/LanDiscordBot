using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;
using LanDiscordBot.Chat;

namespace LanDiscordBot.Dad.Commands
{
    public class AddDadServerCommand : ChatCommand
    {
        public AddDadServerCommand(BotService service) 
            : base(service)
        {
            AccessFlag = "DadManageServer";
            Description = "Adds the current server to the Dad Mode whitelist.";
        }

        public override void Execute(ChatMessageArgs message, string args)
        {
            if (!Service.Accounts.CheckAccess(message.User.Id, AccessFlag))
            {
                Service.Chat.SendDirectMessage(message.User, "You do not have permission to add servers to the Dad Mode whitelist!");

                return;
            }

            var server = message.Server;

            if (server == null)
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " This is not a valid server!");

                return;
            }

            if (Service.Settings.DadTalkServers.Add(server.Id))
            {
                Service.SaveChanges();

                Service.Chat.SendMessage(message.Channel, message.User.Mention + " This server has been added to the Dad Mode whitelist!");
            }
            else
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " This server was already in the Dad Mode whitelist!");
            }

            return;
        }
    }
}
