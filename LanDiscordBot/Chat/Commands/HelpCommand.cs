using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;

namespace LanDiscordBot.Chat.Commands
{
    public class HelpCommand : ChatCommand
    {
        public HelpCommand(BotService service)
            : base(service)
        {
            ShowInHelp = false;
        }

        public override void Execute(ChatMessageArgs message, string args)
        {
            int page;

            if (Int32.TryParse(args, out page))
            {
                if (page < 1)
                    page = 1;
            }
            else
            {
                page = 1;
            }

            List<KeyValuePair<String, ChatCommand>> cmds = Service.Chat.GetHelpListing(message.User.Id, page);

            String output;

            if (cmds.Count > 0)
            {
                output = message.User.Mention + "\nHere is your command listing (page " + page + "):\n```";

                foreach (KeyValuePair<String, ChatCommand> cmd in cmds)
                {
                    output += cmd.Key + " - " + cmd.Value.Description + "\n";
                }

                if (cmds.Count == 10)
                {
                    output += "```\nPlease use \"" + Service.Settings.ChatCommandPrefix + "help " + (page + 1) + "\" to view the next page.";
                }
                else
                {
                    output += "```";
                }
            }
            else
            {
                output = message.User.Mention + "\nSorry, I couldn't find any commands!";
            }

            Service.Chat.SendMessage(message.Channel, output);

            return;
        }
    }
}
