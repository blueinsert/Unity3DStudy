using UnityEngine;

namespace Flux
{
	public abstract class FTweenEvent<T> : FEvent where T : FTweenBase {

		[SerializeField]
		protected T _tween = default(T);
		public T Tween { get { return _tween; } }


		protected override void OnTrigger( float timeSinceTrigger )
		{
			OnUpdateEvent( timeSinceTrigger );
		}
        //更新每帧的位置
		protected override void OnUpdateEvent( float timeSinceTrigger )
		{
			float t = timeSinceTrigger / LengthTime;
			ApplyProperty( t );
		}

		protected override void OnFinish()
		{
            ApplyProperty(1f);
            base.OnFinish();
        }

        protected override void OnStop()
		{
            ApplyProperty(0f);
        }

		protected abstract void ApplyProperty( float t );
	}
}
