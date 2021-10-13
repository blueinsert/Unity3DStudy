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

        protected override void OnTrigger(float timeSinceTrigger)
        {
            _startPosition = Owner.localPosition;
            if (_relativePositionToSelf)
            {
                _offset = Owner.localPosition - _tween.From;
            }
            base.OnTrigger(timeSinceTrigger);
        }

        protected override void OnStop()
        {
            base.OnStop();
           
        }
        protected override void OnFinish()
        {
            base.OnFinish();
        }

        protected override void SetDefaultValues()
        {
            _tween = new FTweenVector3(Vector3.zero, Vector3.forward);
        }

        protected override void ApplyProperty(float t)
        {
            Vector3 value = _tween.GetValue(t);
            if (_relativePositionToSelf)
            {
                value += _offset;
            }
            Owner.localPosition = value;
        }
	}
  

}
