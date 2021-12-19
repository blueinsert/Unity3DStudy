using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class PlayableTest : MonoBehaviour
{
    public AnimationClip jumpAnimationClip;
    public AnimationClip rotateAnimationClip;
    public AnimationClip scaleAnimationClip;

    AnimationMixerPlayable m_mixerAnimationPlayable;
    public PlayableGraph m_graph;
    [Range(0, 1)] public float weight;

    // Start is called before the first frame update
    void Start()
    {
        m_graph = PlayableGraph.Create("TestPlayableGraph");

        var animationOutputPlayable = AnimationPlayableOutput.Create(m_graph, "AnimationOutput", GetComponent<Animator>());

        //inputCount=2，即有两个输入节点
        m_mixerAnimationPlayable = AnimationMixerPlayable.Create(m_graph, 2);
        animationOutputPlayable.SetSourcePlayable(m_mixerAnimationPlayable, 0);

        var jumpAnimationClipPlayable = AnimationClipPlayable.Create(m_graph, jumpAnimationClip);
        var rotateAnimationClipPlayable = AnimationClipPlayable.Create(m_graph, rotateAnimationClip);

        //使用Connect方法连接Playable节点，如下面的jumpAnimationClipPlayable第0个输出口连接到m_mixerAnimationPlayable的第0个输入口
        m_graph.Connect(jumpAnimationClipPlayable, 0, m_mixerAnimationPlayable, 0);
        m_graph.Connect(rotateAnimationClipPlayable, 0, m_mixerAnimationPlayable, 1);

        //同时可以利用Disconnect方法来断开连接，如断开m_mixerAnimationPlayable第0个输入端
        //m_graph.Disconnect(m_mixerAnimationPlayable, 0);

        m_graph.Play();

    }

    // Update is called once per frame
    void Update()
    {
        m_mixerAnimationPlayable.SetInputWeight(0, weight);
        m_mixerAnimationPlayable.SetInputWeight(1, 1 - weight);
    }

    void OnDisable()
    {
        // 销毁graph中所有的Playables和PlayableOutputs
        m_graph.Destroy();
    }
}
