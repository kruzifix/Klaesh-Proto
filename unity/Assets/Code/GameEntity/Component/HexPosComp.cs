using Klaesh.Core;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.GameEntity.Component
{
    public class HexPosComp : MonoBehaviour
    {
        protected IHexMap _map;
        protected Entity _owner;

        public virtual HexCubeCoord Position { get; set; }

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

        private void OnDestroy()
        {
            var tile = _map.GetTile(Position);
            if (tile != null)
                tile.Entity = null;
        }
    }
}
