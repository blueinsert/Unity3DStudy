using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bluebean;

public class Driver : MonoBehaviour
{

    PhysicsWorld physicsWorld = new PhysicsWorld();

    // Start is called before the first frame update
    void Start()
    {
        physicsWorld.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        physicsWorld.UpdateSimulation();
    }
}
