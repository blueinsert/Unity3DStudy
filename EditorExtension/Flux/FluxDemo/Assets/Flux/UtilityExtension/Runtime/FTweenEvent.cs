using UnityEngine;

namespace Flux
{
	public abstract class FTweenEvent<T> : FEvent where T : FTweenBase {

		[SerializeField]
		protected T _tween = default(T);
		public T Tween { get { return _tween; } }

	}
}
