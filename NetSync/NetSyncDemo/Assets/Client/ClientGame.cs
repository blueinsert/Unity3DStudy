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
        Debug.Log("ClientGame:OnDisConnectedServer");
        Application.Quit();
    }

    private void OnAllocateTargetNtf(NetworkMessage message)
    {
        ProtoAllocateTargetNtf msg = new ProtoAllocateTargetNtf();
        msg.Deserialize(message.reader);
        m_targetId = msg.m_targetId;
        Debug.Log(string.Format("ClientGame:OnAllocateTargetNtf targetId:{0}", m_targetId)); 
    }

    private void OnStateBroadcastNtf(NetworkMessage message)
    {

    }


    public void ConnectToServer(string ip, int port)
    {
        m_client = new NetworkClient();
        m_client.RegisterHandler(MsgType.Connect, OnConnectedServer);
        m_client.RegisterHandler(MsgType.Disconnect, OnDisConnectedServer);
        m_client.RegisterHandler(MsgType.Error, OnError);
        m_client.RegisterHandler(ProtoType.AllocateTargetNtf, OnAllocateTargetNtf);
        m_client.RegisterHandler(ProtoType.StateBroadcastNtf, OnStateBroadcastNtf);
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
        command.target = m_targetId;
        m_gameWorld.AddCommandToExe(command);
        m_gameWorld.Step();
        if (m_connect != null && m_gameWorld.m_frame % m_sendRate == 0)
        {
            Packet packet = m_packetPool.Get();
            m_gameWorld.PacketUnsendCommands(packet);
            Send(CustomMessageId.ClientCommandUpload, packet.data);
            m_packetPool.Back(packet);
        }
    }

    public void Send(ProtoMessageBase msg)
    {
        m_connect.Send(msg.GetProtoType(), msg);
    }

    int m_sendRate = 1;
    GameWorld m_gameWorld;
    int m_targetId;//entity id
    NetworkClient m_client;
    NetworkConnection m_connect;
}
