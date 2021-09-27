using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TrackExample
{
	[TrackClipType(typeof(Track_LightControlAsset))]
	[TrackBindingType(typeof(Light))]
	public class Track_LightControlTrack : TrackAsset {}
}