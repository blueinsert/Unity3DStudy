using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bluebean;

public class RopeDemo : MonoBehaviour
{
    Rope2D rope;

    // Start is called before the first frame update
    void Start()
    {
        rope = new Rope2D(new Vector3(0, 0, 0), Vector3.right, 10, 50);
    }

    // Update is called once per frame
    void Update()
    {
        rope.UpdateSimulation(Time.deltaTime);
        rope.Render(Color.blue);
    }
}
