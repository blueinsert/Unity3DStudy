using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using NodeCanvas;

namespace zEditorWindow
{

	public class CanvasEditor : EditorWindow
	{
		#region Member

		VoidDelegate drawCalback;


		bool PlatformIOS { // 苹果平台下设置ture，其他平台设为false
			get { 
				return true;
			}
		}


		CanvasEditorData datas;


		/// <summary>
		/// 是否可以拖拽节点
		/// </summary>
		/// <value><c>true</c> if can drag node; otherwise, <c>false</c>.</value>
		bool canDragNode {
			get {
				return true;
			}
		}

		bool drawMainTool = true;
		bool canDraw = false;

		float GridMinorSize = 12f;
		float GridMajorSize = 120f;

		/// <summary>
		/// 本来想做为Node 大小缩放的,暂时没有开放
		/// </summary>
		protected const float _scale = 1;

		/// <summary>
		/// 是否绘制左侧面板
		/// </summary>
		/// <value><c>true</c> if draw left panel; otherwise, <c>false</c>.</value>
		protected virtual bool _drawLeftPanel {
			get{ return true; }
		}

		/// <summary>
		/// 是否绘制右侧面板
		/// </summary>
		/// <value><c>true</c> if draw right panel; otherwise, <c>false</c>.</value>
		protected virtual bool _drawRightPanel {
			get{ return true; }
		}

	

		/// <summary>
		/// 当前选择的东西
		/// n没有选择,选Node,选NodeGroup,选过渡线
		/// </summary>
		/// <value>The type of the select.</value>
		protected SelectType _selectType {
			get { 
				if (__selectType == SelectType.Node || __selectType == SelectType.NodeGroud) {
					if (datas.GetSelectedNode ().Count == 0)
						__selectType = SelectType.None;
				}
				return __selectType; 
			}
			set {
				__selectType = value;
				Repaint ();
			}
		}

		SelectType __selectType;

		/// <summary>
		/// 当前事件
		/// </summary>
		protected Event _currentEvent;
		Vector2 cachedMousePosition;
		Vector2 cachedMouseStart;
		Vector2 _offset;

		#region 画布成员变量

		/// <summary>
		/// 画布偏移位置
		/// </summary>
		/// <value>The offset.</value>
		protected Vector2 _canvasOffset {
			get{ return _offset; }
			set { 
				_offset = value; 
				_offset.x = Mathf.Clamp (_offset.x, -_maxCanvasScale, 0);
				_offset.y = Mathf.Clamp (_offset.y, -_maxCanvasScale, 0);
			}
		}

		/// <summary>
		/// 画布尺寸
		/// </summary>
		/// <value>The max scale.</value>
		protected virtual int _maxCanvasScale {
			get{ return 1000; }
		}

		/// <summary>
		/// 左侧面板宽度
		/// </summary>
		/// <value>The width of the left panel.</value>
		protected virtual float _leftPanelWidth {
			get{ return 200; }
		}

		/// <summary>
		/// 右侧面板宽度
		/// </summary>
		/// <value>The width of the right panel.</value>
		protected virtual float _rightPanelWidth {
			get{ return 200; }
		}

		float _rightxPosition {
			get { 
				if (_drawRightPanel)
					return ScreenWidth - _rightPanelWidth;
				return ScreenWidth;
			}
		}

		float ScreenWidth {
			get {
				float _result = Screen.width;
				if (PlatformIOS)
					_result = _result / 2;
				return _result;
			}
		}

		/// <summary>
		/// 中间画布大小
		/// </summary>
		/// <value>The size of the scaled canvas.</value>
		Rect scaledCanvasSize {
			get {
				float width = ScreenWidth;

				if (_drawLeftPanel)
					width -= _leftPanelWidth;
				if (_drawRightPanel)
					width -= _rightPanelWidth;

				return new Rect (0, 0, width, Screen.height);
			}
		}

		/// <summary>
		/// 是否绘制迷你地图
		/// </summary>
		/// <value><c>true</c> if can draw mini node; otherwise, <c>false</c>.</value>
		protected virtual bool canDrawMiniNode {
			get {
				return true;
			}
		}

