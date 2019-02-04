using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class PhysicsWorld
    {
        int frameCount = 0;
        RigidBody2D craft;

        public void Initialize()
        {
            craft = new RigidBody2D();
        }

        public void UpdateSimulation()
        {
            craft.SetThrusters(false, false);
            if (Input.GetKey(KeyCode.S))
            {
                craft.ModulateThrust(false);
            }
            if (Input.GetKey(KeyCode.W))
            {
                craft.ModulateThrust(true);
            }
            if (Input.GetKey(KeyCode.A))
            {
                craft.SetThrusters(true, false);
            }
            if (Input.GetKey(KeyCode.D))
            {
                craft.SetThrusters(false, true);
            }
            craft.UpdateBodyEuler(Time.deltaTime);
            DrawCraft(craft, Color.blue);
            frameCount++;
        }

        void DrawCraft(RigidBody2D craft, Color color)
        {
            craft.Draw(color);
        }

        public static Vector3 VRotate2D(float angle, Vector3 u)
        {
            float x, y;
            x = u.x * Mathf.Cos(Mathf.Deg2Rad * angle) - u.y * Mathf.Sin(Mathf.Deg2Rad * angle);
            y = u.x * Mathf.Sin(Mathf.Deg2Rad * angle) + u.y * Mathf.Cos(Mathf.Deg2Rad * angle);
            return new Vector3(x, y, 0);
        }
    }
}
