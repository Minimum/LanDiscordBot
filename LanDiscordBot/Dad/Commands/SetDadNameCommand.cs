using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;
using LanDiscordBot.Chat;

namespace LanDiscordBot.Dad.Commands
{
    public class SetDadNameCommand : ChatCommand
    {
        public SetDadNameCommand(BotService service) 
            : base(service)
        {
            AccessFlag = "DadSetName";
            Description = "Sets the name used for Dad Mode.";
        }

        public override void Execute(ChatMessageArgs message, string args)
        {
            if (!Service.Accounts.CheckAccess(message.User.Id, AccessFlag))
            {
                Service.Chat.SendDirectMessage(message.User, "You do not have permission to change the name used for Dad Mode!");

                return;
            }

            if (args.Length < 1)
            {
                Service.Chat.SendMessage(message.Channel, message.User.Mention + " Usage: " + Service.Settings.ChatCommandPrefix + "setdadname <NAME>");
            }
            else
            {
                Service.Settings.DadTalkName = args;

                Service.SaveChanges();

                Service.Chat.SendMessage(message.Channel, message.User.Mention + " The name used for Dad Mode has been set to \"" + args + "\"!");
            }

            return;
        }
    }
}
