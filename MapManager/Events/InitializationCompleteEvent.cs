using BotBits;

namespace Bombot.Scanner.Events
{
    /// <summary>
    /// Initialization complete event.
    /// Fired when bot connects to the database world and finishes loading of map spots.
    /// </summary>
    public sealed class InitializationCompleteEvent : Event<InitializationCompleteEvent>
    {
        internal InitializationCompleteEvent()
        {
        }
    }
}

