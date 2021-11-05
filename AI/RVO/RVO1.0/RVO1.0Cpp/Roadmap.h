/* \file Roadmap.h Contains the class Roadmap. */

#ifndef __ROADMAP_H__
#define __ROADMAP_H__

#include "RVODef.h"
#include "RoadmapVertex.h"

namespace RVO {

	/* The class defining the roadmap. */
	class Roadmap
	{
	private:
		/* Constructor. */
		Roadmap();
		/* Deconstructor. */
		~Roadmap();

		/* Initializes visibility of roadmap vertices after obstacles and roadmap have been read in. */
		void init();

		/* Flag defining whether the mutually visible roadmap vertices should automatically be connected by edges. */
		bool _automatic;

		/* The list of roadmap vertices. */
		std::vector<RoadmapVertex*> _vertices;

		/* Adds an edge between two vertices to the roadmap (i.e. add neighbors to each of the roadmap vertices).
		\param vID_1 The ID of the fisrt vertex.
		\param vID_2 The ID of the second vertex. */
		void addEdge(int vID_1, int vID_2);

		/* Adds a vertex to the roadmap.
		\param p The position of the vertex.
		*/
		void addVertex(const Vector2& p);

	protected:
		/* A reference to the singleton simulator. */
		static RVOSimulator* _sim;

		friend class Agent;
		friend class RoadmapVertex;
		friend class Goal;
		friend class RVOSimulator;
	};

} // RVO namespace 
#endif