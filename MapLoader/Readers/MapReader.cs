using System;
using System.Linq;
using BotBits;
using MapLoader.Readers;
using System.Collections.Generic;

namespace MapLoader
{
    /// <summary>
    /// The maps reader. used to load maps from database world.
    /// </summary>
    public class MapReader : MapReaderBase
    {
        protected override List<Map> OnLoadMaps()
        {
            var spots = new List<MapSpot>();
            var blocks = Blocks.Of(BotBits);

            for (var i = 0; i < 187; i++)
                spots.Add(new MapSpot(i, blocks));

            return spots
                .Where(spot => spot.HasMap)
                .Select(spot => spot.Map)
                .ToList();
        }
    }
}

