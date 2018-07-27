using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;
using LanDiscordBot.Chat;

namespace LanDiscordBot.Highlights.Commands
{
    public class AddReactCommand : ChatCommand
    {
        public AddReactCommand(BotService service) 
            : base(service)
        {
            AccessFlag = "HighlightManageReacts";
            Description = "Adds a react to the current server.";
        }

        public override void Execute(ChatMessageArgs message, string args)
        {
            if (!Service.Accounts.CheckAccess(message.User.Id, AccessFlag))
            {
                Service.Chat.SendDirectMessage(message.User, "You do not have permission to add highlight reacts to this server!");

                return;
            }

            var server = message.Server;

            if (server == null)
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " This is not a valid server!");

                return;
            }

            String emoji = args.Trim();

            if (emoji.Length < 1)
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " Usage: " + Service.Settings.ChatCommandPrefix + "addhighlightreact <EMOJI>");

                return;
            }

            if (!Service.Highlights.Servers.ContainsKey(server.Id))
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " This server is not setup for the highlight system!");

                return;
            }

            HighlightServer highlightServer = Service.Highlights.Servers[server.Id];

            highlightServer.ReactsAllowed.Add(emoji);

            Service.Highlights.SaveChanges();

            Service.Chat.SendMessage(message.Channel, message.User.Mention + " " + emoji + " has been added to this server's highlight reacts list!");

            return;
        }
    }
}
