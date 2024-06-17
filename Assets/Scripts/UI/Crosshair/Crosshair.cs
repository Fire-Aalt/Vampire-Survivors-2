using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Crosshair : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        private RectTransform _rect;

        private void Start()
        {
            _rect = GetComponent<RectTransform>();
        }

        private void Update()
        {
            PositionCrosshair();
        }

        private void PositionCrosshair()
        {
            if (Cursor.visible) Cursor.visible = false;
            
            Vector2 pos = Input.mousePosition / _canvas.scaleFactor;
            _rect.anchoredPosition = pos;
        }
    }
}
