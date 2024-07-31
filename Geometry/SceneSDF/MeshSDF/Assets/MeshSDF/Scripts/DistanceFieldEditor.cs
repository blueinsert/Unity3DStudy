using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

namespace bluebean
{
	[CustomEditor(typeof(DistanceField))]
	public class DistanceFieldEditor : Editor
	{

		DistanceField m_distanceField;

		PreviewHelpers m_previewHelper;
		Vector2 m_previewDir;
		Material m_previewMaterial;

		Mesh m_previewMesh;
		Texture3D m_volumeTexture;

		protected IEnumerator m_routine;

		private void UpdatePreview(){

			CleanupPreview();

			if (m_distanceField.InputMesh != null){

				m_previewMesh = CreateMeshForBounds(m_distanceField.FieldBounds);
				m_previewMesh.hideFlags = HideFlags.HideAndDontSave;

				m_volumeTexture = m_distanceField.GetVolumeTexture(64);
				m_volumeTexture.hideFlags = HideFlags.HideAndDontSave;

				m_previewMaterial = Resources.Load<Material>("DistanceFieldPreview");
				m_previewMaterial.SetTexture("_Volume",m_volumeTexture);
				m_previewMaterial.SetVector("_AABBMin",-m_distanceField.FieldBounds.extents);
                m_previewMaterial.SetVector("_AABBMax",m_distanceField.FieldBounds.extents);
			}

		}

		private void CleanupPreview(){
			GameObject.DestroyImmediate(m_previewMesh);
			GameObject.DestroyImmediate(m_volumeTexture);
		}

		public void OnEnable(){
			m_distanceField = (DistanceField) target;
			m_previewHelper = new PreviewHelpers();
			UpdatePreview();
		}

		public void OnDisable(){
			EditorUtility.ClearProgressBar();
			m_previewHelper.Cleanup();
			CleanupPreview();
		}

		public override void OnInspectorGUI() {

			serializedObject.UpdateIfRequiredOrScript();	

			Editor.DrawPropertiesExcluding(serializedObject,"m_Script");

			GUI.enabled = (m_distanceField.InputMesh != null);
			if (GUILayout.Button("Generate")){
				// Start a coroutine job in the editor.
				EditorUtility.SetDirty(target);
				CoroutineJob job = new CoroutineJob();
				m_routine = job.Start( m_distanceField.Generate());
				EditorCoroutine.ShowCoroutineProgressBar("Generating distance field",ref m_routine);
				UpdatePreview();
				EditorGUIUtility.ExitGUI();
			}
			GUI.enabled = true;		

			int nodeCount = (m_distanceField.m_nodes != null ? m_distanceField.m_nodes.Count : 0);
			float resolution = m_distanceField.FieldBounds.size.x / m_distanceField.EffectiveSampleSize;
			EditorGUILayout.HelpBox("Nodes: "+ nodeCount+"\n"+
									"Size in memory: "+ (nodeCount * 0.062f).ToString("0.#") +" kB\n"+
									"Compressed to: " + (nodeCount / Mathf.Pow(resolution,3) * 100).ToString("0.##") + "%",MessageType.Info);

			if (GUI.changed)
				serializedObject.ApplyModifiedProperties();

		}

		public override bool HasPreviewGUI(){
			return true;
		}

		public override void OnInteractivePreviewGUI(Rect region, GUIStyle background)
		{
			m_previewDir = PreviewHelpers.Drag2D(m_previewDir, region);

			if (Event.current.type != EventType.Repaint || m_previewMesh == null)
			{
                return;
            }

			Quaternion quaternion = Quaternion.Euler(this.m_previewDir.y, 0f, 0f) * Quaternion.Euler(0f, this.m_previewDir.x, 0f) * Quaternion.Euler(0, 120, -20f);

			m_previewHelper.BeginPreview(region, background);

			Bounds bounds = m_previewMesh.bounds;
			float magnitude = Mathf.Sqrt(bounds.extents.sqrMagnitude);
			float num = 4f * magnitude;
			m_previewHelper.m_Camera.transform.position = -Vector3.forward * num;
			m_previewHelper.m_Camera.transform.rotation = Quaternion.identity;
			m_previewHelper.m_Camera.nearClipPlane = num - magnitude * 1.1f;
			m_previewHelper.m_Camera.farClipPlane = num + magnitude * 1.1f;

			// Compute matrix to rotate the mesh around the center of its bounds:
			Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero,quaternion,Vector3.one) * Matrix4x4.TRS(-bounds.center,Quaternion.identity,Vector3.one);

			Graphics.DrawMesh(m_previewMesh, matrix, m_previewMaterial,1, m_previewHelper.m_Camera, 0);

			Texture texture = m_previewHelper.EndPreview();
			GUI.DrawTexture(region, texture, ScaleMode.StretchToFill, true);

        }

		/**
		 * Creates a solid mesh from some Bounds. This is used to display the distance field volumetric preview.
		 */
		private Mesh CreateMeshForBounds(Bounds b){
			Mesh m = new Mesh();

			/** Indices of bounds corners:

			  		   Y
			  		   2	   6
			    	   +------+
			 	  3  .'|  7 .'|
				   +---+--+'  |
				   |   |  |   |
				   |   +--+---+   X
				   | .' 0 | .' 4
				   +------+'
				Z 1        5

			*/
			Vector3[] vertices = new Vector3[8]{
				b.center + new Vector3(-b.extents.x,-b.extents.y,-b.extents.z), //0
				b.center + new Vector3(-b.extents.x,-b.extents.y,b.extents.z),  //1
				b.center + new Vector3(-b.extents.x,b.extents.y,-b.extents.z),  //2
				b.center + new Vector3(-b.extents.x,b.extents.y,b.extents.z),   //3
				b.center + new Vector3(b.extents.x,-b.extents.y,-b.extents.z),  //4
				b.center + new Vector3(b.extents.x,-b.extents.y,b.extents.z),   //5
				b.center + new Vector3(b.extents.x,b.extents.y,-b.extents.z),   //6
				b.center + new Vector3(b.extents.x,b.extents.y,b.extents.z)     //7
			};
			int[] triangles = new int[36]{
				2,3,7,
				6,2,7,

				7,5,4,
				6,7,4,

				3,1,5,
				7,3,5,

				2,0,3,
				3,0,1,

				6,4,2,
				2,4,0,

				4,5,0,
				5,1,0
			};

			m.vertices = vertices;
			m.triangles = triangles;
			return m;
		}
	}
}
