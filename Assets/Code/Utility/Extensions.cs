using System.Collections.Generic;
using UnityEngine;

namespace Klaesh.Utility
{
    public static class Extensions
    {
        public static void DestroyAllChildrenImmediate(this Transform transform)
        {
            var children = new List<GameObject>();
            foreach (Transform t in transform)
                children.Add(t.gameObject);
            children.ForEach(c => GameObject.DestroyImmediate(c));
        }
    }
}
