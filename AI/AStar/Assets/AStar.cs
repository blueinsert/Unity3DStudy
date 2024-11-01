using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

namespace bluebean.PathFinding
{
	// https://github.com/justinhj/astar-algorithm-cpp

	public struct NodePosition
	{
		public int x;
		public int y;

		public NodePosition(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

	}

	public class Map
	{
		private int m_width;
		private int m_height;
		private int[] m_map;

		public Map(int[] mapData, int width, int height)
		{
			m_map = mapData;
			m_width = width;
			m_height = height;
		}

		public int GetMap(int x, int y)
		{
			if (x < 0 || x >= m_width || y < 0 || y >= m_height)
			{
				return 9;
			}

			return m_map[(y * m_width) + x];
		}

		/*
		const int MAP_WIDTH = 20;
		const int MAP_HEIGHT = 20;

		static int[] map = new int[MAP_WIDTH * MAP_HEIGHT]
		{
		// 0001020304050607080910111213141516171819
			1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,   // 00
			1,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,1,   // 01
			1,9,9,1,1,9,9,9,1,9,1,9,1,9,1,9,9,9,1,1,   // 02
			1,9,9,1,1,9,9,9,1,9,1,9,1,9,1,9,9,9,1,1,   // 03
			1,9,1,1,1,1,9,9,1,9,1,9,1,1,1,1,9,9,1,1,   // 04
			1,9,1,1,9,1,1,1,1,9,1,1,1,1,9,1,1,1,1,1,   // 05
			1,9,9,9,9,1,1,1,1,1,1,9,9,9,9,1,1,1,1,1,   // 06
			1,9,9,9,9,9,9,9,9,1,1,1,9,9,9,9,9,9,9,1,   // 07
			1,9,1,1,1,1,1,1,1,1,1,9,1,1,1,1,1,1,1,1,   // 08
			1,9,1,9,9,9,9,9,9,9,1,1,9,9,9,9,9,9,9,1,   // 09
			1,9,1,1,1,1,9,1,1,9,1,1,1,1,1,1,1,1,1,1,   // 10
			1,9,9,9,9,9,1,9,1,9,1,9,9,9,9,9,1,1,1,1,   // 11
			1,9,1,9,1,9,9,9,1,9,1,9,1,9,1,9,9,9,1,1,   // 12
			1,9,1,9,1,9,9,9,1,9,1,9,1,9,1,9,9,9,1,1,   // 13
			1,9,1,1,1,1,9,9,1,9,1,9,1,1,1,1,9,9,1,1,   // 14
			1,9,1,1,9,1,1,1,1,9,1,1,1,1,9,1,1,1,1,1,   // 15
			1,9,9,9,9,1,1,1,1,1,1,9,9,9,9,1,1,1,1,1,   // 16
			1,1,9,9,9,9,9,9,9,1,1,1,9,9,9,1,9,9,9,9,   // 17
			1,9,1,1,1,1,1,1,1,1,1,9,1,1,1,1,1,1,1,1,   // 18
			1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,   // 19
		};

		public static int GetMap(int x, int y)
		{
			if (x < 0 || x >= MAP_WIDTH || y < 0 || y >= MAP_HEIGHT)
			{
				return 9;
			}

			return map[(y * MAP_WIDTH) + x];
		}
		*/
	}

	public class MapSearchNode
	{
		public NodePosition m_position;
		AStar m_pathFinder = null;
		public Map m_map;

		public MapSearchNode(AStar _pathfinder)
		{
			m_position = new NodePosition(0, 0);
			m_pathFinder = _pathfinder;
		}

		public MapSearchNode(NodePosition pos, AStar _pathfinder)
		{
			m_position = new NodePosition(pos.x, pos.y);
			m_pathFinder = _pathfinder;
		}

