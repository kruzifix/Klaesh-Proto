﻿using System.Collections.Generic;
using System.Linq;
using Klaesh.Hex;
using Newtonsoft.Json;
using UnityEngine;

namespace Klaesh.Game.Config
{
    public interface ISquadConfiguration
    {
        int ServerId { get; }
        string Name { get; }
        Color Color { get; }

        IHexCoord Origin { get; }
        IList<IUnitConfiguration> Units { get; }
    }

    public class SquadConfiguration : ISquadConfiguration
    {
        [JsonProperty("id")]
        public int ServerId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("color")]
        public Color Color { get; set; }

        [JsonProperty("origin")]
        public HexOffsetCoord OffsetOrigin { get; set; }
        [JsonProperty("units")]
        public List<UnitConfiguration> UnitsConfig { get; set; }

        [JsonIgnore]
        public IHexCoord Origin => OffsetOrigin;
        [JsonIgnore]
        public IList<IUnitConfiguration> Units  => UnitsConfig.ToList<IUnitConfiguration>();
    }
}
