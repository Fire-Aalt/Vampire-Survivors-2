using RenderDream.GameEssentials;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class WorldGenerator : Singleton<WorldGenerator>
    {
        public static readonly int chunkSizeInPixels = 256;
        public static readonly int chunkSizeInTiles = 16;

        [SerializeField] private bool _optimizeDataUsage;

        [field: SerializeField, Title("References")] public RuntimeWorldHolder RuntimeWorldHolder { get; private set; }
        [field: SerializeField] public WallTilesGenerator WallTilesGenerator { get; private set; }
        [field: SerializeField] public ObjectsGenerator ObjectsGenerator { get; private set; }

        [field: SerializeField, Title("Generation")] public WorldGenerationDataSO GenerationData { get; private set; }
        [SerializeField] private bool _randomizeSeed;
        [SerializeField, HideIf("_randomizeSeed")] private int _seed;

        public ChunkDataGenerator DataGenerator { get; private set; }
        public ThreadedDataRequester ThreadedDataRequester { get; private set; }

        public void InitializeRuntimeGeneration()
        {
            if (_randomizeSeed)
            {
                _seed = Random.Range(0, int.MaxValue);
            }

            InitializeGeneration(_seed);            
        }

        public void InitializeGeneration(int seed)
        {
            _seed = seed;

            DataGenerator = new ChunkDataGenerator(GenerationData, seed);
            ThreadedDataRequester = new ThreadedDataRequester(DataGenerator);
            WallTilesGenerator.Initialize();
            ObjectsGenerator.Initialize();
        }

        private void Update()
        {
            ThreadedDataRequester.UpdateThreads();
        }

        public void GenerateObjects(Chunk chunk, in ChunkData data)
        {
            WallTilesGenerator.GenerateColliders();
            ObjectsGenerator.GenerateObjects(chunk, data.objectsData);
        }

        public void GenerateChunk(Vector2Int chunkCoord, in ChunkData data)
        {
            WallTilesGenerator.GenerateTiles(chunkCoord, data.tilesData);
        }

        public void DestroyChunk(Vector2Int chunkCoord, in ChunkData data)
        {
            WallTilesGenerator.DestroyTiles(chunkCoord, data.tilesData);
        }
    }
}
