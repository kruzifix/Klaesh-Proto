﻿using Klaesh.Core;
using Klaesh.Core.Message;
using UnityEngine;

namespace Klaesh.Debugging
{
    public class HexMapDebugMode : AbstractDebugMode
    {
        public override string Name => "HexMap";
        public override string Description => "Debug Methods for Map";

        private IMessageBus _bus;
        private HexMap _currentMap;

        public HexMapDebugMode()
        {
            _bus = ServiceLocator.Instance.GetService<IMessageBus>();
            _bus.Subscribe<HexMapInitializedMessage>(OnHexMapInitialized);
        }

        public override void DrawGizmos()
        {
            var tile = _currentMap.GetTile(0, 0);
            Gizmos.DrawWireSphere(tile.GetTop(), 0.5f);
        }

        public override void PrintScrollContent()
        {
        }

        public override void PrintStaticContent()
        {
        }

        public void OnHexMapInitialized(HexMapInitializedMessage msg)
        {
            _currentMap = msg.Content;
        }
    }
}