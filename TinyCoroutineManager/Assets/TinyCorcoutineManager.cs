using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace bluebean.utils
{
    public class YieldInstruction
    {
        public virtual void Update() { }
        public virtual bool IsPassed() { return true; }
    }
    public class WaitForNextFrame : YieldInstruction
    {

    }

    public class Break : YieldInstruction
    {

    }

    public class WaitForSeconds : YieldInstruction
    {
        private float m_duration;

        public WaitForSeconds(float duration)
        {
            m_duration = duration;
        }

        public override void Update()
        {
            m_duration -= Time.deltaTime;
        }

        public override bool IsPassed()
        {
            return m_duration <= 0;
        }
    }

    public class WaitForFrames: YieldInstruction
    {
        private int m_frames;

        public WaitForFrames(int frames)
        {
            m_frames = frames;
        }

        public override void Update()
        {
            m_frames--;
        }

        public override bool IsPassed()
        {
            return m_frames <= 0;
        }
    }

    public class TinyCorcoutineManager
    {
        private LinkedList<IEnumerator> corcoutines = new LinkedList<IEnumerator>();
        private List<IEnumerator> deadCorcoutines = new List<IEnumerator>();

        public void Update()
        {
            var first = corcoutines.First;
            var node = first;
            while (node != null)
            { 
                if (!MoveNext(node.Value))
                {
                    deadCorcoutines.Add(node.Value);
                }
                node = node.Next;
            }
            if (deadCorcoutines.Count != 0)
            {
                foreach (var r in deadCorcoutines)
                {
                    corcoutines.Remove(r);
                }
                deadCorcoutines.Clear();
            }
        }

        private bool MoveNext(IEnumerator corcoutine)
        {
            var current = corcoutine.Current; 
            if (current is IEnumerator && MoveNext(corcoutine.Current as IEnumerator)) //优先执行子协程
            {
                return true;
            }else if(current is YieldInstruction)
            {
                if(current is Break)
                {
                    return false;
                }else
                {
                    var yieldInstruction = current as YieldInstruction;
                    yieldInstruction.Update();
                    if (yieldInstruction.IsPassed())
                    {
                        return corcoutine.MoveNext();
                    }
                    return true;
                }  
            }
            else
            {
                return corcoutine.MoveNext();
            }
        }

        public void StartCorcoutine(IEnumerator corcoutine)
        {
            corcoutines.AddLast(corcoutine);
        }

    }
}
