using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotBits;

namespace Bombot.Scanner.Events
{
    public sealed class MapForReviewEvent : Event<MapForReviewEvent>
    {
        internal MapForReviewEvent(string name, string creators, int mapNumber, int totalMaps)
        {
            this.Name = name;
            this.Creators = creators;
            this.MapNumber = mapNumber;
            this.TotalMaps = totalMaps;
        }

        public string Name { get; private set; }
        public string Creators { get; private set; }
        public int MapNumber { get; private set; }
        public int TotalMaps { get; private set; }
    }
}