		/// <summary>
		/// 迷你地图区域
		/// </summary>
		/// <value>The mini canvas position.</value>
		protected virtual Vector2 miniCanvasPosition {
			get { 
				return new Vector2 (_rightxPosition - _rightPanelWidth, EditorStyles.toolbar.fixedHeight);
			}
		}

		/// <summary>
		/// 迷你画布大小
		/// </summary>
		/// <value>The size of the mini canvas.</value>
		protected virtual Vector2 miniCanvasSize {
			get {
				return new Vector2 (_rightPanelWidth, 200);
			}
		}

		/// <summary>
		/// 迷你画布里光标的坐标
		/// </summary>
		/// <value>The mini seer.</value>
		Rect miniSeer {
			get {
				Rect _result = new Rect ();
				_result.position = new Vector2 (_canvasOffset.x * -1 * miniCanvasScaleRate.x, _canvasOffset.y * -1 * miniCanvasScaleRate.y);
				_result.size = new Vector2 (scaledCanvasSize.width * miniCanvasScaleRate.x, scaledCanvasSize.height * miniCanvasScaleRate.y);
				return _result;
			}
		}

		/// <summary>
		/// 迷你地图缩放比例
		/// </summary>
		/// <value>The mini canvas scale rate.</value>
		Vector2 miniCanvasScaleRate {
			get {
				Vector2 _result = new Vector2 ();
				_result.x = miniCanvasSize.x / (scaledCanvasSize.width + _maxCanvasScale);
				_result.y = miniCanvasSize.y / (scaledCanvasSize.height + _maxCanvasScale);
				return _result;
			}
		}

		/// <summary>
		/// 迷你地图中节点大小
		/// </summary>
		/// <value>The size of the mini node.</value>
		Vector2 miniNodeSize {
			get {
				return new Vector2 (5, 5);
			}
		}


		#endregion

		/// <summary>
		/// 当前GroupPath
		/// </summary>
		protected string _currentGroupPath = "Test/xxx/wer";

		#endregion

		[MenuItem ("Assets/TestEditorWindow")]
		public static void onInit ()
		{
			GetWindow<CanvasEditor> ();
		}

	


		#region 原生OnGUI绘制

		void OnGUI ()
		{
//			return;
			OnDrawMainTool ();
			if (!canDraw)
				return;

			OnDrawLeftPanel ();
			OnDrawMiddlePanelBegin ();

			OnDrawNode ();

			DrawMiddlePanel ();

		

			OnDrawMiddlePanelEnd ();

			OnDrawRightPanel ();

			OnDrawMiniCanvas ();
			OtherGUI ();
			Repaint ();
			datas.UpdateData (Time.deltaTime);
		}

		void OnDrawMainTool ()
		{
			if (!drawMainTool)
				return;

			GUILayout.BeginHorizontal (EditorStyles.toolbar);
			GUILayout.BeginHorizontal (GUILayout.Width (_leftPanelWidth));
			DrawMainTool1 ();
			GUILayout.EndHorizontal ();
			DrawMainTool2 ();
			GUILayout.EndHorizontal ();
		}

		void OnDrawLeftPanel ()
		{
			if (!_drawLeftPanel)
				return;
	
			GUILayout.BeginArea (new Rect (0, EditorStyles.toolbar.fixedHeight, _leftPanelWidth, Screen.height - EditorStyles.toolbar.fixedHeight));
			DrawLeftPanel ();
			GUILayout.EndArea ();
		}

