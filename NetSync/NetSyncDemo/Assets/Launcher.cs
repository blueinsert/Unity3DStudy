using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Launcher : MonoBehaviour {

    public void SetFixedDeltaTimeForServer()
    {
        Time.fixedDeltaTime = 1f / 60;
    }

    public void SetFixedDeltaTimeForClient()
    {
        Time.fixedDeltaTime = 1f / 60;
    }

    private void OnGUI()
    {

        if (m_state == 0 && GUI.Button(new Rect(0, 0, 100, 50), "StartServer")) {
            SetFixedDeltaTimeForServer();
            m_server = new Server();
            m_server.StartServer(20086);
            m_state = 2;
        }
        if(m_state == 0 && GUI.Button(new Rect(110, 0, 100, 50), "StartClient"))
        {
            SetFixedDeltaTimeForClient();
            m_clientGame = new ClientGame();
            m_clientGame.Initialize();
            m_clientGame.ConnectToServer("127.0.0.1", 20086);
            m_state = 1;
        }  
        if(m_state == 1)
        {
            GUI.Label(new Rect(0, 0, 150, 60), "Start as Client");
        }else if(m_state == 2)
        {
            GUI.Label(new Rect(0, 0, 150, 60), "Start as Server");
        }

    }

    private void FixedUpdate()
    {
        if (m_server != null)
        {
            m_server.Step();
        }else if (m_clientGame != null)
        {
            m_clientGame.Step();
        }
    }

    int m_state = 0;

    Server m_server;

    ClientGame m_clientGame;
}
