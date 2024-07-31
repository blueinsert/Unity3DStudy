using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace bluebean
{

    [CreateAssetMenu(fileName = "distance field", menuName = "Distance Field", order = 181)]
	[ExecuteInEditMode]	
	public class DistanceField : ScriptableObject
	{
		[SerializeProperty("InputMesh")]
		[SerializeField] private Mesh m_input = null;

		[HideInInspector][SerializeField] private float m_minNodeSize = 0;
		[HideInInspector][SerializeField] private Bounds m_bounds = new Bounds(); 
		[HideInInspector] public List<DFNode> m_nodes;		/**< list of distance field nodes*/

		[Range(0.0000001f,0.1f)]
		public float m_maxError = 0.01f;

		[Range(1, 15)]
		public int m_maxDepth = 5;

		public bool Initialized{
			get{return m_nodes != null;}
		}

		public Bounds FieldBounds {
			get{return m_bounds;}
		}

		public float EffectiveSampleSize {
			get{return m_minNodeSize;}
		}

		public Mesh InputMesh{
			set{
				if (value != m_input){
					Reset();
					m_input = value;
				}
			}
			get{return m_input;}
		}

		public void Reset(){
			m_nodes = null;
			if (m_input != null)
				m_bounds = m_input.bounds;
		}

		public IEnumerator Generate(){

			Reset();

			if (m_input == null)
				yield break;

            int[] tris = m_input.triangles;
            Vector3[] verts = m_input.vertices;

            m_nodes = new List<DFNode>();
            var buildingCoroutine = ASDF.Build(m_maxError, m_maxDepth, verts, tris, m_nodes);

            while (buildingCoroutine.MoveNext())
                yield return new CoroutineJob.ProgressInfo("Processed nodes: " + m_nodes.Count, 1);

            // calculate min node size;
            m_minNodeSize = float.PositiveInfinity;
            for (int j = 0; j < m_nodes.Count; ++j)
                m_minNodeSize = Mathf.Min(m_minNodeSize, m_nodes[j].center[3] * 2);

            // get bounds:
            float max = Mathf.Max(m_bounds.size[0], Mathf.Max(m_bounds.size[1], m_bounds.size[2])) + 0.2f;
            m_bounds.size = new Vector3(max, max, max);

        }

		/**
		 * Return a volume texture containing a representation of this distance field.
		 */
		public Texture3D GetVolumeTexture(int size){

			if (!Initialized)
				return null;
	
			// upper bound of the distance from any point inside the bounds to the surface.
			float maxDist = Mathf.Max(m_bounds.size.x,m_bounds.size.y,m_bounds.size.z);				
	
			float spacingX = m_bounds.size.x / (float)size;
			float spacingY = m_bounds.size.y / (float)size;
			float spacingZ = m_bounds.size.z / (float)size;
	
			Texture3D tex = new Texture3D (size, size, size, TextureFormat.Alpha8, false);
	
			var cols = new Color[size*size*size];
			int idx = 0;
			Color c = Color.black;
	
			for (int z = 0; z < size; ++z)
			{
				for (int y = 0; y < size; ++y)
				{
					for (int x = 0; x < size; ++x, ++idx)
					{
						Vector3 samplePoint = m_bounds.min + new Vector3(spacingX * x + spacingX*0.5f,
						                                 			   spacingY * y + spacingY*0.5f,
						                                  			   spacingZ * z + spacingZ*0.5f);

                        float distance = ASDF.Sample(m_nodes,samplePoint);
	
						if (distance >= 0)
							c.a = distance.Remap(0,maxDist*0.1f,0.5f,1);
						else 
							c.a = distance.Remap(-maxDist*0.1f,0,0,0.5f);
	
						cols[idx] = c;
					}
				}
			}
			tex.SetPixels (cols);
			tex.Apply ();
			return tex;
	
		}
	}
}

