#include "RVOSimulator.h"
#include "RoadmapVertex.h"
#include "Roadmap.h"
#include "Obstacle.h"

namespace RVO {
	RVOSimulator*  RoadmapVertex::_sim = RVOSimulator::Instance();

	RoadmapVertex::RoadmapVertex(const Vector2& p)
	{
		_p = p;
	}

	RoadmapVertex::~RoadmapVertex()
	{
	}

	/* A class defining an operator for sorting the obstacles along a sweepline used for computing the visibility of roadmap vertices. */
	class SweeplineComparator
	{
	public:
		/* Constructor.
		\param rv A pointer to the roadmap vertex relative to which the line segments are sorted. */
		SweeplineComparator(RoadmapVertex* rv) {
			_rv = rv;
		}

		/* Operator defining the order of line segments along the sweepline from the roadmap vertex.
		\param l1 The ID of the first obstacle
		\param l2 The ID of the second obstacle
		\returns Returns true when line segment l1 comes before line segment l2 when viewed from the roadmap vertex _rv. Returns false when line segment l2 comes before line segment l1 when viewed from the roadmap vertex _rv.
		*/
		bool operator()(const int l1, const int l2) const
		{
			if (l1 == l2) {
				return false;
			}

			Vector2 l1p1(_rv->_sim->_obstacles[l1]->_p1);
			Vector2 l1p2(_rv->_sim->_obstacles[l1]->_p2);
			Vector2 l2p1(_rv->_sim->_obstacles[l2]->_p1);
			Vector2 l2p2(_rv->_sim->_obstacles[l2]->_p2);

			float p_leftof_l1 = leftOf(l1p1, l1p2, _rv->_p);
			float p_leftof_l2 = leftOf(l2p1, l2p2, _rv->_p);

			float l1p1_leftof_l2 = leftOf(l2p1, l2p2, l1p1);
			float l1p2_leftof_l2 = leftOf(l2p1, l2p2, l1p2);

			float l2p1_leftof_l1 = leftOf(l1p1, l1p2, l2p1);
			float l2p2_leftof_l1 = leftOf(l1p1, l1p2, l2p2);

			//ʹ��l2��ֶ�ά�ռ䣬���l1��l2�����
			if (l1p1_leftof_l2 >= 0 && l1p2_leftof_l2 >= 0) {
				//���p��l2���ұߣ�˵��l2�����p����
				if (p_leftof_l2 < 0) {
					return false;
				}
				else if (p_leftof_l2 > 0) {
					//���pҲ��l2����ߣ�������˵��l1�����p������
					return true;
				}
			}
			if (l1p1_leftof_l2 <= 0 && l1p2_leftof_l2 <= 0) {
				if (p_leftof_l2 < 0) {
					return true;
				}
				else if (p_leftof_l2 > 0) {
					return false;
				}
			}
			//ʹ��l1��ֶ�άƽ��
			//���l2��l1�����
			if (l2p1_leftof_l1 >= 0 && l2p2_leftof_l1 >= 0) {
				//���p��l1���ұ�
				//˵����p����l1����
				if (p_leftof_l1 < 0) {
					return true;
				}
				//�����������
				else if (p_leftof_l1 > 0) {
					return false;
				}
			}
			if (l2p1_leftof_l1 <= 0 && l2p2_leftof_l1 <= 0) {
				if (p_leftof_l1 < 0) {
					return false;
				}
				else if (p_leftof_l1 > 0) {
					return true;
				}
			}

			// undefined
			return (l1 < l2);
		}

	private:
		/* The pointer to the roadmap vertex relative to which the line segments are sorted. */
		RoadmapVertex * _rv;
	};

