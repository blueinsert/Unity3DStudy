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
        public float massInverse { get { return m_rigidBody.massInverse; } }
        public Vector3 position { get { return m_rigidBody.m_position; } }
        public float orientation { get { return m_rigidBody.m_orientation; } }
        public float inertia { get { return m_rigidBody.m_inertia; } }
        public float inertiaVerse { get { return m_rigidBody.inertiaInverse; } }

        public LogicGameObject2D(Collider2D collider, Vector3 position, float orientation = 0, float? mass = null, float? inertia = null)
        {
            m_collider = collider;
            m_collider.SetGameObject(this);
            m_rigidBody = new RigidBody2D();
            m_rigidBody.m_position = position;
            m_rigidBody.m_orientation = orientation;
            m_rigidBody.m_mass = (float)(mass == null ? 1 : mass);
            m_rigidBody.m_inertia = (float)(inertia == null ? 10 : inertia);
        }

        public void UpdatePhysics(float deltaTime)
        {

        }

    }
}
