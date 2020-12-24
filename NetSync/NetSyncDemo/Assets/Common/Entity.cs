using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public void Awake()
    {
        m_currentState = new State(this);
    }

    public void SetWorld(GameWorld world)
    {
        m_world = world;
    }

    public void ExecuteCommand(Command command)
    {
        float movingSpeed = 4.0f;
        Vector3 movingDir = Vector3.zero;
        if(command.input.forward || command.input.back)
            movingDir.z = command.input.forward ? 1 : -1;
        if (command.input.left || command.input.right)
            movingDir.x = command.input.right ? 1 : -1;
        Vector3 velocity = movingSpeed * movingDir;
        transform.position = transform.position + velocity * Time.fixedDeltaTime;
        command.result.position = transform.position;
    }

    public int m_id;
    public State m_currentState;
    public GameWorld m_world;
}
