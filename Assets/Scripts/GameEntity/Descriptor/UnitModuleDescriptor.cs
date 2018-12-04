using UnityEngine;

namespace Klaesh.GameEntity.Descriptor
{
    [CreateAssetMenu(fileName = "New Unit Module", menuName = "Entity/Module/Unit")]
    public class UnitModuleDescriptor : ModuleDescriptorBase
    {
        [Header("Movement")]
        public int maxDistance;
        public int jumpHeight;
    }
}
