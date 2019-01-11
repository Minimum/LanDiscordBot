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
        public String Image { get; set; }
        public String Video { get; set; }

        public String Description { get; set; }

        public bool Generated { get; set; }
        public bool Curated { get; set; }

        public DateTime EditTime { get; set; }
        public String EditorName { get; set; }

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
            Image = "";
            Video = "";

            Description = "";

            Generated = false;
            Curated = false;

            EditTime = DateTime.Now;
            EditorName = "";
        }

        public String SetObjectClass(String objectClass)
        {
            String classChangeStatus;

            if (objectClass.Equals("unknown", StringComparison.OrdinalIgnoreCase))
            {
                ObjectClass = ScpObjectClass.Unknown;

                classChangeStatus = "Unknown";
            }
            else if (objectClass.Equals("safe", StringComparison.OrdinalIgnoreCase))
            {
                ObjectClass = ScpObjectClass.Safe;

                classChangeStatus = "Safe";
            }
            else if (objectClass.Equals("euclid", StringComparison.OrdinalIgnoreCase))
            {
                ObjectClass = ScpObjectClass.Euclid;

                classChangeStatus = "Euclid";
            }
            else if (objectClass.Equals("keter", StringComparison.OrdinalIgnoreCase))
            {
                ObjectClass = ScpObjectClass.Keter;

                classChangeStatus = "Keter";
            }
            else
            {
                ObjectClass = ScpObjectClass.Custom;

                ObjectClassCustom = objectClass;

                classChangeStatus = objectClass;
            }

            return classChangeStatus;
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
