#include "RVOSimulator.h"
#include "Roadmap.h"
#include "RoadmapVertex.h"
#include "Goal.h"
#include "Obstacle.h"
#include "Agent.h"
#include "KDTree.h"

namespace RVO {
	//-----------------------------------------------------------//
	//           Implementation for class: Agent                 //
	//-----------------------------------------------------------//

	RVOSimulator*  Agent::_sim = RVOSimulator::Instance();

	//-----------------------------------------------------------

	Agent::Agent() { }

	//-----------------------------------------------------------

	Agent::Agent(const Vector2& p, int goalID) {
		_atGoal = false;
		_subGoal = -2;

		_p = p;
		_goalID = goalID;

		_velSampleCount = _sim->_defaultAgent->_velSampleCount;
		_neighborDist = _sim->_defaultAgent->_neighborDist;
		_maxNeighbors = _sim->_defaultAgent->_maxNeighbors;

		_class = _sim->_defaultAgent->_class;
		_r = _sim->_defaultAgent->_r;
		_gR = _sim->_defaultAgent->_gR;
		_prefSpeed = _sim->_defaultAgent->_prefSpeed;
		_maxSpeed = _sim->_defaultAgent->_maxSpeed;
		_maxAccel = _sim->_defaultAgent->_maxAccel;
		_o = _sim->_defaultAgent->_o;
		_safetyFactor = _sim->_defaultAgent->_safetyFactor;
		_v = _sim->_defaultAgent->_v;

	}

	Agent::Agent(const Vector2& p, int goalID, int velSampleCount, float neighborDist, int maxNeighbors, int classID, float r, const Vector2& v, float maxAccel, float gR, float prefSpeed, float maxSpeed, float o, float safetyFactor) {
		_atGoal = false;
		_subGoal = -2;

		_p = p;
		_goalID = goalID;

		_velSampleCount = velSampleCount;
		_neighborDist = neighborDist;
		_maxNeighbors = maxNeighbors;

		_class = classID;
		_r = r;
		_gR = gR;
		_prefSpeed = prefSpeed;
		_maxSpeed = maxSpeed;
		_maxAccel = maxAccel;
		_o = o;
		_safetyFactor = safetyFactor;
		_v = v;

	}

	//-----------------------------------------------------------

	Agent::~Agent() {  }

	//-----------------------------------------------------------

	// Searching for the best new velocity
	void Agent::computeNewVelocity() {
		float min_penalty = RVO_INFTY;
		Vector2 vCand;

		// Select num_samples candidate velocities within the circle of radius _maxSpeed
		for (int n = 0; n < _velSampleCount; ++n) {
			if (n == 0) {
				vCand = _vPref;
			}
			else {
				float angle = rand() * 2 * RVO_PI / RAND_MAX;
				float radius = sqrt((rand() / (float)RAND_MAX));
				vCand = Vector2(radius * _maxSpeed * cos(angle), radius * _maxSpeed * sin(angle));
			}

			float dV; // distance between candidate velocity and preferred velocity 
			if (_collision) {
				dV = 0;
			}
			else {
				dV = abs(vCand - _vPref);
			}

			// searching for smallest time to collision
			float ct = RVO_INFTY; // time to collision
			// iterate over neighbors
			for (std::set<std::pair<float, std::pair<int, int> > >::iterator j = _neighbors.begin(); j != _neighbors.end(); ++j) {
				float ct_j; // time to collision with agent j
				Vector2 Vab;
				int type = j->second.first;
				int id = j->second.second;

				if (type == AGENT) {
					Agent* other = _sim->_agents[id];
					Vab = 2 * vCand - _v - other->_v;
					float time = timeToCollision(_p, Vab, other->_p, _r + other->_r, _collision);
					if (_collision) {
						ct_j = -ceil(time / _sim->_timeStep);
						ct_j -= absSq(vCand) / sqr(_maxSpeed);
					}
					else {
						ct_j = time;
					}
				}
				else if (type == OBSTACLE) {
					Obstacle* other;
					other = _sim->_obstacles[id];

					float time_1, time_2, time_a, time_b;
					time_1 = timeToCollision(_p, vCand, other->_p1, _r, _collision);
					time_2 = timeToCollision(_p, vCand, other->_p2, _r, _collision);
					time_a = timeToCollision(_p, vCand, other->_p1 + _r * other->_normal, other->_p2 + _r * other->_normal, _collision);
					time_b = timeToCollision(_p, vCand, other->_p1 - _r * other->_normal, other->_p2 - _r * other->_normal, _collision);

					if (_collision) {
						float time = std::max(std::max(std::max(time_1, time_2), time_a), time_b);
						ct_j = -ceil(time / _sim->_timeStep);
						ct_j -= absSq(vCand) / sqr(_maxSpeed);
					}
					else {
						float time = std::min(std::min(std::min(time_1, time_2), time_a), time_b);
						if (time < _sim->_timeStep || sqr(time) < absSq(vCand) / sqr(_maxAccel)) {
							ct_j = time;
						}
						else {//time >= _sim->_timeStep && sqr(time) >= absSq(vCand) / sqr(_maxAccel)
							//下一帧不会碰撞 并且 时间足够从当前速度减速到零
							//这种情况下没有惩罚，将时间设置为最大
							ct_j = RVO_INFTY; // no penalty
						}
					}
				}

				if (ct_j < ct) {
					ct = ct_j;
					// pruning search if no better penalty can be obtained anymore for this velocity
					if (_safetyFactor / ct + dV >= min_penalty) {
						break;
					}
				}
			}

			float penalty = _safetyFactor / ct + dV;
			if (penalty < min_penalty) {
				min_penalty = penalty;
				_vNew = vCand;
			}
		}
	}

