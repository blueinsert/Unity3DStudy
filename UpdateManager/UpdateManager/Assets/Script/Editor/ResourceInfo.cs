using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BundleData
{
    public string bundleName;
    public int version;
    public string hashCode;
    public long size;//byte
    public List<string> allAssets = new List<string>();
    
}

public class ResourceInfo : ScriptableObject
{
    public List<BundleData> resources = new List<BundleData>();
}