		// Here's the heuristic function that estimates the distance from a Node
		// to the Goal. 
		public float GoalDistanceEstimate(MapSearchNode nodeGoal)
		{
			double X = (double)m_position.x - (double)nodeGoal.m_position.x;
			double Y = (double)m_position.y - (double)nodeGoal.m_position.y;
			return ((float)System.Math.Sqrt((X * X) + (Y * Y)));
		}

		public bool IsGoal(MapSearchNode nodeGoal)
		{
			return (m_position.x == nodeGoal.m_position.x && m_position.y == nodeGoal.m_position.y);
		}

		public bool ValidNeigbour(int xOffset, int yOffset)
		{
			// Return true if the node is navigable and within grid bounds
			return (m_map.GetMap(m_position.x + xOffset, m_position.y + yOffset) < 9);
		}

		void AddNeighbourNode(int xOffset, int yOffset, NodePosition parentPos, AStar aStarSearch)
		{
			if (ValidNeigbour(xOffset, yOffset) &&
				!(parentPos.x == m_position.x + xOffset && parentPos.y == m_position.y + yOffset))
			{
				NodePosition neighbourPos = new NodePosition(m_position.x + xOffset, m_position.y + yOffset);
				MapSearchNode newNode = m_pathFinder.AllocateMapSearchNode(neighbourPos);
				aStarSearch.AddSuccessor(newNode);
			}
		}

		// This generates the successors to the given Node. It uses a helper function called
		// AddSuccessor to give the successors to the AStar class. The A* specific initialisation
		// is done for each node internally, so here you just set the state information that
		// is specific to the application
		public bool GetSuccessors(AStar aStarSearch, MapSearchNode parentNode)
		{
			NodePosition parentPos = new NodePosition(0, 0);

			if (parentNode != null)
			{
				parentPos = parentNode.m_position;
			}

			// push each possible move except allowing the search to go backwards
			AddNeighbourNode(-1, 0, parentPos, aStarSearch);
			AddNeighbourNode(0, -1, parentPos, aStarSearch);
			AddNeighbourNode(1, 0, parentPos, aStarSearch);
			AddNeighbourNode(0, 1, parentPos, aStarSearch);

			return true;
		}

		// given this node, what does it cost to move to successor. In the case
		// of our map the answer is the map terrain value at this node since that is 
		// conceptually where we're moving
		public float GetCost(MapSearchNode successor)
		{
			// Implementation specific
			return m_map.GetMap(successor.m_position.x, successor.m_position.y);
		}

		public bool IsSameState(MapSearchNode rhs)
		{
			return (m_position.x == rhs.m_position.x &&
					m_position.y == rhs.m_position.y);
		}
	}

	// Converted to C# originally for use with Unity, hence some apparently odd
	// conventions: foreach is not used as that generates garbage in Unity.
	// Preallocated lists are used to preclude any runtime allocations.
	// The current implementation is no longer dependent on Unity (but will work within it of course).

	public class AStar

	{
		public enum SearchState
		{
			NotInitialized,
			Searching,
			Succeeded,
			Failed,
			OutOfMemory,
			Invalid
		}

		// A node represents a possible state in the search
		// The user provided state type is included inside this type
		class Node
		{
			public Node parent; // used during the search to record the parent of successor nodes
			public Node child; // used after the search for the application to view the search in reverse

			public float g; // cost of this node + it's predecessors
			public float h; // heuristic estimate of distance to goal
			public float f; // sum of cumulative cost of predecessors and self and heuristic

			public Node()
			{
				Reinitialize();
			}

			public void Reinitialize()
			{
				parent = null;
				child = null;
				g = 0.0f;
				h = 0.0f;
				f = 0.0f;
			}

			public MapSearchNode m_UserState;
		};

        #region 内部变量

        // Heap (simple list but used as a heap, cf. Steve Rabin's game gems article)
        LinkedList<Node> m_OpenList;

		// Closed list is a list.
		List<Node> m_ClosedList;

		// Successors is a list filled out by the user each type successors to a node are generated
		List<Node> m_Successors;

