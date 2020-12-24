using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Server {

    private void OnClientConnected(NetworkMessage message)
    {
        Debug.Log(string.Format("OnClientConnected {0}"));
        m_connections.Add(message.conn);
        Connection connection = new Connection(m_connections.Count);
        connection.Initialize(message.conn);
        int[] used = new int[3];
        foreach(var pair in m_connDic)
        {
            if (pair.Value.m_targetId != -1)
            {
                used[pair.Value.m_targetId] = 1;
            }
        }
        for(int i = 0; i < 3; i++)
        {
            if (used[i] == 0)
            {
                connection.SetTargetId(i);
                break;
            }
        }
        m_connDic.Add(message.conn, connection);
    }

    private void OnClientDisConnected(NetworkMessage message)
    {
        var con = message.conn;
        m_connections.Remove(con);
        m_connDic.Remove(con);
    }

    private void OnError(NetworkMessage message)
    {

    }

    private void OnMsgClientCommand(NetworkConnection conn, CustomMessage message)
    {
        Connection connection = m_connDic[conn];
        Packet packet = new Packet(message.bytes);
        List<Command> commands = new List<Command>();
        int count = packet.ReadInt();
        for (int i = 0; i < count; i++)
        {
            Command command = new Command();
            command.Read(packet);
            command.target = connection.m_targetId;
            commands.Add(command);
        }
        m_gameWorld.AddCommandsToExe(commands);
    }

    private void OnRecvCustomMsg(NetworkMessage message)
    {
        Debug.Log("OnRecvCustomMsg");
        CustomMessage msg = new CustomMessage();
        msg.Deserialize(message.reader);
        Debug.LogWarning("OnRecvCustomMsg");
        Debug.LogWarning("messageId:" + msg.messageId);
        Debug.LogWarning("content:" + msg.content);
        Debug.LogWarning("vector:" + msg.vector);
        Debug.LogWarning("bytesLength:" + msg.bytes.Length);
        switch (msg.messageId)
        {
            case CustomMessageId.ClientCommand:
                OnMsgClientCommand(message.conn, msg);
                break;
        }
    }

    public bool StartServer(int port)
    {
        NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);
        NetworkServer.RegisterHandler(MsgType.Disconnect, OnClientDisConnected);
        NetworkServer.RegisterHandler(MsgType.Error, OnError);
        NetworkServer.RegisterHandler(CustomMsgTypes.InGameMsg, OnRecvCustomMsg);
        bool succeed = NetworkServer.Listen(port);
        if (succeed)
            Debug.Log("服务器启动成功");
        else
            Debug.LogError(string.Format("服务器无法启动，端口为{0}", port));
        m_gameWorld = new GameWorld();
        m_gameWorld.Initialize();
        return succeed;
    }

    public void Step()
    {
        m_gameWorld.Step();
        if (m_gameWorld.m_frame % m_sendRate == 0)
        {
            SyncStateToClient();
        }
    }

    public void SyncStateToClient()
    {
        var packet = m_packetPool.Get();
        foreach (var entity in m_gameWorld.m_entityList)
        {
            entity.m_currentState.Pack(packet);
        }
        CustomMessage stateBroardcastMsg = new CustomMessage();
        stateBroardcastMsg.messageId = CustomMessageId.StateBroadcast;
        stateBroardcastMsg.bytes = packet.data;
        foreach (var connect in m_connections)
        {
            connect.Send(CustomMsgTypes.InGameMsg, stateBroardcastMsg);
        }
        m_packetPool.Back(packet);
    }

    int m_sendRate = 10;
    GameWorld m_gameWorld;
    List<NetworkConnection> m_connections = new List<NetworkConnection>();
    Dictionary<NetworkConnection, Connection> m_connDic = new Dictionary<NetworkConnection, Connection>();
    PacketPool m_packetPool = new PacketPool(64, 256);
}
