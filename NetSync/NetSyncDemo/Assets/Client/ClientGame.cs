using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClientGame  {

    private void OnError(NetworkMessage message)
    {

    }

    private void OnConnectedServer(NetworkMessage msg)
    {
        m_connect = msg.conn;
    }

    private void OnDisConnectedServer(NetworkMessage msg)
    {

    }

    private void OnRecvStateBroadcastMsg(CustomMessage msg)
    {

    }

    private void OnRecvCustomMsg(NetworkMessage message)
    {
        Debug.Log("OnRecvCustomMsg");
        CustomMessage msg = new CustomMessage();
        msg.Deserialize(message.reader);
        switch (msg.messageId)
        {
            case CustomMessageId.StateBroadcast:
                break;
        }
    }

    public void ConnectToServer(string ip, int port)
    {
        m_client = new NetworkClient();
        m_client.RegisterHandler(MsgType.Connect, OnConnectedServer);
        m_client.RegisterHandler(MsgType.Disconnect, OnDisConnectedServer);
        m_client.RegisterHandler(MsgType.Error, OnError);
        m_client.RegisterHandler(CustomMsgTypes.InGameMsg, OnRecvCustomMsg);
        m_client.Connect(ip, port);
    }

    public void Initialize()
    {
        m_gameWorld = new GameWorld();
        m_gameWorld.Initialize();
    }

    private CommandInput CollectCommandInput()
    {
        CommandInput input = new CommandInput();
        if (Input.GetKey(KeyCode.W))
            input.forward = true;
        if (Input.GetKey(KeyCode.S))
            input.back = true;
        if (Input.GetKey(KeyCode.A))
            input.left = true;
        if (Input.GetKey(KeyCode.D))
            input.right = true;
        return input;
    }

    public void Step()
    {
        Command command = new Command();
        CommandInput input = CollectCommandInput();
        command.input = input;
        command.result = new CommandResult();
        command.sequence = (uint)( m_gameWorld.m_frame + 1);
        //command.target = -1;
        m_gameWorld.AddCommandToExe(command);
        m_gameWorld.Step();
        if (m_gameWorld.m_frame % m_sendRate == 0)
        {
            Packet packet = m_packetPool.Get();

        }
    }

    int m_sendRate = 1;
    GameWorld m_gameWorld;

    NetworkClient m_client;
    NetworkConnection m_connect;
    PacketPool m_packetPool = new PacketPool(64, 256);
}
