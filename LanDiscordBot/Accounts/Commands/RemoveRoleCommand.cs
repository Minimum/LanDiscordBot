using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;
using LanDiscordBot.Chat;

namespace LanDiscordBot.Accounts.Commands
{
    public class RemoveRoleCommand : ChatCommand
    {
        public RemoveRoleCommand(BotService service) 
            : base(service)
        {
            AccessFlag = "AccountsManageRole";
            Description = "Removes an access role.";
        }

        public override void Execute(ChatMessageArgs message, String args)
        {
            if (!Service.Accounts.CheckAccess(message.User.Id, "AccountsManageRole"))
            {
                Service.Chat.SendDirectMessage(message.User, "You do not have permission to remove access roles.");

                return;
            }

            UserRole role = Service.Accounts.GetRole(args);

            if (role == null)
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " That role does not exist!");

                return;
            }

            Service.Accounts.Roles.Remove(role.Name);

            foreach (UserAccount account in Service.Accounts.Accounts.Values)
            {
                account.Roles.Remove(role);
            }

            Service.Accounts.SaveChanges();

            Service.Chat.SendMessage(message.Channel, message.User.Mention + " I have successfully removed the role \"" + role.Name + "\".");

            return;
        }
    }
}
