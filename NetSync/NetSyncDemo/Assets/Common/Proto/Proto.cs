using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public interface IProto
{
    short GetProtoType();
}

public abstract class ProtoMessageBase : MessageBase, IProto
{
    public abstract short GetProtoType();
}

public class ProtoType
{
    public const short NONE = 1003;
    public const short AllocateTargetNtf = 1004;
    public const short StateBroadcastNtf = 1005;
    public const short ClientCommandUpload = 1006;
}

public class ProtoAllocateTargetNtf : ProtoMessageBase
{
    public int m_targetId;

    public override short GetProtoType()
    {
        return ProtoType.AllocateTargetNtf;
    }

    public ProtoAllocateTargetNtf() { }

    public ProtoAllocateTargetNtf(int targetId)
    {
        m_targetId = targetId;
    }

    public override void Serialize(NetworkWriter writer)
    {
        writer.Write(m_targetId); 
    }

    public override void Deserialize(NetworkReader reader)
    {
        m_targetId = reader.ReadInt32();
    }
}

public class ProtoStateBroadcastNtf : ProtoMessageBase
{
    public int m_count;
    public int[] m_ids;
    public Vector3[] m_positions;

    public override short GetProtoType()
    {
        return ProtoType.StateBroadcastNtf;
    }

    public ProtoStateBroadcastNtf(int count,int[] ids,Vector3[] positions)
    {
        m_count = count;
        m_ids = ids;
        m_positions = positions;
    }

    public override void Serialize(NetworkWriter writer)
    {
        writer.Write(m_count);
        for(int i = 0; i < m_count; i++)
        {
            writer.Write(m_ids[i]);
            writer.Write(m_positions[i]);
        }
    }

    public override void Deserialize(NetworkReader reader)
    {
        m_count = reader.ReadInt32();
        m_ids = new int[m_count];
        m_positions = new Vector3[m_count];
        for(int i = 0; i < m_count; i++)
        {
            m_ids[i] = reader.ReadInt32();
            m_positions[i] = reader.ReadVector3();
        }
    }
}

public class ProtoClientCommandUpload : ProtoMessageBase
{
    public byte[] data;

    public override short GetProtoType()
    {
        return ProtoType.ClientCommandUpload;
    }

    public ProtoClientCommandUpload() { }

    public ProtoClientCommandUpload(Packet packet)
    {
        m_commands = commands;
    }

    public override void Serialize(NetworkWriter writer)
    {
        var count = m_commands.Count;
        writer.Write(count);
        for(int i = 0; i < count; i++)
        {
            writer.Write(m_commands[i]);
        }
    }

    public override void Deserialize(NetworkReader reader)
    {
        var count = reader.ReadInt32();
        m_commands = new List<ProtoClientCommand>();
        for (int i = 0; i < count; i++)
        {
            var command = reader.ReadMessage<ProtoClientCommand>();
            m_commands.Add(command);
        }
    }
}


