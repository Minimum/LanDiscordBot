using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using LanDiscordBot.Bot;

namespace LanDiscordBot.Chat.Commands
{
    public class SayCommand : ChatCommand
    {
        public SayCommand(BotService service) 
            : base(service)
        {
            AccessFlag = "AccountSay";
            Description = "Allows you to speak through me.";
        }

        public override void Execute(ChatMessageArgs message, string arguments)
        {
            if (!Service.Accounts.CheckAccess(message.User.Id, "AccountSay"))
            {
                Service.Chat.SendDirectMessage(message.User, "You do not have permission to speak through me.");

                return;
            }

            List<String> args = ParseArgs(arguments);

            if (args.Count < 2)
            {
                Service.Chat.SendMessage(message.Channel, "Usage: " + Service.Settings.ChatCommandPrefix + "say <MESSAGE> <CHANNEL> [SERVER]");

                return;
            }

            SocketTextChannel targetChannel = null;
            bool selectServer = args.Count > 2;

            foreach (SocketGuild guild in Service.Client.Guilds)
            {
                if (selectServer && !guild.Name.Contains(args[2]))
                {
                    continue;
                }

                foreach (SocketTextChannel channel in guild.TextChannels)
                {
                    if (channel.Name.Contains(args[1]))
                    {
                        if (targetChannel == null)
                        {
                            targetChannel = channel;
                        }
                        else
                        {
                            Service.Chat.SendMessage(message.Channel, message.User.Mention + " The channel descriptor that you supplied is too ambiguous.");

                            return;
                        }
                    }
                }
            }

            if (targetChannel == null)
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " I couldn't find a channel that fit the description!");

                return;
            }

            Service.Chat.SendMessage(targetChannel, args[0]);

            return;
        }
    }
}