		void OnDrawMiddlePanelBegin ()
		{
			
			float widthleft = _leftPanelWidth;
			float widthright = _rightPanelWidth;
			if (!_drawLeftPanel)
				widthleft = 0;
			if (!_drawRightPanel)
				widthright = 0;
			GUILayout.BeginArea (new Rect (_leftPanelWidth, EditorStyles.toolbar.fixedHeight, ScreenWidth - widthleft - widthright, Screen.height - EditorStyles.toolbar.fixedHeight));
			_currentEvent = Event.current;
			cachedMousePosition = _currentEvent.mousePosition;

			if (_currentEvent.type == EventType.ScrollWheel) {
				_canvasOffset = new Vector2 (_canvasOffset.x, _canvasOffset.y + _currentEvent.delta.y * -10);
				UpdateOffset ();
				Event.current.Use ();
			}
			if (_currentEvent.type == EventType.MouseDrag && _currentEvent.button == 2) {
				_canvasOffset += _currentEvent.delta;
				UpdateOffset ();
				Event.current.Use ();
			}
			if (_currentEvent.type == EventType.MouseUp && _currentEvent.button == 1) {
				GenericMenu menu = new GenericMenu ();
				switch (datas.selectedNodeType) {
				case NodeType.enNode:
					ContextMenu_Node (ref menu);	
					break;
				case NodeType.enNodeGroup:
					ContextMenu_NodeGroud (ref menu);	
					break;
				case NodeType.enNodeTransition:
					ContextMenu_Transition (ref menu);	
					break;
				case NodeType.enNull:
					ContextMenu_Canvas (ref menu);	
					break;
			
				}

				menu.ShowAsContext ();
			}
			if (_currentEvent.type == EventType.Repaint) {
				NodeStyles.canvasBackground.Draw (scaledCanvasSize, false, false, false, false);
				DrawGrid ();
			}
		}


		/// <summary>
		/// 绘制迷你地图
		/// </summary>
		void OnDrawMiniCanvas ()
		{
			if (!canDrawMiniNode)
				return;

			GUILayout.BeginArea (new Rect (miniCanvasPosition, miniCanvasSize));

			GUI.backgroundColor = new Color (1, 1, 1, 0.2f);

			GUI.Box (new Rect (new Vector2 (0, 0), miniCanvasSize), "");

			GUI.backgroundColor = Color.red;
			for (int i = 0; i < datas.GetSelectedNode ().Count; i++) {
				if(datas.GetSelectedNode()[i].type == NodeType.enNode)
					GUI.Box (new Rect (PositionToMiniCanvas (datas.GetSelectedNode () [i].middlePos), miniNodeSize), "");
			}

			GUI.backgroundColor = Color.blue;
			for (int i = 0; i < datas.GetUnselectNode ().Count; i++) {
				if(datas.GetUnselectNode()[i].type == NodeType.enNode)
					GUI.Box (new Rect (PositionToMiniCanvas (datas.GetUnselectNode () [i].middlePos), miniNodeSize), "");
			}


			GUI.backgroundColor = Color.white;

			GUI.Box (miniSeer, "", NodeStyles.ControlHighlight);

			GUILayout.EndArea ();
		}


		Vector2 PositionToMiniCanvas (Vector2 _pos)
		{
			_pos.x = _pos.x * miniCanvasScaleRate.x;
			_pos.y = _pos.y * miniCanvasScaleRate.y;
			return _pos;
		}

		void OnDrawMiddlePanelEnd ()
		{
			GUILayout.EndArea ();
		}

		void OnDrawRightPanel ()
		{
			if (!_drawLeftPanel)
				return;
			
			GUILayout.BeginArea (new Rect (_rightxPosition, EditorStyles.toolbar.fixedHeight, _rightPanelWidth, Screen.height - EditorStyles.toolbar.fixedHeight));

			DrawRightPanel ();
			GUILayout.EndArea ();
		}



		#endregion

		#region 鼠标与节点事件

		/// <summary>
		/// 画节点
		/// </summary>
		void OnDrawNode ()
		{
			for (int i = 0; i < datas.GetUnselectNode ().Count; i++) {
				if(datas.GetUnselectNode()[i].type == NodeType.enNode)
					OnDoNode (datas.GetUnselectNode () [i], false);
			}
			for (int i = 0; i < datas.GetSelectedNode ().Count; i++) {
				if(datas.GetSelectedNode()[i].type == NodeType.enNode)
					OnDoNode (datas.GetSelectedNode () [i], true);
			}

			OnSelectNode ();
			OnDragNode ();
			MakeTransition ();
		}

