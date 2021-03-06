﻿using BotBits;

namespace MapManager.Events
{
    /// <summary>
    ///     Initialization complete event.
    ///     Raised when bot connects to the database world and finishes loading of map spots.
    /// </summary>
    public sealed class InitializationCompleteEvent : Event<InitializationCompleteEvent>
    {
        internal InitializationCompleteEvent()
        {
        }
    }
}