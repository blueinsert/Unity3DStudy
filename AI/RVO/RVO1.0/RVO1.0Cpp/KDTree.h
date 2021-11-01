#ifndef __KDTREE_H__
#define __KDTREE_H__

#include "RVODef.h"

#define HORIZONTAL_SPLIT -1
#define VERTICAL_SPLIT -2

namespace RVO {
  class KDTree
  {

  struct AgentTreeNode {
    int agentID; // is negative when vertical or horizontal split
    //float splitValue;
    float minX, maxX, minY, maxY;
    int left;
    int right;
  };

  struct ObstacleTreeNode {
    int obstacleID; // is negative when trivial empty node
    ObstacleTreeNode* left;
    ObstacleTreeNode* right;
  };

  private:
    KDTree(void);
    ~KDTree(void);

    std::vector<AgentTreeNode> _agentTree;
    std::vector<int> _agentIDs;

    void buildAgentTreeRecursive(int begin, int end, int node);
    inline void buildAgentTree() { if (!_agentIDs.empty()) buildAgentTreeRecursive(0, (int) _agentIDs.size(), 0); }
    void queryAgentTreeRecursive(Agent* agent, float& rangeSq, int node) const;
    inline void computeAgentNeighbors(Agent* agent, float& rangeSq) const { queryAgentTreeRecursive(agent, rangeSq, 0); }

    ObstacleTreeNode* _obstacleTree;
    void deleteObstacleTree(ObstacleTreeNode* node);
    ObstacleTreeNode* buildObstacleTreeRecursive(const std::vector<int>& obstacles);
    void buildObstacleTree();
    void queryObstacleTreeRecursive(Agent* agent, float& rangeSq, ObstacleTreeNode* node) const;
    inline void computeObstacleNeighbors(Agent* agent, float& rangeSq) const { queryObstacleTreeRecursive(agent, rangeSq, _obstacleTree); }

  protected:
    /* A reference to the singleton simulator. */
    static RVOSimulator*  _sim;

    friend class Agent;
    friend class RVOSimulator;
  };
}

#endif