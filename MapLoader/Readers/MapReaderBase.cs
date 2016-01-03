using System;
using BotBits;
using System.Threading.Tasks;
using System.Collections.Generic;
using BotBits.Events;

namespace MapLoader.Readers
{
    /// <summary>
    /// Base for map readers.
    /// </summary>
    public abstract class MapReaderBase
    {
        protected BotBitsClient BotBits;

        protected int MapWidth { get; private set; }
        protected int MapHeight { get; private set; }

        public MapReaderBase(int mapWidth, int mapHeight)
        {
            MapWidth = mapWidth;
            MapHeight = mapHeight;
        }

        /// <summary>
        /// The connect result.
        /// Set after bot connects.
        /// </summary>
        private readonly TaskCompletionSource<bool> connectResult
            = new TaskCompletionSource<bool>();

        protected abstract List<Map> OnLoadMaps();

        /// <summary>
        /// Asynchronously load the maps from world with specified id.
        /// </summary>
        /// <returns>The loaded maps.</returns>
        /// <param name="worldId">World identifier.</param>
        public async Task<List<Map>> LoadMapsAsync(string worldId)
        {
            BotBits = new BotBitsClient();

            await Login.Of(BotBits)
                .AsGuestAsync()
                .CreateJoinRoomAsync(worldId);

            Console.WriteLine("Connected");

            WaitForInfoEvent();
            WaitForInitEvent();

            await connectResult.Task;

            var maps = OnLoadMaps();

            ConnectionManager.Of(BotBits).Connection.Disconnect();

            return maps;
        }

        private async void WaitForInitEvent()
        {
            await InitEvent.Of(BotBits).WaitOneAsync();
            connectResult.SetResult(true);
        }

        private async void WaitForInfoEvent()
        {
            var e = await InfoEvent.Of(BotBits).WaitOneAsync();
            connectResult.SetException(new MapLoadException(e.Title, e.Text));
        }

        /// <summary>
        /// Loads the maps from world with specified id.
        /// </summary>
        /// <returns>The loaded maps.</returns>
        /// <param name="worldId">World identifier.</param>
        public List<Map> LoadMaps(string worldId)
        {
            return LoadMapsAsync(worldId).Result;
        }
    }
}

