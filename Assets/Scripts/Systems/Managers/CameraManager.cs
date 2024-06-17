using UnityEngine;

namespace Game
{
    public class CameraManager : MonoBehaviour
    {
        [field: SerializeField] public Camera MainCamera { get; private set; }
        [field: SerializeField] public Camera UICamera { get; private set; }

        public static CameraManager Active
        {
            get
            {
                if (Current == null)
                {
                    Current = FindObjectOfType<CameraManager>();
                    Initialize();
                }
                return Current;
            }
        }
        public static CameraManager Current;

        public Bounds Bounds { get; private set; }

        private static void Initialize()
        {
            Active.Bounds = CalculateCameraBounds();
        }

        private static Bounds CalculateCameraBounds()
        {
            float horizontal, vertical;
            var camera = Active.MainCamera;

            horizontal = camera.orthographicSize * Screen.width / Screen.height * 2;
            vertical = camera.orthographicSize * 2;

            return new Bounds
            {
                size = new Vector2(horizontal, vertical)
            };
        }
    }
}
