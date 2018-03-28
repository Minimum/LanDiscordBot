using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;
using LanDiscordBot.Chat;

namespace LanDiscordBot.Dad.Commands
{
    public class DisableDadCommand : ChatCommand
    {
        public DisableDadCommand(BotService service) 
            : base(service)
        {
            AccessFlag = "DadEnable";
            Description = "Disables Dad Mode.";
        }

        public override void Execute(ChatMessageArgs message, string args)
        {
            if (!Service.Accounts.CheckAccess(message.User.Id, AccessFlag))
            {
                Service.Chat.SendDirectMessage(message.User, "You do not have permission to disable Dad Mode!");

                return;
            }

            if (Service.Dad.DisableTalk())
            {
                Service.SaveChanges();

                Service.Chat.SendMessage(message.Channel, message.User.Mention + " Dad Mode has been disabled!");
            }
            else
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " Dad Mode is already disabled!");
            }

            return;
        }
    }
}
