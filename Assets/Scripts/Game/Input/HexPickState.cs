﻿using System;
using System.Collections.Generic;
using Klaesh.Core;
using Klaesh.Hex;
using Klaesh.Utility;
using UnityEngine;

namespace Klaesh.Game.Input
{
    public class HexPickState : AbstractInputState
    {
        private HashSet<HexTile> _pickableTiles;
        private Action<HexTile> _pickCallback;
        private Action _otherCallback;

        public Color HighlightColor { get; set; } = Colors.HighlightOrange;

        public HexPickState(InputStateMachine context, IEnumerable<HexTile> pickableTiles, Action<HexTile> pickCallback = null, Action otherCallback = null)
            : base(context)
        {
            _pickableTiles = new HashSet<HexTile>(pickableTiles);
            _pickCallback = pickCallback;
            _otherCallback = otherCallback;
        }

        public override void Enter()
        {
            foreach (var tile in _pickableTiles)
            {
                tile.SetColor(HighlightColor);
            }
        }

        public override void Exit()
        {
            var map = ServiceLocator.Instance.GetService<IHexMap>();
            map.DeselectAllTiles();
        }

        public override void ProcessHexTile(HexTile tile)
        {
            if (_pickableTiles.Contains(tile))
            {
                _pickCallback?.Invoke(tile);
            }
            else
            {
                _otherCallback?.Invoke();
            }
        }
    }
}
