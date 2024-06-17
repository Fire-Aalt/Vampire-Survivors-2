using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Game.CoreSystem
{
    public class SpriteBlink : CoreComponent
    {
        [Title("Initialization")]
        [SerializeField] private Transform _targetRoot;
        [Tooltip("Used to exclude SpriteRenderers with filter (Lowercases everything)")]
        [SerializeField] private string[] _forbiddenSubSpriteNames;

        [Title("Settings")]
        [SerializeField] private float _blinkAlpha;
        [SerializeField] private int _numberOfBlinks;

        private List<GameObject> _gameObjectsToBlink = new();
        private CancellationTokenSource _blinkCTS;

        protected override void Awake()
        {
            base.Awake();

            FindGameObjectsToBlink();
        }

        public void PlayBlink(float duration)
        {
            _blinkCTS?.Cancel();
            _ = BlinkOperation(duration);
        }

        private async UniTaskVoid BlinkOperation(float iFramesDuration)
        {
            _blinkCTS = new CancellationTokenSource();
            float flashDuration = iFramesDuration / _numberOfBlinks;

            for (int i = 0; i < _numberOfBlinks; i++)
            {
                float blinkToDuration = flashDuration / 2;

                for (int j = 0; j < _gameObjectsToBlink.Count; j++)
                {
                    _gameObjectsToBlink[j].LeanAlpha(_blinkAlpha, blinkToDuration);
                }

                await UniTask.WaitForSeconds(blinkToDuration, cancellationToken: _blinkCTS.Token);

                for (int j = 0; j < _gameObjectsToBlink.Count; j++)
                {
                    _gameObjectsToBlink[j].LeanAlpha(1f, blinkToDuration);
                }

                await UniTask.WaitForSeconds(blinkToDuration, cancellationToken: _blinkCTS.Token);
            }
        }

        private void FindGameObjectsToBlink()
        {
            SpriteRenderer[] spriteRenderers = _targetRoot.GetComponentsInChildren<SpriteRenderer>(true);
            _gameObjectsToBlink = spriteRenderers
                .Where(s => IsAllowedName(s.gameObject.name))
                .Select(s => s.gameObject)
                .ToList();

            _gameObjectsToBlink.Add(spriteRenderers[0].gameObject);
        }

        private bool IsAllowedName(string name)
        {
            for (int j = 0; j < _forbiddenSubSpriteNames.Length; j++)
            {
                if (name.ToLower() == _forbiddenSubSpriteNames[j].ToLower())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
