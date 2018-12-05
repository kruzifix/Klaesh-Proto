using Klaesh.Game;
using Klaesh.Utility;
using UnityEngine;

namespace Klaesh.GameEntity.Module
{
    //public class MeshModule : IEntityModule
    //{
    //    private GameObject _mesh;

    //    private GameObject _meshPrefab;
    //    private Vector3 _meshOffset;

    //    public IEntity Owner { get; set; }

    //    public string Name { get { return _meshPrefab.name; } }
    //    public bool Enabled { get { return _mesh.activeSelf; } set { _mesh.SetActive(value); } }

    //    public MeshModule(GameObject meshPrefab, Vector3 meshOffset)
    //    {
    //        _meshPrefab = meshPrefab;
    //        _meshOffset = meshOffset;
    //    }

    //    public void Init()
    //    {
    //        CreateMesh();
    //    }

    //    private void CreateMesh()
    //    {
    //        var go = (Owner as Entity).gameObject;

    //        if (_mesh != null)
    //        {
    //            Object.Destroy(_mesh);

    //            var oldColliders = go.GetComponents<Collider>();
    //            foreach (var c in oldColliders)
    //                Object.Destroy(c);
    //        }

    //        _mesh = Object.Instantiate(_meshPrefab, go.transform);
    //        _mesh.transform.localPosition = _meshOffset;

    //        var squad = Owner.GetModule<ISquad>();
    //        if (squad != null)
    //        {
    //            var colorizer = _mesh.GetComponent<ModelColorizer>();
    //            colorizer?.Colorize(squad.Config.Color);
    //        }

    //        MoveBoxColliders(_mesh, go);
    //    }

    //    private void MoveBoxColliders(GameObject original, GameObject destination)
    //    {
    //        var comps = original.GetComponents<BoxCollider>();

    //        foreach (var c in comps)
    //        {
    //            var newComp = destination.AddComponent<BoxCollider>();
    //            newComp.center = c.center + _meshOffset;
    //            newComp.size = c.size;
    //            newComp.material = c.material;
    //            newComp.isTrigger = c.isTrigger;
    //            Object.Destroy(c);
    //        }
    //    }
    //}
}
