using UnityEngine;
using UnityEditor;

using Flux;

using FluxEditor;

namespace FluxEditor
{
    //Fish：在Inspector面板中繪製
	[CustomEditor(typeof(FTransformEvent), true)]
	public class FTransformEventInspector : FTweenEventInspector
	{

        protected SerializedProperty _relativeToSelf;
        protected bool relativeToSelf;

        protected override void OnEnable()
        {
            _relativeToSelf = serializedObject.FindProperty("_relativePositionToSelf");
            base.OnEnable();
        }

        public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI();
			if( _tween.isExpanded )
			{
				serializedObject.Update();

                float arg = 1;
                if (target is FTweenPositionEvent)
                {
                    arg = 1.55f;
                }

				float doubleLineHeight = EditorGUIUtility.singleLineHeight * 2;

				Rect tweenRect = GUILayoutUtility.GetLastRect();

				tweenRect.yMin = tweenRect.yMax - doubleLineHeight* arg;
				tweenRect.height = EditorGUIUtility.singleLineHeight;

				tweenRect.xMin = tweenRect.xMax - 80;

				if( GUI.Button( tweenRect, "Set To", EditorStyles.miniButton ) )
					_to.vector3Value = GetPropertyValue();

				tweenRect.y -= doubleLineHeight+2.5f;

				if( GUI.Button( tweenRect, "Set From", EditorStyles.miniButton ) )
					_from.vector3Value = GetPropertyValue();

              
				serializedObject.ApplyModifiedProperties();
			}
		}

		public Vector3 GetPropertyValue()
		{
			FTransformEvent transformEvt = (FTransformEvent)target;
			if( transformEvt is FTweenPositionEvent )
				return transformEvt.Owner.localPosition;
			Debug.LogWarning( "Unexpected child of FTransformEvent, setting (0,0,0)" );
			return Vector3.zero;
		}
	}
}
