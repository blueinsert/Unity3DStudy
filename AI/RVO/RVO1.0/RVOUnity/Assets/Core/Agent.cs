﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RVO
{
    public class Agent
    {
        public float m_safetyFactor;
        public int m_velSampleCount;
        public int m_angleSampleCount;
        public float m_neighborDist;
        public int m_maxNeighbors = 0;
        public float m_maxSpeed;
        public float m_maxAccel;
        public float m_radius;
        public float m_prefSpeed;
        public Vector2 m_position;
        public Vector2 m_goalPosition;
        public Vector2 m_vel;
        public float m_orient;
        public Vector2 m_velPref;
        public Vector2 m_velNew;
        public int m_priority = 1;
        public bool m_isCollide = false;
        public IList<KeyValuePair<float, Agent>> m_agentNeighbors = new List<KeyValuePair<float, Agent>>();
        private Simulator m_sim;

        public Agent() { }

        public Agent(Vector2 pos, Vector2 goalPos)
        {
            m_position = pos;
            m_goalPosition = goalPos;
            this.m_safetyFactor = Simulator.Instance.m_agentDefault.m_safetyFactor;
            this.m_velSampleCount = Simulator.Instance.m_agentDefault.m_velSampleCount;
            this.m_angleSampleCount = Simulator.Instance.m_agentDefault.m_angleSampleCount;
            this.m_neighborDist = Simulator.Instance.m_agentDefault.m_neighborDist;
            this.m_maxNeighbors = Simulator.Instance.m_agentDefault.m_maxNeighbors;
            this.m_maxSpeed = Simulator.Instance.m_agentDefault.m_maxSpeed;
            this.m_maxAccel = Simulator.Instance.m_agentDefault.m_maxAccel;
            this.m_prefSpeed = Simulator.Instance.m_agentDefault.m_prefSpeed;
            this.m_radius = Simulator.Instance.m_agentDefault.m_radius;
            m_sim = Simulator.Instance;
        }

        public Agent(Vector2 pos)
        {
            m_position = pos;
            this.m_safetyFactor = Simulator.Instance.m_agentDefault.m_safetyFactor;
            this.m_velSampleCount = Simulator.Instance.m_agentDefault.m_velSampleCount;
            this.m_angleSampleCount = Simulator.Instance.m_agentDefault.m_angleSampleCount;
            this.m_neighborDist = Simulator.Instance.m_agentDefault.m_neighborDist;
            this.m_maxNeighbors = Simulator.Instance.m_agentDefault.m_maxNeighbors;
            this.m_maxSpeed = Simulator.Instance.m_agentDefault.m_maxSpeed;
            this.m_maxAccel = Simulator.Instance.m_agentDefault.m_maxAccel;
            this.m_prefSpeed = Simulator.Instance.m_agentDefault.m_prefSpeed;
            this.m_radius = Simulator.Instance.m_agentDefault.m_radius;
            m_sim = Simulator.Instance;
        }

        public void SetGoalPos(Vector2 pos)
        {
            m_goalPosition = pos;
        }

        private void DoInsertAgentNeighbor(Agent agent, float distSq, ref float rangeSq)
        {
            if (m_agentNeighbors.Count < m_maxNeighbors)
            {
                m_agentNeighbors.Add(new KeyValuePair<float, Agent>(distSq, agent));
            }

            int i = m_agentNeighbors.Count - 1;
            //插入排序，距离近的排前面
            while (i != 0 && distSq < m_agentNeighbors[i - 1].Key)
            {
                m_agentNeighbors[i] = m_agentNeighbors[i - 1];
                --i;
            }

            m_agentNeighbors[i] = new KeyValuePair<float, Agent>(distSq, agent);

            if (m_agentNeighbors.Count == m_maxNeighbors)
            {
                //邻居数量已到达最大数量，更新邻居的最远距离
                //这种情况下，只有更近的单位才能加入到邻居集合中
                rangeSq = m_agentNeighbors[m_agentNeighbors.Count - 1].Key;
            }
        }

        internal void insertAgentNeighbor(Agent agent, ref float rangeSq)
        {
            if (this != agent)
            {
                var v = m_position - agent.m_position;
                float distSq = v.x * v.x + v.y * v.y;
                if (distSq < (this.m_radius + agent.m_radius) * (this.m_radius + agent.m_radius) && distSq < rangeSq)
                {//发生碰撞
                    if (m_isCollide == false)
                    {
                        m_isCollide = true;
                        m_agentNeighbors.Clear();
                    }
                    //发生碰撞时，邻居集合中只记录了与之碰撞的单位
                    DoInsertAgentNeighbor(agent, distSq, ref rangeSq);
                }
                else if (!m_isCollide && distSq < rangeSq)
                {
                    DoInsertAgentNeighbor(agent, distSq, ref rangeSq);
                }

            }
        }

        internal void computePreferredVelocity()
        {
            var dist = (m_position - m_goalPosition).magnitude;
            if (m_prefSpeed * Simulator.Instance.m_timeStep > dist)
            {
                m_velPref = (m_goalPosition - m_position) / Simulator.Instance.m_timeStep;
            }
            else
            {
                m_velPref = (m_goalPosition - m_position).normalized * m_prefSpeed;
            }
        }

        internal void computeNeighbors()
        {
            m_isCollide = false;
            m_priority = 1;
            m_agentNeighbors.Clear();
            float rangeSq = m_neighborDist * m_neighborDist;
            Simulator.Instance.m_kdTree.computeAgentNeighbors(this, ref rangeSq);
            //if (m_isCollide)
            //{
            //    Debug.LogError(string.Format("agent isCollide"));
            //    m_priority = 99;
            //}
        }

        /// <summary>
        /// Time to collision of a ray to a disc.
        /// </summary>
        /// <param name="p">The start position of the ray</param>
        /// <param name="v">The velocity vector of the ray</param>
        /// <param name="p2">The center position of the disc</param>
        /// <param name="radius">The radius of the disc</param>
        /// <param name="collision">Specifies whether the time to collision is computed (false), or the time from collision (true).计算将要发生碰撞的时间，或者发生碰撞后经过的时间</param>
        /// <returns>Returns the time to collision of ray p + tv to disc D(p2, radius), and #RVO_INFTY when the disc is not hit. If collision is true, the value is negative.</returns>
        //点到直线距离公式：d = |AX0+BY0+C|/sqrt(A*A+B*B)
        //直线AX+BY+C=0,A=-V.y B=V.x C =P.x*V.y-V.x*P.y
        //可能碰撞：d<r=>d*d-r*r<0
        //p2带入AX0+BY0+C
        //AX0+BY0+C = -v.y*p2.x+v.x*p2.y+p.x*v.y-v.x*p.y=v.x(p2.y-p.y)-v.y(p2.x-p.x)=det(v, ba)
        //d*d = det(v, ba)*det(v, ba)/absSq(v)
        //r*r-d*d>0 => r*r-det(v, ba)*det(v, ba)/absSq(v)>0 => discr = r*r*absSq(v) - det(v, ba)*det(v, ba)>0
        //
        private float time2Collision(Vector2 p, Vector2 v, Vector2 p2, float radius, bool collision)
        {
            Vector2 ba = p2 - p;
            float sq_diam = radius * radius;
            float time;

            float discr = -RVOMath.Sqr(RVOMath.Det(v, ba)) + sq_diam * RVOMath.AbsSq(v);
            if (discr > 0)
            {
                if (collision)
                {
                    time = (Vector2.Dot(v, ba) + Mathf.Sqrt(discr)) / RVOMath.AbsSq(v);//脱落碰撞需要的时间
                    if (time < 0)
                    {
                        time = -float.MaxValue;//
                        Debug.LogError("isCollide time < 0 ");//?
                    }
                }
                else
                {
                    time = (Vector2.Dot(v, ba) - Mathf.Sqrt(discr)) / RVOMath.AbsSq(v);//将要发生碰撞的时间
                    if (time < 0)
                    {
                        time = float.MaxValue;//永远不会发生碰撞
                    }
                }
            }
            else
            {//不碰撞
                if (collision)
                {
                    time = -float.MaxValue;
                }
                else
                {
                    time = float.MaxValue;
                }
            }
            return time;
        }

        internal void computeNewVelocity()
        {
            float minPenalty = float.MaxValue;
            Vector2 velCand;
            for (int i = 0; i < m_angleSampleCount; i++)
            {
                float angle = m_orient + i / (float)m_angleSampleCount * 2 * Mathf.PI;
                for (int j = 0; j < m_velSampleCount; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        velCand = m_velPref;
                    }
                    else
                    {
                        float radius = (j + 1) / (float)m_velSampleCount;
                        velCand = new Vector2(radius * m_maxSpeed * Mathf.Cos(angle), radius * m_maxSpeed * Mathf.Sin(angle));
                    }
                    float dv = 0;
                    dv = (velCand - m_velPref).magnitude;
                    if (m_isCollide)
                        dv = 0;
                    float collisionTime = float.MaxValue;
                    foreach (var pair in m_agentNeighbors)
                    {
                        var distSq = pair.Key;
                        var other = pair.Value;
                        //Vector2 vab = velCand - other.m_vel; //VO
                        float effort = 1 - this.m_priority / (float)(this.m_priority + other.m_priority);
                        float effortInv = 1 / effort;
                        Vector2 vab = effortInv * velCand + (1 - effortInv) * m_vel - other.m_vel;//RVO
                        float time = time2Collision(m_position, vab, other.m_position, m_radius + other.m_radius, m_isCollide);
                        if (m_isCollide)
                        {
                            time = -Mathf.Ceil(time / m_sim.m_timeStep);
                            time -= RVOMath.Sqr(velCand.magnitude / m_maxSpeed);
                        }
                        if (time < collisionTime)
                        {
                            collisionTime = time;
                            if (m_safetyFactor / collisionTime + dv > minPenalty)
                                break;
                        }
                    }
                    float penalty = m_safetyFactor / collisionTime + dv;
                    if (penalty <= minPenalty - 0.001f)
                    {
                        minPenalty = penalty;
                        m_velNew = velCand;
                    }

                }
            }

        }

        internal void update()
        {
            float dv = (m_velNew - m_vel).magnitude;
            if (dv < m_maxAccel * Simulator.Instance.m_timeStep)
            {
                m_vel = m_velNew;
            }
            else
            {
                var l = m_maxAccel * Simulator.Instance.m_timeStep / dv;
                m_vel = Vector2.Lerp(m_vel, m_velNew, 1 - l);
            }
            m_position += m_vel * Simulator.Instance.m_timeStep;

            m_orient = Mathf.Atan2(m_vel.y, m_vel.x);
        }
    }
}
