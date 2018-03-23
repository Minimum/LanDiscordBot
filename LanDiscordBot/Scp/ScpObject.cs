using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LanDiscordBot.Scp
{
    public class ScpObject
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public ScpObjectClass ObjectClass { get; set; }
        public String ObjectClassCustom { get; set; }

        public String Description { get; set; }

        [JsonIgnore] public String ViewId => GetViewId(Id);

        [JsonIgnore]
        public String ObjectClassName
        {
            get
            {
                switch (ObjectClass)
                {
                    default:
                    {
                        return "Unknown";
                    }

                    case ScpObjectClass.Safe:
                    {
                        return "Safe";
                    }

                    case ScpObjectClass.Euclid:
                    {
                        return "Euclid";
                    }

                    case ScpObjectClass.Keter:
                    {
                        return "Keter";
                    }

                    case ScpObjectClass.Custom:
                    {
                        return ObjectClassCustom;
                    }
                }
            }
        }

        public ScpObject()
        {
            Id = 0;
            Name = "";
            ObjectClass = ScpObjectClass.Unknown;
            ObjectClassCustom = "";

            Description = "";
        }

        public static String GetViewId(int id)
        {
            String viewId = id.ToString();

            if (id < 100)
            {
                viewId = "0" + viewId;

                if (id < 10)
                {
                    viewId = "0" + viewId;
                }
            }

            return viewId;
        }
    }

    public enum ScpObjectClass
    {
        Unknown,
        Safe,
        Euclid,
        Keter,
        Custom
    }
}
