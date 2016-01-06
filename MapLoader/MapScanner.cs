using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BotBits;
using BotBits.Events;
using JetBrains.Annotations;

namespace MapLoader
{
    /// <summary>
    ///     The maps scanner.
    /// </summary>
    public class MapScanner
    {
        public MapScanner(int mapWidth, int mapHeight)
        {
            MapWidth = mapWidth;
            MapHeight = mapHeight;
        }

        private int MapWidth { get; }
        private int MapHeight { get; }

        private List<Map> OnLoadMaps(BotBitsClient botBits)
        {
            var maps = new List<Map>();
            var blocks = Blocks.Of(botBits);

            for (var x = 1; x < blocks.Width - MapWidth; x++)
            {
                for (var y = 1; y < blocks.Height - MapHeight; y++)
                {
                    var block = blocks.At(x, y).Foreground.Block;

                    if (block.Type != ForegroundType.Text ||
                        !block.Text.ToLower().StartsWith("scan:") && !block.Text.Contains("================")) continue;
                    var data = new SignData(block.Text, Room.Of(botBits).Owner);

                    maps.Add(new Map(blocks, new Rectangle(x, y + 1, MapWidth, MapHeight), data.Name, data.Creators));
                }
            }

            return maps;
        }

        /// <summary>
        ///     Asynchronously load the maps from world with the specified <see cref="worldId"/>.
        /// </summary>
        /// <param name="worldId">World identifier.</param>
        /// <returns>The loaded maps.</returns>
        public async Task<List<Map>> LoadMapsAsync(string worldId)
        {
            var botBits = new BotBitsClient();

            await Login.Of(botBits)
                .AsGuestAsync()
                .CreateJoinRoomAsync(worldId);

            Console.WriteLine("Connected");

            var connectResult = new TaskCompletionSource<bool>();
            var cts = new CancellationTokenSource();

            // Wait for init or info event
            // Only one of them is sent depending on visibility of the world
            WaitForInfoEvent(botBits, connectResult, cts.Token);
            WaitForInitEvent(botBits, connectResult, cts.Token);

            await connectResult.Task;
            cts.Cancel();

            var maps = OnLoadMaps(botBits);

            botBits.Dispose();

            return maps;
        }

        /// <summary>
        ///     Loads the maps from world with the specified <see cref="worldId" />.
        /// </summary>
        /// <param name="worldId">World identifier.</param>
        /// <returns>The loaded maps.</returns>
        [UsedImplicitly]
        public List<Map> LoadMaps(string worldId)
        {
            return LoadMapsAsync(worldId).Result;
        }

        private async void WaitForInitEvent(BotBitsClient botBits, TaskCompletionSource<bool> result,
            CancellationToken token)
        {
            try
            {
                await InitEvent.Of(botBits).WaitOneAsync(token);
                result.SetResult(true);
            }
            catch (TaskCanceledException)
            {
            }
        }

        private async void WaitForInfoEvent(BotBitsClient botBits, TaskCompletionSource<bool> result,
            CancellationToken token)
        {
            try
            {
                var e = await InfoEvent.Of(botBits).WaitOneAsync(token);
                result.SetException(new MapLoadException(e.Title, e.Text));
            }
            catch (TaskCanceledException)
            {
            }
        }
    }
}