using BotBits;

namespace Bombot.Scanner.Events
{
    public sealed class ScanRequestEvent : Event<ScanRequestEvent>
    {
        public ScanRequestEvent(string targetWorldId)
        {
            TargetWorldId = targetWorldId;
        }

        public string TargetWorldId { get; private set; }
    }
}

