#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean {

public static class PathfindingGizmo {
    static float arrowlen = 2f;
    static float cos30 = 0.866f * arrowlen;
    static float sin30 = 0.5f * arrowlen;

    public static void DrawCross(Vector3 pos) {
        Gizmos.DrawLine(pos - new Vector3(3,0,0), pos + new Vector3(3,0,0));
        Gizmos.DrawLine(pos - new Vector3(0,3,0), pos + new Vector3(0,3,0));
    }

    public static void DrawSingleArrowLine(Vector3 start, Vector3 end) {
        Gizmos.DrawLine(start, end);
        var middle = (start + end)/2f;
        var n = (start - end).normalized;

        float x_cos30 = n.x * cos30;
        float x_sin30 = n.x * sin30;
        float y_cos30 = n.y * cos30;
        float y_sin30 = n.y * sin30;

        float x1 = x_cos30 - y_sin30; 
        float y1 = x_sin30 + y_cos30;

        float x2 = x_cos30 + y_sin30;
        float y2 = -x_sin30 + y_cos30;
        Gizmos.DrawLine(middle, middle + new Vector3(x1, y1, middle.z));        
        Gizmos.DrawLine(middle, middle + new Vector3(x2, y2, middle.z));     
    }

    public static void DrawDubleArrowLine(Vector3 start, Vector3 end) {
        Gizmos.DrawLine(start, end);
        var middle = (end - start)/3;
        var n = (start - end).normalized;

        float x_cos30 = n.x * cos30;
        float x_sin30 = n.x * sin30;
        float y_cos30 = n.y * cos30;
        float y_sin30 = n.y * sin30;

        float x1 = x_cos30 - y_sin30;
        float y1 = x_sin30 + y_cos30;

        float x2 = x_cos30 + y_sin30;
        float y2 = -x_sin30 + y_cos30;

        Gizmos.DrawLine(end - middle, end - middle + new Vector3(x1, y1, middle.z));
        Gizmos.DrawLine(end - middle, end - middle + new Vector3(x2, y2, middle.z));

        Gizmos.DrawLine(start + middle, start + middle + new Vector3(-x1, -y1, middle.z));
        Gizmos.DrawLine(start + middle, start + middle + new Vector3(-x2, -y2, middle.z));
    }
}
}
#endif
