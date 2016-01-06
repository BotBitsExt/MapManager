using System;

namespace MapLoader.SignFormat
{
    /// <summary>
    ///     <see cref="MapData" /> reader which acept sign text in the following
    ///     format:
    ///     <code>
    ///   Name of the map
    ///   ===============
    ///   Creators
    ///   </code>
    ///     Where equals divider is replaced with <see cref="dividerChar" /> and
    ///     has length of <see cref="dividerLength" /> characters.
    /// </summary>
    /// <seealso cref="ISignFormat" />
    public class DividerSignFormat : ISignFormat
    {
        /// <summary>
        ///     The character used for divider.
        /// </summary>
        private readonly char dividerChar;

        /// <summary>
        ///     The length of the divider.
        /// </summary>
        private readonly int dividerLength;

        public DividerSignFormat(char dividerChar, int dividerLength)
        {
            this.dividerChar = dividerChar;
            this.dividerLength = dividerLength;
        }

        private string Divider => new string(dividerChar, dividerLength);

        public bool TryGetMapData(string signText, string worldOwner, out MapData mapData)
        {
            signText = signText.Trim();
            mapData = null;

            if (!signText.Contains(Divider))
                return false;

            var split = signText.Split(new[] {Divider}, StringSplitOptions.None);
            mapData = new MapData(split[0].Trim(), split[1].Trim());
            return false;
        }
    }
}