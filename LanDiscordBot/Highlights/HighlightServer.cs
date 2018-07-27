using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanDiscordBot.Highlights
{
    public class HighlightServer
    {
        public UInt64 Server { get; set; }

        public int UniqueReactsRequired { get; set; }
        public HashSet<String> ReactsAllowed { get; set; }

        public UInt64 HighlightChannel { get; set; }
        //public HashSet<UInt64> ChannelWhitelist { get; set; }

        // <OriginalPostID, HighlightPostID>
        public Dictionary<UInt64, UInt64> Highlights { get; set; }

        public HighlightServer()
        {
            Server = 0;

            UniqueReactsRequired = 5;
            ReactsAllowed = new HashSet<String>();

            HighlightChannel = 0;
            //ChannelWhitelist = new HashSet<UInt64>();

            Highlights = new Dictionary<UInt64, UInt64>();
        }
    }
}