		// State
		SearchState m_State = SearchState.NotInitialized;

		// Counts steps
		int m_Steps = 0;

		// Start and goal state pointers
		Node m_Start = null;
		Node m_Goal = null;

		Node m_CurrentSolutionNode = null;

		// Memory
		List<Node> m_FixedSizeAllocator;

		int m_AllocateNodeCount = 0;

		bool m_CancelRequest = false;

		int m_allocatedMapSearchNodes = 0;

		List<MapSearchNode> m_mapSearchNodePool = null;

		int openListHighWaterMark = 0;
		int closedListHighWaterMark = 0;
		int successorListHighWaterMark = 0;

		// Fixed sizes for collections
		readonly int kPreallocatedNodes = 4000;
		readonly int kPreallocatedMapSearchNodes = 1000;

		readonly int kPreallocatedOpenListSlots = 32;
		readonly int kPreallocatedClosedListSlots = 256;
		readonly int kPreallocatedSuccessorSlots = 8;

        #region 业务相关数据
        Map m_map;
        bool m_isSearchRegion;
        int m_movePoint;
        #endregion

        #endregion

        // constructor just initialises private data
        public AStar()
		{
			// Allocate all lists		
			m_OpenList = new LinkedList<Node>();
			m_ClosedList = new List<Node>(kPreallocatedClosedListSlots);
			m_Successors = new List<Node>(kPreallocatedSuccessorSlots);

			m_FixedSizeAllocator = new List<Node>(kPreallocatedNodes);
			for (int i = 0; i < kPreallocatedNodes; ++i)
			{
				Node n = new Node();
				m_FixedSizeAllocator.Add(n);
			}

			m_mapSearchNodePool = new List<MapSearchNode>(kPreallocatedMapSearchNodes);
			for (int i = 0; i < kPreallocatedMapSearchNodes; ++i)
			{
				MapSearchNode msn = new MapSearchNode(this);
				m_mapSearchNodePool.Add(msn);
			}
		}

        #region 内部方法
        // call at any time to cancel the search and free up all the memory
        public void CancelSearch()
		{
			m_CancelRequest = true;
		}

		// Build the open list as sorted to begin with by inserting new elements in the right place
		void SortedAddToOpenList(Node node)
		{
            var current = m_OpenList.First;
            while (current != null)
            {
                Node currentNode = current.Value;
                if (node.f < currentNode.f)
                    break;
                else
                    current = current.Next;
            }

            if (current != null)
                m_OpenList.AddBefore(current, node);
            else
                m_OpenList.AddLast(node);

            if (m_OpenList.Count > openListHighWaterMark)
			{
				openListHighWaterMark = m_OpenList.Count;
			}
		}

		Node AllocateNode()
		{
			if (m_AllocateNodeCount >= kPreallocatedNodes)
			{
				System.Console.WriteLine("FATAL - Pathfinder ran out of preallocated nodes!");
			}

			return m_FixedSizeAllocator[m_AllocateNodeCount++];
		}

		public MapSearchNode AllocateMapSearchNode(NodePosition nodePosition)
		{
			if (m_allocatedMapSearchNodes >= kPreallocatedMapSearchNodes)
			{
				System.Console.WriteLine("FATAL - HexGrid has run out of preallocated MapSearchNodes!");
			}

			var node = m_mapSearchNodePool[m_allocatedMapSearchNodes++];
            node.m_position = nodePosition;
			node.m_map = m_map;
			return node;
		}

        private void InitiatePathfind(Map map)
        {
            m_CancelRequest = false;
            m_AllocateNodeCount = 0;    // Reset our used node tracking
            m_allocatedMapSearchNodes = 0;
            m_map = map;
        }

