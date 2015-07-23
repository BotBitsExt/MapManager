using BotBits;

namespace MapManager
{
    public sealed class MapManagerExtension : Extension<MapManagerExtension>
    {
        protected override void Initialize(BotBitsClient client, object args)
        {
            var dimensions = (Point)args;
            MapManager.Of(client).MapWidth = dimensions.X;
            MapManager.Of(client).MapHeight = dimensions.Y;
        }

        public static void LoadInto(BotBitsClient client, int width, int height)
        {
            LoadInto(client, new Point(width, height));
        }
    }
}

