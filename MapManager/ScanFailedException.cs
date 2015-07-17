using System;

namespace MapManager
{
    /// <summary>
    /// Scan failed exception.
    /// </summary>
    internal sealed class ScanFailedException : Exception
    {
        public ScanFailedException(string title, string text)
            : base(string.Format("{0} - {1}", title, text))
        {
        }
    }
}

