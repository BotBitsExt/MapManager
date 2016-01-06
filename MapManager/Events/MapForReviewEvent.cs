using BotBits;

namespace MapManager.Events
{
    /// <summary>
    ///     Map for review event.
    ///     Raised when new map from the scan is waiting to be reviewed.
    /// </summary>
    public sealed class MapForReviewEvent : Event<MapForReviewEvent>
    {
        internal MapForReviewEvent(string name, string creators, int mapNumber, int totalMaps)
        {
            Name = name;
            Creators = creators;
            MapNumber = mapNumber;
            TotalMaps = totalMaps;
        }

        /// <summary>
        ///     Gets the name of the map.
        /// </summary>
        /// <value>The name of the map.</value>
        public string Name { get; }

        /// <summary>
        ///     Gets the creators of the map.
        /// </summary>
        /// <value>The creators of the map.</value>
        public string Creators { get; }

        /// <summary>
        ///     Gets the number of reviewed maps in current scan request.
        /// </summary>
        /// <value>The number of reviewed maps.</value>
        public int MapNumber { get; }

        /// <summary>
        ///     Gets the number of maps in current scan request.
        /// </summary>
        /// <value>The number of maps in current scan request.</value>
        public int TotalMaps { get; }
    }
}