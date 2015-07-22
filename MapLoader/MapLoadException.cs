using System;

namespace MapLoader
{
    /// <summary>
    /// Map loading exception.
    /// Raised when loading of maps from a world failed.
    /// </summary>
    public sealed class MapLoadException : Exception
    {
        internal MapLoadException(string title, string text)
            : base(string.Format("{0} - {1}", title, text))
        {
        }
    }
}