	void RoadmapVertex::computeVisibility()
	{
		_visibility.clear();

		std::vector<std::pair<float, std::pair<int, int> > > obstacle_vertices; // radially sorted
		SweeplineComparator comp(this);
		std::set< int, SweeplineComparator > sweepline_status(comp); // sorted according to distance along sweepline

		// Initialize radially sorted list of obstacle_vertices, ignore collinear line segments (not visible), and initialize sweepline status
		obstacle_vertices.reserve(_sim->_obstacles.size() * 2);
		for (int i = 0; i < (int)_sim->_obstacles.size(); ++i) {
			Vector2 lp1(_sim->_obstacles[i]->_p1);
			Vector2 lp2(_sim->_obstacles[i]->_p2);

			float p_leftof_l = leftOf(lp1, lp2, _p);
			float angle_lp1 = atan(lp1 - _p);
			float angle_lp2 = atan(lp2 - _p);

			if (p_leftof_l > 0) { // lp1 is right, lp2 is left
				obstacle_vertices.push_back(std::make_pair(angle_lp1, std::make_pair(RIGHT, i)));
				obstacle_vertices.push_back(std::make_pair(angle_lp2, std::make_pair(LEFT, i)));
				//ֻ��lp1λ���Ե�pΪԭ�������ϵ�ĵڶ�����
				//lp2λ�ڵ�������ʱ�Ż�angle_lp1 > angle_lp2
				if (angle_lp1 > angle_lp2) {
					//���������
					//��pΪ��㣬����Ϊ-RVO_PI������һ�����߶�(lp1,lp2)�ཻ
					//_visibility.push_back(std::make_pair(-RVO_PI, i));
					//SweeplineComparator�������򣬾���p�����obstacle�߶λ���set����ǰ��
					sweepline_status.insert(i);
				}
			}
			else if (p_leftof_l < 0) { // lp1 is left, lp2 is right
				obstacle_vertices.push_back(std::make_pair(angle_lp2, std::make_pair(RIGHT, i)));
				obstacle_vertices.push_back(std::make_pair(angle_lp1, std::make_pair(LEFT, i)));

				if (angle_lp2 > angle_lp1) {
					sweepline_status.insert(i);
				}
			}
		}
		//��ʱ��
		std::sort(obstacle_vertices.begin(), obstacle_vertices.end());

		// Compute visibility
		int last_inserted;
		if (sweepline_status.empty()) {
			last_inserted = -1;
		}
		else {
			last_inserted = *(sweepline_status.begin());
		}
		_visibility.push_back(std::make_pair(-RVO_PI, last_inserted));
		// iterate counterclockwise through obstacle vertices ��ʱ��
		for (std::vector<std::pair<float, std::pair<int, int> > >::iterator i = obstacle_vertices.begin(); i != obstacle_vertices.end(); ++i) {
			float angle = i->first;
			int obstacle_vertex_type = i->second.first;
			int line_segment_id = i->second.second;

			//��ʱ��ɨ�裬��ɨ��obstacle�߶ε��Ҷ���(��������ڵ�p)
			//����ɨ���Ҷ���ʱ���߶μ����Զ����򼯺ϣ�ɨ���󶥵�ʱ����ɾ��
			if (obstacle_vertex_type == LEFT) { // remove line segment from sweepline status
				sweepline_status.erase(line_segment_id);
			}
			if (obstacle_vertex_type == RIGHT) { // insert new line segment into sweepline status
				sweepline_status.insert(line_segment_id);
			}

			// possibly insert new line segment in visibility
			if (sweepline_status.empty()) {
				if (last_inserted != -1) {
					last_inserted = -1;
					//����Ƕ�û��obstacle�赲
					_visibility.push_back(std::make_pair(angle, last_inserted));
				}
			}
			//ͬһ�ǶȵĶ���������������˳���Ⱥ��в��죬���ܲ����Ľ���᲻ͬ
			else if (*(sweepline_status.begin()) != last_inserted) {
				last_inserted = *(sweepline_status.begin());
				_visibility.push_back(std::make_pair(angle, last_inserted));
			}
		}
	}

	void RoadmapVertex::computeNeighbors()
	{
		_neighbors.clear();
		for (int i = 0; i < (int)_sim->_roadmap->_vertices.size(); ++i) {
			if (_sim->_roadmap->_vertices[i] != this && this->isVisibleFrom(_sim->_roadmap->_vertices[i]->_p)) {
				_neighbors.push_back(std::make_pair(abs(_sim->_roadmap->_vertices[i]->_p - _p), i));
			}
		}
	}

	bool RoadmapVertex::isVisibleFrom(const Vector2& pos)
	{
		float angle = atan(pos - _p);

		int l = 0;
		int r = (int)_visibility.size();
		while (r - l > 1) {
			int m = (l + r) / 2;
			if (_visibility[m].first > angle) {
				r = m;
			}
			else {
				l = m;
			}
		}

		l = _visibility[l].second;
		if (l == -1) {
			return true;
		}
		else {
			Vector2 lp1(_sim->_obstacles[l]->_p1);
			Vector2 lp2(_sim->_obstacles[l]->_p2);

			float p_leftof_l = leftOf(lp1, lp2, _p);
			float pos_leftof_l = leftOf(lp1, lp2, pos);

			if (p_leftof_l >= 0 && pos_leftof_l >= 0) {
				return true;
			}
			else if (p_leftof_l <= 0 && pos_leftof_l <= 0) {
				return true;
			}
			else {
				return false;
			}
		}
	}

	void RoadmapVertex::addNeighbor(float distance, int neighbor_id) {
		_neighbors.push_back(std::make_pair(distance, neighbor_id));
	}
}