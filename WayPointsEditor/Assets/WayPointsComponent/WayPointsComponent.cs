using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace bluebean
{
    [ExecuteInEditMode]
    [System.Serializable]
    public class WayPointsComponent : MonoBehaviour
    {
        public Transform rootNode;
        public Transform rootEdge;

        [SerializeField]
        public Dictionary<int, EditorNode> nodes = new Dictionary<int, EditorNode>();
        [SerializeField]
        public Dictionary<int, List<EditorEdge>> edges = new Dictionary<int, List<EditorEdge>>();
        [SerializeField]
        [HideInInspector]
        private int m_maxNodeId = 0;
        [SerializeField]
        [HideInInspector]
        private int m_maxEdgeId = 0;

        protected EditorNode CreateNode(int id, Vector3 position)
        {
            var go = new GameObject("node_" + id.ToString());
            if (this.rootNode != null)
            {
                go.transform.SetParent(rootNode, false);
            }
            var node = go.AddComponent<EditorNode>();
            node.SetInfo(id, new Vector3(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), position.z), 3f);
            return node;
        }

        protected EditorEdge CreateEdge(int id, EditorNode startNode, EditorNode endNode)
        {
            var go = new GameObject("edge_" + id.ToString());
            if (this.rootEdge != null)
            {
                go.transform.SetParent(rootEdge, false);
            }
            var edge = go.AddComponent<EditorEdge>();
            edge.SetInfo(id, startNode, endNode);
            return edge;
        }

        private void NormalizeNodeId()
        {
            int nodeCount = nodes.Count;
            List<int> currentNodeIds = new List<int>(nodes.Keys);
            currentNodeIds.Sort();
            for (int i = 0; i < nodeCount; i++)
            {
                if (i != currentNodeIds[i])
                {
                    int idToChange = currentNodeIds[i];
                    nodes[i] = nodes[idToChange];
                    nodes.Remove(idToChange);
                    nodes[i].id = i;
                    nodes[i].gameObject.name = "node_" + i.ToString();
                    if (edges.ContainsKey(idToChange))
                    {
                        edges[i] = edges[idToChange];
                        edges.Remove(idToChange);
                    }
                }
            }
        }

        public EditorNode AddNode(Vector3 pos)
        {
            var node = CreateNode(m_maxNodeId++, pos);
            this.nodes.Add(node.id, node);
            return node;
        }

        public void RemoveNode(EditorNode node)
        {
            List<EditorEdge> edgesToRemove = new List<EditorEdge>();
            if (node != null)
            {
                foreach (int k in edges.Keys)
                {
                    if (edges[k] != null)
                    {
                        foreach (var e in edges[k])
                        {
                            if (e.start.id == node.id || e.end.id == node.id)
                            {
                                edgesToRemove.Add(e);
                            }
                        }
                    }
                }
                GameObject.DestroyImmediate(node.gameObject);
                nodes.Remove(node.id);
            }
            foreach (var e in edgesToRemove)
            {
                RemoveEdge(e);
            }
            NormalizeNodeId();
        }

        public EditorEdge AddEdge(EditorNode startNode, EditorNode endNode)
        {
            var edge = CreateEdge(m_maxEdgeId++, startNode, endNode);
            int startId = edge.start.id;
            if (!edges.ContainsKey(startId))
            {
                edges[edge.start.id] = new List<EditorEdge>();
            }
            edges[startId].Add(edge);
            return edge;
        }

        public void RemoveEdge(EditorEdge edge)
        {
            if (edges.ContainsKey(edge.start.id))
            {
                edges[edge.start.id].Remove(edge);
            }
            if (edges.ContainsKey(edge.end.id))
            {
                edges[edge.end.id].Remove(edge);
            }
            GameObject.DestroyImmediate(edge.gameObject);
        }

        public bool HasEdge(int startNodeId, int endNodeId)
        {
            Assert.IsTrue(nodes.ContainsKey(startNodeId), "Does not contains node with id " + startNodeId.ToString());
            Assert.IsTrue(nodes.ContainsKey(endNodeId), "Does not contains node with id " + endNodeId.ToString());
            if (!nodes.ContainsKey(startNodeId))
            {
                return false;
            }
            if (!nodes.ContainsKey(endNodeId))
            {
                return false;
            }
            bool result = false;
            var startNode = nodes[startNodeId];
            var endNode = nodes[endNodeId];
            if (edges.ContainsKey(startNodeId))
            {
                foreach (var e in edges[startNodeId])
                {
                    if (e.end == endNode)
                    {
                        result = true;
                    }
                }
            }
            if (edges.ContainsKey(endNodeId))
            {
                foreach (var e in edges[endNodeId])
                {
                    if (e.end == startNode)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        public void Clear()
        {
            List<EditorNode> nodesToRemove = new List<EditorNode>(nodes.Values);
            foreach (var n in nodesToRemove)
            {
                RemoveNode(n);
            }
            this.m_maxEdgeId = 0;
            this.m_maxNodeId = 0;
        }

        public void Destroy()
        {
            Clear();
        }

    }
}
