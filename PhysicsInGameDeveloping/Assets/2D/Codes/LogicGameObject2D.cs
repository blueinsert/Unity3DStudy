using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class LogicGameObject2D
    {
        private Collider2D m_collider;
        public Collider2D collider { get { return m_collider; } }

        private RigidBody2D m_rigidBody;
        public RigidBody2D rigidBody {
            get { return m_rigidBody; }
        }

        public float mass { get { return m_rigidBody.m_mass; } }
        public Vector3 position { get { return m_rigidBody.m_position; } }
        public float orientation { get { return m_rigidBody.m_orientation; } }
        public float inertia { get { return m_rigidBody.m_inertia; } }

        public LogicGameObject2D(Collider2D collider)
        {
            m_collider = collider;
            m_collider.SetGameObject(this);
            m_rigidBody = new RigidBody2D();
        }

        public void UpdatePhysics(float deltaTime)
        {

        }

    }
}
