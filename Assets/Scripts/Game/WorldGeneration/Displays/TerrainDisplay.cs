using UnityEngine;

namespace Game
{
    public class TerrainDisplay : MonoBehaviour
    {
        public SpriteRenderer spriteRender;
        private Material _material;

        private readonly int _textureProperty = Shader.PropertyToID("_Texture");
        private bool _instantiated;

        private void Awake()
        {
            InstantiateMaterial();
        }

        public void InstantiateMaterial()
        {
            if (!_instantiated)
            {
                if (Application.isPlaying)
                {
                    _material = spriteRender.material;
                }
                else
                {
                    _material = new Material(spriteRender.sharedMaterial);
                    spriteRender.sharedMaterial = _material;
                }
                _instantiated = true;
            }
        }

        public void DrawTextureRuntime(Texture2D texture)
        {   
            _material.SetTexture(_textureProperty, texture);
            spriteRender.transform.localScale = new Vector2(texture.width, texture.height);
        }
    }
}
