using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using LanDiscordBot.Bot;

namespace LanDiscordBot.Chat.Commands
{
    public class UptimeCommand : ChatCommand
    {
        public UptimeCommand(BotService service)
            : base(service)
        {
            Description = "Gets the current uptime of the bot.";
        }

        public override void Execute(ChatMessageArgs message, string arguments)
        {
            String output = "";
            long uptime = Service.Uptime;
            bool firstValue = true;
            
            long value = uptime % 60;

            // Seconds
            if (value != 0)
            {
                output = (value == 1) ? " second" + output : " seconds" + output;
                output = " " + value + output;

                firstValue = false;
            }

            // Minutes
            value = (uptime % 3600) / 60;

            if (value != 0)
            {
                if (firstValue)
                {
                    firstValue = false;
                }
                else
                {
                    output = "," + output;
                }

                output = (value == 1) ? " minute" + output : " minutes" + output;
                output = " " + value + output;
            }

            // Hours
            value = (uptime % 86400) / 3600;

            if (value != 0)
            {
                if (firstValue)
                {
                    firstValue = false;
                }
                else
                {
                    output = "," + output;
                }

                output = (value == 1) ? " hour" + output : " hours" + output;
                output = " " + value + output;
            }

            // Days
            value = (uptime % 604800) / 86400;

            if (value != 0)
            {
                if (firstValue)
                {
                    firstValue = false;
                }
                else
                {
                    output = "," + output;
                }

                output = (value == 1) ? " day" + output : " days" + output;
                output = " " + value + output;
            }

            // Weeks
            value = (uptime % 31449600) / 604800;

            if (value != 0)
            {
                if (firstValue)
                {
                    firstValue = false;
                }
                else
                {
                    output = "," + output;
                }

                output = (value == 1) ? " week" + output : " weeks" + output;
                output = " " + value + output;
            }

            // Years
            value = uptime / 31449600;

            if (value != 0)
            {
                if (firstValue)
                {
                    firstValue = false;
                }
                else
                {
                    output = "," + output;
                }

                output = (value == 1) ? " year" + output : " years" + output;
                output = " " + value + output;
            }

            Service.Chat.SendMessage(message.Channel, message.User.Mention + " I have been online for" + output + ".");

            return;
        }
    }
}
