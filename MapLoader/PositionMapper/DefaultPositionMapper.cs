using BotBits;

namespace MapLoader
{
    public class DefaultPositionMapper : IPositionMapper
    {
        public Rectangle GetMapRectangle(Point signPosition, int mapWidth, int mapHeight)
        {
            return new Rectangle(signPosition.X + 1, signPosition.Y + 1, mapWidth, mapHeight);
        }
    }
}