using System;

namespace MapLoader
{
    public class SignData
    {
        public SignData(string text, string owner)
        {
            var lower = text.ToLower();

            if (lower.StartsWith("scan:"))
            {
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
            else
            {
                var split = text.Split(new []{"================"}, StringSplitOptions.None);
                Name = split[0].Trim();
                Creators = split[1].Trim();
            }
        }

        public string Name { get; private set; }
        public string Creators { get; private set; }
    }
}

