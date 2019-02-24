using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace zEditorWindow
{
	/// <summary>
	/// 鼠标选中节点的类型 
	/// </summary>
	public enum SelectionMode
	{
		/// <summary>
		/// 已有选中物体
		/// </summary>
		enHasNode,
		/// <summary>
		/// 未选中物体
		/// </summary>
		enNotHasNode,
		/// <summary>
		/// 鼠标按下时选中物体
		/// </summary>
		enMouseDownSelectNode,
		/// <summary>
		/// 连线状态
		/// </summary>
		enMakeTransition,
	}

	public enum SelectType{
		Node,
		NodeGroud,
		Transition,
		None
	}


	public class CanvasBaseData<T> : CanvasScriptable<T> where T : ScriptableObject
	{
		[SerializeField]
		List<zBaseNode> allNode = new List<zBaseNode> ();
		[SerializeField]
		List<zBaseNode> selectedNode = new List<zBaseNode> ();
		[SerializeField]
		List<zBaseNode> unselectNode = new List<zBaseNode> ();

		[HideInInspector]
		public SelectionMode selectionMode;
		[HideInInspector]
		public NodeType selectedNodeType = NodeType.enNull;
		[HideInInspector]
		public bool IsPressCtrl;

		#region 选中节点

		/// <summary>
		/// 清空选中节点
		/// </summary>
		public void ClearSelectedNode ()
		{
			for (int i = 0; i < selectedNode.Count; i++) {
				if (!unselectNode.Contains (selectedNode [i])) {
					unselectNode.Add (selectedNode [i]);
					selectedNode [i].IsSelected = false;
				}
			}


			selectedNode.Clear ();
		}

		/// <summary>
		/// 添加选中的节点 
		/// </summary>
		/// <param name="_node">Node.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void AddSelectedNode<T> (T _node) where T : zBaseNode
		{
			if (selectedNode.Contains (_node))
				return;

			selectedNode.Add (_node);
			_node.IsSelected = true;

			if (unselectNode.Contains (_node))
				unselectNode.Remove (_node);

		}

		public void AddSelectedNode (zBaseNode _node)
		{
			AddSelectedNode<zBaseNode> (_node);
		}

		/// <summary>
		/// 移除选中的节点
		/// </summary>
		/// <param name="_node">Node.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void RemoveSelectedNode<T> (T _node) where T : zBaseNode
		{
			if (selectedNode.Contains (_node)) {
				selectedNode.Remove (_node);
				_node.IsSelected = false;
			}
			

			if (!unselectNode.Contains (_node))
				unselectNode.Add (_node);

		}

		/// <summary>
		/// 是否包含
		/// </summary>
		/// <returns><c>true</c>, if selected node was containsed, <c>false</c> otherwise.</returns>
		/// <param name="_node">Node.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public bool ContainsSelectedNode<T> (T _node) where T : zBaseNode
		{
			return selectedNode.Contains (_node);
		}

		public List<zBaseNode> GetSelectedNode ()
		{
			return selectedNode;
		}

		#endregion


		#region 所有节点

		/// <summary>
		/// 清空所有节点
		/// </summary>
		public void ClearNode ()
		{
			allNode.Clear ();
		}

		/// <summary>
		/// 添加节点 
		/// </summary>
		/// <param name="_node">Node.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void AddNode<T> (T _node) where T : zBaseNode
		{
			if (allNode.Contains (_node))
				return;

			allNode.Add (_node);
			unselectNode.Add (_node);
		}

		public void AddNode (zBaseNode _node)
		{
			AddNode<zBaseNode> (_node);
		}

		/// <summary>
		/// 移除选中节点 
		/// </summary>
		/// <param name="_node">Node.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void RemoveNode<T> (T _node) where T: zBaseNode
		{
			if (_node == null)
				return;

			_node.Despown ();
			if (allNode.Contains (_node))
				allNode.Remove (_node);
			if (selectedNode.Contains (_node))
				selectedNode.Remove (_node);
			if (unselectNode.Contains (_node))
				unselectNode.Remove (_node);
		}

		public List<zBaseNode> GetNode ()
		{
			return allNode;
		}

		#endregion


		#region 未选中节点

		/// <summary>
		/// 获取未选中节点
		/// </summary>
		/// <returns>The unselect node.</returns>
		public List<zBaseNode> GetUnselectNode ()
		{
			return unselectNode;
		}

		#endregion

		public void UpdateData(float dt){
			for(int i = 0;i < allNode.Count;i++){
				if (!allNode [i].IsUse) {
					RemoveNode (allNode[i]);
					i--;
				}
			}

		}

	}






}
