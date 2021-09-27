using UnityEngine;
using UnityEngine.Playables;

public class MyPlayableAsset : PlayableAsset
{
    public string m_name;
    public float param1;
    public float param2;

    public override string ToString()
    {
        return m_name;
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<MyPlayableBehavior>.Create(graph);    //通过这里将PlayableAsset 与 PlayableBehaviour 联系在了一起

        return playable;

    }
}

