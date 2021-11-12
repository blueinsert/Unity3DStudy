using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RVO
{
	public class Simulator
	{
        public static Simulator Instance
        {
            get
            {
                return s_instance_;
            }
        }

        private static Simulator s_instance_ = new Simulator();
        internal IList<Agent> m_agents;
        internal KdTree m_kdTree = new KdTree();
        internal Agent m_agentDefault = new Agent();
        internal float m_timeStep = 0.25f;

        private Simulator() {
            m_agents = new List<Agent>();
        }

        public void DoStep()
        {
            m_kdTree.buildAgentTree();
            for (int i = 0; i < m_agents.Count; i++)
            {
                m_agents[i].computePreferredVelocity();
                m_agents[i].computeNeighbors();
                m_agents[i].computeNewVelocity();
            }
            for (int i = 0; i < m_agents.Count; i++)
            {
                m_agents[i].update();
            }
        }

        public void SetTimeStep(float timeStep)
        {
            m_timeStep = timeStep;
        }

        public void SetAgentDefaults(float saftyFactor, int velSampleCount, float neighborDist,int maxNeighbors,float maxSpeed, float maxAccel,float prefSpeed, float radius)
        {
            m_agentDefault.m_safetyFactor = saftyFactor;
            m_agentDefault.m_velSampleCount = velSampleCount;
            m_agentDefault.m_neighborDist = neighborDist;
            m_agentDefault.m_maxNeighbors = maxNeighbors;
            m_agentDefault.m_maxSpeed = maxSpeed;
            m_agentDefault.m_maxAccel = maxAccel;
            m_agentDefault.m_prefSpeed = prefSpeed;
            m_agentDefault.m_radius = radius;
        }

        public int AddAgent(Vector2 startPos, Vector2 goalPos)
        {
            Agent agent = new Agent(startPos, goalPos);
            m_agents.Add(agent);
            return m_agents.Count - 1;
        }

        public int AddAgent(Vector2 startPos)
        {
            Agent agent = new Agent(startPos);
            m_agents.Add(agent);
            return m_agents.Count - 1;
        }

        public void ClearAgents() {
            m_agents.Clear();
        }

        public Vector2 GetAgentPos(int id)
        {
            return m_agents[id].m_position;
        }

        public Vector2 GetAgentVel(int id)
        {
            return m_agents[id].m_vel;
        }

        public float GetAgentOrient(int id)
        {
            return m_agents[id].m_orient;
        }

        public float GetAgentRadius(int id)
        {
            return m_agents[id].m_radius;
        }

        public void SetAgentTarget(int id,Vector2 target)
        {
            m_agents[id].SetGoalPos(target);
        }

    }
}
