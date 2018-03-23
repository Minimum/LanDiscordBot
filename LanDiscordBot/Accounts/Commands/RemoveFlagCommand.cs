using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Accounts;
using LanDiscordBot.Bot;
using LanDiscordBot.Chat;

namespace LanDiscordBot.Accounts.Commands
{
    public class RemoveFlagCommand : ChatCommand
    {
        public RemoveFlagCommand(BotService service) 
            : base(service)
        {
            AccessFlag = "AccountsManageRole";
            Description = "Removes an access flag from a role.";
        }

        public override void Execute(ChatMessageArgs message, string arguments)
        {
            if (!Service.Accounts.CheckAccess(message.User.Id, "AccountsManageRole"))
            {
                Service.Chat.SendDirectMessage(message.User, "You do not have permission to remove access flags.");

                return;
            }

            List<String> args = ParseArgs(arguments);

            if (args.Count < 2)
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " Usage: " + Service.Settings.ChatCommandPrefix + "removeflag <ROLE> <FLAG>");

                return;
            }

            UserRole role = Service.Accounts.GetRole(args[0]);

            if (role == null)
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " \"" + args[0] + "\" does not currently exist as a role!");

                return;
            }

            if (args[1].Length < 1)
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " The flag supplied is invalid!");

                return;
            }

            role.RemoveFlag(args[1]);

            Service.Chat.SendMessage(message.Channel, message.User.Mention + " Flag \"" + args[1] + "\" has been removed from role \"" + args[0] + "\"!");

            Service.Accounts.SaveChanges();

            return;
        }
    }
}
