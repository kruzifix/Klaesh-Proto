using System;
using Klaesh.Core;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.Entity
{
    public interface IGameEntity
    {
        int Id { get; }
        HexCubeCoord Position { get; }
        GameEntityDescriptor Descriptor { get; }

        void Initialize(int id, GameEntityDescriptor descriptor);

        bool TryMoveTo(HexCubeCoord position);
    }

    public class GameEntity : MonoBehaviour, IGameEntity
    {
        private GameObject _mesh;

        private bool _initialized = false;
        private bool _firstMove = true;

        public int Id { get; private set; }
        public HexCubeCoord Position { get; private set; }
        public GameEntityDescriptor Descriptor { get; private set; }

        public bool TryMoveTo(HexCubeCoord position)
        {
            if (!_initialized)
                throw new Exception("GameEntity needs to be initialized before moving");

            var map = ServiceLocator.Instance.GetService<IHexMap>();
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

        public void Initialize(int id, GameEntityDescriptor descriptor)
        {
            if (_initialized)
                throw new Exception("GameEntity already initialized");

            _initialized = true;
            Id = id;
            Descriptor = descriptor;

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

            _mesh = Instantiate(Descriptor.meshPrefab, transform);
            _mesh.transform.localPosition = Descriptor.meshOffset;

            MoveBoxColliders(_mesh, gameObject);
        }

        private void MoveBoxColliders(GameObject original, GameObject destination)
        {
            var comps = original.GetComponents<BoxCollider>();

            foreach (var c in comps)
            {
                var newComp = destination.AddComponent<BoxCollider>();
                newComp.center = c.center + Descriptor.meshOffset;
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
