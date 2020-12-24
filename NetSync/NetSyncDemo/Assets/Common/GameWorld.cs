using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld {
    public int m_frame;
    public List<Entity> m_entityList = new List<Entity>();
    public Dictionary<int, Entity> m_entityDic = new Dictionary<int, Entity>();
    public List<Command> m_commands = new List<Command>();
    public List<Command> m_toExeCommands = new List<Command>();
    public List<Command> m_tempCommands = new List<Command>();

    public void AddCommandsToExe(List<Command> commands)
    {
        m_toExeCommands.AddRange(commands);
    }

    public void AddCommandToExe(Command command)
    {
        m_toExeCommands.Add(command);
    }

    public void Initialize() {
        Entity[] entities = GameObject.FindObjectsOfType<Entity>();
        m_entityList.AddRange(entities);
        foreach(var entity in m_entityList)
        {
            m_entityDic.Add(entity.m_id, entity);
        }
    }

    public void Step()
    {
        m_frame++;
        m_tempCommands.Clear();
        for(int i = 0; i < m_toExeCommands.Count; i++)
        {
            var command = m_toExeCommands[i];
            if (command.sequence <= m_frame)
            {
                Entity target;
                if (m_entityDic.TryGetValue(command.target, out target)) {
                    target.ExecuteCommand(command);
                    m_tempCommands.Add(command);
                } 
            }
        }
        foreach(var command in m_tempCommands)
        {
            m_toExeCommands.Remove(command);
            m_commands.Add(command);
        }
    }
}