        // Set Start and goal states
        private void SetStartAndGoalStates(MapSearchNode Start, MapSearchNode Goal)
        {
            m_Start = AllocateNode();
            m_Goal = AllocateNode();

            System.Diagnostics.Debug.Assert((m_Start != null && m_Goal != null));

            m_Start.m_UserState = Start;
            m_Goal.m_UserState = Goal;

            m_State = SearchState.Searching;

            // Initialise the AStar specific parts of the Start Node
            // The user only needs fill out the state information
            m_Start.g = 0;
            m_Start.h = m_Start.m_UserState.GoalDistanceEstimate(m_Goal.m_UserState);
            m_Start.f = m_Start.g + m_Start.h;
            m_Start.parent = null;

            // Push the start node on the Open list
            m_OpenList.AddLast(m_Start);

            // Initialise counter for search steps
            m_Steps = 0;

#if PATHFIND_DEBUG
		System.Console.WriteLine("Starting pathfind. Start: " + m_Start.m_UserState.position + ", Goal: " + m_Goal.m_UserState.position);
#endif
        }

        private void SetRegionStartState(MapSearchNode start)
        {
            m_Start = AllocateNode();
            m_Goal = null;

            m_Start.m_UserState = start;

            m_State = SearchState.Searching;

            // Initialise the AStar specific parts of the Start Node
            // The user only needs fill out the state information
            m_Start.g = 0;
            m_Start.h = 0;
            m_Start.f = m_Start.g + m_Start.h;
            m_Start.parent = null;

            // Push the start node on the Open list
            m_OpenList.AddLast(m_Start);

            // Initialise counter for search steps
            m_Steps = 0;
        }

