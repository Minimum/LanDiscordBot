using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanDiscordBot.Accounts
{
    public class UserAccount
    {
        public UInt64 Id { get; set; }
        public String Name { get; set; }
        public bool Root { get; set; }

        public HashSet<UserRole> Roles { get; }

        public UserAccount()
        {
            Id = 0;
            Name = "";
            Root = false;

            Roles = new HashSet<UserRole>();
        }

        public bool CheckAccess(String flag)
        {
            bool access = Root;

            if (!access)
            {
                foreach (UserRole role in Roles)
                {
                    access = role.HasFlag(flag);

                    if (access)
                        break;
                }
            }

            return access;
        }
    }
}
