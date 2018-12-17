using Klaesh.Core;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.GameEntity.Component
{
    public class HexPosComp : MonoBehaviour
    {
        private IHexMap _map;
        private Entity _owner;

        public HexCubeCoord Position { get; private set; }

        private void Awake()
        {
            _map = ServiceLocator.Instance.GetService<IHexMap>();
            _owner = GetComponent<Entity>();
        }

        public void SetPosition(IHexCoord position)
        {
            var tile = _map.GetTile(position);

            Position = position.CubeCoord;
            tile.Entity = _owner;
            transform.position = tile.GetTop();
        }
    }
}
