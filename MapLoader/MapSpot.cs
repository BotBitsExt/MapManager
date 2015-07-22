using System;
using BotBits;

namespace MapLoader
{
    /// <summary>
    /// Spot for the map design.
    /// </summary>
    public class MapSpot
    {
        /// <summary>
        /// Gets the spot identifier.
        /// </summary>
        /// <value>The spot identifier.</value>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the start point.
        /// </summary>
        /// <value>The start point.</value>
        public Point StartPoint { get; private set; }

        /// <summary>
        /// Gets the position of the sign containing map information.
        /// </summary>
        /// <value>The sign point.</value>
        public Point SignPoint { get; private set; }

        /// <summary>
        /// Gets the map located in this spot.
        /// </summary>
        /// <value>The map.</value>
        public Map Map { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this spot contains map.
        /// </summary>
        /// <value><c>true</c> if this spot contains map; otherwise, <c>false</c>.</value>
        public bool HasMap
        {
            get { return Map != null; }
        }

        public MapSpot(int id, Blocks blocks)
        {
            Id = id;

            var x = 26*(id%11);
            var y = 17*(id/11);
            StartPoint = new Point(x + 3, y + 5);
            SignPoint = new Point(x + 13, y + 3);

            var block = blocks.At(SignPoint).Foreground.Block;
            if (block.Type != ForegroundType.Text) return;
            var split = block.Text.Split(new[] {"\\n"}, StringSplitOptions.None);
            var name = split[0];
            var creators = split[2];
            Map = new Map(blocks, StartPoint, name, creators);
        }

        public void AddMap(Blocks blocks, Map map)
        {
            Map = map;

            map.BuildAt(blocks, StartPoint);
            var text = string.Format("{0}\\n================\\n{1}", map.Name, map.Creators);
            blocks.Place(SignPoint.X, SignPoint.Y, Foreground.Sign.Block, text);
        }

        public void Clear(Blocks blocks)
        {
            Map = null;

            for (var x = 0; x < 22; x++)
            {
                for (var y = 0; y < 11; y++)
                {
                    blocks.Place(StartPoint.X + x, StartPoint.Y + y, Background.Empty);
                    blocks.Place(StartPoint.X + x, StartPoint.Y + y, Foreground.Empty);
                }
            }

            blocks.Place(SignPoint.X, SignPoint.Y, Foreground.Empty);
        }

        public void BuildBorder(Blocks blocks)
        {
            var startX = StartPoint.X - 2;
            var startY = StartPoint.Y - 4;

            Action<int, int, Foreground.Id> placeBlock
                = (x, y, block) => blocks.Place(startX + x, startY + y, block);

            for (var y = 0; y < 17; y++)
            {
                for (var x = 0; x < 26; x++)
                {
                    if (y == 0 || y == 1 || y == 16)
                    {
                        placeBlock(x, y, Foreground.Gravity.InvisibleDot);
                    }
                    else if (y == 2)
                    {
                        if (x < 2 || x > 23)
                            placeBlock(x, y, Foreground.Gravity.InvisibleDot);
                    }
                    else if (y == 3 || y == 15)
                    {
                        if (x == 0 || x == 25)
                            placeBlock(x, y, Foreground.Gravity.InvisibleDot);
                        else
                            placeBlock(x, y, Foreground.Basic.Black);
                    }
                    else if (y > 3 && y < 15)
                    {
                        if (x == 0 || x == 25)
                            placeBlock(x, y, Foreground.Gravity.InvisibleDot);
                        else if (x == 1 || x == 24)
                            placeBlock(x, y, Foreground.Basic.Black);
                    }
                }
            }
        }
    }
}

