using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using LanDiscordBot.Bot;
using LanDiscordBot.Chat.Commands;

namespace LanDiscordBot.Chat
{
    public class ChatService
    {
        private readonly Dictionary<String, ChatCommand> _commands;

        private readonly BotService _bot;

        public EventHandler<ChatMessageArgs> OnRegularChatMessage = delegate { };

        public ChatService(BotService bot)
        {
            _bot = bot;

            _commands = new Dictionary<String, ChatCommand>(StringComparer.OrdinalIgnoreCase);
        }

        public void Initialize()
        {
            // Register commands
            RegisterCommand("uptime", new UptimeCommand(_bot));
            RegisterCommand("setnick", new SetNickCommand(_bot));
            RegisterCommand("help", new HelpCommand(_bot));
            RegisterCommand("version", new VersionCommand(_bot));
            RegisterCommand("say", new SayCommand(_bot));
            RegisterCommand("setstatus", new SetStatusCommand(_bot));

            _bot.Client.MessageReceived += ParseMessage;

            return;
        }

        public bool RegisterCommand(String name, ChatCommand command)
        {
            bool success = false;

            if (!_commands.ContainsKey(name))
            {
                _commands.Add(name, command);

                success = true;
            }

            return success;
        }

        public List<KeyValuePair<String, ChatCommand>> GetHelpListing(UInt64 discordId, int page)
        {
            List<KeyValuePair<String, ChatCommand>> commands = new List<KeyValuePair<String, ChatCommand>>();
            int skips = (page - 1) * 10;
            int skipCount = 0;
            int resultCount = 0;

            foreach (KeyValuePair<String, ChatCommand> command in _commands)
            {
                if (command.Value.ClientVisible(discordId))
                {
                    if (skipCount < skips)
                    {
                        skipCount++;
                    }
                    else
                    {
                        commands.Add(command);

                        resultCount++;

                        if (resultCount == 10)
                            break;
                    }
                }
            }

            return commands;
        }

        public void SendMessage(ISocketMessageChannel channel, String message)
        {
            channel.SendMessageAsync(message);

            return;
        }

        public async void SendDirectMessage(SocketUser user, String message)
        {
            if (user == null)
                return;

            var channel = await user.GetOrCreateDMChannelAsync();

            await channel.SendMessageAsync(message);

            return;
        }

        public Task ParseMessage(SocketMessage arg)
        {
            var message = (SocketUserMessage) arg;

            if (message != null && !message.Author.IsBot)
            {
                // Commands
                if (message.Content.IndexOf(_bot.Settings.ChatCommandPrefix, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    String cmd = message.Content.Substring(_bot.Settings.ChatCommandPrefix.Length - 1);

                    int cmdNameEnd = cmd.IndexOf(' ');
                    String cmdName = cmdNameEnd == -1 ? cmd.Substring(_bot.Settings.ChatCommandPrefix.Length) : cmd.Substring(_bot.Settings.ChatCommandPrefix.Length, cmdNameEnd-1);

                    if (_commands.ContainsKey(cmdName))
                    {
                        String args = cmdNameEnd == -1 ? "" : cmd.Substring(cmdNameEnd + 1);

                        _commands[cmdName].Execute(new ChatMessageArgs(message), args);

                    }
                    else if(cmdName.Length > 2)
                    {
                        SendMessage(arg.Channel, "I do not recognize that command " + arg.Author.Mention + ".");
                    }
                }
                else
                {
                    OnRegularChatMessage?.Invoke(this, new ChatMessageArgs(message));
                }
            }

            return Task.CompletedTask;
        }
    }

    public class ChatMessageArgs
    {
        public SocketUserMessage Data { get; }

        public SocketUser User => Data.Author;
        public ISocketMessageChannel Channel => Data.Channel;
        public String Message => Data.Content;

        public SocketGuild Server
        {
            get
            {
                SocketGuild server = null;

                if (Data.Channel is SocketGuildChannel channel)
                {
                    server = channel.Guild;
                }

                return server;
            }
        }

        public ChatMessageArgs(SocketUserMessage message)
        {
            Data = message;
        }
    }
}