		/// <summary>
		/// 绘制各个节点
		/// </summary>
		/// <param name="_node">Node.</param>
		/// <param name="_selected">If set to <c>true</c> selected.</param>
		void OnDoNode (zBaseNode _node, bool _selected)
		{
			var _nodeStyle = NodeStyles.GetNodeStyle (_selected ? 1 : 2, _selected, false);
			_node.Draw (_canvasOffset, _nodeStyle);
		}


		bool selectionDown;
		/// <summary>
		/// 选择节点
		/// </summary>
		void OnSelectNode ()
		{
			int controlID = GUIUtility.GetControlID (FocusType.Passive);

			if (!scaledCanvasSize.Contains (_currentEvent.mousePosition))
				return;

			switch (_currentEvent.rawType) {
			case EventType.MouseDown:
				selectionDown = true;
				GUIUtility.hotControl = controlID;

				cachedMouseStart = _currentEvent.mousePosition;

				zBaseNode tMouseOverNode = MouseOverNode ();
				if (datas.IsPressCtrl && tMouseOverNode != null) { // 按下Ctrl键
					bool _addNode = false;

					if (datas.selectedNodeType == NodeType.enNull)
						_addNode = true;
					if ((datas.selectedNodeType == NodeType.enNode || datas.selectedNodeType == NodeType.enNodeGroup) && tMouseOverNode.type == NodeType.enNode)
						_addNode = true;
					if (tMouseOverNode.type == NodeType.enNodeTransition && datas.selectedNodeType == NodeType.enNodeTransition)
						_addNode = true;

					if (_addNode) {
						datas.AddSelectedNode (tMouseOverNode);
						datas.selectedNodeType = tMouseOverNode.type;
					}

				} else {
					if (tMouseOverNode == null) {
						datas.ClearSelectedNode ();
						datas.selectionMode = SelectionMode.enNotHasNode;
						datas.selectedNodeType = NodeType.enNull;
					} else {
						datas.selectedNodeType = tMouseOverNode.type;
						if (_currentEvent.button == 0) {
							if (datas.selectionMode == SelectionMode.enNotHasNode || datas.selectionMode == SelectionMode.enMouseDownSelectNode) {
								datas.ClearSelectedNode ();
								datas.selectionMode = SelectionMode.enMouseDownSelectNode;
								datas.AddSelectedNode (tMouseOverNode);
							} else if (datas.selectionMode == SelectionMode.enHasNode && !tMouseOverNode.IsSelected) {
								datas.ClearSelectedNode ();
								datas.selectionMode = SelectionMode.enMouseDownSelectNode;
								datas.AddSelectedNode (tMouseOverNode);
							}
						} else if (_currentEvent.button == 1) {
							if (datas.selectionMode == SelectionMode.enNotHasNode) {
								datas.ClearSelectedNode ();
								datas.selectionMode = SelectionMode.enMouseDownSelectNode;
								datas.AddSelectedNode (tMouseOverNode);
							}
						}
					}

				}

				break;

			case EventType.MouseUp:
				selectionDown = false;
				if (datas.selectionMode != SelectionMode.enMakeTransition) {
					GUIUtility.hotControl = 0;

					if (datas.selectedNodeType != NodeType.enNodeTransition) {
						if (datas.GetSelectedNode ().Count > 1) {
							datas.selectionMode = SelectionMode.enHasNode;
							datas.selectedNodeType = NodeType.enNodeGroup;
						} else if (datas.GetSelectedNode ().Count == 1) {
							datas.selectionMode = SelectionMode.enMouseDownSelectNode;
							datas.selectedNodeType = NodeType.enNode;
						} else {
							datas.selectionMode = SelectionMode.enNotHasNode;
							datas.selectedNodeType = NodeType.enNull;
						}
					}

				}

				break;

			case EventType.MouseDrag:
				if (controlID == GUIUtility.hotControl && datas.selectionMode == SelectionMode.enNotHasNode) {
					datas.ClearSelectedNode ();
					var tnodes = SelectNodesInRect (FromToRect (cachedMouseStart, _currentEvent.mousePosition), datas.GetNode ());
					for (int i = 0; i < tnodes.Count; i++) {
						datas.AddSelectedNode (tnodes [i]);
					}
				}

				break;

			case EventType.Repaint:
				if (selectionDown && datas.selectionMode == SelectionMode.enNotHasNode && _currentEvent.button == 0) {
					NodeStyles.selectionRect.Draw (FromToRect (cachedMouseStart, _currentEvent.mousePosition), false, false, false, false);
				}
				break;

			}
		}

