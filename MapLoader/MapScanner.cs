using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BotBits;
using BotBits.Events;
using JetBrains.Annotations;
using MapLoader.SignFormat;

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

        private List<Map> OnLoadMaps(BotBitsClient botBits, ISignFormat signFormat, IPositionMapper positionMapper)
        {
            var maps = new List<Map>();
            var blocks = Blocks.Of(botBits);

            for (var x = 1; x < blocks.Width - MapWidth; x++)
            {
                for (var y = 1; y < blocks.Height - MapHeight; y++)
                {
                    var block = blocks.At(x, y).Foreground.Block;

                    if (block.Type != ForegroundType.Sign)
                        continue;

                    MapData mapData;
                    if (!signFormat.TryGetMapData(block.Text, Room.Of(botBits).Owner, out mapData))
                        continue;

                    maps.Add(new Map(blocks, positionMapper.GetMapRectangle(new Point(x, y), MapWidth, MapHeight),
                        mapData.Name, mapData.Creators));
                }
            }

            return maps;
        }

        /// <summary>
        ///     Asynchronously loads maps from world with the specified <see cref="worldId" />.
        /// </summary>
        /// <param name="worldId">World identifier.</param>
        /// <param name="signFormat"><see cref="MapData" /> reader.</param>
        /// <param name="positionMapper">The position mapper used to get map rectangle.</param>
        /// <returns>The loaded maps.</returns>
        public async Task<List<Map>> LoadMapsAsync(string worldId, ISignFormat signFormat, IPositionMapper positionMapper)
        {
            var botBits = new BotBitsClient();

            await Login.Of(botBits).AsGuestAsync().CreateJoinRoomAsync(worldId);
            Console.WriteLine("Connected");

            var connectResult = new TaskCompletionSource<bool>();
            var cts = new CancellationTokenSource();

            // Wait for init or info event
            // Only one of them is sent depending on visibility of the world
            WaitForInfoEvent(botBits, connectResult, cts.Token);
            WaitForInitEvent(botBits, connectResult, cts.Token);

            await connectResult.Task;
            cts.Cancel();

            var maps = OnLoadMaps(botBits, signFormat, positionMapper);

            botBits.Dispose();

            return maps;
        }

        /// <summary>
        ///     Loads maps from world with the specified <see cref="worldId" />.
        /// </summary>
        /// <param name="worldId">World identifier.</param>
        /// <param name="signFormat"><see cref="MapData" /> reader.</param>
        /// <param name="positionMapper">The position mapper used to get map rectangle.</param>
        /// <returns>The loaded maps.</returns>
        [UsedImplicitly]
        public List<Map> LoadMaps(string worldId, ISignFormat signFormat, IPositionMapper positionMapper)
        {
            return LoadMapsAsync(worldId, signFormat, positionMapper).Result;
        }

        private static async void WaitForInitEvent(BotBitsClient botBits, TaskCompletionSource<bool> result,
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

        private static async void WaitForInfoEvent(BotBitsClient botBits, TaskCompletionSource<bool> result,
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