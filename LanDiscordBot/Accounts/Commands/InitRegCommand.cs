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
    public class InitRegCommand : ChatCommand
    {
        public InitRegCommand(BotService service)
            : base(service)
        {
            ShowInHelp = false;
        }

        public override void Execute(ChatMessageArgs message, string arguments)
        {
            UserAccount account = Service.Accounts.InitReg(message.User.Id, arguments);

            if (account != null)
            {
                Service.Chat.SendDirectMessage(message.User, "You have successfully registered as a root user!");
            }
            else
            {
                System.Console.WriteLine("[Accounting] WARNING: A user has attempted to initreg, password \"" + arguments + "\"");
            }

            return;
        }
    }
}
