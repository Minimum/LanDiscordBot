using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using LanDiscordBot.Bot;
using LanDiscordBot.Chat;
using LanDiscordBot.Dao;
using LanDiscordBot.Scp.Commands;

namespace LanDiscordBot.Scp
{
    public class ScpService
    {
        private readonly BotService _bot;

        public Dictionary<int, ScpObject> Scps { get; private set; }

        public List<String> UnknownResponses { get; set; }

        public ScpService(BotService bot)
        {
            _bot = bot;

            Scps = null;
            UnknownResponses = null;
        }

        public void Initialize()
        {
            Console.WriteLine("Initializing SCP service...");

            // Load SCPs
            try
            {
                Scps = ScpDao.LoadScps();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (Scps == null)
            {
                Scps = new Dictionary<int, ScpObject>();

                Scps.Add(173, new ScpObject()
                {
                    Id = 173,
                    Name = "The Sculpture",
                    ObjectClass = ScpObjectClass.Euclid,
                    Description = "It is constructed from concrete and rebar with traces of Krylon brand spray paint. SCP-173 is animate and extremely hostile. The object cannot move while within a direct line of sight. Line of sight must not be broken at any time with SCP-173. Personnel assigned to enter container are instructed to alert one another before blinking. Object is reported to attack by snapping the neck at the base of the skull, or by strangulation. In the event of an attack, personnel are to observe Class 4 hazardous object containment procedures."
                });

                ScpDao.SaveScps(Scps);
            }

            // Load Unknown Responses
            try
            {
                UnknownResponses = ScpDao.LoadUnknownResponses();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (UnknownResponses == null)
            {
                UnknownResponses = new List<String>();

                UnknownResponses.Add("I do not know of a SCP-%s, %u, please try again.");

                ScpDao.SaveUnknownResponses(UnknownResponses);
            }

            // Hook chat
            _bot.Chat.OnRegularChatMessage += ChatMessageHook;

            // Register Commands
            _bot.Chat.RegisterCommand("addscp", new AddScpCommand(_bot));
            _bot.Chat.RegisterCommand("editscp", new EditScpCommand(_bot));

            Console.WriteLine("SCP service initialization complete.");

            return;
        }

        private void ChatMessageHook(object sender, ChatMessageArgs e)
        {
            String[] words = e.Message.Split(' ', '.', ',', '?', '!', ';', ':', '\'', '"');
            int argPos = 0;
            bool marvinFound = e.Data.HasMentionPrefix(_bot.Client.CurrentUser, ref argPos);
            
            // Find Marv or Marvin in message
            if (!marvinFound)
            {
                foreach (String word in words)
                {
                    if (word.Length == 4
                        && (word[0] == 'M' || word[0] == 'm')
                        && (word[1] == 'A' || word[1] == 'a')
                        && (word[2] == 'R' || word[2] == 'r')
                        && (word[3] == 'V' || word[3] == 'v')
                        || word.Length == 6
                        && (word[0] == 'M' || word[0] == 'm')
                        && (word[1] == 'A' || word[1] == 'a')
                        && (word[2] == 'R' || word[2] == 'r')
                        && (word[3] == 'V' || word[3] == 'v')
                        && (word[4] == 'I' || word[4] == 'i')
                        && (word[5] == 'N' || word[5] == 'n'))
                    {
                        marvinFound = true;

                        break;
                    }
                }
            }

            if (marvinFound)
            {
                int num = 0;
                bool numFound = false;

                // Find a number in message
                foreach (String word in words)
                {
                    int length = word.Length;

                    if (word.IndexOf("SCP", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (length > 3 && word[3] == '-'
                            && Int32.TryParse(word.Substring(4), out num))
                        {
                            numFound = true;

                            break;
                        }

                        if (Int32.TryParse(word.Substring(3), out num))
                        {
                            numFound = true;

                            break;
                        }
                    }
                    else if(Int32.TryParse(word, out num))
                    {
                        numFound = true;

                        break;
                    }
                }

                if (numFound)
                {
                    if (Scps.ContainsKey(num))
                    {
                        ScpObject scp = Scps[num];

                        _bot.Chat.SendMessage(e.Channel,
                            "**Item #:** SCP-" + scp.ViewId + " - " + scp.Name + "\n\n"
                            + "**Object Class:** " + scp.ObjectClassName + "\n\n"
                            + "**Briefing:**\n```" + scp.Description + "```\n"
                            + "**Wiki Link:** http://www.scp-wiki.net/scp-" + scp.ViewId);
                    }
                    else
                    {
                        Random rand = new Random();
                        String response = UnknownResponses[rand.Next(UnknownResponses.Count)].Replace("%s", ScpObject.GetViewId(num)).Replace("%u", e.User.Mention);

                        _bot.Chat.SendMessage(e.Channel, response);
                    }
                }
            }

            return;
        }

        public void SaveChanges()
        {
            try
            {
                ScpDao.SaveScps(Scps);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
        }
    }
}
