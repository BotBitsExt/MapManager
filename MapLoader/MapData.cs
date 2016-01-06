namespace MapLoader
{
    /// <summary>
    ///     The map data used to provide information about the map to <see cref="MapScanner" />.
    /// </summary>
    public class MapData
    {
        public MapData(string name, string creators)
        {
            Name = name;
            Creators = creators;
        }

        public string Name { get; }
        public string Creators { get; }
    }
}