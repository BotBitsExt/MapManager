using BotBits;
using JetBrains.Annotations;

namespace MapLoader
{
    /// <summary>
    ///     Map class.
    /// </summary>
    public class Map
    {
        private BackgroundBlock[,] background;
        private ForegroundBlock[,] foreground;

        public Map(Blocks source, Rectangle area, string name, string creators)
        {
            Name = name;
            Creators = creators;
            Area = area;

            ReadFrom(source, area.X, area.Y);
        }

        /// <summary>
        ///     Gets the name of the map.
        /// </summary>
        /// <value>The name of the map.</value>
        public string Name { get; }

        /// <summary>
        ///     Gets the creators of the map.
        /// </summary>
        /// <value>The creators of the map.</value>
        public string Creators { get; }

        /// <summary>
        ///     Gets the area of the map.
        /// </summary>
        /// <value>The area of the map.</value>
        [UsedImplicitly]
        public Rectangle Area { get; }

        private void ReadFrom(Blocks source, int startX, int startY)
        {
            background = new BackgroundBlock[Area.Width, Area.Height];
            foreground = new ForegroundBlock[Area.Width, Area.Height];

            for (var x = 0; x < Area.Width; x++)
            {
                for (var y = 0; y < Area.Height; y++)
                {
                    var block = source.At(startX + x, startY + y);
                    background[x, y] = block.Background.Block;
                    foreground[x, y] = block.Foreground.Block;
                }
            }
        }

        /// <summary>
        ///     Builds the map at the specified target.
        /// </summary>
        /// <param name="blocks">Blocks.</param>
        /// <param name="target">Target.</param>
        public void BuildAt(Blocks blocks, Point target)
        {
            for (var x = 0; x < Area.Width; x++)
            {
                for (var y = 0; y < Area.Height; y++)
                {
                    blocks.Place(target.X + x, target.Y + y, background[x, y]);
                    blocks.Place(target.X + x, target.Y + y, foreground[x, y]);
                }
            }
        }
    }
}