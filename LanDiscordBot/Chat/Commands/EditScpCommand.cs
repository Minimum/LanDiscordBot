using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;
using LanDiscordBot.Scp;

namespace LanDiscordBot.Chat.Commands
{
    public class EditScpCommand : ChatCommand
    {
        public EditScpCommand(BotService service)
            : base(service)
        {
            Description = "Edits current SCP objects in the database.";

            AccessFlag = "ScpEditObject";
        }

        public override void Execute(ChatMessageArgs message, string arguments)
        {
            if (!Service.Accounts.CheckAccess(message.User.Id, "ScpEditObject"))
            {
                Service.Chat.SendDirectMessage(message.User, "You do not have permission to edit SCP objects.");

                return;
            }

            List<String> args = ParseArgs(arguments);

            if (args.Count < 3)
            {
                Service.Chat.SendMessage(message.Channel, "Usage: " + Service.Settings.ChatCommandPrefix + "editscp <ID> <FIELD> <INFO>");

                return;
            }

            int id = 0;

            if (!Int32.TryParse(args[0], out id))
            {
                Service.Chat.SendMessage(message.Channel, "Usage: " + Service.Settings.ChatCommandPrefix + "editscp <ID> <FIELD> <INFO>");

                return;
            }

            if (!Service.Scp.Scps.ContainsKey(id))
            {
                Service.Chat.SendMessage(message.Channel, "SCP-" + ScpObject.GetViewId(id) + " does not exist!");

                return;
            }

            ScpObject scp = Service.Scp.Scps[id];

            if (args[1].Equals("name", StringComparison.OrdinalIgnoreCase))
            {
                scp.Name = args[2];

                Service.Chat.SendMessage(message.Channel, "Successfully changed SCP-" + ScpObject.GetViewId(id) + "'s name to \"" + args[2] + "\"!");
            }
            else if (args[1].Equals("class", StringComparison.OrdinalIgnoreCase))
            {
                String classChangeStatus = "";

                if (args[2].Equals("unknown", StringComparison.OrdinalIgnoreCase))
                {
                    scp.ObjectClass = ScpObjectClass.Unknown;

                    classChangeStatus = "Unknown";
                }
                else if (args[2].Equals("safe", StringComparison.OrdinalIgnoreCase))
                {
                    scp.ObjectClass = ScpObjectClass.Safe;

                    classChangeStatus = "Safe";
                }
                else if (args[2].Equals("euclid", StringComparison.OrdinalIgnoreCase))
                {
                    scp.ObjectClass = ScpObjectClass.Euclid;

                    classChangeStatus = "Euclid";
                }
                else if (args[2].Equals("keter", StringComparison.OrdinalIgnoreCase))
                {
                    scp.ObjectClass = ScpObjectClass.Keter;

                    classChangeStatus = "Keter";
                }
                else
                {
                    scp.ObjectClass = ScpObjectClass.Custom;

                    scp.ObjectClassCustom = args[2];

                    classChangeStatus = args[2];
                }

                Service.Chat.SendMessage(message.Channel, "Successfully changed SCP-" + ScpObject.GetViewId(id) + "'s class to \"" + classChangeStatus + "\"!");
            }
            else if (args[1].Equals("description", StringComparison.OrdinalIgnoreCase))
            {
                scp.Description = args[2];

                Service.Chat.SendMessage(message.Channel, "Successfully changed SCP-" + ScpObject.GetViewId(id) + "'s description to \n```" + args[2] + "```");
            }
            else
            {
                Service.Chat.SendMessage(message.Channel, "Valid field types: \"Name\", \"Class\", or \"Description\".");

                return;
            }

            Service.Scp.SaveChanges();

            return;
        }
    }
}
