using System;

namespace MapManager
{
    public class SignData
    {
        public SignData(string text, string owner)
        {
            var lower = text.ToLower();

            if (lower.Contains(" by "))
            {
                var i = lower.LastIndexOf(" by ", StringComparison.Ordinal);

                Name = text.Substring(0, i).Trim();
                Creators = text.Substring(i + 4).Trim();
            }
            else if (lower.Contains("by:"))
            {
                var i = lower.LastIndexOf("by:", StringComparison.Ordinal);

                Name = text.Substring(0, i).Trim();
                Creators = text.Substring(i + 3).Trim();
            }
            else
            {
                Name = text;
                Creators = owner;
            }
        }

        public string Name { get; private set; }
        public string Creators { get; private set; }
    }
}

