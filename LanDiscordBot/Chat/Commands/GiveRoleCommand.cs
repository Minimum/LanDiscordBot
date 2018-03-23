using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using LanDiscordBot.Accounts;
using LanDiscordBot.Bot;

namespace LanDiscordBot.Chat.Commands
{
    public class GiveRoleCommand : ChatCommand
    {
        public GiveRoleCommand(BotService service) 
            : base(service)
        {
            AccessFlag = "AccountsManageRole";
            Description = "Adds a role to specified user accounts.";
        }

        public override void Execute(ChatMessageArgs message, string arguments)
        {
            if (!Service.Accounts.CheckAccess(message.User.Id, "AccountsManageRole"))
            {
                Service.Chat.SendDirectMessage(message.User, "You do not have permission to add roles to user accounts.");

                return;
            }

            List<String> args = ParseArgs(arguments);

            if (args.Count < 2 || message.Data.MentionedUsers.Count < 1)
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " Usage: " + Service.Settings.ChatCommandPrefix + "giverole <ROLE> [MENTIONED USERS]");

                return;
            }

            UserRole role = Service.Accounts.GetRole(args[0]);

            if (role == null)
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " \"" + args[0] + "\" is an invalid role!");

                return;
            }

            String resultMessage = message.User.Mention + " You have successfully assigned the role \"" + args[0] + "\" to";

            foreach (SocketUser user in message.Data.MentionedUsers)
            {
                UserAccount account = Service.Accounts.CreateAccount(user.Id);

                if (!account.Roles.Contains(role))
                {
                    account.Roles.Add(role);

                    resultMessage += " " + user.Mention;
                }
            }

            Service.Chat.SendMessage(message.Channel, resultMessage + "!");

            Service.Accounts.SaveChanges();

            return;
        }
    }
}
