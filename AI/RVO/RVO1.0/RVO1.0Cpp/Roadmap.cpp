#include "RVOSimulator.h"
#include "Agent.h"
#include "Roadmap.h"

namespace RVO {
	RVOSimulator*  Roadmap::_sim = RVOSimulator::Instance();

	Roadmap::Roadmap() {
		_automatic = false;
	}

	Roadmap::~Roadmap()
	{
		for (int i = 0; i < (int)_vertices.size(); ++i) {
			delete (_vertices[i]);
		}
	}

	void Roadmap::addEdge(int vID_1, int vID_2) {
		float dist = abs(_vertices[vID_1]->_p - _vertices[vID_2]->_p);
		_vertices[vID_1]->addNeighbor(dist, vID_2);
		_vertices[vID_2]->addNeighbor(dist, vID_1);
	}


	void Roadmap::addVertex(const Vector2& p) {
		RoadmapVertex * vertex = new RoadmapVertex(p);
		_vertices.push_back(vertex);
	}

	void Roadmap::init() {
#pragma omp parallel for
		for (int i = 0; i < (int)_vertices.size(); ++i) {
			_vertices[i]->computeVisibility();
			if (_automatic) {
				_vertices[i]->computeNeighbors();
			}
		}
	}

}  // RVO namespace
