#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace bluebean {

    public class WayPointsEditorModule {
        private WayPointsEditorView m_viewManager;
        public Dictionary<int, EditorNode> nodes = new Dictionary<int, EditorNode>();
        public Dictionary<int, List<EditorEdge>> edges = new Dictionary<int, List<EditorEdge>>();
        private int m_maxNodeId = 0;
        private int m_maxEdgeId = 0;

        public WayPointsEditorModule( WayPointsEditorView view) {
            m_viewManager = view;
        }

        private void NormalizeNodeId() {
            int nodeCount = nodes.Count;
            List<int> currentNodeIds = new List<int>(nodes.Keys);
            currentNodeIds.Sort();
            for (int i = 0; i < nodeCount; i++) {
                if (i != currentNodeIds[i]) {
                    int idToChange = currentNodeIds[i];
                    nodes[i] = nodes[idToChange];
                    nodes.Remove(idToChange);
                    nodes[i].id = i;
                    nodes[i].gameObject.name = "node_" + i.ToString();
                    if (edges.ContainsKey(idToChange)) {
                        edges[i] = edges[idToChange];
                        edges.Remove(idToChange);
                    }
                }
            }
        }

        public EditorNode AddNode(Vector3 pos) {
            var node = m_viewManager.AddNode(m_maxNodeId++, pos);
            this.nodes.Add(node.id, node);
            return node;
        }

       
        public void RemoveNode(EditorNode node) {
            List<EditorEdge> edgesToRemove = new List<EditorEdge>();
            if (node != null) {
                foreach (int k in edges.Keys) {
                    if (edges[k] != null) {
                        foreach (var e in edges[k]) {
                            if (e.start.id == node.id || e.end.id == node.id) {
                                edgesToRemove.Add(e);
                            }
                        }
                    }
                }
                GameObject.DestroyImmediate(node.gameObject);
                nodes.Remove(node.id);
            }
            foreach (var e in edgesToRemove) {
                RemoveEdge(e);
            }
            NormalizeNodeId();
        }

        public EditorEdge AddEdge(EditorNode startNode, EditorNode endNode) {
            var edge = m_viewManager.AddEdge(m_maxEdgeId++, startNode, endNode);
            int startId = edge.start.id;
            if (!edges.ContainsKey(startId)) {
                edges[edge.start.id] = new List<EditorEdge>();
            }
            edges[startId].Add(edge);
            return edge;
        }

        public void RemoveEdge(EditorEdge edge) {
            if (edges.ContainsKey(edge.start.id)) {
                edges[edge.start.id].Remove(edge);
            }
            if (edges.ContainsKey(edge.end.id)) {
                edges[edge.end.id].Remove(edge);
            }
            GameObject.DestroyImmediate(edge.gameObject);
        }

        public bool HasEdge(int startNodeId, int endNodeId) {
            Assert.IsTrue(nodes.ContainsKey(startNodeId), "Does not contains node with id " + startNodeId.ToString());
            Assert.IsTrue(nodes.ContainsKey(endNodeId), "Does not contains node with id " + endNodeId.ToString());
            if (!nodes.ContainsKey(startNodeId)) {
                return false;
            }
            if (!nodes.ContainsKey(endNodeId)) {
                return false;
            }
            bool result = false;
            var startNode = nodes[startNodeId];
            var endNode = nodes[endNodeId];
            if (edges.ContainsKey(startNodeId)) {
                foreach (var e in edges[startNodeId]) {
                    if (e.end == endNode) {
                        result = true;
                    }
                }
            }
            if (edges.ContainsKey(endNodeId)) {
                foreach (var e in edges[endNodeId]) {
                    if (e.end == startNode) {
                        result = true;
                    }
                }
            }
            return result;
        }
 
        public void Clear() {
            List<EditorNode> nodesToRemove = new List<EditorNode>(nodes.Values);
            foreach (var n in nodesToRemove) {
                RemoveNode(n);
            }
            this.m_maxEdgeId = 0;
            this.m_maxNodeId = 0;
        }

        public void Destroy() {
            Clear();
        }

    }
}
#endif