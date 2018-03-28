using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using LanDiscordBot.Bot;
using LanDiscordBot.Chat;
using LanDiscordBot.Dad.Commands;

namespace LanDiscordBot.Dad
{
    public class DadService
    {
        private BotService _bot;

        private bool _enabled;

        public DadService(BotService bot)
        {
            _bot = bot;

            _enabled = false;
        }

        public void Initialize()
        {
            _enabled = _bot.Settings.DadTalkEnabled;

            if (_enabled)
                _bot.Chat.OnRegularChatMessage += ChatMessageHook;

            // Commands
            _bot.Chat.RegisterCommand("adddadserver", new AddDadServerCommand(_bot));
            _bot.Chat.RegisterCommand("removedadserver", new RemoveDadServerCommand(_bot));
            _bot.Chat.RegisterCommand("enabledad", new EnableDadCommand(_bot));
            _bot.Chat.RegisterCommand("disabledad", new DisableDadCommand(_bot));
            _bot.Chat.RegisterCommand("setdadname", new SetDadNameCommand(_bot));

            return;
        }

        public bool EnableTalk()
        {
            if (!_enabled)
            {
                _bot.Settings.DadTalkEnabled = true;
                _enabled = true;

                _bot.Chat.OnRegularChatMessage += ChatMessageHook;

                return true;
            }

            return false;
        }

        public bool DisableTalk()
        {
            if (_enabled)
            {
                _bot.Settings.DadTalkEnabled = false;
                _enabled = false;

                _bot.Chat.OnRegularChatMessage -= ChatMessageHook;

                return true;
            }

            return false;
        }

        private void ChatMessageHook(object sender, ChatMessageArgs e)
        {
            SocketGuild server = e.Server;

            if (server != null && _bot.Settings.DadTalkServers.Contains(server.Id))
            {
                int messageStart = 0;

                if (e.Message.IndexOf("i'm ", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    messageStart = 4;
                }
                else if (e.Message.IndexOf("im ", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    messageStart = 3;
                }

                if (messageStart != 0)
                {
                    _bot.Chat.SendMessage(e.Channel, "Hi " + e.Message.Substring(messageStart) + ", I'm " + _bot.Settings.DadTalkName + "!");
                }
            }

            return;
        }
    }
}
