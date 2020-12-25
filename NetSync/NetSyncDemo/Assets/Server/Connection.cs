using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

public class Connection
{
    public Connection(int id)
    {
        m_id = id;
    }

    public void Initialize(NetworkConnection conn)
    {
        m_conn = conn;
    }

    public void SetTargetId(int targetId)
    {
        m_targetId = targetId;
    }

    public void SendTargetId()
    {
        ProtoAllocateTargetNtf msg = new ProtoAllocateTargetNtf(m_targetId);
        Send(msg);
    }

    public void Send(ProtoMessageBase message)
    {
        m_conn.Send(message.GetProtoType(), message);
    }

    public int m_targetId = -1;//entity id
    public int m_id;
    public NetworkConnection m_conn;

}

