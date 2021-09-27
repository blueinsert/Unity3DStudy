using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Simple
{
	public class Simple_LightControlBehaviour : PlayableBehaviour
	{
		public Light light;
		public Color color;
		public float intensity;

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			if (light != null)
			{
				light.color = color;
				light.intensity = intensity;
			}
		}
	}
}