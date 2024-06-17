using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public class WallTilesGenerator : MonoBehaviour
    {
        [Title("Data")]
        [SerializeField] private Tilemap _tilemapPrefab;

        private int _tilesInLine;
        private Vector3Int _pos;
        private Vector3Int _offset;
        private TilesData _tilesData;

        private WorldGenerator _chunkGenerator;
        private WallTilesDataSO _data;

        private Tilemap[] _tilemaps;
        private TilemapCollider2D[] _tilemapsColliders;
        private int _layersAmount;

        public void Initialize()
        {
            _chunkGenerator = GetComponentInParent<WorldGenerator>();
            _data = _chunkGenerator.GenerationData.wallTilesData;
            _layersAmount = _chunkGenerator.GenerationData.terrainData.noiseToTilesSettings.tileLayers.Count;

            _tilemaps = new Tilemap[_layersAmount];
            _tilemapsColliders = new TilemapCollider2D[_layersAmount];
            for (int i = 0; i < _layersAmount; i++)
            {
                var tilemap = Instantiate(_tilemapPrefab);
                tilemap.name = "Layer_" + i;
                tilemap.transform.SetParent(_chunkGenerator.RuntimeWorldHolder.TilemapsHolder);

                _tilemaps[i] = tilemap;
                _tilemapsColliders[i] = tilemap.gameObject.GetComponent<TilemapCollider2D>();
            }
        }

        public void GenerateColliders()
        {
            foreach (var collider in _tilemapsColliders)
            {
                collider.ProcessTilemapChanges();
            }
        }

        public void GenerateTiles(Vector2Int chunkCoord, in TilesData tilesData)
        {
            SetTilesToChunk(chunkCoord, tilesData, _data.ruleTile);
        }

        public void DestroyTiles(Vector2Int chunkCoord, in TilesData tilesData)
        {
            SetTilesToChunk(chunkCoord, tilesData, null);
        }

        private void SetTilesToChunk(Vector2Int chunkCoord, in TilesData tilesData, TileBase tile)
        {
            _tilesData = tilesData;
            _tilesInLine = WorldGenerator.chunkSizeInTiles;
            _offset = new Vector3Int(-_tilesInLine / 2, -_tilesInLine / 2) + new Vector3Int(chunkCoord.x * _tilesInLine, chunkCoord.y * _tilesInLine);

            List<Vector3Int>[] positions = new List<Vector3Int>[_layersAmount];
            List<TileBase>[] tiles = new List<TileBase>[_layersAmount];

            for (int i = 0; i < _layersAmount; i++)
            {
                positions[i] = new List<Vector3Int>();
                tiles[i] = new List<TileBase>();
            }

            for (int y = 0; y < _tilesInLine; y++)
            {
                for (int x = 0; x < _tilesInLine; x++)
                {
                    _pos = new Vector3Int(x, y);

                    int layer = GetCellLayer(Vector3Int.zero);

                    for (int i = 0; i <= layer; i++)
                    {
                        positions[i].Add(_pos + _offset);
                        tiles[i].Add(tile);
                    }
                }
            }

            for (int i = 0; i < _layersAmount; i++)
            {
                _tilemaps[i].SetTiles(positions[i].ToArray(), tiles[i].ToArray());
            }
        }
        
        private int GetCellLayer(Vector3Int offset)
        {
            Vector3Int pos = _pos + offset;
            if ((pos.x < 0 || pos.x >= _tilesInLine) || (pos.y < 0 || pos.y >= _tilesInLine))
            {
                return -1;
            }
            return _tilesData.tilesMatrix[pos.x, pos.y];
        }
    }
}
