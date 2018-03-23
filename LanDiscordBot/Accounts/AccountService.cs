using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;
using LanDiscordBot.Chat.Commands;
using LanDiscordBot.Dao;
using Newtonsoft.Json;

namespace LanDiscordBot.Accounts
{
    public class AccountService
    {
        public Dictionary<UInt64, UserAccount> Accounts { get; }
        public Dictionary<String, UserRole> Roles { get; }

        private bool InitEnabled { get; set; }
        private String InitPassword { get; set; }
        private BotService _bot;

        public AccountService()
        {
            Accounts = new Dictionary<UInt64, UserAccount>();
            Roles = new Dictionary<String, UserRole>(StringComparer.Ordinal);

            InitEnabled = false;
            InitPassword = "";
        }

        public void Initialize(BotService bot)
        {
            _bot = bot;

            if (Accounts.Count < 1)
            {
                InitEnabled = true;
                InitPassword = GetRandomPassword(21);

                System.Console.WriteLine("[Accounting] No users detected!\nYou may create a root account by messaging the bot the following: \".initreg "+InitPassword+"\".");
            }

            // Register Commands
            _bot.Chat.RegisterCommand("initreg", new InitRegCommand(_bot));

            _bot.Chat.RegisterCommand("addflag", new AddFlagCommand(_bot));
            _bot.Chat.RegisterCommand("removeflag", new RemoveFlagCommand(_bot));
            _bot.Chat.RegisterCommand("addrole", new AddRoleCommand(_bot));
            // remove role
            _bot.Chat.RegisterCommand("giverole", new GiveRoleCommand(_bot));
            _bot.Chat.RegisterCommand("takerole", new TakeRoleCommand(_bot));

            return;
        }

        public UserAccount InitReg(UInt64 discordId, String password)
        {
            UserAccount account = null;

            if (InitEnabled && InitPassword.Equals(password))
            {
                account = CreateAccount(discordId);

                account.Root = true;

                InitEnabled = false;
                InitPassword = "";

                SaveChanges();
            }

            return account;
        }

        public String GetRandomPassword(int length)
        {
            const String chars =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "abcdefghijklmnopqrstuvwxyz" +
                "0123456789";

            var bytes = new byte[length * 8];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            var result = new char[length];

            for (int x = 0; x < length; x++)
            {
                ulong value = BitConverter.ToUInt64(bytes, x * 8);

                result[x] = chars[(int) (value % (uint) chars.Length)];
            }

            return new String(result);
        }

        public UserRole CreateRole(String name)
        {
            if (!Roles.ContainsKey(name))
            {
                UserRole role = new UserRole();

                role.Name = name;

                Roles.Add(name, role);
            }

            return Roles[name];
        }

        public UserRole GetRole(String name)
        {
            UserRole role = null;

            if (Roles.ContainsKey(name))
            {
                role = Roles[name];
            }

            return role;
        }

        public UserAccount CreateAccount(UInt64 discordId)
        {
            if (!Accounts.ContainsKey(discordId))
            {
                UserAccount account = new UserAccount();

                account.Id = discordId;

                Accounts.Add(discordId, account);
            }

            return Accounts[discordId];
        }

        public UserAccount GetAccount(UInt64 discordId)
        {
            UserAccount account = null;

            if (Accounts.ContainsKey(discordId))
            {
                account = Accounts[discordId];
            }

            return account;
        }

        public bool CheckAccess(UInt64 discordId, String flag)
        {
            UserAccount account = GetAccount(discordId);
            bool access = false;

            if (account != null)
            {
                access = account.CheckAccess(flag);
            }

            return access;
        }

        public void SaveChanges()
        {
            try
            {
                AccountDao.SaveAccounts(this);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }

            return;
        }
    }
}
