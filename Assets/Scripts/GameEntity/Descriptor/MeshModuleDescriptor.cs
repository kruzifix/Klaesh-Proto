using UnityEngine;

namespace Klaesh.GameEntity.Descriptor
{
    [CreateAssetMenu(fileName = "New Mesh Module", menuName = "Entity/Module/Mesh")]
    public class MeshModuleDescriptor : ModuleDescriptorBase
    {
        public GameObject meshPrefab;
        public Vector3 meshOffset;
    }
}
