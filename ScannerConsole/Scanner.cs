using System;
using BotBits;
using MapManager;
using System.IO;
using System.Threading;
using MapManager.Events;

namespace ScannerConsole
{
    class Scanner
    {
        private BotBitsClient bot = new BotBitsClient();

        public static void Main(string[] args)
        {
            new Scanner();
            Thread.Sleep(Timeout.Infinite);
        }

        public Scanner()
        {
            using (var r = new StreamReader("Login.txt"))
            {
                var email = r.ReadLine().Trim();
                var password = r.ReadLine().Trim();
                var worldId = r.ReadLine().Trim();
                var dimensions = r.ReadLine().Trim().Split('x');
                var width = int.Parse(dimensions[0]);
                var height = int.Parse(dimensions[1]);

                EventLoader.Of(bot).Load(this);
                MapManagerExtension.LoadInto(bot, width, height);

                ConnectionManager.Of(bot)
                    .EmailLogin(email, password)
                    .CreateJoinRoom(worldId);
            }
        }

        [EventListener]
        private void OnInit(InitializationCompleteEvent e)
        {
            AskForWorld();
        }

        private void AskForWorld()
        {
            Console.Write("Enter id of world to scan maps from: ");
            var r = Console.ReadLine().Trim();
            new ScanRequestEvent(r).RaiseIn(bot);
        }

        [EventListener]
        private void OnMap(MapForReviewEvent e)
        {
            Console.WriteLine("Scanning: {0} by {1} ({2}/{3})", e.Name, e.Creators, e.MapNumber, e.TotalMaps);

            Console.Write("Accept? [y/n]");
            var r = Console.ReadLine().Trim().ToLower();

            if (r == "y" || r == "yes")
                new MapReviewedEvent(ReviewResult.Accepted).RaiseIn(bot);
            else if (r == "n" || r == "no")
                new MapReviewedEvent(ReviewResult.Rejected).RaiseIn(bot);
            else if (r == "s" || r == "stop")
                new MapReviewedEvent(ReviewResult.Stopped).RaiseIn(bot);
        }

        [EventListener]
        private void OnScanResult(ScanResultEvent e)
        {
            Console.WriteLine("{0} ({1}/{2})", e.Message, e.AcceptedMapsCount, e.ScannedMapsCount);
            AskForWorld();
        }
    }
}
