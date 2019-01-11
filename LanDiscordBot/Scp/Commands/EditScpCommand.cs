using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;
using LanDiscordBot.Chat;

namespace LanDiscordBot.Scp.Commands
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
                String classChangeStatus = scp.SetObjectClass(args[2]);

                Service.Chat.SendMessage(message.Channel, "Successfully changed SCP-" + ScpObject.GetViewId(id) + "'s class to \"" + classChangeStatus + "\"!");
            }
            else if (args[1].Equals("description", StringComparison.OrdinalIgnoreCase))
            {
                scp.Description = args[2];

                Service.Chat.SendMessage(message.Channel, "Successfully changed SCP-" + ScpObject.GetViewId(id) + "'s description to \n```" + args[2] + "```");
            }
            else if (args[1].Equals("image", StringComparison.OrdinalIgnoreCase))
            {
                scp.Image = args[2];

                Service.Chat.SendMessage(message.Channel, "Successfully changed SCP-" + ScpObject.GetViewId(id) + "'s image URL to \n```" + args[2] + "```");
            }
            else if (args[1].Equals("video", StringComparison.OrdinalIgnoreCase))
            {
                scp.Video = args[2];

                Service.Chat.SendMessage(message.Channel, "Successfully changed SCP-" + ScpObject.GetViewId(id) + "'s video URL to \n```" + args[2] + "```");
            }
            else
            {
                Service.Chat.SendMessage(message.Channel, "Valid field types: \"Name\", \"Class\", or \"Description\".");

                return;
            }

            scp.EditTime = DateTime.Now;
            scp.Curated = true;
            scp.EditorName = message.User.Username + "#" + message.User.Discriminator;

            Service.Scp.SaveChanges();

            return;
        }
    }
}
