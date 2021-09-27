using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace MixerExample
{
	[TrackClipType(typeof(Mixer_LightControlAsset))]
	[TrackBindingType(typeof(Light))]
	public class Mixer_LightControlTrack : TrackAsset
	{
		public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			return ScriptPlayable<Mixer_LightControlMixerBehaviour>.Create(graph, inputCount);
		}
	}
}