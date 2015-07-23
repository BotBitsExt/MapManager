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
        public MapReader(int mapWidth, int mapHeight)
            : base(mapWidth, mapHeight)
        {
        }

        protected override List<Map> OnLoadMaps()
        {
            var spots = new List<MapSpot>();
            var blocks = Blocks.Of(BotBits);

            var numberOfSpots = (blocks.Width / (MapWidth + 4)) * (blocks.Height / (MapHeight + 5));

            for (var i = 0; i < numberOfSpots; i++)
                spots.Add(new MapSpot(i, blocks, MapWidth, MapHeight));

            return spots
                .Where(spot => spot.HasMap)
                .Select(spot => spot.Map)
                .ToList();
        }
    }
}

