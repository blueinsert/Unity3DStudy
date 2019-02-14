using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bluebean;

public class ContactDemo : MonoBehaviour
{
    PhysicsWorld2D world = new PhysicsWorld2D();

    private void Initial()
    {
        LogicGameObject2D obj1 = new LogicGameObject2D(new Box2DCollider(1,1), new Vector3(0,10), 45);
        world.AddObj(obj1);
        LogicGameObject2D obj2 = new LogicGameObject2D(new Box2DCollider(100, 100f), new Vector3(0, -50), 0, 0);
        world.AddObj(obj2);
    }

    void Start()
    {
        Initial();
    }

    void Update()
    {
        world.Update(Time.deltaTime);
    }

   
}
