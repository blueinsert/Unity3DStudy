#include "RVOSimulator.h"
#include "Agent.h"
#include "Obstacle.h"
#include "KDTree.h"

namespace RVO {
  RVOSimulator*  KDTree::_sim = RVOSimulator::Instance();

  KDTree::KDTree(void)
  {
    for (int i = 0; i < (int) _sim->_agents.size(); ++i) {
      _agentIDs.push_back(i);
    }
    _agentTree.resize(2*_sim->_agents.size()-1);

    _obstacleTree = 0;
  }

  KDTree::~KDTree(void)
  {
    if (_obstacleTree != 0) {
      deleteObstacleTree(_obstacleTree);
    }
  }

  void KDTree::buildAgentTreeRecursive(int begin, int end, int node) {
    _agentTree[node].minX = _agentTree[node].maxX = _sim->_agents[_agentIDs[begin]]->_p.x();
    _agentTree[node].minY = _agentTree[node].maxY = _sim->_agents[_agentIDs[begin]]->_p.y();
    if (end - begin == 1) { // leaf node
      _agentTree[node].agentID = _agentIDs[begin];
    } else {
      for (int i = begin + 1; i != end; ++i) {
        if (_sim->_agents[_agentIDs[i]]->_p.x() > _agentTree[node].maxX) {
          _agentTree[node].maxX = _sim->_agents[_agentIDs[i]]->_p.x();
        } else if (_sim->_agents[_agentIDs[i]]->_p.x() < _agentTree[node].minX) {
          _agentTree[node].minX = _sim->_agents[_agentIDs[i]]->_p.x();
        }
        if (_sim->_agents[_agentIDs[i]]->_p.y() > _agentTree[node].maxY) {
          _agentTree[node].maxY = _sim->_agents[_agentIDs[i]]->_p.y();
        } else if (_sim->_agents[_agentIDs[i]]->_p.y() < _agentTree[node].minY) {
          _agentTree[node].minY = _sim->_agents[_agentIDs[i]]->_p.y();
        }
      }

      bool vertical = (_agentTree[node].maxX - _agentTree[node].minX > _agentTree[node].maxY - _agentTree[node].minY); // vertical split
      _agentTree[node].agentID = (vertical ? VERTICAL_SPLIT : HORIZONTAL_SPLIT);
      float splitValue = (vertical ? (_agentTree[node].maxX + _agentTree[node].minX) / 2 : (_agentTree[node].maxY + _agentTree[node].minY) / 2);
      //_agentTree[node].splitValue = splitValue;

      int l = begin;
      int r = end - 1;
      while (true) {
        while (l <= r && (vertical ? _sim->_agents[_agentIDs[l]]->_p.x() : _sim->_agents[_agentIDs[l]]->_p.y()) < splitValue) {
          ++l;
        } 
        while (r >= l && (vertical ? _sim->_agents[_agentIDs[r]]->_p.x() : _sim->_agents[_agentIDs[r]]->_p.y()) >= splitValue) {
          --r;
        } 
        if (l > r) {
          break;
        } else {
          std::swap(_agentIDs[l], _agentIDs[r]);
          ++l;
          --r;
        }
      }

      int leftsize = l - begin;
      
      _agentTree[node].left = node + 1;
      _agentTree[node].right = node + 1 + (2 * leftsize - 1);

      buildAgentTreeRecursive(begin, l, _agentTree[node].left);
      buildAgentTreeRecursive(l, end, _agentTree[node].right);
    }
  }

  void KDTree::queryAgentTreeRecursive(Agent* agent, float& rangeSq, int node) const {
    if (_agentTree[node].agentID >= 0) {
      agent->insertAgentNeighbor(_agentTree[node].agentID, rangeSq);
    } else {
      float distSqLeft = 0;
      float distSqRight = 0;
      if (agent->_p.x() < _agentTree[_agentTree[node].left].minX) {
        distSqLeft += sqr(_agentTree[_agentTree[node].left].minX - agent->_p.x());
      } else if (agent->_p.x() > _agentTree[_agentTree[node].left].maxX) {
        distSqLeft += sqr(agent->_p.x() - _agentTree[_agentTree[node].left].maxX);
      }
      if (agent->_p.y() < _agentTree[_agentTree[node].left].minY) {
        distSqLeft += sqr(_agentTree[_agentTree[node].left].minY - agent->_p.y());
      } else if (agent->_p.y() > _agentTree[_agentTree[node].left].maxY) {
        distSqLeft += sqr(agent->_p.y() - _agentTree[_agentTree[node].left].maxY);
      }
      if (agent->_p.x() < _agentTree[_agentTree[node].right].minX) {
        distSqRight += sqr(_agentTree[_agentTree[node].right].minX - agent->_p.x());
      } else if (agent->_p.x() > _agentTree[_agentTree[node].right].maxX) {
        distSqRight += sqr(agent->_p.x() - _agentTree[_agentTree[node].right].maxX);
      }
      if (agent->_p.y() < _agentTree[_agentTree[node].right].minY) {
        distSqRight += sqr(_agentTree[_agentTree[node].right].minY - agent->_p.y());
      } else if (agent->_p.y() > _agentTree[_agentTree[node].right].maxY) {
        distSqRight += sqr(agent->_p.y() - _agentTree[_agentTree[node].right].maxY);
      }
      
      if (distSqLeft < distSqRight) {
        if (distSqLeft < rangeSq) {
          queryAgentTreeRecursive(agent, rangeSq, _agentTree[node].left);
          if (distSqRight < rangeSq) {
            queryAgentTreeRecursive(agent, rangeSq, _agentTree[node].right);
          }
        }
      } else {
        if (distSqRight < rangeSq) {
          queryAgentTreeRecursive(agent, rangeSq, _agentTree[node].right);
          if (distSqLeft < rangeSq) {
            queryAgentTreeRecursive(agent, rangeSq, _agentTree[node].left);
          }
        }
      }
      
    }
    
  }

