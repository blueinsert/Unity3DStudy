using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bluebean;

public class ContactDemo : MonoBehaviour
{
    PhysicsWorld2D world = new PhysicsWorld2D();

    private void Initial()
    {
        LogicGameObject2D obj1 = new LogicGameObject2D(new Box2DCollider(1, 1), new Vector3(0, 1));
        world.AddObj(obj1);
        LogicGameObject2D obj2 = new LogicGameObject2D(new Box2DCollider(1, 1), new Vector3(0, 3));
        world.AddObj(obj2);
        LogicGameObject2D obj3 = new LogicGameObject2D(new Box2DCollider(1, 1), new Vector3(0, 5));
        world.AddObj(obj3);

        LogicGameObject2D plane = new LogicGameObject2D(new Box2DCollider(100, 100f), new Vector3(0, -50), 0, 0, 0);
        world.AddObj(plane);
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
