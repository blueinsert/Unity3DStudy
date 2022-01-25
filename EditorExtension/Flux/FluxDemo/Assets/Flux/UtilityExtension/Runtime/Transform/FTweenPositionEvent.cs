using UnityEngine;

namespace Flux
{
    [FEvent("Transform/Tween Position", typeof(FTransformTrack))]
    public class FTweenPositionEvent : FTransformEvent
    {
        [SerializeField]
        protected bool _relativePositionToSelf;

        private Vector3 _startPosition;

        private Vector3 _offset;


        private Vector3 _cacheFrom;

        private Vector3 _cacheTo;

        public override string Text { get => "position"; set => base.Text = value; }
    }
  

}
