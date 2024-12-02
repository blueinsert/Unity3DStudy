using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace AnimationExample
{
	public class Anim_LightControlAsset : PlayableAsset
	{
		public Anim_LightControlBehaviour template;

		public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
		{
			var playable = ScriptPlayable<Anim_LightControlBehaviour>.Create(graph, template);
			
			return playable;
		}
	}
}