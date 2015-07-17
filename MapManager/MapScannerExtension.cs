using BotBits;

namespace Bombot.Scanner
{
    public sealed class MapScannerExtension : Extension<MapScannerExtension>
    {
        public static void LoadInto(BotBitsClient client)
        {
            LoadInto(client, null);
        }
    }
}

