using System.Collections;
using System.Collections.Generic;

namespace bluebean
{
    public class PhysicsWorld2D
    {
        private List<LogicGameObject2D> m_objs = new List<LogicGameObject2D>();
        private List<LogicGameObject2D> m_objsForAdd = new List<LogicGameObject2D>();

        public void AddObj(LogicGameObject2D obj)
        {
            m_objsForAdd.Add(obj);
        }

        private void DebugDraw(float deltaTime)
        {
            foreach (var obj in m_objs)
            {
                obj.collider.DebugDraw(UnityEngine.Color.blue, deltaTime);
            }
        }

        public void Update(float deltaTime)
        {
            if(m_objsForAdd.Count != 0)
            {
                m_objs.AddRange(m_objsForAdd);
                m_objsForAdd.Clear();
            }
            foreach(var obj in m_objs)
            {
                obj.UpdatePhysics(deltaTime);
            }

            DebugDraw(deltaTime);
        }
    }
}
