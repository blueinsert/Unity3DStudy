using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace AnimationExample
{
	[System.Serializable]
	public class Anim_LightControlBehaviour : PlayableBehaviour
	{
		public Color color = Color.white;
		public float intensity = 1f;
	}
}