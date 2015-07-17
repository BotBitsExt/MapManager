using BotBits;

namespace Bombot.Scanner
{
    public class Map
    {
        public string Name { get; private set; }
        public string Creators { get; private set; }

        private BackgroundBlock[,] background;
        private ForegroundBlock[,] foreground;

        public Map(Blocks source, Point startPoint, string name, string creators)
            : this(source, startPoint.X, startPoint.Y, name, creators)
        {
        }

        public Map(Blocks source, int x, int y, string name, string creators)
        {
            Name = name;
            Creators = creators;

            ReadFrom(source, x, y);

            RemoveIllegalBlocks();
        }

        private void ReadFrom(Blocks source, int startX, int startY)
        {
            background = new BackgroundBlock[22, 11];
            foreground = new ForegroundBlock[22, 11];

            for (var x = 0; x < 22; x++)
            {
                for (var y = 0; y < 11; y++)
                {
                    var block = source.At(startX + x, startY + y);
                    background[x, y] = block.Background.Block;
                    foreground[x, y] = block.Foreground.Block;
                }
            }
        }

        public void BuildAt(Blocks blocks, Point target)
        {
            for (var x = 0; x < 22; x++)
            {
                for (var y = 0; y < 11; y++)
                {
                    blocks.Place(target.X + x, target.Y + y, background[x, y]);
                    blocks.Place(target.X + x, target.Y + y, foreground[x, y]);
                }
            }
        }

        private void RemoveIllegalBlocks()
        {
            for (var x = 0; x < 22; x++)
            {
                for (var y = 0; y < 11; y++)
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

