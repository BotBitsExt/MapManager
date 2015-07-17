using BotBits;

namespace MapManager.Events
{
    /// <summary>
    /// Scan request event.
    /// Should be fired when user wants to scan maps from world.
    /// </summary>
    public sealed class ScanRequestEvent : Event<ScanRequestEvent>
    {
        public ScanRequestEvent(string targetWorldId)
        {
            TargetWorldId = targetWorldId;
        }

        /// <summary>
        /// Gets the identifier of world from which to scan maps.
        /// </summary>
        /// <value>The identifier of world from which to scan maps.</value>
        public string TargetWorldId { get; private set; }
    }
}

