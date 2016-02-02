using BotBits;

namespace MapLoader
{
    public interface IPositionMapper
    {
        Rectangle GetMapRectangle(Point signPosition, int mapWidth, int mapHeight);
    }
}