using System;
using System.Collections.Generic;
using Klaesh.Core;
using Klaesh.Hex;
using Klaesh.Utility;
using UnityEngine;

namespace Klaesh.Game.Input
{
    public class HexPickState : BaseInputState
    {
        private HashSet<HexTile> _pickableTiles;
        private Func<HexTile, IInputState> _pickCallback;

        public Color HighlightColor { get; set; } = Colors.HighlightOrange;

        public HexPickState(IEnumerable<HexTile> pickableTiles, Func<HexTile, IInputState> pickCallback)
        {
            _pickableTiles = new HashSet<HexTile>(pickableTiles);
            _pickCallback = pickCallback;
        }

        public override void OnEnabled()
        {
            foreach (var tile in _pickableTiles)
            {
                tile.SetColor(HighlightColor);
            }
        }

        public override void OnDisabled()
        {
            var map = ServiceLocator.Instance.GetService<IHexMap>();
            map.DeselectAllTiles();
        }

        public override IInputState OnPickHexTile(HexTile tile)
        {
            if (_pickableTiles.Contains(tile))
            {
                return _pickCallback(tile);
            }

            return null;
        }
    }
}
