using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace MixerExample
{
	public class Mixer_LightControlAsset : PlayableAsset, ITimelineClipAsset
	{
		public Color color = Color.white;
		public float intensity = 1f;

		public ClipCaps clipCaps
		{
			get { return ClipCaps.Blending; }
		}

		public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
		{
			var playable = ScriptPlayable<Mixer_LightControlBehaviour>.Create(graph);
			
			var lightControlBehaviour = playable.GetBehaviour();
			lightControlBehaviour.color = color;
			lightControlBehaviour.intensity = intensity;

			return playable;   
		}
	}
}