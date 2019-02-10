using System.Collections;
using System.Collections.Generic;

namespace bluebean
{
    public abstract class Collider2D
    {
        protected LogicGameObject2D m_owner;

        public Collider2D(LogicGameObject2D go)
        {
            SetGameObject(go);
        }

        public Collider2D() { }

        public void SetGameObject(LogicGameObject2D go)
        {
            this.m_owner = go;
        }

        public virtual void DebugDraw(UnityEngine.Color color, float deltaTime)
        {

        }
    }
}
