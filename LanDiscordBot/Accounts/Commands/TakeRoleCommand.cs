using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using LanDiscordBot.Accounts;
using LanDiscordBot.Bot;
using LanDiscordBot.Chat;

namespace LanDiscordBot.Accounts.Commands
{
    public class TakeRoleCommand : ChatCommand
    {
        public TakeRoleCommand(BotService service) 
            : base(service)
        {
            AccessFlag = "AccountsManageRole";
            Description = "Removes a role from specified users.";
        }

        public override void Execute(ChatMessageArgs message, string arguments)
        {
            if (!Service.Accounts.CheckAccess(message.User.Id, "AccountsManageRole"))
            {
                Service.Chat.SendDirectMessage(message.User, "You do not have permission to remove roles from user accounts.");

                return;
            }

            List<String> args = ParseArgs(arguments);

            if (args.Count < 2 || message.Data.MentionedUsers.Count < 1)
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " Usage: " + Service.Settings.ChatCommandPrefix + "takerole <ROLE> [MENTIONED USERS]");

                return;
            }

            UserRole role = Service.Accounts.GetRole(args[0]);

            if (role == null)
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " \"" + args[0] + "\" is an invalid role!");

                return;
            }

            String resultMessage = message.User.Mention + " You have successfully taken the role \"" + args[0] + "\" from";

            foreach (SocketUser user in message.Data.MentionedUsers)
            {
                UserAccount account = Service.Accounts.CreateAccount(user.Id);

                if (!account.CheckAccess("AccountsManageUserRole") && !account.Roles.Contains(role))
                {
                    account.Roles.Remove(role);

                    resultMessage += " " + user.Mention;
                }
            }

            Service.Chat.SendMessage(message.Channel, resultMessage + "!");

            Service.Accounts.SaveChanges();

            return;
        }
    }
}
