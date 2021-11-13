using System.Collections.Generic;
using System;

namespace RVO
{
    /**
     * <summary>Defines k-D trees for agents and static obstacles in the
     * simulation.</summary>
     */
    internal class KdTree
    {
        /**
         * <summary>Defines a node of an agent k-D tree.</summary>
         */
        ///end-begin+1<MAX_LEAF_SIZE时为叶子节点，
        ///非叶子本身不存放数据，只是用来拆分空间
        ///叶子节点的数据就是begin-end之间指示的数据
        private struct AgentTreeNode
        {
            internal int begin_;//agent数组中开始索引
            internal int end_;//agent数组中结束索引
            internal int left_;//树节点数组中左子树索引
            internal int right_;//树节点数组中右子树索引
            //范围大小
            internal float maxX_;
            internal float maxY_;
            internal float minX_;
            internal float minY_;
        }

        /**
         * <summary>Defines a pair of scalar values.</summary>
         */
        private struct FloatPair
        {
            private float a_;
            private float b_;

            /**
             * <summary>Constructs and initializes a pair of scalar
             * values.</summary>
             *
             * <param name="a">The first scalar value.</returns>
             * <param name="b">The second scalar value.</returns>
             */
            internal FloatPair(float a, float b)
            {
                a_ = a;
                b_ = b;
            }

            /**
             * <summary>Returns true if the first pair of scalar values is less
             * than the second pair of scalar values.</summary>
             *
             * <returns>True if the first pair of scalar values is less than the
             * second pair of scalar values.</returns>
             *
             * <param name="pair1">The first pair of scalar values.</param>
             * <param name="pair2">The second pair of scalar values.</param>
             */
            public static bool operator <(FloatPair pair1, FloatPair pair2)
            {
                return pair1.a_ < pair2.a_ || !(pair2.a_ < pair1.a_) && pair1.b_ < pair2.b_;
            }

            /**
             * <summary>Returns true if the first pair of scalar values is less
             * than or equal to the second pair of scalar values.</summary>
             *
             * <returns>True if the first pair of scalar values is less than or
             * equal to the second pair of scalar values.</returns>
             *
             * <param name="pair1">The first pair of scalar values.</param>
             * <param name="pair2">The second pair of scalar values.</param>
             */
            public static bool operator <=(FloatPair pair1, FloatPair pair2)
            {
                return (pair1.a_ == pair2.a_ && pair1.b_ == pair2.b_) || pair1 < pair2;
            }

            /**
             * <summary>Returns true if the first pair of scalar values is
             * greater than the second pair of scalar values.</summary>
             *
             * <returns>True if the first pair of scalar values is greater than
             * the second pair of scalar values.</returns>
             *
             * <param name="pair1">The first pair of scalar values.</param>
             * <param name="pair2">The second pair of scalar values.</param>
             */
            public static bool operator >(FloatPair pair1, FloatPair pair2)
            {
                return !(pair1 <= pair2);
            }

            /**
             * <summary>Returns true if the first pair of scalar values is
             * greater than or equal to the second pair of scalar values.
             * </summary>
             *
             * <returns>True if the first pair of scalar values is greater than
             * or equal to the second pair of scalar values.</returns>
             *
             * <param name="pair1">The first pair of scalar values.</param>
             * <param name="pair2">The second pair of scalar values.</param>
             */
            public static bool operator >=(FloatPair pair1, FloatPair pair2)
            {
                return !(pair1 < pair2);
            }
        }

        /**
         * <summary>The maximum size of an agent k-D tree leaf.</summary>
         */
        private const int MAX_LEAF_SIZE = 10;

        private Agent[] agents_;
        private AgentTreeNode[] agentTree_;

        internal void Clear()
        {
            agents_ = null;
        }

        /**
         * <summary>Builds an agent k-D tree.</summary>
         */
        internal void buildAgentTree()
        {
            if (agents_ == null || agents_.Length != Simulator.Instance.m_agents.Count)
            {
                agents_ = new Agent[Simulator.Instance.m_agents.Count];

                for (int i = 0; i < agents_.Length; ++i)
                {
                    agents_[i] = Simulator.Instance.m_agents[i];
                }

                agentTree_ = new AgentTreeNode[2 * agents_.Length];//?

                for (int i = 0; i < agentTree_.Length; ++i)
                {
                    agentTree_[i] = new AgentTreeNode();
                }
            }

            if (agents_.Length != 0)
            {
                buildAgentTreeRecursive(0, agents_.Length, 0);
            }
        }


        /**
         * <summary>Computes the agent neighbors of the specified agent.
         * </summary>
         *
         * <param name="agent">The agent for which agent neighbors are to be
         * computed.</param>
         * <param name="rangeSq">The squared range around the agent.</param>
         */
        internal void computeAgentNeighbors(Agent agent, ref float rangeSq)
        {
            queryAgentTreeRecursive(agent, ref rangeSq, 0);
        }

