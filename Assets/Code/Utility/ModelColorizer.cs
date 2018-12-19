using UnityEngine;

namespace Klaesh.Utility
{
    public class ModelColorizer : MonoBehaviour
    {
        public MeshRenderer[] meshes;

        public void Colorize(Color color)
        {
            foreach (var m in meshes)
            {
                m.material.color = color;
            }
        }
    }
}
