using Klaesh.GameEntity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Klaesh.Hex
{
    public class HexTile : MonoBehaviour//, IPointerEnterHandler
    {
        private const float heightScale = 0.5f;

        public GameObject model;

        public int Height { get; set; }
        public HexCubeCoord Position { get; set; }

        private MeshRenderer _renderer;

        public Entity Entity { get; set; }
        public bool HasEntityOnTop { get { return Entity != null; } }

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

        //public void OnPointerEnter(PointerEventData eventData)
        //{
        //    Debug.Log($"ENTER {name}");
        //}
    }
}
