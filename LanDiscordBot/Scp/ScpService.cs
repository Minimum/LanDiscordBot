using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
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
                    ScpObject scp = null;

                    if (Scps.ContainsKey(num))
                    {
                        scp = Scps[num];

                        SendMessage(e, scp);
                    }
                    else
                    {
                        try
                        {
                            scp = GrabOnlineVersion(num);
                        }
                        catch (Exception exception)
                        {
                            _bot.WriteConsole("[SCP] Attempt to pull SCP-" + num + " from website has failed!\nDisplaying error info:\n" + exception);
                        }

                        if (scp != null)
                        {
                            Scps.Add(num, scp);

                            SaveChanges();

                            SendMessage(e, scp);
                        }
                        else
                        {
                            Random rand = new Random();
                            String response = UnknownResponses[rand.Next(UnknownResponses.Count)].Replace("%s", ScpObject.GetViewId(num)).Replace("%u", e.User.Mention);

                            _bot.Chat.SendMessage(e.Channel, response);
                        }
                    }
                }
            }

            return;
        }

        private async void SendMessage(ChatMessageArgs e, ScpObject scp)
        {
            Color scpColor;

            switch (scp.ObjectClass)
            {
                case ScpObjectClass.Safe:
                    {
                        scpColor = new Color(0, 255, 0);

                        break;
                    }

                case ScpObjectClass.Euclid:
                    {
                        scpColor = new Color(255, 255, 0);

                        break;
                    }

                case ScpObjectClass.Keter:
                    {
                        scpColor = new Color(255, 0, 0);

                        break;
                    }

                default:
                    {
                        scpColor = new Color(255, 255, 255);

                        break;
                    }
            }

            EmbedBuilder embed = new EmbedBuilder
            {
                Title = "SCP-" + scp.ViewId + " - " + scp.Name,
                Color = scpColor,
                Description = scp.Description,
                Url = "http://www.scp-wiki.net/scp-" + scp.ViewId,
                Timestamp = scp.EditTime
            };

            // Object Class
            EmbedFieldBuilder classField = new EmbedFieldBuilder
            {
                Name = "Object Class",
                Value = scp.ObjectClassName
            };
            embed.Fields.Add(classField);

            if (!String.IsNullOrWhiteSpace(scp.Video))
            {
                EmbedFieldBuilder videoField = new EmbedFieldBuilder
                {
                    Name = "Video (O5 ACCESS REQUIRED)",
                    Value = scp.Video
                };
                embed.Fields.Add(videoField);
            }

            // Footer
            EmbedFooterBuilder embedFooter = new EmbedFooterBuilder();
            if (scp.Generated && !scp.Curated)
            {
                embedFooter.Text = "This info was automatically pulled from wiki via the bot.";
            }
            else
            {
                embedFooter.Text = "This info was altered by " + scp.EditorName + ".";
            }
            embed.Footer = embedFooter;

            if (!String.IsNullOrWhiteSpace(scp.Image))
            {
                embed.ImageUrl = scp.Image;
            }

            await e.Channel.SendMessageAsync("",
                                             false,
                                             embed.Build());

            return;
        }

        private ScpObject GrabOnlineVersion(int id)
        {
            ScpObject scp = null;
            String page = "";

            WebClient client = new WebClient();

            _bot.WriteConsole("[SCP] Attempting to pull SCP-" + id + " from website.");

            page = client.DownloadString("http://www.scp-wiki.net/scp-" + ScpObject.GetViewId(id));

            var match = Regex.Match(page, "<strong>Object Class:*</strong>");
            int classStart = match.Index;

            if (classStart != -1)
            {
                scp = new ScpObject
                {
                    Id = id
                };

                classStart += 30;

                int classEnd = page.IndexOf("</p>", classStart, StringComparison.OrdinalIgnoreCase);

                if (classEnd != -1)
                {
                    String objectClass = page.Substring(classStart, classEnd - classStart).Trim();

                    int anomaly = objectClass.LastIndexOf('<');

                    if (anomaly != -1)
                    {
                        int anomalyStart = 0;

                        for (int x = anomaly; x >= 0; x--)
                        {
                            if (objectClass[x] == '>')
                            {
                                anomalyStart = x;

                                break;
                            }
                        }

                        objectClass = objectClass.Substring(anomalyStart + 1, anomaly - anomalyStart - 1).Trim();
                    }

                    scp.SetObjectClass(objectClass);
                }

                match = Regex.Match(page, "<strong>Description:*</strong>");
                int descStart = match.Index;

                if (descStart != -1)
                {
                    descStart += 29;

                    int descEnd = page.IndexOf("</p>", descStart,
                        StringComparison.OrdinalIgnoreCase);

                    var desc = descEnd != 1
                        ? page.Substring(descStart, descEnd - descStart).Trim().Replace("&quot;", "\"")
                            .Replace("&#160;", " ")
                        : "[REDACTED]";

                    scp.Description = Regex.Replace(desc, "<[^>]*>", "");
                }
                else
                {
                    scp.Description = "[REDACTED]";
                }

                scp.Generated = true;

                String series = "";

                if (id > 999)
                {
                    series = "-" + (id / 1000 + 1);
                }

                page = client.DownloadString("http://www.scp-wiki.net/scp-series" + series);

                int nameStart = page.IndexOf("SCP-" + scp.ViewId + "</a> -", StringComparison.OrdinalIgnoreCase);

                if (nameStart != -1)
                {
                    nameStart += 14;

                    int nameEnd = page.IndexOf("</li>", nameStart, StringComparison.OrdinalIgnoreCase);

                    if (nameEnd != -1)
                    {
                        String name = page.Substring(nameStart, nameEnd - nameStart).Trim();

                        int anomaly = name.LastIndexOf('<');

                        if (anomaly != -1)
                        {
                            int anomalyStart = 0;

                            for (int x = anomaly; x >= 0; x--)
                            {
                                if (name[x] == '>')
                                {
                                    anomalyStart = x;

                                    break;
                                }
                            }

                            name = name.Substring(anomalyStart + 1, anomaly - anomalyStart - 1).Trim();
                        }

                        scp.Name = name;
                    }
                    else
                    {
                        scp.Name = "[REDACTED]";
                    }
                }
                else
                {
                    scp.Name = "[REDACTED]";
                }
            }
            else
            {
                _bot.WriteConsole("[SCP] Couldn't detect object class for SCP-" + id + "!");
            }

            client.Dispose();

            return scp;
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
