using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanDiscordBot.Bot;

namespace LanDiscordBot
{
    class Program
    {
        static void Main(string[] args)
            => StartBot().GetAwaiter().GetResult();


        public static async Task StartBot()
        {
            BotService service = new BotService();

            bool success = await service.Initialize();

            if(success)
                await Task.Delay(-1);
        }
    }
}
