#include "RVOSimulator.h"
#include "Roadmap.h"
#include "RoadmapVertex.h"
#include "Goal.h"

namespace RVO {
  RVOSimulator*  Goal::_sim = RVOSimulator::Instance();

  Goal::Goal(const Vector2& p)
  {
    _vertex = new RoadmapVertex(p);
  }

  Goal::~Goal(void)
  {
    delete _vertex;
  }

  //-----------------------------------------------------------

  void Goal::computeShortestPathTree() {
    _vertex->computeVisibility();
    _vertex->computeNeighbors();

    std::multimap<float, int> Q;
    Roadmap * roadmap = _sim->_roadmap;
    _dist.assign(roadmap->_vertices.size(), std::make_pair(RVO_INFTY, -1));
    std::vector<std::multimap<float, int>::iterator> pos_in_Q(roadmap->_vertices.size(), Q.end());

    for (int j = 0; j < (int) _vertex->_neighbors.size(); ++j) {
      int u = _vertex->_neighbors[j].second;
      float distance = _vertex->_neighbors[j].first;
      _dist[u] = std::make_pair(distance, -1);
      pos_in_Q[u] = Q.insert(std::make_pair(distance, u));
    }

    int u, v;
    while (!Q.empty()) {
      u = Q.begin()->second;
      Q.erase(Q.begin());
      pos_in_Q[u] = Q.end();

      for (int j = 0; j < (int) roadmap->_vertices[u]->_neighbors.size(); ++j) {
        v = roadmap->_vertices[u]->_neighbors[j].second;
        float dist_uv = roadmap->_vertices[u]->_neighbors[j].first;
        if (_dist[v].first > _dist[u].first + dist_uv) {
          _dist[v] = std::make_pair(_dist[u].first + dist_uv, u);
          if (pos_in_Q[v] == Q.end()) {
            pos_in_Q[v] = Q.insert(std::make_pair(_dist[v].first, v));
          } else {
            Q.erase(pos_in_Q[v]);
            pos_in_Q[v] = Q.insert(std::make_pair(_dist[v].first, v));
          }
        }
      }
    }

  }
}
