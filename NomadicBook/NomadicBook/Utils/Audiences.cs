using System;
using System.Collections.Generic;

namespace NomadicBook.Utils
{
    public class Audiences
    {
        private static IDictionary<string, string> Audience = new Dictionary<string, string>();

        public static string UpdateAudience(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }
            var audience = $"{name}_{DateTime.Now}";
            Audience[name] = audience;

            return audience;
        }

        public static bool IsNewestAudience(string audience)
        {
            if (string.IsNullOrWhiteSpace(audience))
            {
                return false;
            }
            int first = 0;
            var name = audience.Split('_')[first];

            if (!Audience.ContainsKey(name))
            {
                return false;
            }
            else 
            {
                return Audience[name] == audience;
            }
                
        }
    }
}
