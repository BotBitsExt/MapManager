using System.Collections.Generic;
using BotBits;

namespace MapLoader.Readers
{
    /// <summary>
    /// The maps scanner. Used to load map submissions.
    /// </summary>
    public class MapScanner : MapReaderBase
    {
        protected override List<Map> OnLoadMaps()
        {
            var maps = new List<Map>();
            var blocks = Blocks.Of(BotBits);

            for (var x = 1; x < blocks.Width - 22; x++)
            {
                for (var y = 1; y < blocks.Height - 11; y++)
                {
                    var block = blocks.At(x, y).Foreground.Block;

                    if (block.Type != ForegroundType.Text || !block.Text.ToLower().StartsWith("scan:")) continue;
                    var data = new SignData(block.Text.Substring(5).Trim(), Room.Of(BotBits).Owner);

                    maps.Add(new Map(blocks, x, y, data.Name, data.Creators));
                }
            }

            return maps;
        }
    }
}