        // Advances search one step 
        private SearchState SearchStep()
        {
            // Firstly break if the user has not initialised the search
            System.Diagnostics.Debug.Assert((m_State > SearchState.NotInitialized) && (m_State < SearchState.Invalid));

            // Next I want it to be safe to do a searchstep once the search has succeeded...
            if (m_State == SearchState.Succeeded || m_State == SearchState.Failed)
            {
                return m_State;
            }

            // Failure is defined as emptying the open list as there is nothing left to 
            // search...
            if (m_OpenList.Count == 0 )
            {
                if (m_isSearchRegion)
                {
                    m_State = SearchState.Succeeded;
                }
                else
                {
                    m_State = SearchState.Failed;
                }
                FreeSolutionNodes();               
                return m_State;
            }
            // New: Allow user abort
            if (m_CancelRequest)
            {
                m_State = SearchState.Failed;
                return m_State;
            }

            // Incremement step count
            m_Steps++;

            // Pop the best node (the one with the lowest f) 
            Node n = m_OpenList.First.Value; // get pointer to the node
            m_OpenList.RemoveFirst();

            //System.Console.WriteLine("Checking node at " + n.m_UserState.position + ", f: " + n.f);

            // Check for the goal, once we pop that we're done
            if (!m_isSearchRegion && n.m_UserState.IsGoal(m_Goal.m_UserState))
            {
                // The user is going to use the Goal Node he passed in 
                // so copy the parent pointer of n 
                m_Goal.parent = n.parent;
                m_Goal.g = n.g;

                // A special case is that the goal was passed in as the start state
                // so handle that here
                if (false == n.m_UserState.IsSameState(m_Start.m_UserState))
                {
                    // set the child pointers in each node (except Goal which has no child)
                    Node nodeChild = m_Goal;
                    Node nodeParent = m_Goal.parent;

                    do
                    {
                        nodeParent.child = nodeChild;
                        nodeChild = nodeParent;
                        nodeParent = nodeParent.parent;
                    }
                    while (nodeChild != m_Start); // Start is always the first node by definition
                }

                // delete nodes that aren't needed for the solution
                //FreeUnusedNodes();

#if PATHFIND_DEBUG
			System.Console.WriteLine("GOAL REACHED! Steps: " + m_Steps + ", allocated nodes: " + m_AllocateNodeCount + ", MapSearchNodes: " + allocatedMapSearchNodes);
			System.Console.WriteLine("High water marks - Open:" + openListHighWaterMark + ", Closed: " + closedListHighWaterMark + ", Successors: " + successorListHighWaterMark);
#endif

                m_State = SearchState.Succeeded;
                return m_State;
            }
            else // not goal
            {
                // We now need to generate the successors of this node
                // The user helps us to do this, and we keep the new nodes in m_Successors ...
                m_Successors.Clear(); // empty vector of successor nodes to n

                // User provides this functions and uses AddSuccessor to add each successor of
                // node 'n' to m_Successors
                bool ret = false;
                if (n.parent != null)
                {
                    ret = n.m_UserState.GetSuccessors(this, n.parent.m_UserState);
                }
                else
                {
                    ret = n.m_UserState.GetSuccessors(this, null);
                }

                if (!ret)
                {
                    m_Successors.Clear(); // empty vector of successor nodes to n

                    // free up everything else we allocated
                    FreeSolutionNodes();

                    m_State = SearchState.OutOfMemory;
                    return m_State;
                }

                // Now handle each successor to the current node ...
                Node successor = null;
                int successors_size = m_Successors.Count;
                for (int i = 0; i < successors_size; ++i)
                {
                    successor = m_Successors[i];

                    // 	The g value for this successor ...
                    float newg = n.g + n.m_UserState.GetCost(successor.m_UserState);

                    if (m_movePoint > 0 && newg > m_movePoint)
                        continue;

                    // Now we need to find whether the node is on the open or closed lists
                    // If it is but the node that is already on them is better (lower g)
                    // then we can forget about this successor

                    // First linear search of open list to find node
                    Node openlist_result = null;
                    bool foundOpenNode = false;
                    foreach(var node in m_OpenList)
                    {
                        openlist_result = node;
                        if (openlist_result.m_UserState.IsSameState(successor.m_UserState))
                        {
                            foundOpenNode = true;
                            break;
                        }
                    }

                    if (foundOpenNode)
                    {
                        // we found this state on open
                        if (openlist_result.g <= newg)
                        {
                            // the one on Open is cheaper than this one
                            continue;
                        }
                    }

                    Node closedlist_result = null;
                    int closedlist_size = m_ClosedList.Count;
                    bool foundClosedNode = false;
                    for (int k = 0; k < closedlist_size; ++k)
                    {
                        closedlist_result = m_ClosedList[k];
                        if (closedlist_result.m_UserState.IsSameState(successor.m_UserState))
                        {
                            foundClosedNode = true;
                            break;
                        }
                    }

                    if (foundClosedNode)
                    {
                        // we found this state on closed
                        if (closedlist_result.g <= newg)
                        {
                            // the one on Closed is cheaper than this one
                            continue;
                        }
                    }

                    // This node is the best node so far with this particular state
                    // so lets keep it and set up its AStar specific data ...
                    successor.parent = n;
                    successor.g = newg;
                    if (!m_isSearchRegion)
                        successor.h = successor.m_UserState.GoalDistanceEstimate(m_Goal.m_UserState);
                    else successor.h = 0;

                    successor.f = successor.g + successor.h;

                    // Remove successor from closed if it was on it
                    if (foundClosedNode)
                    {
                        // remove it from Closed
                        m_ClosedList.Remove(closedlist_result);
                    }

                    // Update old version of this node
                    if (foundOpenNode)
                    {
                        m_OpenList.Remove(openlist_result);
                    }

                    SortedAddToOpenList(successor);
                }

                // push n onto Closed, as we have expanded it now
                m_ClosedList.Add(n);

                if (m_ClosedList.Count > closedListHighWaterMark)
                {
                    closedListHighWaterMark = m_ClosedList.Count;
                }
            } // end else (not goal so expand)

            return m_State; // 'Succeeded' bool is false at this point. 
        }

        // User calls this to add a successor to a list of successors
        // when expanding the search frontier
        

