using BotBits;

namespace Bombot.Scanner.Events
{
    /// <summary>
    /// Map reviewed event.
    /// Should be fired when map has been sucessfully reviewed.
    /// </summary>
    public sealed class MapReviewedEvent : Event<MapReviewedEvent>
    {
        public MapReviewedEvent(ReviewResult result)
        {
            Result = result;
        }

        /// <summary>
        /// Gets the result of the review.
        /// </summary>
        /// <value>The result of the review.</value>
        public ReviewResult Result { get; private set; }
    }
}