		/// <summary>
		/// 拖拽节点
		/// </summary>
		void OnDragNode ()
		{
			int controlID = GUIUtility.GetControlID (FocusType.Passive);
			switch (_currentEvent.rawType) {
			case EventType.MouseDown:
				break;
			case EventType.MouseUp:
				break;
			case EventType.MouseDrag:
				if ((datas.selectionMode == SelectionMode.enHasNode || datas.selectionMode == SelectionMode.enMouseDownSelectNode)) {
					for (int i = 0; i < datas.GetSelectedNode ().Count; i++) {
						if(datas.GetSelectedNode()[i].type == NodeType.enNode)
							datas.GetSelectedNode () [i].position.position += _currentEvent.delta;
					}
				}
				break;
			}
		}


		/// <summary>
		/// 连线
		/// </summary>
		void MakeTransition ()
		{
			int controlId = GUIUtility.GetControlID (FocusType.Passive);

			switch (_currentEvent.rawType) {
			case EventType.MouseDown:
				break;
			case EventType.MouseUp:
				if (datas.selectionMode == SelectionMode.enMakeTransition) {
					GUIUtility.hotControl = 0;
					if (MouseOverNode () != null) {
						for (int i = 0; i < datas.GetSelectedNode ().Count; i++) {
							var tTransition = (datas.GetSelectedNode () [i] as zNode).AddTransition (MouseOverNode ());
							if (tTransition != null)
								datas.AddNode (tTransition); // 把线也添加到所有节点中
						}

						datas.AddSelectedNode (MouseOverNode ());
						datas.selectionMode = SelectionMode.enHasNode;
					}
				}

				break;

			}
		}

		/// <summary>
		/// 画连线
		/// </summary>
		void DrawMakeTransition ()
		{
			if (datas.selectionMode == SelectionMode.enMakeTransition) {
				for (int i = 0; i < datas.GetSelectedNode ().Count; i++) {
					Vector2 _startPos = new Vector2 (datas.GetSelectedNode () [i].middlePos.x + _canvasOffset.x, datas.GetSelectedNode () [i].middlePos.y + _canvasOffset.y);
					Vector2 _endPos = new Vector2 (_currentEvent.mousePosition.x, _currentEvent.mousePosition.y);
					zNodeTransitionLine.DrawLine (_startPos, _endPos, NodeStyles.transitionLineOff);
				}
			}
		}

		/// <summary>
		/// 绘制各个节点间的连线
		/// </summary>
		void DrawNodeTransition ()
		{

			for (int i = 0; i < datas.GetNode ().Count; i++) {
				var temp = datas.GetNode () [i];
				if (temp.type == NodeType.enNode) {
					var tNode = (temp as zNode);
					for (int j = 0; j < tNode.GetTransition ().Count; j++) {
						var tTransition = tNode.GetTransition () [j];
						GUIStyle _style = tTransition.IsSelected ? NodeStyles.transitionLineOn : NodeStyles.transitionLineOff;
						tTransition.Draw (_canvasOffset, _style);
					}
				}
			}
		}

		#endregion

		#region 画格子


		void DrawGrid ()
		{
			GL.PushMatrix ();
			GL.Begin (GL.LINES);
			this.DrawGridLines (scaledCanvasSize, GridMinorSize, _canvasOffset, NodeStyles.gridMinorColor);
			this.DrawGridLines (scaledCanvasSize, GridMajorSize, _canvasOffset, NodeStyles.gridMajorColor);
			GL.End ();
			GL.PopMatrix ();

			if (drawCalback != null)
				drawCalback ();
		}

