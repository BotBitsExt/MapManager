using BotBits;

namespace MapManager.Events
{
    /// <summary>
    ///     Map reviewed event.
    ///     Should be raised when map has been reviewed.
    /// </summary>
    public sealed class MapReviewedEvent : Event<MapReviewedEvent>
    {
        public MapReviewedEvent(ReviewResult result)
        {
            Result = result;
        }

        /// <summary>
        ///     Gets the result of the review.
        /// </summary>
        /// <value>The result of the review.</value>
        public ReviewResult Result { get; private set; }
    }
}