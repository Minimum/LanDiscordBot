using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;
using Microsoft.Extensions.DependencyInjection;

namespace LanDiscordBot.Chat.Commands
{
    public class SetStatusCommand : ChatCommand
    {
        public SetStatusCommand(BotService service) 
            : base(service)
        {
            AccessFlag = "AccountSetStatus";
            Description = "Sets the bot's game status.";
        }

        public override void Execute(ChatMessageArgs message, String args)
        {
            if (!Service.Accounts.CheckAccess(message.User.Id, AccessFlag))
            {
                Service.Chat.SendDirectMessage(message.User, "You do not have permission to change my status.");

                return;
            }

            Service.Client.SetGameAsync(args);

            Service.Settings.PlayStatus = args;

            Service.SaveChanges();

            Service.Chat.SendMessage(message.Channel, message.User.Mention + " My status has been changed to \"" + args + "\".");

            return;
        }


    }
}
