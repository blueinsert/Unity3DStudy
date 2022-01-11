using UnityEngine;
using UnityEditor;

using Flux;

namespace FluxEditor
{
	public class FSequenceWindowToolbar
	{
		private const int ROW_HEIGHT = 20;
		private const int NUM_ROWS = 1;
		private const int SPACE = 5;
		public const int HEIGHT = ROW_HEIGHT * NUM_ROWS + SPACE*(NUM_ROWS+1);
		private const int FRAME_FIELD_WIDTH = 100;

		private FSequenceEditorWindow _window = null;

		private bool _showViewRange;
		private GUIContent _viewRangeLabel = null;
		private GUIContent _viewRangeDash = null;

		private Rect _viewRangeLabelRect;
		private Rect _viewRangeDashRect;
		private Rect _viewRangeStartRect;
		private Rect _viewRangeEndRect;

		public FSequenceWindowToolbar( FSequenceEditorWindow window )
		{
			_window = window;

			_viewRangeLabel = new GUIContent( "View Range" );
			_viewRangeDash = new GUIContent( " - " );
		}

		public void RebuildLayout( Rect rect )
		{
			rect.xMin += SPACE;
			rect.yMin += SPACE;
			rect.xMax -= SPACE;
			rect.yMax -= SPACE;

			float reminderWidth = rect.width;

			_viewRangeLabelRect = rect;
			_viewRangeLabelRect.width = EditorStyles.label.CalcSize( _viewRangeLabel ).x + SPACE;
			_viewRangeDashRect = rect;
			_viewRangeDashRect.width = EditorStyles.label.CalcSize( _viewRangeDash ).x;
			_viewRangeStartRect = rect;
			_viewRangeStartRect.width = FRAME_FIELD_WIDTH;
			_viewRangeEndRect = rect;
			_viewRangeEndRect.width = FRAME_FIELD_WIDTH;

			_viewRangeEndRect.x = rect.xMax - _viewRangeEndRect.width;
			_viewRangeDashRect.x = _viewRangeEndRect.xMin - _viewRangeDashRect.width;
			_viewRangeStartRect.x = _viewRangeDashRect.xMin - _viewRangeStartRect.width;
			_viewRangeLabelRect.x = _viewRangeStartRect.xMin - _viewRangeLabelRect.width;

			reminderWidth -= _viewRangeLabelRect.width + _viewRangeStartRect.width + _viewRangeDashRect.width + _viewRangeEndRect.width;

			_showViewRange = reminderWidth >= 0;
		}

		public void OnGUI()
		{
			FrameRange viewRange = _window.GetSequenceEditor().ViewRange;

			GUI.backgroundColor = Color.white;
			GUI.contentColor = FGUI.GetTextColor();
			GUIStyle numberFieldStyle = new GUIStyle(EditorStyles.numberField);
			numberFieldStyle.alignment = TextAnchor.MiddleCenter;

			if( _showViewRange )
			{
				EditorGUI.PrefixLabel( _viewRangeLabelRect, _viewRangeLabel );
				EditorGUI.BeginChangeCheck();
				viewRange.Start = EditorGUI.IntField( _viewRangeStartRect, viewRange.Start, numberFieldStyle );
				EditorGUI.PrefixLabel( _viewRangeDashRect, _viewRangeDash );
				viewRange.End = EditorGUI.IntField( _viewRangeEndRect, viewRange.End, numberFieldStyle );
				if( EditorGUI.EndChangeCheck() )
					_window.GetSequenceEditor().SetViewRange( viewRange );
			}
		}
	}
}