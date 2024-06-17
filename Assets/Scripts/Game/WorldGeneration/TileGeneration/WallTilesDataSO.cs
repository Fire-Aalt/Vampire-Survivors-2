using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    [CreateAssetMenu(menuName = "Data/Wall Tiles Data")]
    public class WallTilesDataSO : ScriptableObject
    {
        [Title("RuleTile")]
        public RuleTile ruleTile;
    }

    [System.Serializable]
    public struct RuleTileData
    {
        [PreviewField] public UnityEngine.Tilemaps.Tile tile;
        public int weight;
    }
}
