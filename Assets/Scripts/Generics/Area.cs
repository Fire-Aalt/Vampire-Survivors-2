using UnityEngine;

namespace Game
{
    public class Area
    {
        public Bounds bounds;
        public Vector3 position;

        public Vector3 Min => bounds.min + position;
        public Vector3 Max => bounds.max + position;

        public Area(Bounds bounds)
        {
            this.bounds = bounds;
        }

        public bool ContainsPoint(Vector3 point)
        {
            point -= position;
            return bounds.Contains(point);
        }

        public float OuterDistance(Vector3 point)
        {
            point -= position;
            if (!bounds.Contains(point))
            {
                point -= bounds.center;
                Vector2 cwed = new Vector2(Mathf.Abs(point.x), Mathf.Abs(point.y)) - (Vector2)bounds.size / 2f;
                float outsideDistance = new Vector2(Mathf.Max(cwed.x, 0f), Mathf.Max(cwed.y, 0f)).magnitude;

                return outsideDistance;
            }
            return 0f;
        }
    }
}
