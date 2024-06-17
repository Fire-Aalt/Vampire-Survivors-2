using RenderDream.GameEssentials;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class AudioListenerManager : Singleton<AudioListenerManager>
    {
        [SerializeField] private AudioListener _audioListener;

        private Transform _followTarget;

        public void SetFollowTarget(Transform target)
        {
            _followTarget = target;
        }
        //
        private void Update()
        {
            if (_followTarget != null)
            {
                _audioListener.transform.position = _followTarget.position;
            }
            else
            {
                _audioListener.transform.position = Vector3.zero;
            }
        }
    }
}