  void KDTree::deleteObstacleTree(ObstacleTreeNode* node) {
    if (node->obstacleID == -1) {
      delete node;
    } else {
      deleteObstacleTree(node->left);
      deleteObstacleTree(node->right);
      delete node;
    }
  }

  KDTree::ObstacleTreeNode* KDTree::buildObstacleTreeRecursive(const std::vector<int>& obstacles) {
    ObstacleTreeNode* node = new ObstacleTreeNode;
    if (obstacles.empty()) {
      node->obstacleID = -1;
      return node;
    } else {
      int optimalSplit = 0;
      int minLeft = (int) obstacles.size();
      int minRight = (int) obstacles.size();
      
      // Compute optimal split node
      for (int i = 0; i < (int)obstacles.size(); ++i) {
        int leftSize = 0;
        int rightSize = 0;
        for (int j = 0; j < (int)obstacles.size(); ++j) {
          if (i != j) {
            float j1_leftof_i = leftOf(_sim->_obstacles[obstacles[i]]->_p1, _sim->_obstacles[obstacles[i]]->_p2, _sim->_obstacles[obstacles[j]]->_p1);
            float j2_leftof_i = leftOf(_sim->_obstacles[obstacles[i]]->_p1, _sim->_obstacles[obstacles[i]]->_p2, _sim->_obstacles[obstacles[j]]->_p2);
            if (j1_leftof_i >= 0 && j2_leftof_i >= 0) {
              ++leftSize;
            } else if (j1_leftof_i <= 0 && j2_leftof_i <= 0) {
              ++rightSize;
            } else {
              ++leftSize;
              ++rightSize;
            }
            if (std::make_pair(std::max(leftSize, rightSize), std::min(leftSize, rightSize)) >= std::make_pair(std::max(minLeft, minRight), std::min(minLeft, minRight))) {
              break;
            }
          }
        }

        if (std::make_pair(std::max(leftSize, rightSize), std::min(leftSize, rightSize)) < std::make_pair(std::max(minLeft, minRight), std::min(minLeft, minRight))) {
          minLeft = leftSize;
          minRight = rightSize;
          optimalSplit = i;
        }
      }

      // Build split node
      std::vector<int> leftObstacles(minLeft);
      std::vector<int> rightObstacles(minRight);
      int leftCounter = 0;
      int rightCounter = 0;
      int i = optimalSplit;
      for (int j = 0; j < (int)obstacles.size(); ++j) {
        if (i != j) {
          float j1_leftof_i = leftOf(_sim->_obstacles[obstacles[i]]->_p1, _sim->_obstacles[obstacles[i]]->_p2, _sim->_obstacles[obstacles[j]]->_p1);
          float j2_leftof_i = leftOf(_sim->_obstacles[obstacles[i]]->_p1, _sim->_obstacles[obstacles[i]]->_p2, _sim->_obstacles[obstacles[j]]->_p2);
          if (j1_leftof_i >= 0 && j2_leftof_i >= 0) {
            leftObstacles[leftCounter++] = obstacles[j];
          } else if (j1_leftof_i <= 0 && j2_leftof_i <= 0) {
            rightObstacles[rightCounter++] = obstacles[j];
          } else { 
            leftObstacles[leftCounter++] = obstacles[j];
            rightObstacles[rightCounter++] = obstacles[j];
          }
        }
      }

      node->obstacleID = obstacles[optimalSplit];
      node->left = buildObstacleTreeRecursive(leftObstacles);
      node->right = buildObstacleTreeRecursive(rightObstacles);
      return node;
    }
  }
  
  
  void KDTree::buildObstacleTree() {
    if (_obstacleTree != 0) {
      deleteObstacleTree(_obstacleTree);
    }

    std::vector<int> obstacles(_sim->_obstacles.size());
    for (int i = 0; i < (int)_sim->_obstacles.size(); ++i) {
      obstacles[i] = i;
    }

    _obstacleTree = buildObstacleTreeRecursive(obstacles);
  }

  void KDTree::queryObstacleTreeRecursive(Agent* agent, float& rangeSq, ObstacleTreeNode* node) const {
    if (node->obstacleID == -1) {
      return;
    } else {
      Obstacle* obstacle = _sim->_obstacles[node->obstacleID];
      float agent_leftof_line = leftOf(obstacle->_p1, obstacle->_p2, agent->_p);
      
      queryObstacleTreeRecursive(agent, rangeSq, (agent_leftof_line >= 0 ? node->left : node->right));
        
      float distSqLine = sqr(agent_leftof_line) / absSq(obstacle->_p2 - obstacle->_p1);
      if (distSqLine < rangeSq) { // try obstacle at this node
        agent->insertObstacleNeighbor(node->obstacleID, rangeSq);
        if (distSqLine < rangeSq) { // try other side of line
          queryObstacleTreeRecursive(agent, rangeSq, (agent_leftof_line >= 0 ? node->right : node->left));
        }
      }
    }
  }

}
