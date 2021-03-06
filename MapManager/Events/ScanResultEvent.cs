﻿using BotBits;

namespace MapManager.Events
{
    /// <summary>
    ///     Scan result event.
    ///     Raised when scan is finished.
    /// </summary>
    public sealed class ScanResultEvent : Event<ScanResultEvent>
    {
        internal ScanResultEvent(bool result, string message, int numAccepted = 0, int numRejected = 0)
        {
            Result = result;
            Message = message;
            AcceptedMapsCount = numAccepted;
            RejectedMapsCount = numRejected;
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="ScanResultEvent" /> succeeded.
        /// </summary>
        /// <value><c>true</c> if succeeded; otherwise, <c>false</c>.</value>
        public bool Result { get; }

        /// <summary>
        ///     Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; }

        /// <summary>
        ///     Gets the amount of accepted maps.
        /// </summary>
        /// <value>The amount of accepted maps.</value>
        public int AcceptedMapsCount { get; }

        /// <summary>
        ///     Gets the amount of rejected maps.
        /// </summary>
        /// <value>The amount of rejected maps.</value>
        public int RejectedMapsCount { get; }

        /// <summary>
        ///     Gets the total amount of scanned maps.
        /// </summary>
        /// <value>The amount of scanned maps.</value>
        public int ScannedMapsCount => AcceptedMapsCount + RejectedMapsCount;
    }
}