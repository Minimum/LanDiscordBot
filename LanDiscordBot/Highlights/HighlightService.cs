using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using LanDiscordBot.Bot;
using LanDiscordBot.Dao;
using LanDiscordBot.Highlights.Commands;

namespace LanDiscordBot.Highlights
{
    public class HighlightService
    {
        private BotService _bot;

        public Dictionary<UInt64, HighlightServer> Servers;

        private int _highlightCount;

        public HighlightService(BotService bot)
        {
            _bot = bot;

            Servers = null;

            _highlightCount = 0;
        }

        public void Initialize()
        {
            Console.WriteLine("Initializing Highlights service...");

            // Load servers
            try
            {
                Servers = HighlightDao.LoadServers();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (Servers == null)
            {
                Servers = new Dictionary<UInt64, HighlightServer>();

                HighlightDao.SaveServers(Servers);
            }

            // Hook chat
            _bot.Client.ReactionAdded += MessageChange;
            _bot.Client.ReactionRemoved += MessageChange;

            // Commands
            _bot.Chat.RegisterCommand("addhighlightserver", new AddHighlightServerCommand(_bot));
            _bot.Chat.RegisterCommand("addhighlightreact", new AddReactCommand(_bot));
            _bot.Chat.RegisterCommand("sethighlightchannel", new SetHighlightChannelCommand(_bot));

            Console.WriteLine("Highlights service initialization complete.");

            return;
        }

        public void SaveChanges()
        {
            try
            {
                HighlightDao.SaveServers(Servers);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return;
        }

        private async Task MessageChange(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel socketMessageChannel, SocketReaction arg3)
        {
            // Get message
            var message = await cacheable.GetOrDownloadAsync();

            // Check if message is valid
            if (message == null)
                return;

            // Check if channel is public and not a DM
            if (!(arg3.Channel is SocketGuildChannel channel))
                return;

            // Check if channel is in supported server
            if (!Servers.ContainsKey(channel.Guild.Id))
                return;

            // Get server info
            HighlightServer server = Servers[channel.Guild.Id];

            // Check if highlight channel is valid
            if (!(_bot.Client.GetChannel(server.HighlightChannel) is ISocketMessageChannel highlightChannel))
                return;

            // Check if channel is in whitelist
            //if(!server.ChannelWhitelist.Contains(channel.Id))
            //    return;

            // Get emote name
            String emote = "";

            if (arg3.Emote is Emoji)
            {
                Emoji emoteData = (Emoji) arg3.Emote;

                emote = emoteData.Name;
            }
            else if (arg3.Emote is Emote)
            {
                Emote emoteData = (Emote) arg3.Emote;

                emote = emoteData.Name + ":" + emoteData.Id;
            }

            // Check if emote is allowed
            if(!server.ReactsAllowed.Contains(emote))
                return;

            // Recalculate unique react total
            bool highlightPost = false;
            Dictionary<String, int> postReacts = new Dictionary<String, int>();

            foreach (KeyValuePair<IEmote, ReactionMetadata> react in message.Reactions)
            {
                String reactType = "";

                if (react.Key is Emoji)
                {
                    Emoji emoteData = (Emoji) react.Key;
                    
                    reactType = emoteData.Name;
                }
                else if (react.Key is Emote)
                {
                    Emote emoteData = (Emote) react.Key;

                    reactType = emoteData.Name + ":" + emoteData.Id;
                }

                if (server.ReactsAllowed.Contains(reactType))
                {
                    postReacts.Add(reactType, react.Value.ReactionCount);

                    highlightPost = highlightPost || react.Value.ReactionCount >= server.UniqueReactsRequired;
                }
            }

            // Check if there are enough reacts
            if (!highlightPost)
                return;

            // Format highlight message
            var embed = new EmbedBuilder()
                .WithColor(255, 255, 0)
                .WithAuthor(message.Author.Username)
                .WithDescription(message.Resolve())
                .WithTimestamp(message.Timestamp);

            String content = "#" + message.Channel.ToString() + "\n";

            foreach (KeyValuePair<String, int> react in postReacts)
            {
                content += react.Value + " " + react.Key + "  ";
            }

            bool modified = false;

            if (server.Highlights.ContainsKey(message.Id))
            {
                // Remove old message
                var oldMessage = await highlightChannel.GetMessageAsync(server.Highlights[message.Id]);
                var oldSocketMessage = oldMessage as SocketUserMessage;

                modified = oldSocketMessage != null;

                if (modified)
                {
                    await oldSocketMessage.ModifyAsync(m => { m.Content = content; });
                }
                else
                {
                    var delet = new List<IMessage> { oldMessage };

                    await highlightChannel.DeleteMessagesAsync(delet);

                    server.Highlights.Remove(message.Id);
                }
            }

            if (!modified)
            {
                // Create new message
                var highlightMessage = await highlightChannel.SendMessageAsync(content, false, embed);

                server.Highlights.Add(message.Id, highlightMessage.Id);

                // Save every 5 highlights
                // Note: It would probably be better to do a timer func inst
                _highlightCount++;

                if (_highlightCount % 5 == 0)
                    SaveChanges();
            }

            return;
        }
    }
}
