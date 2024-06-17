using UnityEngine;

namespace Game.UI
{
    public abstract class PageUI : MonoBehaviour
    {
        protected PageController _controller;

        protected void Start()
        {
            _controller = GetComponentInParent<PageController>();
            InitUI();
        }

        protected abstract void InitUI();

        public void ReturnToPrevPage()
        {
            _controller.TransitionBack();
        }
    }
}
