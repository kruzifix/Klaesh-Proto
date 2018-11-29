using UnityEngine;

namespace Klaesh.Entity
{
    [CreateAssetMenu(fileName = "New Entity", menuName = "Entity/Descriptor", order = 1)]
    public class GameEntityDescriptor : ScriptableObject
    {
        public string entityId = "entity1";
        public string entityName = "My Entity";

        [Header("Mesh")]
        public GameObject meshPrefab;
        public Vector3 meshOffset = Vector3.zero;

        [Header("Movement")]
        public int maxDistance;
        public int jumpHeight;
    }
}
