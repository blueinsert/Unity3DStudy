using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

[TrackClipType(typeof(MyPlayableAsset))]        //通过这里，将 track 与p layableAsset 绑定联系了起来
[TrackBindingType(typeof(GameObject))]
[TrackColor(0, 1, 0)]
public class MyPlayableTrack : PlayableTrack
{

    protected override Playable CreatePlayable(PlayableGraph graph, GameObject go, TimelineClip clip)
    {
        Playable res = base.CreatePlayable(graph, go, clip);

        return res;
    }

}
