using MoreMountains.Feedbacks;
using RenderDream.GameEssentials;

namespace Game
{
    public class PauseManager : Singleton<PauseManager>
    {
        public static bool IsGamePaused { get => State != PauseStates.None; }
        public static PauseStates State { get; private set; }

        private void Start()
        {
            UpdatePauseState(PauseStates.None);            
        }

        public static void UpdatePauseState(PauseStates newState)
        {
            if (State == newState) return;

            switch (newState)
            {
                case PauseStates.None:
                    //SetPauseState(false);
                    break;
                case PauseStates.PauseMenu:
                    //SetPauseState(true);
                    break;
                case PauseStates.Inventory:
                    //SetPauseState(true);
                    break;
                case PauseStates.InGamePause:
                    //SetPauseState(true);
                    break;
            }

            State = newState;
        }

        private void SetPauseState(bool pause)
        {
            if (pause)
            {
                MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, 0f, 0f, false, 1f, true);
            }
            else
            {
                MMTimeScaleEvent.Trigger(MMTimeScaleMethods.Reset, 1f, 0f, false, 1f, true);
            }
        }

    }

    public enum PauseStates
    {
        None,
        PauseMenu,
        Inventory,
        InGamePause
    }
}
