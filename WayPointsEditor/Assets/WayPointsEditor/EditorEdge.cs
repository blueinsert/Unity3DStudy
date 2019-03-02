#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace bluebean {

[RequireComponent(typeof(EdgeCollider2D))]
public class EditorEdge : MonoBehaviour {
    public enum EDirection {
        None,
        Single,  // from start to end
        Inverse, // from end to start
        Dual,    // bidirectional
    }
    public int id;
    public EditorNode start;
    public EditorNode end;
    public EDirection direction;
  
    private Vector3    m_lastStartPosition;
    private Vector3    m_lastEndPosition;

    public void SetInfo(int id, EditorNode start, EditorNode end, EDirection direction = EDirection.Dual) {
        this.id = id;
        this.start = start;
        this.end   = end;
        this.direction = direction;
        UpdateCollider();
    }

    void OnDrawGizmos() {
        if(start != null && end != null && direction != EDirection.None) {
            if(direction == EDirection.Single) {
                Gizmos.color = UnityEngine.Color.green;
                PathfindingGizmo.DrawSingleArrowLine(start.position, end.position);
            }
            else if(direction == EDirection.Inverse) {
                Gizmos.color = UnityEngine.Color.green;
                PathfindingGizmo.DrawSingleArrowLine(end.position, start.position);
            }
            else if(direction == EDirection.Dual) {
                Gizmos.color = UnityEngine.Color.green;
                PathfindingGizmo.DrawDubleArrowLine(start.position, end.position);
            }
        }
    }

    void OnDrawGizmosSelected() {
        if(start != null && end != null && direction != EDirection.None) {
            if(direction == EDirection.Single) {
                Gizmos.color = UnityEngine.Color.red;
                PathfindingGizmo.DrawSingleArrowLine(start.position, end.position);
            }
            else if(direction == EDirection.Inverse) {
                Gizmos.color = UnityEngine.Color.red;
                PathfindingGizmo.DrawSingleArrowLine(end.position, start.position);
            }
            else if(direction == EDirection.Dual) {
                Gizmos.color = UnityEngine.Color.red;
                PathfindingGizmo.DrawDubleArrowLine(start.position, end.position);
            }
        }    
    }

    void Update() {
        if(start != null && end != null) {
            if(m_lastStartPosition != start.position || m_lastEndPosition != end.position) {
                UpdateCollider();
            }          
            m_lastStartPosition = start.position;
            m_lastEndPosition   = end.position;
        }
    }

    void UpdateCollider() {
        if(start != null && end != null && direction != EDirection.None) {
            transform.position = (start.position + end.position)/2f + new Vector3(0f, 0f, 1f);
            var collider = GetComponent<EdgeCollider2D>();
            var middle = (end.position - start.position)/2f;
            var points = new Vector2[2];
            points[0] = new Vector2(-middle.x, -middle.y);
            points[1] = new Vector2( middle.x,  middle.y);
            collider.points = points;     
            collider.edgeRadius = 2f;
        }     
        else {
            var collider = GetComponent<EdgeCollider2D>();
            collider.Reset();            
        }   
    }

    void OnValidate() {
        UpdateCollider();
    }

}
}
#endif