	//---------------------------------------------------------------
	void Agent::insertObstacleNeighbor(int id, float& rangeSq) {
		Obstacle* obstacle = _sim->_obstacles[id];
		float distSq = distSqPointLineSegment(obstacle->_p1, obstacle->_p2, _p);
		if (distSq < sqr(_r) && distSq < rangeSq) { // COLLISION!
			if (!_collision) {
				_collision = true;
				_neighbors.clear();
				rangeSq = sqr(_r);
			}

			if (_neighbors.size() == _maxNeighbors) {
				_neighbors.erase(--_neighbors.end());
			}
			_neighbors.insert(std::make_pair(distSq, std::make_pair(OBSTACLE, id)));
			if (_neighbors.size() == _maxNeighbors) {
				rangeSq = (--_neighbors.end())->first;
			}
		}
		else if (!_collision && distSq < rangeSq) {
			if (_neighbors.size() == _maxNeighbors) {
				_neighbors.erase(--_neighbors.end());
			}
			_neighbors.insert(std::make_pair(distSq, std::make_pair(OBSTACLE, id)));
			if (_neighbors.size() == _maxNeighbors) {
				rangeSq = (--_neighbors.end())->first;
			}
		}
	}

	void Agent::insertAgentNeighbor(int id, float& rangeSq) {
		Agent* other = _sim->_agents[id];
		if (this != other) {
			float distSq = absSq(_p - other->_p);
			if (distSq < sqr(_r + other->_r) && distSq < rangeSq) { // COLLISION!
				if (!_collision) {
					_collision = true;
					_neighbors.clear();
				}

				if (_neighbors.size() == _maxNeighbors) {
					_neighbors.erase(--_neighbors.end());
				}
				_neighbors.insert(std::make_pair(distSq, std::make_pair(AGENT, id)));
				if (_neighbors.size() == _maxNeighbors) {
					rangeSq = (--_neighbors.end())->first;
				}
			}
			else if (!_collision && distSq < rangeSq) {
				if (_neighbors.size() == _maxNeighbors) {
					_neighbors.erase(--_neighbors.end());
				}
				_neighbors.insert(std::make_pair(distSq, std::make_pair(AGENT, id)));
				if (_neighbors.size() == _maxNeighbors) {
					rangeSq = (--_neighbors.end())->first;
				}
			}
		}
	}

	void Agent::computeNeighbors() {
		// Compute new neighbors of agent; 
		// sort them according to distance (optimized effect of pruning heuristic in search for new velocities); 
		// seperate between colliding and near agents

		_collision = false;
		_neighbors.clear();

		// check obstacle neighbors
		float rangeSq = std::min(sqr(_neighborDist), sqr(std::max(_sim->_timeStep, _maxSpeed / _maxAccel)*_maxSpeed + _r));
		_sim->_kdTree->computeObstacleNeighbors(this, rangeSq);

		/*for (int j = 0; j < (int) _sim->_obstacles.size(); ++j) {
		  insertObstacleNeighbor(j, rangeSq);
		}*/

		if (_collision) {
			return;
		}

		// Check other agents
		if (_neighbors.size() != _maxNeighbors) {
			rangeSq = sqr(_neighborDist);
		}
		_sim->_kdTree->computeAgentNeighbors(this, rangeSq);

		/*for (int j = 0; j < (int) _sim->_agents.size(); ++j) {
		  insertAgentNeighbor(j, rangeSq);
		}*/
	}

