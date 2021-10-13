using UnityEngine;
using UnityEditor;

using Flux;

namespace FluxEditor
{
	[CustomEditor(typeof( FTweenEvent<> ), true)]
	public class FTweenEventInspector : FEventInspector {

		protected SerializedProperty _tween;
		protected SerializedProperty _from;
		protected SerializedProperty _to;

		protected override void OnEnable()
		{
			base.OnEnable();

            _tween = serializedObject.FindProperty("_tween");
            if (_tween != null)
            {
                _tween.isExpanded = true;
                _from = _tween.FindPropertyRelative("_from");
                _to = _tween.FindPropertyRelative("_to");

            }

        }

	}
}
