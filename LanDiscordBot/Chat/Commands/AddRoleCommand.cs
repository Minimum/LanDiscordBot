using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;

namespace LanDiscordBot.Chat.Commands
{
    public class AddRoleCommand : ChatCommand
    {
        public AddRoleCommand(BotService service) 
            : base(service)
        {
            AccessFlag = "AccountsManageRole";
            Description = "Creates an access role.";
        }

        public override void Execute(ChatMessageArgs message, string args)
        {
            if(!Service.Accounts.CheckAccess(message.User.Id, "AccountsManageRole"))
            {
                Service.Chat.SendDirectMessage(message.User, "You do not have permission to create access roles.");

                return;
            }

            if (Service.Accounts.GetRole(args) != null)
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " Role \"" + args + "\" already exists!");
            }
            else
            {
                Service.Accounts.CreateRole(args);

                Service.Chat.SendMessage(message.Channel, message.User.Mention + " Role \"" + args + "\" has been created!");
            }

            Service.Accounts.SaveChanges();

            return;
        }
    }
}
