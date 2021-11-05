#ifndef __GOAL_H__
#define __GOAL_H__

#include "RVODef.h"

namespace RVO {
  class Goal
  {
  private:
    Goal(const Vector2& p);
    ~Goal();

    /* Computes the distance from every vertex in the roadmap to the end goal */
    void computeShortestPathTree();

    /* The goal vertex of the agent. Used for computing the shortest path tree. */
    RoadmapVertex* _vertex;
    /* A list of roadmap vertex distances to this goal. The float stores the distance. The int stores the ID of the parent vertex of the roadmap vertex in the shortest path tree (which is -1 if it is the goal). */
    std::vector<std::pair<float, int> > _dist;

  protected:
    /* A reference to the singleton simulator. */
    static RVOSimulator*  _sim;

    friend class Agent;
    friend class RVOSimulator;
  };
}
#endif
