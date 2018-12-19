using UnityEngine;

namespace Klaesh.GameEntity.Component
{
    public class LineArrowComp : MonoBehaviour
    {
        public LineRenderer lineRenderer;
        public AnimationCurve lineCurve;

        public void SetColor(Color color)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }

        public void UpdateLine(Vector3 start, Vector3 end)
        {
            int vertexCount = 11;
            if (lineRenderer.positionCount != vertexCount)
                lineRenderer.positionCount = vertexCount;

            for (int i = 0; i < vertexCount; i++)
            {
                float t = i / (vertexCount - 1f);

                var pos = Vector3.Lerp(start, end, t);
                pos.y += lineCurve.Evaluate(t);
                lineRenderer.SetPosition(i, pos);
            }
        }
    }
}
