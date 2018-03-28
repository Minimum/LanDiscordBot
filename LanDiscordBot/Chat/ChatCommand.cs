using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;

namespace LanDiscordBot.Chat
{
    public abstract class ChatCommand
    {
        protected BotService Service { get; }

        public String Description { get; protected set; }
        public String AccessFlag { get; protected set; }
        public bool ShowInHelp { get; protected set; }

        protected ChatCommand(BotService service)
        {
            Service = service;

            Description = "";
            ShowInHelp = true;
            AccessFlag = "";
        }

        public bool ClientVisible(UInt64 discordId)
        {
            return ShowInHelp && (AccessFlag.Length == 0 || Service.Accounts.CheckAccess(discordId, AccessFlag));
        }

        public static List<String> ParseArgs(String arguments)
        {
            List<String> args = new List<String>();
            int len = arguments.Length;
            bool escape = false;
            bool quote = false;
            String arg = "";

            for (int x = 0; x < len; x++)
            {
                char symbol = arguments[x];

                if (escape)
                {
                    arg += symbol;

                    escape = false;
                }
                else if (symbol == '\"' || symbol == '“' || symbol == '”')
                {
                    if (arg.Length > 0)
                    {
                        args.Add(arg);

                        arg = "";

                        quote = !quote;
                    }
                    else if (quote)
                    {
                        args.Add(arg);
                    }
                    else
                    {
                        quote = true;
                    }
                }
                else if (symbol == ' ' && !quote)
                {
                    if(arg.Length > 0)
                        args.Add(arg);

                    arg = "";
                }
                else if (symbol == '\\')
                {
                    escape = true;
                }
                else
                {
                    arg += symbol;
                }
            }

            if (arg.Length > 0)
            {
                args.Add(arg);
            }

            return args;
        }

        public abstract void Execute(ChatMessageArgs message, String args);
    }
}
