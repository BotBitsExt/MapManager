using BotBits;

namespace MapLoader
{
    /// <summary>
    /// Map class.
    /// </summary>
    public class Map
    {
        /// <summary>
        /// Gets the name of the map.
        /// </summary>
        /// <value>The name of the map.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the creators of the map.
        /// </summary>
        /// <value>The creators of the map.</value>
        public string Creators { get; private set; }

        public Rectangle Rect { get; private set; }

        private BackgroundBlock[,] background;
        private ForegroundBlock[,] foreground;

        public Map(Blocks source, Rectangle rect, string name, string creators)
        {
            Name = name;
            Creators = creators;
            Rect = rect;

            ReadFrom(source, rect.X, rect.Y);

            RemoveIllegalBlocks();
        }

        private void ReadFrom(Blocks source, int startX, int startY)
        {
            background = new BackgroundBlock[Rect.Width, Rect.Height];
            foreground = new ForegroundBlock[Rect.Width, Rect.Height];

            for (var x = 0; x < Rect.Width; x++)
            {
                for (var y = 0; y < Rect.Height; y++)
                {
                    var block = source.At(startX + x, startY + y);
                    background[x, y] = block.Background.Block;
                    foreground[x, y] = block.Foreground.Block;
                }
            }
        }

        /// <summary>
        /// Builds the map at specified target.
        /// </summary>
        /// <param name="blocks">Blocks.</param>
        /// <param name="target">Target.</param>
        public void BuildAt(Blocks blocks, Point target)
        {
            for (var x = 0; x < Rect.Width; x++)
            {
                for (var y = 0; y < Rect.Height; y++)
                {
                    blocks.Place(target.X + x, target.Y + y, background[x, y]);
                    blocks.Place(target.X + x, target.Y + y, foreground[x, y]);
                }
            }
        }

        private void RemoveIllegalBlocks()
        {
            for (var x = 0; x < Rect.Width; x++)
            {
                for (var y = 0; y < Rect.Height; y++)
                {
                    switch (foreground[x, y].Type)
                    {
                        case ForegroundType.Label:
                        case ForegroundType.Portal:
                        case ForegroundType.Text:
                        case ForegroundType.WorldPortal:
                            foreground[x, y] = new ForegroundBlock(Foreground.Empty);
                            continue;
                    }

                    switch (foreground[x, y].Id)
                    {
                        case Foreground.Tool.Checkpoint:
                        case Foreground.Tool.SpawnPoint:
                        case Foreground.Tool.Trophy:
                        case Foreground.Hazard.Spike:
                        case Foreground.Hologram.Block:
                        case Foreground.Cake.Block:
                        case Foreground.Effect.Curse:
                        case Foreground.Effect.Fly:
                        case Foreground.Effect.Jump:
                        case Foreground.Effect.Protection:
                        case Foreground.Effect.Speed:
                        case Foreground.Team.Door:
                        case Foreground.Team.Gate:
                        case Foreground.Team.Effect:
                        case Foreground.Zombie.Door:
                        case Foreground.Zombie.Effect:
                        case Foreground.Zombie.Gate:
                            foreground[x, y] = new ForegroundBlock(Foreground.Empty);
                            continue;
                    }
                }
            }
        }
    }
}