        /**
         * <summary>Recursive method for building an agent k-D tree.</summary>
         *
         * <param name="begin">The beginning agent k-D tree node node index.
         * </param>
         * <param name="end">The ending agent k-D tree node index.</param>
         * <param name="node">The current agent k-D tree node index.</param>
         */
        private void buildAgentTreeRecursive(int begin, int end, int node)
        {
            agentTree_[node].begin_ = begin;
            agentTree_[node].end_ = end;
            agentTree_[node].minX_ = agentTree_[node].maxX_ = agents_[begin].m_position.x;
            agentTree_[node].minY_ = agentTree_[node].maxY_ = agents_[begin].m_position.y;

            for (int i = begin + 1; i < end; ++i)
            {
                agentTree_[node].maxX_ = Math.Max(agentTree_[node].maxX_, agents_[i].m_position.x);
                agentTree_[node].minX_ = Math.Min(agentTree_[node].minX_, agents_[i].m_position.x);
                agentTree_[node].maxY_ = Math.Max(agentTree_[node].maxY_, agents_[i].m_position.y);
                agentTree_[node].minY_ = Math.Min(agentTree_[node].minY_, agents_[i].m_position.y);
            }

            if (end - begin > MAX_LEAF_SIZE)
            {
                /* No leaf node. */
                bool isVertical = agentTree_[node].maxX_ - agentTree_[node].minX_ > agentTree_[node].maxY_ - agentTree_[node].minY_;
                float splitValue = 0.5f * (isVertical ? agentTree_[node].maxX_ + agentTree_[node].minX_ : agentTree_[node].maxY_ + agentTree_[node].minY_);

                int left = begin;
                int right = end;

                while (left < right)
                {
                    while (left < right && (isVertical ? agents_[left].m_position.x : agents_[left].m_position.y) < splitValue)
                    {
                        ++left;
                    }

                    while (right > left && (isVertical ? agents_[right - 1].m_position.x : agents_[right - 1].m_position.y) >= splitValue)
                    {
                        --right;
                    }

                    if (left < right)
                    {
                        Agent tempAgent = agents_[left];
                        agents_[left] = agents_[right - 1];
                        agents_[right - 1] = tempAgent;
                        ++left;
                        --right;
                    }
                }

                int leftSize = left - begin;

                if (leftSize == 0)
                {
                    ++leftSize;
                    ++left;
                    ++right;
                }

                agentTree_[node].left_ = node + 1;
                agentTree_[node].right_ = node + 2 * leftSize;//?

                buildAgentTreeRecursive(begin, left, agentTree_[node].left_);
                buildAgentTreeRecursive(left, end, agentTree_[node].right_);
            }
        }

        /**
         * <summary>Recursive method for computing the agent neighbors of the
         * specified agent.</summary>
         *
         * <param name="agent">The agent for which agent neighbors are to be
         * computed.</param>
         * <param name="rangeSq">The squared range around the agent.</param>
         * <param name="node">The current agent k-D tree node index.</param>
         */
        private void queryAgentTreeRecursive(Agent agent, ref float rangeSq, int node)
        {
            if (agentTree_[node].end_ - agentTree_[node].begin_ <= MAX_LEAF_SIZE)
            {
                for (int i = agentTree_[node].begin_; i < agentTree_[node].end_; ++i)
                {
                    agent.insertAgentNeighbor(agents_[i], ref rangeSq);
                }
            }
            else
            {
                //在范围内部返回零，在外边返回距离矩形的最短长度平方
                float distSqLeft = RVOMath.Sqr(Math.Max(0.0f, agentTree_[agentTree_[node].left_].minX_ - agent.m_position.x)) 
                    + RVOMath.Sqr(Math.Max(0.0f, agent.m_position.x - agentTree_[agentTree_[node].left_].maxX_)) 
                    + RVOMath.Sqr(Math.Max(0.0f, agentTree_[agentTree_[node].left_].minY_ - agent.m_position.y)) 
                    + RVOMath.Sqr(Math.Max(0.0f, agent.m_position.y - agentTree_[agentTree_[node].left_].maxY_));
                float distSqRight = RVOMath.Sqr(Math.Max(0.0f, agentTree_[agentTree_[node].right_].minX_ - agent.m_position.x)) 
                    + RVOMath.Sqr(Math.Max(0.0f, agent.m_position.x - agentTree_[agentTree_[node].right_].maxX_))
                    + RVOMath.Sqr(Math.Max(0.0f, agentTree_[agentTree_[node].right_].minY_ - agent.m_position.y)) 
                    + RVOMath.Sqr(Math.Max(0.0f, agent.m_position.y - agentTree_[agentTree_[node].right_].maxY_));

                if (distSqLeft < distSqRight)
                {
                    if (distSqLeft < rangeSq)
                    {
                        queryAgentTreeRecursive(agent, ref rangeSq, agentTree_[node].left_);

                        if (distSqRight < rangeSq)
                        {
                            queryAgentTreeRecursive(agent, ref rangeSq, agentTree_[node].right_);
                        }
                    }
                }
                else
                {
                    if (distSqRight < rangeSq)
                    {
                        queryAgentTreeRecursive(agent, ref rangeSq, agentTree_[node].right_);

                        if (distSqLeft < rangeSq)
                        {
                            queryAgentTreeRecursive(agent, ref rangeSq, agentTree_[node].left_);
                        }
                    }
                }

            }
        }

    }
}
