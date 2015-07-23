using System.Collections.Generic;
using BotBits;

namespace MapLoader.Readers
{
    /// <summary>
    /// The maps scanner. Used to load map submissions.
    /// </summary>
    public class MapScanner : MapReaderBase
    {
        public MapScanner(int mapWidth, int mapHeight)
            : base(mapWidth, mapHeight)
        {
        }

        protected override List<Map> OnLoadMaps()
        {
            var maps = new List<Map>();
            var blocks = Blocks.Of(BotBits);

            for (var x = 1; x < blocks.Width - MapWidth; x++)
            {
                for (var y = 1; y < blocks.Height - MapHeight; y++)
                {
                    var block = blocks.At(x, y).Foreground.Block;

                    if (block.Type != ForegroundType.Text || !block.Text.ToLower().StartsWith("scan:")) continue;
                    var data = new SignData(block.Text.Substring(5).Trim(), Room.Of(BotBits).Owner);

                    maps.Add(new Map(blocks, new Rectangle(x, y + 1, MapWidth, MapHeight), data.Name, data.Creators));
                }
            }

            return maps;
        }
    }
}