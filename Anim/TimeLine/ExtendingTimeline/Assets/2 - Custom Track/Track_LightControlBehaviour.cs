using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TrackExample
{
	public class Track_LightControlBehaviour : PlayableBehaviour
	{
		//public Light light = null;
		public Color color = Color.white;
		public float intensity = 1f;

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			Light light = playerData as Light;

			if (light != null)
			{
				light.color = color;
				light.intensity = intensity;
			}
		}
	}
}