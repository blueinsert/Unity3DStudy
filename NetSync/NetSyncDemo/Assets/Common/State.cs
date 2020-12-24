using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public Entity entity;                                          //所属的Entity

    public State(Entity e)
    {
        entity = e;
    }

    public int Pack(Packet packet)
    {
        //packet.Write(frame);
        //将属性数据写入消息包packet
        return 0;
    }
    public int Read(Packet packet)
    {
        //frame = packet.ReadInt();
        //从消息包中取出属性数据

        return 0;
    }
}
