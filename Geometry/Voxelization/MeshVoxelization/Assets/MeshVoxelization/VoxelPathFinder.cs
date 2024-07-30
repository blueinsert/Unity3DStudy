using System;
using UnityEngine;

namespace bluebean
{
    public class VoxelPathFinder
    {
        private MeshVoxelizer m_voxelizer = null;
        private bool[,,] m_closed;
        private PriorityQueue<TargetVoxel> m_open;

        public struct TargetVoxel : IEquatable<TargetVoxel>, IComparable<TargetVoxel>
        {
            public Vector3Int m_coordinates;
            public float m_distance;
            public float m_heuristic;
            //public TargetVoxel parent;

            public float cost
            {
                get { return m_distance + m_heuristic; }
            }

            public TargetVoxel(Vector3Int coordinates, float distance, float heuristic)
            {
                this.m_coordinates = coordinates;
                this.m_distance = distance;
                this.m_heuristic = heuristic;
            }

            public bool Equals(TargetVoxel other)
            {
                return this.m_coordinates.Equals(other.m_coordinates);
            }

            public int CompareTo(TargetVoxel other)
            {
                return this.cost.CompareTo(other.cost);
            }
        }

        public VoxelPathFinder(MeshVoxelizer voxelizer)
        {
            this.m_voxelizer = voxelizer;
            m_closed = new bool[voxelizer.m_resolution.x, voxelizer.m_resolution.y, voxelizer.m_resolution.z];
            m_open = new PriorityQueue<TargetVoxel>();
        }

        private TargetVoxel AStar(in Vector3Int start, Func<TargetVoxel,bool> termination, Func<Vector3Int, float> heuristic)
        {
            Array.Clear(m_closed, 0, m_closed.Length);

            // A* algorithm:
            m_open.Clear();
            m_open.Enqueue(new TargetVoxel(start, 0, 0));

            while (m_open.Count() != 0)
            {
                var current = m_open.Dequeue();

                if (termination(current))
                    return current;

                m_closed[current.m_coordinates.x, current.m_coordinates.y, current.m_coordinates.z] = true;

                for (int i = 0; i < MeshVoxelizer.fullNeighborhood.Length; ++i)
                {
                    var successorCoords = current.m_coordinates + MeshVoxelizer.fullNeighborhood[i];

                    if (m_voxelizer.VoxelExists(successorCoords) &&
                        m_voxelizer[successorCoords.x, successorCoords.y, successorCoords.z] != MeshVoxelizer.Voxel.Outside &&
                        !m_closed[successorCoords.x, successorCoords.y, successorCoords.z])
                    {
                        var successor = new TargetVoxel(successorCoords, current.m_distance + m_voxelizer.GetDistanceToNeighbor(i),
                                                        heuristic(successorCoords));
                        //successor.parent = current;

                        int index = -1;
                        for (int j = 0; j < m_open.Count(); ++j)
                            if (m_open.data[j].m_coordinates == successorCoords)
                            { index = j; break; }

                        if (index < 0)
                            m_open.Enqueue(successor);
                        else if (successor.m_distance < m_open.data[index].m_distance)
                            m_open.data[index] = successor;
                    }
                }
            }

            return new TargetVoxel(Vector3Int.zero, -1, -1);
        }

        public TargetVoxel FindClosestNonEmptyVoxel(in Vector3Int start)
        {
            if (m_voxelizer == null) return new TargetVoxel(Vector3Int.zero, -1, -1);

            if (!m_voxelizer.VoxelExists(start))
                return new TargetVoxel(Vector3Int.zero, -1, -1);

            if (m_voxelizer[start.x, start.y, start.z] != MeshVoxelizer.Voxel.Outside)
                return new TargetVoxel(start, 0, 0);

            Array.Clear(m_closed, 0, m_closed.Length);

            return AStar(start,
            (TargetVoxel v) => {
                return m_voxelizer[v.m_coordinates.x, v.m_coordinates.y, v.m_coordinates.z] != MeshVoxelizer.Voxel.Outside;
            },
            (Vector3Int c) => {
                return 0;
            });
        }

        public TargetVoxel FindPath(in Vector3Int start, Vector3Int end)
        {
            if (m_voxelizer == null) return new TargetVoxel(Vector3Int.zero,-1, -1);

            if (!m_voxelizer.VoxelExists(start) || !m_voxelizer.VoxelExists(end))
                return new TargetVoxel(Vector3Int.zero, -1, -1);

            if (m_voxelizer[start.x, start.y, start.z] == MeshVoxelizer.Voxel.Outside ||
                m_voxelizer[end.x, end.y, end.z] == MeshVoxelizer.Voxel.Outside)
                return new TargetVoxel(Vector3Int.zero, -1, -1);

            return AStar(start,
            (TargetVoxel v) => {
                return v.m_coordinates == end;
            },
            (Vector3Int c) => {
                return Vector3.Distance(c, end) * m_voxelizer.m_voxelSize;
            });
        }
    }
}
