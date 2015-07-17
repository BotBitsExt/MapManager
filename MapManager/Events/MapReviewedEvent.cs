using BotBits;

namespace Bombot.Scanner.Events
{
    public sealed class MapReviewedEvent : Event<MapReviewedEvent>
    {
        public MapReviewedEvent(ReviewResult result)
        {
            Result = result;
        }

        public ReviewResult Result { get; private set; }
    }
}

