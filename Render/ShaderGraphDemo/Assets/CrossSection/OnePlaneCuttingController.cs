using UnityEngine;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class OnePlaneCuttingController : MonoBehaviour
{

    public GameObject plane;
    Material mat;
    public Vector3 normal;
    public Vector3 position;
    public List<Renderer> rends = new List<Renderer>();

    // Use this for initialization
    void Start()
    {
        rends.AddRange(GetComponentsInChildren<Renderer>());
        normal = plane.transform.TransformVector(new Vector3(0, 0, -1));
        position = plane.transform.position;
        UpdateShaderProperties();
    }
    void Update()
    {
        UpdateShaderProperties();
    }

    private void UpdateShaderProperties()
    {

        normal = plane.transform.TransformVector(new Vector3(0, 0, -1));
        position = plane.transform.position;
        foreach (var rend in rends)
        {
            for (int i = 0; i < rend.materials.Length; i++)
            {
                if (rend.materials[i].shader.name.Contains("OnePlaneBsp"))
                {
                    rend.materials[i].SetVector("_PlaneNormal", normal);
                    rend.materials[i].SetVector("_PlanePosition", position);
                }
            }
        }
    }
}
