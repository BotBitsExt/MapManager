using System;

namespace MapLoader.SignFormat
{
    /// <summary>
    ///     <see cref="MapData" /> reader which accept text starting with
    ///     "SCAN:" followed by name of the map and optionally creators.
    /// </summary>
    /// <seealso cref="ISignFormat" />
    public class ScanSignFormat : ISignFormat
    {
        private const string ScanText = "scan:";

        public bool TryGetMapData(string signText, string worldOwner, out MapData mapData)
        {
            signText = signText.Trim();
            mapData = null;

            if (!signText.StartsWith(ScanText, StringComparison.OrdinalIgnoreCase))
                return false;

            signText = signText.Substring(ScanText.Length).Trim();
            var lowerText = signText.ToLower();

            // Try to find specified creators in text by searching for special text
            foreach (var byText in new[] {" by ", " by:"})
            {
                if (!lowerText.Contains(byText))
                    continue;

                var splitIndex = lowerText.LastIndexOf(byText, StringComparison.OrdinalIgnoreCase);

                // Name is before 'byText' and creators after
                mapData = new MapData(
                    signText.Substring(splitIndex).Trim(),
                    signText.Substring(splitIndex + byText.Length).Trim());

                return true;
            }

            mapData = new MapData(signText, worldOwner);

            return true;
        }
    }
}