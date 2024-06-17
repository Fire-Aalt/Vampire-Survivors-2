using Cysharp.Threading.Tasks;
using RenderDream.GameEssentials;
using UnityEngine;

namespace Game
{
    public class Gameplay : MonoBehaviour
    {
        public static bool Initialized { get; private set; }

        private void Start()
        {
            InitializeGameplay();
        }

        private void InitializeGameplay()
        {
            // Set AudioListenerManager Follow Target
            AudioListenerManager.Current.SetFollowTarget(Player.Active.transform);
            Initialized = true;
        }

        //private void SetActiveBlits(bool isActive)
        //{
        //    UniversalRenderPipelineUtils.SetRendererFeatureActive("FullScreenPassRendererFeature", "LowHealth", isActive);
        //}

        private void OnEnable()
        {
            //SetActiveBlits(true);
        }

        private void OnDisable()
        {
            //SetActiveBlits(false);
        }
    }
}
