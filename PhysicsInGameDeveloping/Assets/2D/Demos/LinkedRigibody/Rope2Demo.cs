using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bluebean;

public class Rope2Demo : MonoBehaviour
{
    Rope2 rope;

    // Start is called before the first frame update
    void Start()
    {
        rope = new Rope2();
    }

    // Update is called once per frame
    void Update()
    {
        rope.UpdateSimulation(Time.deltaTime);
        rope.Render(Color.blue);
    }
}
