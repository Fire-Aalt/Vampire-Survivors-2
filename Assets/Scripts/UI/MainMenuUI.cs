using UnityEngine;
using RenderDream.GameEssentials;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Game
{
    public class MainMenuUI : MonoBehaviour, IGameDataPersistence
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private string _scoreFormat;

        public void LoadData(GameData data)
        {
            _scoreText.text = _scoreFormat.Replace("{score}", HUD.FormatTime(data.scoreTime)); 
        }

        public void SaveData(GameData data)
        {
        }

        public void Gameplay()
        {
            SceneLoader.Current.LoadSceneGroup(1, false, SceneTransition.TransitionInAndOut).Forget();
        }

        public void QuitApplication()
        {
            Application.Quit();
        }
    }
}
