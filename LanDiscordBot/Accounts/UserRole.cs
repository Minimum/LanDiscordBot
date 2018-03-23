using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanDiscordBot.Accounts
{
    public class UserRole
    {
        public String Name { get; set; }

        public HashSet<String> Flags { get; }

        public UserRole()
        {
            Name = "";

            Flags = new HashSet<String>(StringComparer.OrdinalIgnoreCase);
        }

        public void AddFlag(String flag)
        {
            Flags.Add(flag);

            return;
        }

        public void RemoveFlag(String flag)
        {
            Flags.Remove(flag);

            return;
        }

        public bool HasFlag(String flag)
        {
            return Flags.Contains(flag);
        }
    }
}
