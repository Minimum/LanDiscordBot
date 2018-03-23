using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LanDiscordBot.Accounts;
using LanDiscordBot.Chat;
using LanDiscordBot.Dao;
using LanDiscordBot.Scp;

namespace LanDiscordBot.Bot
{
    public class BotService
    {
        public const String Version = "1.0 (Build: 3-23-18)";

        public DiscordSocketClient Client { get; }

        public BotSettings Settings { get; private set; }

        private DateTime StartTime { get; }

        public AccountService Accounts { get; private set; }
        public ChatService Chat { get; }
        public ScpService Scp { get; }

        public long Uptime => (long) DateTime.UtcNow.Subtract(StartTime).TotalSeconds;

        public BotService()
        {
            Client = new DiscordSocketClient();

            StartTime = DateTime.UtcNow;

            Settings = null;

            // Bot services
            Accounts = null;
            Chat = new ChatService(this);
            Scp = new ScpService(this);
        }

        public async Task<bool> Initialize()
        {
            Console.WriteLine("Welcome to LanDiscordBot!\nBeginning initialization process...");

            // Load settings
            Console.WriteLine("Loading bot settings...");

            try
            {
                Settings = BotDao.LoadSettings();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (Settings == null)
            {
                Console.WriteLine("Invalid settings file.  Creating new settings file.");

                Settings = new BotSettings();

                BotDao.SaveSettings(Settings);
            }

            if (Settings.AuthToken.Length < 1)
            {
                Console.WriteLine("Invalid auth token!  Please set your bot's auth token in config/bot.json.");

                return false;
            }

            Console.WriteLine("Bot settings loaded.");

            // Login to Discord
            Console.WriteLine("Logging into Discord...");
            try
            {
                await Client.LoginAsync(TokenType.Bot, Settings.AuthToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (Client.LoginState != LoginState.LoggedIn)
            {
                Console.WriteLine("Login failure!  Please verify your auth token in config/bot.json.");

                return false;
            }

            Console.WriteLine("Login successful!");

            // Accounts init
            Console.WriteLine("Loading accounting...");

            try
            {
                Accounts = AccountDao.LoadAccounts();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (Accounts == null)
            {
                Console.WriteLine("Invalid accounting file.  Creating new accounting file.");

                if (Accounts == null)
                {
                    Accounts = new AccountService();
                }

                AccountDao.SaveAccounts(Accounts);
            }

            Accounts.Initialize(this);

            Console.WriteLine("Accounting loaded.");

            Scp.Initialize();
            Chat.Initialize();

            Console.WriteLine("Starting Discord client...");
            await Client.StartAsync();

            Console.WriteLine("Finishing up initialization...");
            await Client.SetGameAsync(Settings.PlayStatus);

            Console.WriteLine("Initialization complete!");

            return true;
        }
    }
}
