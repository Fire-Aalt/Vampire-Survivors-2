using RenderDream.GameEssentials;

namespace Game
{
    public class GameData : GameDataModel
    {
        public float health;
        public float scoreTime;

        public GameData() 
        {
            health = 5f;
            scoreTime = 0;
        }
    }
}
