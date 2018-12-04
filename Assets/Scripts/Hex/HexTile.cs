using Klaesh.Entity;
using UnityEngine;

namespace Klaesh.Hex
{
    public class HexTile : MonoBehaviour
    {
        private const float heightScale = 0.5f;

        public GameObject model;

        public int Height { get; set; }
        public HexCubeCoord Position { get; set; }

        private MeshRenderer _renderer;

        public IGameEntity Entity { get; set; }
        public bool HasEntityOnTop { get { return Entity != null; } }

        private void Start()
        {
            _renderer = model.GetComponent<MeshRenderer>();
        }

        public void Refresh()
        {
            var scale = transform.localScale;
            scale.y = Height * heightScale;
            transform.localScale = scale;
        }

        public void SetColor(Color color)
        {
            _renderer.material.color = color;
        }

        public Vector3 GetTop()
        {
            return model.transform.position + new Vector3(0, Height * heightScale, 0);
        }
    }
}
