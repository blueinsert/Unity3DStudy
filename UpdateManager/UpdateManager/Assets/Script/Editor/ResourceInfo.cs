using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BundleData
{
    public string resourceSrcPath;
    public string bundleName;
    public int version;
}

public class ResourceInfo : ScriptableObject
{
    public List<BundleData> resources = new List<BundleData>();
}
