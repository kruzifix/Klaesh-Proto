using System;
using System.Collections.Generic;
using System.Linq;
using Klaesh.Game.Job;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.Game.Cards
{
    [Serializable]
    public struct TerraTile
    {
        public Vector2Int coord;
        public int amount;
    }

    [CreateAssetMenu(fileName = "Terraform_", menuName = "Cards/Terraform Card")]
    public class TerraformCardData : ScriptableObject, ICardData
    {
        [Header("Card Data")]
        public string Name;
        public string Description;
        public Sprite Art;

        [Header("Terraform Data")]
        public TerraTile[] Tiles;

        string ICardData.Name => Name;
        string ICardData.Description => Description;
        public string Type => "Terraform";
        Sprite ICardData.Art => Art;

        public List<TerrainChange> GetTerrainChanges()
        {
            return Tiles.Select(t => new TerrainChange { coord = new HexOffsetCoord(t.coord.x, t.coord.y), amount = t.amount }).ToList();
        }
    }
}
