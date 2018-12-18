using Klaesh.Core;
using Klaesh.GameEntity;
using UnityEngine;

namespace Klaesh.Hex
{
    public class HexTile : MonoBehaviour
    {
        private const float heightScale = 0.5f;

        private static IHexMap _map;

        private int _height;
        private Entity _entity;

        public GameObject model;

        public int Height { get => _height; set { _height = value; _map.StateChanged(); } }
        public HexCubeCoord Position { get; set; }

        private MeshRenderer _renderer;

        public Entity Entity { get => _entity; set { _entity = value; _map.StateChanged(); } }
        public bool HasEntityOnTop { get { return Entity != null; } }

        private void Awake()
        {
            if (_map == null)
                _map = ServiceLocator.Instance.GetService<IHexMap>();
        }

        public void Refresh()
        {
            var scale = transform.localScale;
            scale.y = Height * heightScale;
            transform.localScale = scale;
        }

        public void SetColor(Color color)
        {
            if (_renderer == null)
                _renderer = model.GetComponent<MeshRenderer>();
            _renderer.material.color = color;
        }

        public Vector3 GetTop()
        {
            return model.transform.position + new Vector3(0, Height * heightScale, 0);
        }
    }
}
