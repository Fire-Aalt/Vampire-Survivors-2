using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game
{
    [ExecuteInEditMode]
    public class WorldGenerationPreview : MonoBehaviour
    {
        [Title("In Editor Runtime")]
        [SerializeField, OnValueChanged("HandleDrawModeChanged")] private ChunkDrawMode _chunkDrawMode;

        [Title("In Editor Preview")]
        [SerializeField] private int _areaRadiusInChunks;
        [SerializeField] private bool _useThreads;
        [SerializeField] private bool _randomizeSeed;
        [SerializeField, HideIf("_randomizeSeed")] private int _seed;

        public EndlessTerrain EndlessTerrain { get; private set; }
        public WorldGenerator ChunkGenerator { get; private set; }
        public ThreadedDataRequester ThreadedDataRequester { get; private set; }

        private readonly List<PreviewChunk> _activeChunks = new();

        public void HandleDrawModeChanged()
        {
            foreach (var activeChunk in _activeChunks)
            {
                activeChunk?.RedrawChunk(_chunkDrawMode);
            }
        }

        private void Awake()
        {
            EndlessTerrain = GetComponentInParent<EndlessTerrain>();
            ChunkGenerator = GetComponentInParent<WorldGenerator>();
            
            if (Application.isPlaying)
            {
                DestroyPreviewObjects();
            }
        }

        [Button]
        public void GenerateWorld()
        {
            DestroyPreviewObjects();
            if (_randomizeSeed)
            {
                _seed = Random.Range(0, int.MaxValue);
            }

            ActiveChunk.InitializeChunks();
            ChunkGenerator.InitializeGeneration(_seed);
            ThreadedDataRequester = ChunkGenerator.ThreadedDataRequester;

            for (int yOffset = -_areaRadiusInChunks; yOffset <= _areaRadiusInChunks; yOffset++)
            {
                for (int xOffset = -_areaRadiusInChunks; xOffset <= _areaRadiusInChunks; xOffset++)
                {
                    Vector2Int viewedChunkCoord = new(xOffset, yOffset);

                    var previewChunk = new PreviewChunk(viewedChunkCoord, this, _chunkDrawMode, _useThreads);
                    previewChunk.Initialize();
                    _activeChunks.Add(previewChunk);
                }
            }
        }

        [Button]
        public void DestroyPreviewObjects()
        {
            foreach (var activeChunk in _activeChunks)
            {
                activeChunk.Dispose();
            }
            _activeChunks.Clear();

            ChunkGenerator.RuntimeWorldHolder.ClearEverything();
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                ChunkGenerator.GenerationData.OnValuesUpdated -= GenerateWorld;
                ChunkGenerator.GenerationData.OnValuesUpdated += GenerateWorld;
                ThreadedDataRequester?.UpdateThreads();
            }
        }

        private class PreviewChunk : ActiveChunk
        {
            public override bool UseChunkPool => false;

            private readonly WorldGenerationPreview _preview;

            public PreviewChunk(Vector2Int coord, WorldGenerationPreview preview, ChunkDrawMode drawMode, bool useThreads) 
                : base(coord, drawMode, preview.ChunkGenerator, preview.EndlessTerrain.ChunkPrefab, preview.EndlessTerrain.ChunksHolder, useThreads: useThreads)
            {
                _preview = preview;
            }

            public void Initialize()
            {
                if (useThreads)
                    generator.ThreadedDataRequester.RequestLayersMatrix(ChunkCenter, OnLayersMatrixReceived);
                else
                    OnLayersMatrixReceived(generator.DataGenerator.GenerateRawData(ChunkCenter));
            }

            protected override void OnChunkDataReceived(ChunkData data)
            {
                _preview.ChunkGenerator.GenerateChunk(coord, data);
                base.OnChunkDataReceived(data);
            }
        }
    }

    public enum ChunkDrawMode
    {
        NoiseMap,
        TilesMap,
        ColorMap
    }
}
