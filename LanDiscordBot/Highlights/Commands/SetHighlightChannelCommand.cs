using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using LanDiscordBot.Bot;
using LanDiscordBot.Chat;

namespace LanDiscordBot.Highlights.Commands
{
    public class SetHighlightChannelCommand : ChatCommand
    {
        public SetHighlightChannelCommand(BotService service)
            : base(service)
        {
            AccessFlag = "HighlightManageServer";
            Description = "Sets the highlights channel for the current server.";
        }

        public override void Execute(ChatMessageArgs message, string args)
        {
            if (!Service.Accounts.CheckAccess(message.User.Id, AccessFlag))
            {
                Service.Chat.SendDirectMessage(message.User, "You do not have permission to configure the highlights channel!");

                return;
            }

            var server = message.Server;

            if (server == null)
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " This is not a valid server!");

                return;
            }

            SocketGuildChannel channel = message.Data.MentionedChannels.FirstOrDefault();

            if (channel != null)
            {
                if (!Service.Highlights.Servers.ContainsKey(server.Id))
                {
                    Service.Chat.SendMessage(message.Channel, message.User.Mention + " Please explicitly designate this server for the highlights system!");

                    return;
                }

                Service.Highlights.Servers[server.Id].HighlightChannel = channel.Id;

                Service.Highlights.SaveChanges();

                Service.Chat.SendMessage(message.Channel, message.User.Mention + " The highlights channel in this server has been set to #" + channel.Name + "!");

                return;
            }
            else
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " Please specify a valid channel!");
            }

            return;
        }
    }
}
