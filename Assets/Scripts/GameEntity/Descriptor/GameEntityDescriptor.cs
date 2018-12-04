using UnityEngine;

namespace Klaesh.GameEntity.Descriptor
{
    [CreateAssetMenu(fileName = "New Entity", menuName = "Entity/Descriptor", order = 1)]
    public class GameEntityDescriptor : ScriptableObject
    {
        public string type;

        public ModuleDescriptorBase[] modules;
    }
}
