using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotBits;
using BotBits.Events;

namespace MapManager
{
    /// <summary>
    /// The guest connection.
    /// Used to connect to worlds and load map submissions.
    /// </summary>
    internal class GuestConnection : IDisposable
    {
        private BotBitsClient BotBits = new BotBitsClient();
        private LoginClient client;

        /// <summary>
        /// Value indicating whether this guest connection was already used to scan maps.
        /// </summary>
        private bool used = false;

        /// <summary>
        /// The scan result.
        /// Set after maps are scanned.
        /// </summary>
        private readonly TaskCompletionSource<List<Map>> scanResult = new TaskCompletionSource<List<Map>>();

        /// <summary>
        /// The login result.
        /// Set after guest logins.
        /// </summary>
        private readonly TaskCompletionSource<bool> loginResult = new TaskCompletionSource<bool>();

        public GuestConnection()
        {
            Login();
        }

        private async void Login()
        {
            BotBits = new BotBitsClient();
            EventLoader.Of(BotBits).Load(this);

            client = await ConnectionManager.Of(BotBits).GuestLoginAsync();

            loginResult.SetResult(true);
        }

        [EventListener]
        private void OnInfo(InfoEvent e)
        {
            scanResult.SetException(new ScanFailedException(e.Title, e.Text));
        }

        [EventListener]
        private void OnJoinComplete(JoinCompleteEvent e)
        {
            var scannedMaps = new List<Map>();
            var blocks = Blocks.Of(BotBits);

            for (var x = 1; x < blocks.Width - 22; x++)
            {
                for (var y = 1; y < blocks.Height - 11; y++)
                {
                    var block = blocks.At(x, y).Foreground.Block;

                    if (block.Type != ForegroundType.Text || !block.Text.ToLower().StartsWith("scan:")) continue;
                    var data = new SignData(block.Text.Substring(5).Trim(), Room.Of(BotBits).Owner);

                    scannedMaps.Add(new Map(blocks, x, y, data.Name, data.Creators));
                }
            }

            scanResult.SetResult(scannedMaps);
        }

        /// <summary>
        /// Scans the maps from world with specified id.
        /// </summary>
        /// <returns>The scanned maps.</returns>
        /// <param name="world">World identifier.</param>
        public async Task<List<Map>> ScanMapsFrom(string worldId)
        {
            if (used)
                throw new InvalidOperationException("Single guest connection can't be used multiple times.");
            used = true;

            await loginResult.Task;
            await client.CreateJoinRoomAsync(worldId);
            return await scanResult.Task;
        }

        public void Dispose()
        {
            if (client != null)
                ConnectionManager.Of(BotBits).Connection.Disconnect();
        }
    }
}