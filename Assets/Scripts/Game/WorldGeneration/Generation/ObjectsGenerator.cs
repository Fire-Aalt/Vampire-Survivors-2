using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public class ObjectsGenerator : MonoBehaviour
    {
        [Title("Data")]
        [SerializeField] private LayerMask _groundLayer;

        private int _tilesInLineHalf;
        private Vector3 _tileOffset;

        private WorldGenerator _worldGenerator;
        private ObjectPlacementDataSO _data;

        private int _layersAmount;

        public void Initialize()
        {
            _worldGenerator = GetComponentInParent<WorldGenerator>();
            _data = _worldGenerator.GenerationData.objectPlacementData;

            _tileOffset = Vector2.one / 2f;
            _tilesInLineHalf = WorldGenerator.chunkSizeInTiles / 2;
            _layersAmount = _data.layers.Count;
            _data.PrecalculateWeightSum();
        }

        public void GenerateObjects(Chunk chunk, in ObjectsData data)
        {
            for (int layerId = 0; layerId < _layersAmount; layerId++)
            {
                var layerData = _data.layers[layerId];
                var layerHeightMap = data.layersHeightMaps[layerId];

                int tries = 0;
                int spawned = 0;
                while (tries < layerData.placementTries && spawned < layerData.maxObjects)
                {
                    Vector2Int tile = new(Random.Range(0, _tilesInLineHalf * 2), Random.Range(0, _tilesInLineHalf * 2));
                    Vector2Int offset = new(-_tilesInLineHalf, -_tilesInLineHalf);
                    Vector2 pos = (Vector3Int)tile + (Vector3Int)offset + chunk.transform.position + _tileOffset;

                    float height = layerHeightMap[tile.x, tile.y];
                    float heightThreshold = Random.value;
                    if (height > heightThreshold)
                    {
                        if (Physics2D.OverlapBox(pos, Vector2.one, 0f, _groundLayer))
                        {
                            tries++;
                            continue;
                        }

                        var prefab = _data.GetWeightedObject(layerId);
                        var obj = Instantiate(prefab);

                        obj.transform.position = pos;
                        obj.transform.SetParent(chunk.objectsHolder);
                        obj.transform.rotation = Random.Range(0, 2) == 0 ? Quaternion.Euler(new Vector3(0, 0, 0)) : Quaternion.Euler(new Vector3(0, 180, 0));

                        spawned++;
                    }
                    else
                    {
                        tries++;
                    }
                }
            }
        }
    }
}
