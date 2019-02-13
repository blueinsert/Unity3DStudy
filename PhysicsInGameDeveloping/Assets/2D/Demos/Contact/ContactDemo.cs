using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bluebean;

public class ContactDemo : MonoBehaviour
{
    List<RigidBody2D> m_objects = new List<RigidBody2D>();

    private void Initial()
    {
        Initial();
    }

    void Start()
    {

    }

    void Update()
    {
        UpdateSimulation(Time.deltaTime);
        Render(Color.blue);
    }

    private void UpdateSimulation(float deltaTime)
    {
    }

    private void Render(Color color)
    {
    }

}
