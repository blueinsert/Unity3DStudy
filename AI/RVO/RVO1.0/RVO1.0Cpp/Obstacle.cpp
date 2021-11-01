#include "RVOSimulator.h"
#include "Obstacle.h"

namespace RVO {
  Obstacle::Obstacle(const Vector2& a, const Vector2& b) {
    _p1 = a;
    _p2 = b;

    _normal = normal(_p1, _p2);//法线指向内部?
  }

  Obstacle::~Obstacle()
  {
  }

}  // RVO namespace