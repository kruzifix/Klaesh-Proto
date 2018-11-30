using UnityEngine;

namespace Klaesh.Entity.Module
{
    public class MeshModule : IGameEntityModule
    {
        private GameObject _mesh;

        public IGameEntity Owner { get; set; }

        public void CreateMesh()
        {
            var go = (Owner as GameEntity).gameObject;

            if (_mesh != null)
            {
                Object.Destroy(_mesh);

                var oldColliders = go.GetComponents<Collider>();
                foreach (var c in oldColliders)
                    Object.Destroy(c);
            }

            _mesh = Object.Instantiate(Owner.Descriptor.meshPrefab, go.transform);
            _mesh.transform.localPosition = Owner.Descriptor.meshOffset;

            MoveBoxColliders(_mesh, go);
        }

        private void MoveBoxColliders(GameObject original, GameObject destination)
        {
            var comps = original.GetComponents<BoxCollider>();

            foreach (var c in comps)
            {
                var newComp = destination.AddComponent<BoxCollider>();
                newComp.center = c.center + Owner.Descriptor.meshOffset;
                newComp.size = c.size;
                newComp.material = c.material;
                newComp.isTrigger = c.isTrigger;
                Object.Destroy(c);
            }
        }
    }
}
