using BotBits;

namespace MapManager
{
    public sealed class MapManagerExtension : Extension<MapManagerExtension>
    {
        public static void LoadInto(BotBitsClient client)
        {
            LoadInto(client, null);
        }
    }
}

