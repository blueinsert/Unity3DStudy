using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace zEditorWindow
{

	public enum NodeType
	{
		/// <summary>
		/// 节点
		/// </summary>
		enNode,
		/// <summary>
		/// 节点组
		/// </summary>
		enNodeGroup,
		/// <summary>
		/// 连线
		/// </summary>
		enNodeTransition,
		/// <summary>
		/// 空
		/// </summary>
		enNull
	}



	[System.Serializable]
	public class zNode : zBaseNode
	{
		public override NodeType type {
			get {
				return NodeType.enNode;
			}
		}

		[SerializeField]
		List<zBaseNode> transition = new List<zBaseNode> ();
		// To Other Node

		[SerializeField]
		List<zBaseNode> otherTransition = new List<zBaseNode>();

		public zNode ()
		{
		}

		public zNode (string _name) : base ()
		{
			name = _name;
		}

		public zNode (Vector2 _pos) : base()
		{
			position.position = new Vector2 (_pos.x - position.size.x / 2, _pos.y - position.size.y / 2);
		}

		public zNode (string _name, Vector2 _pos) :base()
		{
			name = _name;
			position.position = _pos;
		}

	
		#region 连线
		public zBaseNode AddTransition (zBaseNode _toNode)
		{
			if (otherTransition.Contains (_toNode) || _toNode == this)
				return null;

			otherTransition.Add (_toNode);
			var _transition = new zNodeTransitionLine (this, _toNode);
			transition.Add (_transition);
			return _transition;
		}

		public void RemoveTransition(zBaseNode _toNode){
			if (!otherTransition.Contains (_toNode) || _toNode == this)
				return;

			otherTransition.Remove (_toNode);
			for(int i = 0;i < transition.Count;i++){
				if ((transition [i] as zNodeTransitionLine).GetStartNode () == this) {
					transition.RemoveAt (i);
					break;
				}
			}
		}

		public List<zBaseNode> GetTransition(){
			return transition;
		}
		#endregion

		#region Override

		public override void Draw (Vector2 _canvasOffset, GUIStyle _style)
		{
			base.Draw (_canvasOffset, _style);

			GUI.Box (new Rect(position.position + _canvasOffset, position.size), name + "\n" + IsSelected.ToString () + " : " + position, _style);
		}

		public override bool Contains (Vector2 _mousePos, Vector2 _canvasOffset)
		{
			return position.Contains (_mousePos - _canvasOffset);
		}

		public override bool IntersectRect (Rect _r, Vector2 _canvasOffset)
		{
			return zTool.IsRectIntersect (_r, position);
		}

		public override void Despown ()
		{
			base.Despown ();

			for(int i = 0;i < transition.Count;i++){
				transition [i].Despown ();
			}
		}

		#endregion

	}

	#region 节点基类

	[System.Serializable]
	public class zBaseNode{
		[SerializeField]
		public string name = "zBaseNode";
		[SerializeField]
		public int hash;
		/// <summary>
		/// 是否被选中
		/// </summary>
		public bool IsSelected;
		public bool IsUse;

		[SerializeField]
		public Rect position = new Rect (0, 0, 150, 30);

		public Vector2 middlePos {
			get { 
				return new Vector2 (position.x + position.width / 2, position.y + position.height / 2);
			}
		}

		public virtual NodeType type{
			get{ 
				return NodeType.enNull;
			}
		}

		public zBaseNode(){
			IsUse = true;
		}

		public virtual void Despown(){
			IsUse = false;
		}

		public virtual void Draw(Vector2 _canvasOffset, GUIStyle _style = null){

		}

		public virtual bool Contains(Vector2 _mousePos, Vector2 _canvasOffset){
			return false;
		}

		public virtual bool IntersectRect(Rect _r, Vector2 _canvasOffset){
			return false;
		}
	}

	#endregion
}

