using UnityEngine;
using System;
using System.Collections;


namespace zEditorWindow
{
	[System.Serializable]
	public class zNodeTransitionLine : zBaseNode
	{
		#region Member

		public override NodeType type {
			get {
				return NodeType.enNodeTransition;
			}
		}

		static float LineWidth = 10.0f;
		static float LineHeight = 10.0f;

		[HideInInspector]
		zBaseNode startNode;
		[HideInInspector]
		zBaseNode endNode;


		Rect startRect = new Rect();
		Rect middleRect = new Rect();
		Rect endRect = new Rect();
		#endregion


		public zNodeTransitionLine (zBaseNode _startNode, zBaseNode _endNode) : base()
		{
			startNode = _startNode;
			endNode = _endNode;
			name = "Line";
		}

		public override void Draw (Vector2 _canvasOffset, GUIStyle _style)
		{
			base.Draw (_canvasOffset, _style);

			resetRect(_canvasOffset);

			GUI.Box (startRect, "", _style);
			GUI.Box (middleRect, "", _style);
			GUI.Box (endRect, "", _style);
		}
	

		public override bool Contains (Vector2 _mousePos, Vector2 _canvasOffset)
		{
			resetRect(_canvasOffset);

			if (startRect.Contains (_mousePos))
				return true;
			if (middleRect.Contains (_mousePos))
				return true;
			if (endRect.Contains (_mousePos))
				return true;
			return false;
		}

		public override bool IntersectRect (Rect _r, Vector2 _canvasOffset)
		{
			resetRect(_canvasOffset);

			_r = new Rect (_r.position + _canvasOffset, _r.size);

			if (zTool.IsRectIntersect (_r, startRect))
				return true;
			if (zTool.IsRectIntersect (_r, middleRect))
				return true;
			if (zTool.IsRectIntersect (_r, endRect))
				return true;
			return false;
		}

		public override void Despown ()
		{
			base.Despown ();

			(startNode as zNode).RemoveTransition (endNode);
		}

		#region Interface

		public zBaseNode GetStartNode(){
			return startNode;
		}
		public zBaseNode GetEndNode(){
			return endNode;
		}
		#endregion

		#region Private
	
		/// <summary>
		/// 刷新线的区域
		/// </summary>
		/// <param name="_canvasOffset">Canvas offset.</param>
		void resetRect(Vector2 _canvasOffset){
			if (!endNode.IsUse) {
				Despown ();
				return;
			}


			float xDis = startNode.middlePos.x - endNode.middlePos.x;
			float yDis = startNode.middlePos.y - endNode.middlePos.y;
			bool up = yDis > 0 ? true : false;
			bool right = xDis < 0 ? true : false;

			Vector2 startRSize = new Vector2 (LineWidth, Mathf.Abs (yDis) / 2);
			Vector2 middleRSize = new Vector2 (Mathf.Abs (xDis), LineHeight);
			Vector2 endRSize = new Vector2 (LineWidth, Mathf.Abs (yDis) / 2);


			Vector2 startRPos = startNode.middlePos + _canvasOffset;
			Vector2 middleRPos = getMiddlePos (startRPos, up, startRSize.y);
			Vector2 endRPos = endNode.middlePos + _canvasOffset;

			startRect = getStartRect (startRPos, startRSize, up);
			middleRect = getMiddleRect (middleRPos, middleRSize, right);
			endRect = getEndRect (endRPos, endRSize, up);
		}

		#endregion

		#region Tools


		public static void DrawLine(Vector2 _startPos, Vector2 _endPos, GUIStyle _style){
			float xDis = _startPos.x - _endPos.x;
			float yDis = _startPos.y - _endPos.y;
			bool up = yDis > 0 ? true : false;
			bool right = xDis < 0 ? true : false;

			Vector2 startRSize = new Vector2 (LineWidth, Mathf.Abs (yDis) / 2);
			Vector2 middleRSize = new Vector2 (Mathf.Abs (xDis), LineHeight);
			Vector2 endRSize = new Vector2 (LineWidth, Mathf.Abs (yDis) / 2);

			Vector2 middleRPos = getMiddlePos (_startPos, up, startRSize.y);

			Rect _startR = getStartRect (_startPos, startRSize, up);
			Rect _middleR = getMiddleRect (middleRPos, middleRSize, right);
			Rect _endR = getEndRect (_endPos, endRSize, up);

			GUI.Box (_startR, "", _style);
			GUI.Box (_middleR, "", _style);
			GUI.Box (_endR, "", _style);
		}

		static Vector2 getMiddlePos (Vector2 _startPos, bool _up, float _height)
		{
			Vector2 _result = new Vector2 (_startPos.x, _startPos.y);
			if (_up)
				_result.y -= _height;
			else
				_result.y += _height;
			return _result;
		}

		static Rect getStartRect (Vector2 _pos, Vector2 _size, bool _up)
		{
			Rect _result = new Rect (new Vector2 (_pos.x, _pos.y), new Vector2(_size.x, _size.y + LineHeight));
			if (_up)
				_result = new Rect (new Vector2 (_pos.x, _pos.y - _size.y), _size);
		
			return _result;
		}

		static Rect getMiddleRect (Vector2 _pos, Vector2 _size, bool _right)
		{
			Rect _result = new Rect (new Vector2 (_pos.x, _pos.y), _size);
			if (!_right)
				_result = new Rect (new Vector2 (_pos.x - _size.x, _pos.y), _size);

			return _result;
		}

		static Rect getEndRect (Vector2 _pos, Vector2 _size, bool _up)
		{
			Rect _result = new Rect (new Vector2 (_pos.x, _pos.y), new Vector2(_size.x, _size.y + LineHeight));
			if (!_up)
				_result = new Rect (new Vector2 (_pos.x, _pos.y - _size.y), _size);

			return _result;
		}

		#endregion
	}
}