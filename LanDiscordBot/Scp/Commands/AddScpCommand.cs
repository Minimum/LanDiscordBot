using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;
using LanDiscordBot.Chat;

namespace LanDiscordBot.Scp.Commands
{
    public class AddScpCommand : ChatCommand
    {
        public AddScpCommand(BotService service)
            : base(service)
        {
            Description = "Creates SCP object entries in the database.";

            AccessFlag = "ScpAddObject";
        }

        public override void Execute(ChatMessageArgs message, string arguments)
        {
            if (!Service.Accounts.CheckAccess(message.User.Id, "ScpAddObject"))
            {
                Service.Chat.SendDirectMessage(message.User, "You do not have permission to add SCP objects.");

                return;
            }

            int id = 0;

            if (!Int32.TryParse(arguments, out id))
            {
                Service.Chat.SendMessage(message.Channel, "Usage: " + Service.Settings.ChatCommandPrefix + "addscp <ID>");

                return;
            }

            if (Service.Scp.Scps.ContainsKey(id))
            {
                Service.Chat.SendMessage(message.Channel, "SCP-" + ScpObject.GetViewId(id) + " already exists!");
            }
            else
            {
                ScpObject scp = new ScpObject();

                scp.Id = id;

                Service.Scp.Scps.Add(id, scp);

                Service.Scp.SaveChanges();

                Service.Chat.SendMessage(message.Channel, "SCP-" + ScpObject.GetViewId(id) + " has been created!");
            }

            return;
        }
    }
}
