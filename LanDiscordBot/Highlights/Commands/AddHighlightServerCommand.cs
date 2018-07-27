using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;
using LanDiscordBot.Chat;

namespace LanDiscordBot.Highlights.Commands
{
    public class AddHighlightServerCommand : ChatCommand
    {
        public AddHighlightServerCommand(BotService service)
            : base(service)
        {
            AccessFlag = "HighlightManageServer";
            Description = "Adds the current server to the highlights whitelist.";
        }

        public override void Execute(ChatMessageArgs message, string args)
        {
            if (!Service.Accounts.CheckAccess(message.User.Id, AccessFlag))
            {
                Service.Chat.SendDirectMessage(message.User, "You do not have permission to add servers to the highlights whitelist!");

                return;
            }

            var server = message.Server;

            if (server == null)
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " This is not a valid server!");

                return;
            }

            if(!Service.Highlights.Servers.ContainsKey(server.Id))
            {
                HighlightServer highlightServer = new HighlightServer();

                Service.Highlights.Servers.Add(server.Id, highlightServer);

                Service.Highlights.SaveChanges();

                Service.Chat.SendMessage(message.Channel, message.User.Mention + " This server has been added to the highlights whitelist!");
            }
            else
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " This server was already in the highlights whitelist!");
            }

            return;
        }
    }
}
