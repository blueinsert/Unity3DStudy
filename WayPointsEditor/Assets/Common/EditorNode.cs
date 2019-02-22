#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

namespace bluebean {

[RequireComponent(typeof(CircleCollider2D))]
public class EditorNode : MonoBehaviour,  IBeginDragHandler, IDragHandler, IEndDragHandler {
    public int id;
    public float range = 1f;

    public Vector3 position {
        get {
            return transform.position;
        }
    }

    float lastRange = 0f;

    public void SetInfo(int id, Vector3 position, float range) {
        this.id = id;
        transform.position = position;
        this.range = range;
    }

    void UpdateCollider() {
        var collider = GetComponent<CircleCollider2D>();
        collider.radius = range;                   
    }

    void Start() {

    }

    void Update() {
        if(lastRange != range) {
            UpdateCollider();
            lastRange = range;
        }
    }

    void OnDrawGizmosSelected() {
        if(id >= 0) {
            Gizmos.color = UnityEngine.Color.red;
            Gizmos.DrawIcon(position, "circle_red", false);
            Gizmos.DrawWireSphere(position, range);
        }        
    }

    void OnDrawGizmos() {
    	if(id >= 0) {
    		Gizmos.color = UnityEngine.Color.green;
    		Gizmos.DrawIcon(position, "circle_green", false);
            Gizmos.DrawWireSphere(position, range);
    	}
    }

    void OnValidate() {
        UpdateCollider();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Undo.RecordObject (gameObject.transform, "Drag Node");
    }

    public void OnDrag(PointerEventData data)
    {
        if(Selection.activeGameObject == gameObject && data.button == PointerEventData.InputButton.Left) {
            var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(data.position.x, data.position.y, 10f));
            transform.position = new Vector3(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y), Mathf.RoundToInt(worldPos.z));
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
}
#endif