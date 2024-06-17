using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Data/World Generation Data")]
    public class WorldGenerationDataSO : UpdatableData
    {
        [InlineEditor] public NoiseDataSO worldLayersNoiseData;
        [InlineEditor] public TerrainDataSO terrainData;
        [InlineEditor] public WallTilesDataSO wallTilesData;
        [InlineEditor] public ObjectPlacementDataSO objectPlacementData;
    }
}
