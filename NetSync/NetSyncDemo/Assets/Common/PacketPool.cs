using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketPool {
    private int m_size;
    private int m_packetSize;
    private List<Packet> m_packets = new List<Packet>();

	public PacketPool(int size,int packetSize)
    {
        m_size = size;
        m_packetSize = packetSize;
        for(int i = 0; i < m_size; i++)
        {
            Packet packet = new Packet(new byte[packetSize]);
            m_packets.Add(packet);
        }
    }

    public Packet Get()
    {
        if (m_packets.Count > 0)
        {
            Packet p = m_packets[0];
            m_packets.RemoveAt(0);
            p.ptr = 0;
            return p;
        }
        else
        {
            Packet packet = new Packet(new byte[m_packetSize]);
            return packet;
        }
    }

    public void Back(Packet packet)
    {
        if(!m_packets.Contains(packet))
            m_packets.Add(packet);
    }
}