        // Get start node
        private MapSearchNode GetSolutionStart()
        {
            m_CurrentSolutionNode = m_Start;

            if (m_Start != null)
            {
                return m_Start.m_UserState;
            }
            else
            {
                return null;
            }
        }

        // Get next node
        private MapSearchNode GetSolutionNext()
        {
            if (m_CurrentSolutionNode != null)
            {
                if (m_CurrentSolutionNode.child != null)
                {
                    Node child = m_CurrentSolutionNode.child;
                    m_CurrentSolutionNode = m_CurrentSolutionNode.child;
                    return child.m_UserState;
                }
            }

            return null;
        }

        // Free the solution nodes
        // This is done to clean up all used Node memory when you are done with the
        // search
        private void FreeSolutionNodes()
        {
            m_OpenList.Clear();
            m_ClosedList.Clear();
            m_Successors.Clear();

            for (int i = 0; i < kPreallocatedNodes; ++i)
            {
                m_FixedSizeAllocator[i].Reinitialize();
            }
        }

        #endregion

        public bool AddSuccessor(MapSearchNode state)
        {
            Node node = AllocateNode();

            if (node != null)
            {
                node.m_UserState = state;
                m_Successors.Add(node);

                if (m_Successors.Count > successorListHighWaterMark)
                {
                    successorListHighWaterMark = m_Successors.Count;
                }
                return true;
            }

            return false;
        }

        #region 对外方法

        public bool FindPath(Map map, NodePosition start,NodePosition goal, List<NodePosition> path, int movePoint = 0)
		{
            path.Clear();
            m_isSearchRegion = false;
            m_movePoint = movePoint;

            InitiatePathfind(map);
            MapSearchNode nodeStart = AllocateMapSearchNode(start);
            MapSearchNode nodeEnd = AllocateMapSearchNode(goal);
            SetStartAndGoalStates(nodeStart, nodeEnd);

            SearchState searchState = SearchState.Searching;
            uint searchSteps = 0;
            do
            {
                searchState = SearchStep();
                searchSteps++;
            }
            while (searchState == SearchState.Searching);

            // Search complete
            bool pathfindSucceeded = (searchState ==SearchState.Succeeded);
            if (pathfindSucceeded)
            {
				// Success
                int numSolutionNodes = 0;   // Don't count the starting cell in the path length

                // Get the start node
                MapSearchNode node = GetSolutionStart();
                path.Add(node.m_position);
                ++numSolutionNodes;

                // Get all remaining solution nodes
                for (; ; )
                {
                    node = GetSolutionNext();

                    if (node == null)
                    {
                        break;
                    }

                    ++numSolutionNodes;
                    path.Add(node.m_position);
                };

                // Once you're done with the solution we can free the nodes up
                FreeSolutionNodes();
                //Debug.Log("Solution path length: " + numSolutionNodes);
				return true;
            }
            else
            {
                // FAILED, no path to goal
                Debug.Log("Pathfind FAILED!");
                FreeSolutionNodes();
                return false;
            }
        }

        public bool FindRegion(Map map, NodePosition start, int movePoint, List<NodePosition> region)
        {
            region.Clear();

            m_isSearchRegion = true;
            m_movePoint = movePoint;

            InitiatePathfind(map);
            MapSearchNode startNode = AllocateMapSearchNode(start);
            SetRegionStartState(startNode);

            do
            {
                SearchStep();
            }
            while (m_State == SearchState.Searching);

            if (m_State == SearchState.Succeeded)
            {
                foreach (var node in m_ClosedList)
                {
                    if (!region.Contains(node.m_UserState.m_position))
                    {
                        region.Add(node.m_UserState.m_position);
                    }
                }

                // Once you're done with the solution we can free the nodes up
                FreeSolutionNodes();
                return true;
            }
            else
            {
                FreeSolutionNodes();
                return false;
            }
        }

        #endregion

        
	}
}
