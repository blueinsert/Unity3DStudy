/* \file RoadmapVertex.h Contains the class RoadmapVertex, as well as the class SweeplineComparator. */

#ifndef __ROADMAP_VERTEX_H__
#define __ROADMAP_VERTEX_H__

#define LEFT 0
#define RIGHT 1

#include "RVODef.h"

namespace RVO {

	class SweeplineComparator;

	/* The class defining a roadmap vertex. */
	class RoadmapVertex
	{
	private:
		/* Constructor. Constructs a roadmap vertex.
		  \param p The position of the roadmap vertex. */
		RoadmapVertex(const Vector2& p);
		/* Deconstructor. */
		~RoadmapVertex();

		/* Computes the visibility of a roadmap vertex using a sweepline algorithm. Constructs _visibility
		*/
		void computeVisibility();
		/* Adds all visible roadmap vertices to the list of neighbors. Should be called after computeVisibility()
		*/
		void computeNeighbors();
		/* Determines whether this roadmap vertex is visible from a position. Uses binary search on _visibility. Should be called after computeVisibility()
		  \param p The position
		  \returns Returns true when the position p is visible from this roadmap vertex, false otherwise. */
		bool isVisibleFrom(const Vector2& p);
		/* Adds a roadmap vertex to the list of neighbors.
		  \param distance The distance to the neighboring roadmap vertex (used for shortest path planning).
		  \param neighbor_id The ID of the neighboring roadmap vertex. */
		void addNeighbor(float distance, int neighbor_id);

		/* The position of the roadmap vertex */
		Vector2 _p;

		/* The visibility of the roadmap vertex. The vector contains pairs of angle and obstacle ID. The angles are sorted. The obstacle is visible for the angles ranging from the angle in the pair to the angle of the next item in the vector. If no obstacle blocks the visibility, the obstacle ID is -1. */
		std::vector<std::pair<float, int> > _visibility;  // visibility (angle, index of obstacle)
		/* The list of neighbors of the roadmap vertex. The vector contains pairs of distance to the neighbor and roadmap vertex ID of the neighbor. */
		std::vector<std::pair<float, int> > _neighbors;  // list of neighbors (distance to neighboring vertex, index of neighbor vertex)

	protected:
		/* A reference to the singleton simulator. */
		static RVOSimulator* _sim;

		friend class SweeplineComparator;
		friend class Agent;
		friend class Roadmap;
		friend class Goal;
		friend class RVOSimulator;
	};
}
#endif