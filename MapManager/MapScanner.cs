using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bombot.Scanner.Events;
using BotBits;
using BotBits.Events;

namespace Bombot.Scanner
{
    public sealed class MapScanner : EventListenerPackage<MapScanner>
    {
        /// <summary>
        /// Gets the map spots.
        /// </summary>
        /// <value>The map spots.</value>
        public List<MapSpot> MapSpots { get; private set; }

        /// <summary>
        /// The review result.
        /// Set when single map is reviewed.
        /// </summary>
        private TaskCompletionSource<ReviewResult> reviewResult;

        private bool waitingForResponse = false;

        public MapScanner()
        {
            MapSpots = new List<MapSpot>();
        }

        [EventListener]
        private void OnInit(InitEvent e)
        {
            var blocks = Blocks.Of(BotBits);
            for (var i = 0; i < 187; i++)
            {
                MapSpots.Add(new MapSpot(i, blocks));
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
                using (var guest = new GuestConnection())
                {
                    maps = await guest.ScanMapsFrom(e.TargetWorldId);
                }
            }
            catch (ScanFailedException ex)
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
                            "Scan not finished completly. Ran out of free spots.",
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

        /// <summary>
        /// Gets the empty map spots.
        /// </summary>
        /// <value>The empty map spots.</value>
        public List<MapSpot> EmptyMapSpots
        {
            get { return MapSpots.Where(spot => !spot.HasMap).ToList(); }
        }

        /// <summary>
        /// Gets the full map spots.
        /// </summary>
        /// <value>The full map spots.</value>
        public List<MapSpot> FullMapSpots
        {
            get { return MapSpots.Where(spot => spot.HasMap).ToList(); }
        }

        public void ClearEmptySpots()
        {
            foreach (var spot in EmptyMapSpots)
                spot.Clear(Blocks.Of(BotBits));
        }

        public void BuildBorders()
        {
            foreach (var spot in MapSpots)
                spot.BuildBorder(Blocks.Of(BotBits));
        }
    }
}

