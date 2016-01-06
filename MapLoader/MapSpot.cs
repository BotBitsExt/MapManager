using System;
using BotBits;
using JetBrains.Annotations;
using MapLoader.SignFormat;

namespace MapLoader
{
    /// <summary>
    ///     Spot for the map design.
    /// </summary>
    public class MapSpot
    {
        public MapSpot(int id, Blocks blocks, int width, int height, ISignFormat signFormat)
        {
            Id = id;
            Width = width;
            Height = height;

            var spotWidth = Width + 4;
            var spotHeight = Height + 4;
            var mapsInRow = (blocks.Width - 2)/spotWidth;
            var x = spotWidth*(id%mapsInRow);
            var y = spotHeight*(id/mapsInRow);

            SignPoint = new Point(x + 2, y + 2);
            MapPoint = new Point(x + 3, y + 3);

            var block = blocks.At(SignPoint).Foreground.Block;
            if (block.Type != ForegroundType.Text)
                return;

            MapData mapData;
            if (!signFormat.TryGetMapData(block.Text, "", out mapData))
                return;
            Map = new Map(blocks, new Rectangle(MapPoint.X, MapPoint.Y, Width, Height), mapData.Name, mapData.Creators);
        }

        /// <summary>
        ///     Gets the spot identifier.
        /// </summary>
        /// <value>The spot identifier.</value>
        [UsedImplicitly]
        public int Id { get; }

        /// <summary>
        ///     Gets the point at which map is getting built.
        /// </summary>
        /// <value>The map point.</value>
        [UsedImplicitly]
        public Point MapPoint { get; }

        /// <summary>
        ///     Gets the position of the sign containing map information.
        /// </summary>
        /// <value>The sign point.</value>
        [UsedImplicitly]
        public Point SignPoint { get; }

        /// <summary>
        ///     Gets the map located in this spot.
        /// </summary>
        /// <value>The map.</value>
        [UsedImplicitly]
        public Map Map { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this spot contains a map.
        /// </summary>
        /// <value><c>true</c> if this spot contains a map; otherwise, <c>false</c>.</value>
        public bool HasMap => Map != null;

        /// <summary>
        ///     Gets the width of the map.
        /// </summary>
        /// <value>The width.</value>
        [UsedImplicitly]
        public int Width { get; }

        /// <summary>
        ///     Gets the height of the map.
        /// </summary>
        /// <value>The height.</value>
        [UsedImplicitly]
        public int Height { get; }

        /// <summary>
        ///     Adds the map.
        /// </summary>
        /// <param name="blocks">The blocks.</param>
        /// <param name="map">The map.</param>
        public void AddMap(Blocks blocks, Map map)
        {
            Map = map;

            map.BuildAt(blocks, MapPoint);
            var text = $"{map.Name}\\n================\\n{map.Creators}";
            blocks.Place(SignPoint.X, SignPoint.Y, Foreground.Sign.Block, text);
        }

        /// <summary>
        ///     Clears the spot.
        /// </summary>
        /// <param name="blocks">The blocks.</param>
        public void Clear(Blocks blocks)
        {
            Map = null;

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    blocks.Place(MapPoint.X + x, MapPoint.Y + y, Background.Empty);
                    blocks.Place(MapPoint.X + x, MapPoint.Y + y, Foreground.Empty);
                }
            }

            blocks.Place(SignPoint.X, SignPoint.Y, Foreground.Basic.Black);
        }

        /// <summary>
        ///     Builds the border.
        /// </summary>
        /// <param name="blocks">The blocks.</param>
        public void BuildBorder(Blocks blocks)
        {
            var startX = MapPoint.X - 2;
            var startY = MapPoint.Y - 2;

            Action<int, int, Foreground.Id> placeBlock
                = (x, y, block) =>
                {
                    blocks.Place(startX + x, startY + y, block);
                    blocks.Place(startX + x, startY + y, Background.Empty);
                };

            for (var y = 0; y < Height + 4; y++)
            {
                for (var x = 0; x < Width + 4; x++)
                {
                    // Do not remove map sign
                    if (x == 1 && y == 1 &&
                        blocks.At(startX + x, startY + y).Foreground.Block.Id == Foreground.Sign.Block)
                        continue;

                    if (x == 0 || x == Width + 3 || y == 0 || y == Height + 3)
                    {
                        placeBlock(x, y, Foreground.Gravity.InvisibleDot);
                    }
                    else if (x == 1 || x == Width + 2 || y == 1 || y == Height + 2)
                    {
                        placeBlock(x, y, Foreground.Basic.Black);
                    }
                }
            }
        }
    }
}