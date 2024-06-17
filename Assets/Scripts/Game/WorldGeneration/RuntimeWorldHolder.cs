using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class RuntimeWorldHolder : MonoBehaviour
    {
        public Transform TilemapsHolder;
        public Transform ChunksHolder;

        public void ClearEverything()
        {
            for (int i = TilemapsHolder.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(TilemapsHolder.GetChild(i).gameObject);
            }
            for (int i = ChunksHolder.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(ChunksHolder.GetChild(i).gameObject);
            }
        }
    }
}
