 using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class ThreeAAPlaneCuttingController : MonoBehaviour
{
    public GameObject planeYZ;
    public GameObject planeXZ;
    public GameObject planeXY;

    public GameObject planeYZInternal;
    public GameObject planeXZInternal;
    public GameObject planeXYInternal;

    public List<GameObject> m_rendersRoot = null;

    public float m_rangeXMin = 0;
    public float m_rangeXMax = 0;
    public float m_rangeYMin = 0;
    public float m_rangeYMax = 0;
    public float m_rangeZMin = 0;
    public float m_rangeZMax = 0;

    public List<Renderer> rends;
    // Use this for initialization
    public bool m_isEnabled;

    //private List<MaterialPropertyBlock> m_propBlocks = null;
    private Dictionary<Renderer, MaterialPropertyBlock> m_cacheBlocks = new Dictionary<Renderer, MaterialPropertyBlock>();

    private void OnEnable()
    {
        Refresh();
    }

    private void Refresh()
    {
        rends = new List<Renderer>();
        if (m_rendersRoot != null && m_rendersRoot.Count != 0)
        {
            foreach (var root in m_rendersRoot)
            {
                var rs = root.GetComponentsInChildren<Renderer>();
                rends.AddRange(rs);
            }
        }
        m_cacheBlocks.Clear();
    }

    public void RefreshFromExternal()
    {
        Refresh();
    }

    private MaterialPropertyBlock GetCacheBlock(Renderer renderer)
    {
        if (m_cacheBlocks.ContainsKey(renderer))
        {
            return m_cacheBlocks[renderer];
        }
        var block = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(block);
        m_cacheBlocks.Add(renderer, block);
        return block;
    }

    void Start()
    {

        Refresh();

        UpdateShaderProperties();
    }

    public void OnDestroy()
    {
        if (this.rends != null)
        {
            this.rends.Clear();
            this.rends = null;
        }
        if (this.m_cacheBlocks != null)
        {
            this.m_cacheBlocks.Clear();
            this.m_cacheBlocks = null;
        }
    }

    public void SetPlaneVisible(bool isVisible)
    {
        planeYZ.GetComponent<Renderer>().enabled = isVisible;
        planeXY.GetComponent<Renderer>().enabled = isVisible;
        planeXZ.GetComponent<Renderer>().enabled = isVisible;
    }

    public void SetClipPlaneYZPosition(float factor)
    {
        var x = m_rangeXMin + (m_rangeXMax - m_rangeXMin) * factor;
        this.planeYZ.transform.localPosition = new Vector3(x, 0, 0);
    }

    public void SetClipPlaneXZPosition(float factor)
    {
        var y = m_rangeYMin + (m_rangeYMax - m_rangeYMin) * factor;
        this.planeXZ.transform.localPosition = new Vector3(0, y, 0);
    }

    public void SetClipPlaneXYPosition(float factor)
    {
        var z = m_rangeZMin + (m_rangeZMax - m_rangeZMin) * factor;
        this.planeXY.transform.localPosition = new Vector3(0, 0, z);
    }


    void Update()
    {
        UpdateShaderProperties();
    }

    private void UpdateShaderProperties()
    {
        var positionYZ = planeYZ.transform.position;
        var positionXZ = planeXZ.transform.position;
        var positionXY = planeXY.transform.position;
        for(int i = 0; i < this.rends.Count; i++)
        {
            var rend = this.rends[i];
            var block = GetCacheBlock(rend);

            block.SetVector("_Plane1Position", positionYZ);
            block.SetVector("_Plane2Position", positionXZ);
            block.SetVector("_Plane3Position", positionXY);
            block.SetInt("_EnableBSP", m_isEnabled ? 1 : 0);

            rend.SetPropertyBlock(block);
        } 
    }
}
