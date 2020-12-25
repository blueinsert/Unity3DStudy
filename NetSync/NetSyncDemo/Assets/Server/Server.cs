using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Server {

    private void OnClientConnected(NetworkMessage message)
    {
        Debug.Log(string.Format("OnClientConnected"));
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
        connection.SendTargetId();
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


    private void OnClientCommandUpload(NetworkMessage message)
    {
        Debug.Log("Server:OnClientCommandUpload");
        Connection connection = m_connDic[message.conn];
        ProtoClientCommandUpload msg = new ProtoClientCommandUpload();
        msg.Deserialize(message.reader);
        List<Command> commands = new List<Command>();
        for (int i = 0; i < msg.m_commands.Count; i++)
        {
            var commandProto = msg.m_commands[i];
            Command command = new Command();
            command.sequence = commandProto.m_frame;
            command.input = new CommandInput();
            command.input.left = commandProto.m_left;
            command.input.right = commandProto.m_right;
            command.input.forward = commandProto.m_forward;
            command.input.back = commandProto.m_backward;
            command.target = connection.m_targetId;
            commands.Add(command);
        }
        m_gameWorld.AddCommandsToExe(commands);
    }

    public bool StartServer(int port)
    {
        NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);
        NetworkServer.RegisterHandler(MsgType.Disconnect, OnClientDisConnected);
        NetworkServer.RegisterHandler(MsgType.Error, OnError);
        NetworkServer.RegisterHandler(ProtoType.ClientCommandUpload, OnClientCommandUpload);
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
        
    }

    int m_sendRate = 10;
    GameWorld m_gameWorld;
    List<NetworkConnection> m_connections = new List<NetworkConnection>();
    Dictionary<NetworkConnection, Connection> m_connDic = new Dictionary<NetworkConnection, Connection>();
}