		void DrawGridLines (Rect rect, float gridSize, Vector2 _offset, Color gridColor)
		{
			_offset *= _scale;
			GL.Color (gridColor);
			for (float i = rect.x + (_offset.x < 0f ? gridSize : 0f) + _offset.x % gridSize; i < rect.x + rect.width; i = i + gridSize) {
				DrawLine (new Vector2 (i, rect.y), new Vector2 (i, rect.y + rect.height));
			}
			for (float j = rect.y + (_offset.y < 0f ? gridSize : 0f) + _offset.y % gridSize; j < rect.y + rect.height; j = j + gridSize) {
				DrawLine (new Vector2 (rect.x, j), new Vector2 (rect.x + rect.width, j));
			}
		}

		void DrawLine (Vector2 p1, Vector2 p2)
		{
			GL.Vertex (p1);
			GL.Vertex (p2);
		}

		#endregion

		#region 重载

		/// <summary>
		/// 拓展
		/// </summary>
		protected virtual void OtherGUI ()
		{
		}

		/// <summary>
		/// 绘制右边菜单栏
		/// </summary>
		protected virtual void DrawRightPanel ()
		{
			GUILayout.Label ("right:" + _canvasOffset);
			GUILayout.Label ("selected node:" + datas.GetSelectedNode ().Count);
			GUILayout.Label ("all node:" + datas.GetNode ().Count);
			GUILayout.Label ("selectType:" + datas.selectedNodeType.ToString ());
			GUILayout.Label ("selectMode: " + datas.selectionMode.ToString ());
			GUILayout.Label ("PressCtrl: " + datas.IsPressCtrl);
			GUILayout.Label ("mousePos : " + cachedMousePosition);
			GUILayout.Label ("miniSize : " + miniCanvasSize);

			bool tNodeSelected = false;
			if (MouseOverNode () != null)
				tNodeSelected = MouseOverNode ().IsSelected;
				
			GUILayout.Label ("MouseOverNode IsSelected: " + tNodeSelected.ToString ());
		
		}



		/// <summary>
		/// 绘制中间菜单栏
		/// </summary>
		protected virtual void DrawMiddlePanel ()
		{

		}

		/// <summary>
		/// 更新画布偏移
		/// </summary>
		protected virtual void UpdateOffset ()
		{

		}

		public virtual void DrawLeftPanel ()
		{
			GUILayout.Label ("LeftPanel ");
		}

		public virtual void DrawMainTool1 ()
		{
			GUILayout.Label ("xx");
		}

		public virtual void DrawMainTool2 ()
		{
			if (_currentGroupPath == "")
				_currentGroupPath = "Base";
			string[] groups = _currentGroupPath.Split ('/');
			string path = "";
			for (int i = 0; i < groups.Length; i++) {
				path += groups [i];
				GUIStyle style = i == 0 ? NodeStyles.breadcrumbLeft : NodeStyles.breadcrumbMiddle;
				GUIContent content = new GUIContent (groups [i]);
				float width = style.CalcSize (content).x;
				width = Mathf.Clamp (width, 80f, width);
				style.normal.textColor = i == groups.Length - 1 ? Color.black : Color.grey;
				if (GUILayout.Button (content, style, GUILayout.Width (width))) {
					SelectGroup (path);
				}
				path += "/";
				style.normal.textColor = Color.white;
			}

			GUILayout.FlexibleSpace ();
		}

		/// <summary>
		/// 双击状态组之后
		/// </summary>
		/// <param name="path">Path.</param>
		protected virtual void SelectGroup (string path)
		{
		}

		/// <summary>
		/// 双击状态组之后
		/// </summary>
		/// <param name="path">Path.</param>
		//		protected virtual void SelectGroup (Node nodegroup)
		//		{
		//
		//		}

		/// <summary>
		/// 返回状态组
		/// </summary>
		/// <param name="path">Path.</param>
		//		protected virtual void SelectGroupUp (Node nodegroup)
		//		{
		////			SelectGroup (nodegroup.groupPath);
		//		}

		#endregion


		#region 所有右键上下文

		protected virtual void ContextMenu_Canvas (ref GenericMenu menu)
		{
			menu.AddItem (new GUIContent ("创建节点"), false, delegate {
				zNode _node = new zNode (cachedMousePosition - _canvasOffset);
				datas.AddNode (_node);
			});
		}

