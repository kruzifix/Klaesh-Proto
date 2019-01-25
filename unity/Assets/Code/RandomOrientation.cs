using UnityEngine;

namespace Klaesh
{
    public class RandomOrientation : MonoBehaviour
    {
        public bool randomX;
        public bool randomY;
        public bool randomZ;

        private void Start()
        {
            var origRot = transform.localRotation;

            var rot = Random.rotationUniform.eulerAngles;
            if (!randomX)
                rot.x = origRot.eulerAngles.x;
            if (!randomY)
                rot.y = origRot.eulerAngles.y;
            if (!randomZ)
                rot.z = origRot.eulerAngles.z;

            transform.localRotation = Quaternion.Euler(rot);
        }
    }
}
