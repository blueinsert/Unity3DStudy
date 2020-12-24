using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomMsgTypes
{
    public const short InGameMsg = 1003;
}

public class CustomMessageId
{
    public const int StateBroadcast = 10;
    public const int ClientCommand = 20;
}

public class CustomMessage : MessageBase {
    public int messageId;
    public string content;
    public Vector3 vector;
    public byte[] bytes;

    public override void Serialize(NetworkWriter writer)
    {
        writer.Write(messageId);
        writer.Write(content);
        writer.Write(vector);
        writer.WriteBytesAndSize(bytes, bytes.Length);
    }

    public override void Deserialize(NetworkReader reader)
    {
        messageId = reader.ReadInt32();
        content = reader.ReadString();
        vector = reader.ReadVector3();
        ushort count = reader.ReadUInt16();
        bytes = reader.ReadBytes(count);
    }
}
