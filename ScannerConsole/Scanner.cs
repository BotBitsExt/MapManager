using System;
using BotBits;
using MapManager;
using System.IO;
using System.Threading;
using MapManager.Events;

namespace ScannerConsole
{
    public class Scanner
    {
        private readonly BotBitsClient bot = new BotBitsClient();

        public static void Main()
        {
            // ReSharper disable once ObjectCreationAsStatement
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

                Login.Of(bot)
                    .WithEmail(email, password)
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
            Console.WriteLine($"Scanning: {e.Name} by {e.Creators} ({e.MapNumber}/{e.TotalMaps})");

            Console.Write("Accept? [y/N]");
            var response = Console.ReadKey();

            switch (response.Key)
            {
                case ConsoleKey.Y:
                    new MapReviewedEvent(ReviewResult.Accepted).RaiseIn(bot);
                    break;
                case ConsoleKey.S:
                    new MapReviewedEvent(ReviewResult.Stopped).RaiseIn(bot);
                    break;
                default:
                    new MapReviewedEvent(ReviewResult.Rejected).RaiseIn(bot);
                    break;
            }
        }

        [EventListener]
        private void OnScanResult(ScanResultEvent e)
        {
            Console.WriteLine($"{e.Message} ({e.AcceptedMapsCount}/{e.ScannedMapsCount})");
            AskForWorld();
        }
    }
}
