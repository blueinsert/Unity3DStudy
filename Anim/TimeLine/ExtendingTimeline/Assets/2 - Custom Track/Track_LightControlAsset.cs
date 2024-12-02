using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TrackExample
{
	public class Track_LightControlAsset : PlayableAsset, ITimelineClipAsset
	{
		//public ExposedReference<Light> light;
		public Color color = Color.white;
		public float intensity = 1f;

		public ClipCaps clipCaps
		{
			get { return ClipCaps.None; }
		}

		public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
		{
			var playable = ScriptPlayable<Track_LightControlBehaviour>.Create(graph);
			
			var lightControlBehaviour = playable.GetBehaviour();
			//lightControlBehaviour.light = light.Resolve(graph.GetResolver());
			lightControlBehaviour.color = color;
			lightControlBehaviour.intensity = intensity;

			return playable;   
		}
	}
}