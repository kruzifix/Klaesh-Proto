using UnityEngine;

namespace Klaesh.Utility
{
    public static class Mathk
    {
        /// <summary>
        /// Maps val from [sMin, sMax] to [tMin, tMax]. val is clamped to [sMin, sMax].
        /// </summary>
        public static float Map(float val, float sMin, float sMax, float tMin, float tMax)
        {
            return Mathf.Lerp(tMin, tMax, (val - sMin) / (sMax - sMin));
        }
    }
}
