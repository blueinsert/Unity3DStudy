using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AnimationExample
{
	[TrackClipType(typeof(Anim_LightControlAsset))]
	[TrackBindingType(typeof(Light))]
	public class Anim_LightControlTrack : TrackAsset
	{
		public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			return ScriptPlayable<Anim_LightControlMixerBehaviour>.Create(graph, inputCount);
		}
	}
}