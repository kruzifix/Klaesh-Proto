using System.Collections.Generic;
using Klaesh.Core;
using Klaesh.GameEntity;
using UnityEngine;

namespace Klaesh.Hex
{
    public enum HexTerrain
    {
        Plain,
        Forest
    }

    public class HexTile : MonoBehaviour
    {
        private const float heightScale = 0.5f;

        private static IHexMap _map;

        private MeshRenderer _renderer;
        private Dictionary<string, GameObject> _modifierObjects;

        private int _height;
        private HexTerrain _terrain;
        private Entity _entity;

        public GameObject model;
        public GameObject modifierGroup;

        public HexCubeCoord Position { get; set; }
        public int Height { get => _height; set { _height = value; Refresh(); _map.StateChanged(); } }

        public HexTerrain Terrain { get => _terrain; set { _terrain = value; Refresh(); _map.StateChanged(); } }

        public Entity Entity { get => _entity; set { _entity = value; _map.StateChanged(); } }
        public bool HasEntityOnTop { get { return Entity != null; } }

        private void Awake()
        {
            if (_map == null)
                _map = ServiceLocator.Instance.GetService<IHexMap>();

            _modifierObjects = new Dictionary<string, GameObject>();
            foreach (Transform modifier in modifierGroup.transform)
            {
                modifier.gameObject.SetActive(false);
                _modifierObjects.Add(modifier.name, modifier.gameObject);
            }
        }

        public void Refresh()
        {
            if (Terrain == HexTerrain.Forest)
                _modifierObjects["forest"].SetActive(true);

            var scale = transform.localScale;
            scale.y = Height * heightScale;
            transform.localScale = scale;

            var mpos = modifierGroup.transform.position;
            mpos.y = Height * heightScale;
            modifierGroup.transform.position = mpos;

            var mscale = modifierGroup.transform.localScale;
            mscale.y = 1f / scale.y;
            modifierGroup.transform.localScale = mscale;

            if (HasEntityOnTop)
                Entity.transform.position = GetTop();
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
