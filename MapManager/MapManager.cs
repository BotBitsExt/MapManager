using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotBits;
using BotBits.Events;
using BotBits.SendMessages;
using JetBrains.Annotations;
using MapLoader;
using MapLoader.SignFormat;
using MapManager.Events;

namespace MapManager
{
    public sealed class MapManager : EventListenerPackage<MapManager>
    {
        private readonly ISignFormat signFormat;

        /// <summary>
        ///     The review result.
        ///     Set when single map is reviewed.
        /// </summary>
        private TaskCompletionSource<ReviewResult> reviewResult;

        private bool waitingForResponse;

        public MapManager()
        {
            MapSpots = new List<MapSpot>();
            signFormat = new DividerSignFormat('=', 16);
        }

        /// <summary>
        ///     Gets the map spots.
        /// </summary>
        /// <value>The map spots.</value>
        [UsedImplicitly]
        public List<MapSpot> MapSpots { get; }

        [UsedImplicitly]
        public int MapWidth { get; set; }

        [UsedImplicitly]
        public int MapHeight { get; set; }

        /// <summary>
        ///     Gets the empty map spots.
        /// </summary>
        /// <value>The empty map spots.</value>
        [UsedImplicitly]
        public List<MapSpot> EmptyMapSpots => MapSpots.Where(spot => !spot.HasMap).ToList();

        /// <summary>
        ///     Gets the full map spots.
        /// </summary>
        /// <value>The full map spots.</value>
        [UsedImplicitly]
        public List<MapSpot> FullMapSpots => MapSpots.Where(spot => spot.HasMap).ToList();

        [EventListener]
        private void OnInit(InitEvent e)
        {
            var blocks = Blocks.Of(BotBits);

            var numberOfSpots = (blocks.Width - 2)/(MapWidth + 4)*(blocks.Height - 2)/(MapHeight + 4);

            for (var i = 0; i < numberOfSpots; i++)
            {
                MapSpots.Add(new MapSpot(i, blocks, MapWidth, MapHeight, signFormat));
            }

            new InitializationCompleteEvent().RaiseIn(BotBits);
        }

        [EventListener]
        private async void OnScanRequest(ScanRequestEvent e)
        {
            var spots = EmptyMapSpots;

            if (spots.Count == 0)
            {
                new ScanResultEvent(false, "No empty slots left.").RaiseIn(BotBits);
                return;
            }

            List<Map> maps;

            try
            {
                maps = await new MapScanner(MapWidth, MapHeight).LoadMapsAsync(e.TargetWorldId, new ScanSignFormat());
            }
            catch (MapLoadException ex)
            {
                new ScanResultEvent(false, ex.Message).RaiseIn(BotBits);
                return;
            }

            if (maps.Count == 0)
            {
                new ScanResultEvent(false, "No maps found.").RaiseIn(BotBits);
                return;
            }

            var blocks = Blocks.Of(BotBits);
            var spotNumber = 0;

            var numAccepted = 0;
            var numRejected = 0;

            var result = ReviewResult.Rejected;

            foreach (var map in maps)
            {
                spots[spotNumber].AddMap(blocks, map);

                await PlaceSendMessage.Of(BotBits).FinishQueueAsync();
                await BlockChecker.Of(BotBits).FinishChecksAsync();

                reviewResult = new TaskCompletionSource<ReviewResult>();
                waitingForResponse = true;
                new MapForReviewEvent(map.Name, map.Creators, numAccepted + numRejected + 1, maps.Count).RaiseIn(BotBits);

                result = await reviewResult.Task;

                waitingForResponse = false;

                switch (result)
                {
                    case ReviewResult.Accepted:
                    {
                        numAccepted++;
                        spotNumber++;

                        if (spotNumber < spots.Count) continue;

                        new ScanResultEvent(false,
                            "Scan not finished completely. Ran out of free spots.",
                            numAccepted,
                            numRejected).RaiseIn(BotBits);
                        return;
                    }

                    case ReviewResult.Rejected:
                    {
                        numRejected++;
                        break;
                    }

                    case ReviewResult.Stopped:
                    {
                        spots[spotNumber].Clear(blocks);

                        new ScanResultEvent(false,
                            "Scan stopped.",
                            numAccepted,
                            numRejected).RaiseIn(BotBits);
                        return;
                    }
                }
            }

            // Clear last map when rejected
            if (result == ReviewResult.Rejected)
                spots[spotNumber].Clear(blocks);

            new ScanResultEvent(true, "Scan succeeded.", numAccepted, numRejected).RaiseIn(BotBits);
        }

        [EventListener]
        private void OnMapReviewed(MapReviewedEvent e)
        {
            if (waitingForResponse)
                reviewResult.SetResult(e.Result);
        }

        [UsedImplicitly]
        public void ClearEmptySpots()
        {
            foreach (var spot in EmptyMapSpots)
                spot.Clear(Blocks.Of(BotBits));
        }

        [UsedImplicitly]
        public void BuildBorders()
        {
            foreach (var spot in MapSpots)
                spot.BuildBorder(Blocks.Of(BotBits));
        }
    }
}