using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathHelper
{
    public static string GetPathRelativeToAsset(string fullPath)
    {
        var res = fullPath.Replace("\\", "/").Replace(Application.dataPath + "/", "");
        return res;
    }

    public static string GetPathRelativeResources(string fullPath)
    {
        var res = fullPath.Replace("\\", "/").Replace(Application.dataPath + "/Resources/", "");
        return res;
    }

    public static string GetAssetBundleName(string fullPath)
    {
        var path = GetPathRelativeResources(fullPath);
        path = path.Replace("/", "_");
        return path;
    }

    public static string RemoveExtend(string path)
    {
        int index = path.LastIndexOf(".");
        if (index != -1)
        {
            path = path.Substring(0, index);
        }
        return path;
    }
}
