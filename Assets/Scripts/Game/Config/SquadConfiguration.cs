using System.Collections.Generic;
using System.Linq;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.Game.Config
{
    public interface ISquadConfiguration
    {
        string Name { get; }
        Color Color { get; }

        IHexCoord Origin { get; }
        IList<IUnitConfiguration> Units { get; }
    }

    public class SquadConfiguration : ISquadConfiguration
    {
        public string Name { get; set; }
        public Color Color { get; set; }

        public HexOffsetCoord OffsetOrigin { get; set; }
        public List<UnitConfiguration> UnitsConfig { get; set; }

        public IHexCoord Origin => OffsetOrigin;
        public IList<IUnitConfiguration> Units  => UnitsConfig.ToList<IUnitConfiguration>();
    }
}
