using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;
using LanDiscordBot.Chat;

namespace LanDiscordBot.Accounts.Commands
{
    public class ListRoleCommand : ChatCommand
    {
        public ListRoleCommand(BotService service) : base(service)
        {
            AccessFlag = "AccountsManageRole";
            Description = "Lists all account roles.";
        }

        public override void Execute(ChatMessageArgs message, string args)
        {
            String output = message.User.Mention + " Here are all of the access roles:\n```";

            foreach (UserRole role in Service.Accounts.Roles.Values)
            {
                output += role.Name + "\n";
            }

            Service.Chat.SendMessage(message.Channel, output + "```");

            return;
        }
    }
}
