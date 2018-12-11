﻿using Klaesh.Hex;
using Newtonsoft.Json;

namespace Klaesh.Game.Data
{
    public class MoveUnitData
    {
        [JsonProperty("squad")]
        public int SquadId { get; set; }
        [JsonProperty("member")]
        public int MemberId { get; set; }
        [JsonProperty("target")]
        public HexCubeCoord Target { get; set; }
    }
}