	//---------------------------------------------------------------

	// Prepare for next cycle
	void Agent::computePreferredVelocity() {
		// compute subgoal
		Roadmap * roadmap = _sim->_roadmap;
		Goal * goal = _sim->_goals[_goalID];

		if (_subGoal == -1) { // sub_goal is goal
			if (goal->_vertex->isVisibleFrom(_p)) { // goal is visible
				_subGoal = -1;
			}
			else { // goal not visible, try among neighbors of goal
				_subGoal = -2;
				float mindist = RVO_INFTY;
				for (int i = 0; i < (int)goal->_vertex->_neighbors.size(); ++i) {
					int u = goal->_vertex->_neighbors[i].second;
					float distance = abs(_p - roadmap->_vertices[u]->_p) + goal->_dist[u].first;
					if (distance < mindist && roadmap->_vertices[u]->isVisibleFrom(_p)) {
						_subGoal = u;
						mindist = distance;
					}
				}
			}
		}
		else if (_subGoal >= 0) { // sub_goal is roadmap vertex
			if (roadmap->_vertices[_subGoal]->isVisibleFrom(_p)) { // sub_goal is visible
				int try_advance_sub_goal = goal->_dist[_subGoal].second; // try to advance sub_goal
				if (try_advance_sub_goal == -1) { // advanced sub_goal is goal
					if (goal->_vertex->isVisibleFrom(_p)) { // set new sub_goal if goal is visible
						_subGoal = -1;
					}
				}
				else if (roadmap->_vertices[try_advance_sub_goal]->isVisibleFrom(_p)) { // advanced sub_goal is roadmap vertex
					_subGoal = try_advance_sub_goal; // set new sub_goal if visible
				}
			}
			else { // sub_goal is not visible
				int find_new_sub_goal = -2;
				float mindist = RVO_INFTY; // search for new sub_goal among neighbors of sub_goal
				for (int i = 0; i < (int)roadmap->_vertices[_subGoal]->_neighbors.size(); ++i) {
					int u = roadmap->_vertices[_subGoal]->_neighbors[i].second;
					float distance = abs(_p - roadmap->_vertices[u]->_p) + goal->_dist[u].first;
					if (distance < mindist && roadmap->_vertices[u]->isVisibleFrom(_p)) {
						find_new_sub_goal = u;
						mindist = distance;
					}
				}
				_subGoal = find_new_sub_goal;
			}
		}

		if (_subGoal == -2) { // new visible sub_goal was not found among neighbors, search all vertices
			if (goal->_vertex->isVisibleFrom(_p)) {
				_subGoal = -1;
			}
			else {
				float mindist = RVO_INFTY;
				for (int i = 0; i < (int)goal->_dist.size(); ++i) {
					float distance = goal->_dist[i].first + abs(_p - roadmap->_vertices[i]->_p);
					if (distance < mindist && roadmap->_vertices[i]->isVisibleFrom(_p)) {
						mindist = distance;
						_subGoal = i;
					}
				}
			}
		}

		if (_subGoal == -2) { // no vertex is visible, move in direction of goal
			_subGoal = -1;
		}


		// Set preferred velocity
		Vector2 sub_g;
		if (_subGoal == -1) {
			sub_g = goal->_vertex->_p;
		}
		else {
			sub_g = roadmap->_vertices[_subGoal]->_p;
		}

		float dist2subgoal = abs(sub_g - _p);

		// compute preferred velocity
		if (_subGoal == -1 && _prefSpeed * _sim->_timeStep > dist2subgoal) {
			_vPref = (sub_g - _p) / _sim->_timeStep;
		}
		else {
			_vPref = _prefSpeed * (sub_g - _p) / dist2subgoal;
		}
	}

	//---------------------------------------------------------------

	// update velocity and position of agent
	void Agent::update() {
		// Scale proposed new velocity to obey maximum acceleration
		float dv = abs(_vNew - _v);
		if (dv < _maxAccel * _sim->_timeStep) {
			_v = _vNew;
		}
		else {
			_v = (1 - (_maxAccel * _sim->_timeStep / dv)) * _v + (_maxAccel * _sim->_timeStep / dv) * _vNew;
		}

		// Update position
		_p += _v * _sim->_timeStep;

		// Set reached goal
		if (absSq(_sim->_goals[_goalID]->_vertex->_p - _p) < sqr(_gR)) {
			_atGoal = true;
		}
		else {
			_atGoal = false;
			_sim->_allAtGoals = false;
		}

		// Update orientation
		if (!_atGoal) {
			_o = atan(_vPref);
		}
	}

}    // RVO namespace