using Klaesh.Core;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.Entity
{
    public class Entity : MonoBehaviour
    {
        private EntityDescriptor _descriptor;
        private GameObject _mesh;

        private bool _firstMove = true;

        public HexCubeCoord Position { get; private set; }

        public bool MoveTo(HexCubeCoord position)
        {
            var map = ServiceLocator.Instance.GetService<HexMap>();
            var tile = map.GetTile(position.ToOffset());

            if (tile.HasEntityOnTop)
                return false;

            if (!_firstMove)
            {
                var oldTile = map.GetTile(Position.ToOffset());
                oldTile.Entity = null;
            }
            _firstMove = false;

            Position = position;
            tile.Entity = this;
            transform.position = tile.GetTop();

            return true;
        }

        public void Initialize(EntityDescriptor descriptor)
        {
            _descriptor = descriptor;

            CreateMesh();
        }

        private void CreateMesh()
        {
            if (_mesh != null)
            {
                Destroy(_mesh);

                var oldColliders = GetComponents<Collider>();
                foreach (var c in oldColliders)
                    Destroy(c);
            }

            _mesh = Instantiate(_descriptor.meshPrefab, transform);
            _mesh.transform.localPosition = _descriptor.meshOffset;

            MoveBoxColliders(_mesh, gameObject);
        }

        private void MoveBoxColliders(GameObject original, GameObject destination)
        {
            var comps = original.GetComponents<BoxCollider>();

            foreach (var c in comps)
            {
                var newComp = destination.AddComponent<BoxCollider>();
                newComp.center = c.center + _descriptor.meshOffset;
                newComp.size = c.size;
                newComp.material = c.material;
                newComp.isTrigger = c.isTrigger;
                Destroy(c);
            }
        }

        //public static void CopyAllComponents<T>(GameObject original, GameObject destination)
        //    where T : Component
        //{
        //    var comps = original.GetComponents<T>();

        //    foreach (var c in comps)
        //    {
        //        CopyComponent(c, destination);
        //    }
        //}

        //public static T CopyComponent<T>(T orig, GameObject dest)
        //    where T : Component
        //{
        //    var type = orig.GetType();
        //    var copy = dest.AddComponent(type);
        //    var fields = type.GetFields();
        //    foreach (var field in fields)
        //    {
        //        field.SetValue(copy, field.GetValue(orig));
        //    }
        //    return copy as T;
        //}
    }
}
