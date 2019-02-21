using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean {

    public class WayPointsEditorView {

        public GameObject rootMap;
        public GameObject rootNode;
        public GameObject rootEdge;
        public GameObject rootPlatform;

        public EditorNode AddNode(int id, Vector3 position) {
            var go = new GameObject("node_" + id.ToString());
            if (this.rootNode != null) {
                go.transform.parent = rootNode.transform;
            }
            var node = go.AddComponent<EditorNode>();
            node.SetInfo(id, new Vector3(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), position.z), 3f);
            return node;
        }

        public EditorEdge AddEdge(int id, EditorNode startNode, EditorNode endNode) {
            var go = new GameObject("edge_" + id.ToString());
            if (this.rootEdge != null) {
                go.transform.parent = this.rootEdge.transform;
            }
            var edge = go.AddComponent<EditorEdge>();
            edge.SetInfo(id, startNode, endNode);
            return edge;
        }

    }

}