		protected virtual void ContextMenu_Node (ref GenericMenu menu)
		{
			menu.AddItem (new GUIContent ("连线"), false, delegate {
				datas.selectionMode = SelectionMode.enMakeTransition;
			});
		}

		protected virtual void ContextMenu_NodeGroud (ref GenericMenu menu)
		{
			menu.AddItem (new GUIContent ("点击节点组"), false, delegate {
				Debug.Log ("点击节点组");
			});
		}

		protected virtual void ContextMenu_Transition (ref GenericMenu menu)
		{
			menu.AddItem (new GUIContent ("点击线"), false, delegate {
				Debug.Log ("点击线");
			});
		}

		#endregion

		#region 常用方法

		/// <summary>
		/// 返回鼠标对应的节点
		/// </summary>
		/// <returns>The over node.</returns>
		protected zBaseNode MouseOverNode ()
		{
			for (int i = 0; i < datas.GetNode ().Count; i++) {
				if (datas.GetNode () [i].Contains (_currentEvent.mousePosition, _canvasOffset))
					return datas.GetNode () [i];
			}
//
//			for(int i = 0;i < datas.GetNode().Count;i++){
//				var tNode = (datas.GetNode () [i] as zNode);
//				for(int j = 0;j < tNode.GetTransition().Count;j++){
//					var tTransition = tNode.GetTransition () [j];
//					if (tTransition.Contains (_currentEvent.mousePosition, _canvasOffset))
//						return tTransition;
//				}
//			}

			return null;
		}

		/// <summary>
		/// 两点组成的矩形
		/// </summary>
		/// <returns>The to rect.</returns>
		/// <param name="start">Start.</param>
		/// <param name="end">End.</param>
		protected Rect FromToRect (Vector2 start, Vector2 end)
		{
			Rect rect = new Rect (start.x, start.y, end.x - start.x, end.y - start.y);
			if (rect.width < 0f) {
				rect.x = rect.x + rect.width;
				rect.width = -rect.width;
			}
			if (rect.height < 0f) {
				rect.y = rect.y + rect.height;
				rect.height = -rect.height;
			}
			return rect;
		}

		List<zBaseNode> tSelectNodesInRect = new List<zBaseNode> ();

		/// <summary>
		/// 返回被矩形包围的节点
		/// </summary>
		/// <param name="r">The red component.</param>
		List<zBaseNode> SelectNodesInRect (Rect r, List<zBaseNode> _nodes)
		{
			tSelectNodesInRect.Clear ();
			bool hasNode = false;
			r.position -= _canvasOffset;
			for (int i = 0; i < _nodes.Count; i++) {
				
				if (_nodes[i].IntersectRect(r, _canvasOffset)) {
					tSelectNodesInRect.Add (_nodes [i]);
					if (_nodes [i].type == NodeType.enNode)
						hasNode = true;
				}

			}

			if (hasNode) {
				for(int i = 0;i < tSelectNodesInRect.Count;i++){
					if (tSelectNodesInRect [i].type != NodeType.enNode) {
						tSelectNodesInRect.RemoveAt (i);
						i--;
					}
				}
			}

			return tSelectNodesInRect;
		}


		#endregion

		#region Other

		bool needClose = false;

		void OnLostFocus ()
		{
			needClose = true;
		}

		void Awake ()
		{
			canDraw = true;
			datas = CanvasEditorData.Instance;
			drawCalback += DrawMakeTransition;
			drawCalback += DrawNodeTransition;
		}


		void Update ()
		{
			if (_currentEvent == null)
				return;
			datas.IsPressCtrl = _currentEvent.control;

			if (_currentEvent.isKey) {
				switch (_currentEvent.keyCode) {
				case KeyCode.A:
					for (int i = 0; i < datas.GetSelectedNode ().Count; i++) {
						datas.RemoveNode (datas.GetSelectedNode () [i]);
						i--;
					}

					break;
				case KeyCode.Escape:
					Debug.Log ("Editor  Escape");
					break;
				}
			}	

//			if (needClose)
//				Close ();
		}

		#endregion

	}
}