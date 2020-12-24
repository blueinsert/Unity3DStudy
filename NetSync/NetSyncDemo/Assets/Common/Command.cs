using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInput {
    public bool left;
    public bool right;
    public bool forward;
    public bool back;
}

public class CommandResult {
    public Vector3 position;
}

public class CommandFlags
{
    public const int HAS_EXECUTED = 1 << 0;
}

public class Command {
    public int flag;
    public int target;//entityId
    public uint sequence;
    public CommandInput input;
    public CommandResult result;

    public void Read(Packet packet)
    {
        sequence = packet.ReadUint();
        input = new CommandInput();
        input.left = packet.ReadBool();
        input.right = packet.ReadBool();
        input.forward = packet.ReadBool();
        input.back = packet.ReadBool();
    }

    public void Packet(Packet packet)
    {
        packet.WriteUint(sequence);
        packet.WriteBool(input.left);
        packet.WriteBool(input.right);
        packet.WriteBool(input.forward);
        packet.WriteBool(input.back);
    }

}
