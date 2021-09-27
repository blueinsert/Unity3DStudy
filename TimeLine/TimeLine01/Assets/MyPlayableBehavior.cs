using UnityEngine;
using UnityEngine.Playables;


public class MyPlayableBehavior : PlayableBehaviour
{

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);

        Debug.Log("Hello Timeline");
    }
}
