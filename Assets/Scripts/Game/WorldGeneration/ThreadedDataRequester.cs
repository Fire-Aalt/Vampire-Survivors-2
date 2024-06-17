using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game
{
    public class ThreadedDataRequester
    {
        public readonly ChunkDataGenerator dataGenerator;

        private readonly Queue<ThreadInfo<LayersMatrix>> _layersMatrixThreadInfoQueue = new();
        private readonly Queue<ThreadInfo<ChunkData>> _chunkDataThreadInfoQueue = new();

        public ThreadedDataRequester(ChunkDataGenerator dataGenerator)
        {
            this.dataGenerator = dataGenerator;
        }

        public void UpdateThreads()
        {
            if (_layersMatrixThreadInfoQueue.Count > 0)
            {
                for (int i = 0; i < _layersMatrixThreadInfoQueue.Count; i++)
                {
                    ThreadInfo<LayersMatrix> threadInfo = _layersMatrixThreadInfoQueue.Dequeue();
                    threadInfo.callback(threadInfo.parameter);
                }
            }
            if (_chunkDataThreadInfoQueue.Count > 0)
            {
                for (int i = 0; i < _chunkDataThreadInfoQueue.Count; i++)
                {
                    ThreadInfo<ChunkData> threadInfo = _chunkDataThreadInfoQueue.Dequeue();
                    threadInfo.callback(threadInfo.parameter);
                }
            }
        }

        public void RequestLayersMatrix(Vector2 center, Action<LayersMatrix> callback)
        {
            void threadStart()
            {
                LayersMatrixThread(center, callback);
            }

            new Thread(threadStart).Start();
        }

        private void LayersMatrixThread(Vector2 center, Action<LayersMatrix> callback)
        {
            LayersMatrix layersMatrix = dataGenerator.GenerateRawData(center);

            lock (_layersMatrixThreadInfoQueue)
            {
                _layersMatrixThreadInfoQueue.Enqueue(new ThreadInfo<LayersMatrix>(callback, layersMatrix));
            }
        }

        public void RequestChunkData(ChunkRequestData requestData, Action<ChunkData> callback)
        {
            void threadStart()
            {
                ChunkDataThread(requestData, callback);
            }

            new Thread(threadStart).Start();
        }

        private void ChunkDataThread(ChunkRequestData requestData, Action<ChunkData> callback)
        {
            ChunkData data = dataGenerator.GenerateCompleteData(requestData);

            lock (_chunkDataThreadInfoQueue)
            {
                _chunkDataThreadInfoQueue.Enqueue(new ThreadInfo<ChunkData>(callback, data));
            }
        }

        readonly struct ThreadInfo<T>
        {
            public readonly Action<T> callback;
            public readonly T parameter;

            public ThreadInfo(Action<T> callback, T parameter)
            {
                this.callback = callback;
                this.parameter = parameter;
            }
        }
    }
}
