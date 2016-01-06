namespace MapLoader.SignFormat
{
    /// <summary>
    ///     Interface for <see cref="MapData" /> readers.
    /// </summary>
    public interface ISignFormat
    {
        bool TryGetMapData(string signText, string worldOwner, out MapData mapData);
    }
}