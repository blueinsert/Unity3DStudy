/* \file Obstacle.h Contains the class Obstacle. */

#ifndef __OBSTACLE_H__
#define __OBSTACLE_H__

#include "RVODef.h"

namespace RVO {
  
  /* The class defining a line segment obstacle. */
  class Obstacle
  {
  private:
    /* Constructor. Constructs an obstacle.
      \param a The first endpoint of the obstacle. 
      \param b The second endpoint of the obstacle. */
    Obstacle(const Vector2& a, const Vector2& b);
    /* Deconstructor. */
    ~Obstacle();
        
    /* The first endpoint of the obstacle. */
    Vector2 _p1;
    /* The second endpoint of the obstacle. */
    Vector2 _p2;

    /* The normal vector of the line segment obstacle. */
    Vector2 _normal;

    friend class Agent;
    friend class RoadmapVertex;
    friend class SweeplineComparator;
    friend class KDTree;
    friend class RVOSimulator;
  };
}    // RVO namespace

#endif

