/* \file RVODef.h Defines global functions, constants and includes used throughout the solution. */

#ifndef __RVO_DEF_H__
#define __RVO_DEF_H__

/* Pi. */
#define RVO_PI 3.14159265358979323846f

#include <map>
#include <set>
#include <vector>
#include <algorithm>
#include <time.h>
#include <omp.h>
#include <cmath>
#include "vector2.h"

namespace RVO {
  // forward declarations
  class Obstacle;
  class Roadmap;
  class RoadmapVertex;
  class Agent;
  class RVOSimulator;
  
  // global functions

  /* Square.
     \param a A scalar
     \returns Returns the square of the scalar.
    */
  inline float sqr(float a) {
    return a*a;
  }

  /* Signed distance between point and line.
     \param a The first point of the line
     \param b The second point of the line
     \param c The point
     \returns Returns the signed distance times the length of segment ab between line ab (not the segment) and point c. Is positive when c lies to the left of ab.
    */
  inline float leftOf(const Vector2& a, const Vector2& b, const Vector2& c) {
    return det(a - c, b - a);
  }

  /* Time to collision of a ray to a line segment.
     \param p The start position of the ray
     \param v The velocity vector of the ray
     \param a The first endpoint of the line segment
     \param b The second endpoint of the line segment
     \param collision Specifies whether the time to collision is computed (false), or the time from collision (true).
     \returns Returns the time to collision of ray p + tv to segment ab, and #RVO_INFTY when the line segment is not hit. If collision is true, the value is negative.
    */
  inline float timeToCollision(const Vector2& p, const Vector2& v, const Vector2& a, const Vector2& b, bool collision) {
    float D = det(v, b - a);
    if (D == 0) {  // ray and line are collinear
      if (collision) {
        return -RVO_INFTY;
      } else {
        return RVO_INFTY;
      }
    }

    float t = det(a - p, b - a) / D;
    float s = det(p - a, v) / -D;

    if (t < 0 || s < 0 || s > 1) {
      if (collision) {
        return -RVO_INFTY;
      } else {
        return RVO_INFTY;
      }
    } else {
      return t;
    }
  }
  
  /* Time to collision of a ray to a disc.
     \param p The start position of the ray
     \param v The velocity vector of the ray
     \param p2 The center position of the disc
     \param radius The radius of the disc
     \param collision Specifies whether the time to collision is computed (false), or the time from collision (true).
     \returns Returns the time to collision of ray p + tv to disc D(p2, radius), and #RVO_INFTY when the disc is not hit. If collision is true, the value is negative.
    */
  //点到直线距离公式：d = |AX0+BY0+C|/sqrt(A*A+B*B)
  //直线AX+BY+C=0,A=-V.y B=V.x C =P.x*V.y-V.x*P.y
  //可能碰撞：d<r=>d*d-r*r<0
  //p2带入AX0+BY0+C
  //AX0+BY0+C = -v.y*p2.x+v.x*p2.y+p.x*v.y-v.x*p.y=v.x(p2.y-p.y)-v.y(p2.x-p.x)=det(v, ba)
  //d*d = det(v, ba)*det(v, ba)/absSq(v)
  //r*r-d*d>0 => r*r-det(v, ba)*det(v, ba)/absSq(v)>0 => discr = r*r*absSq(v) - det(v, ba)*det(v, ba)>0
  //
  inline float timeToCollision(const Vector2& p, const Vector2& v, const Vector2& p2, float radius, bool collision) {
    Vector2 ba = p2 - p; 
    float sq_diam = radius * radius;
    float time;

    float discr = -sqr(det(v, ba)) + sq_diam * absSq(v);
    if (discr > 0) {
      if (collision) {
        time = (v * ba + sqrt(discr)) / absSq(v);
        if (time < 0) {
          time = -RVO_INFTY;
        }
      } else {
        time = (v * ba - sqrt(discr)) / absSq(v);//right
        if (time < 0) {
          time = RVO_INFTY;
        }
      }
    } else {
      if (collision) {
        time = -RVO_INFTY;
      } else {
        time = RVO_INFTY;
      }
    }
    return time;
  }
  
  /* Squared distance of a point to a line segment.
     \param a The first endpoint of the line segment
     \param b The second endpoint of the line segment
     \param c The point
     \returns Returns the squared distance between line segment ab and point c.
    */
  inline float distSqPointLineSegment(const Vector2& a, const Vector2& b, const Vector2& c) {
    float r = ((c - a)*(b - a)) / absSq(b - a); 

    if (r < 0) { // point a is closest to c
      return absSq(c - a);
    } else if (r > 1) { // point b is closest to c
      return absSq(c - b);
    } else { // some point in between a and b is closest to c
      return absSq(c - (a + r*(b - a)));
    }
  }

}

#endif