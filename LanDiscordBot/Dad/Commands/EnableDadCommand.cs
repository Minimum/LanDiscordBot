using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;
using LanDiscordBot.Chat;

namespace LanDiscordBot.Dad.Commands
{
    public class EnableDadCommand : ChatCommand
    {
        public EnableDadCommand(BotService service) 
            : base(service)
        {
            AccessFlag = "DadEnable";
            Description = "Enables Dad Mode.";
        }

        public override void Execute(ChatMessageArgs message, string args)
        {
            if (!Service.Accounts.CheckAccess(message.User.Id, AccessFlag))
            {
                Service.Chat.SendDirectMessage(message.User, "You do not have permission to enable Dad Mode!");

                return;
            }

            if (Service.Dad.EnableTalk())
            {
                Service.SaveChanges();

                Service.Chat.SendMessage(message.Channel, message.User.Mention + " Dad Mode has been enabled!");
            }
            else
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " Dad Mode is already enabled!");
            }

            return;
        }
    }
}
