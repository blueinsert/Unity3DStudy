using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ResBuilder 
{
    private static void SetAssetBundleName(DirectoryInfo dirInfo, string assetBundleName)
    {
        FileSystemInfo[] files = dirInfo.GetFileSystemInfos();
        foreach(var file in files)
        {
            if(file is DirectoryInfo)
            {
                SetAssetBundleName(file as DirectoryInfo, assetBundleName);
            }else
            {
                if (file.Name.EndsWith(".meta"))
                    continue;
                var importer = AssetImporter.GetAtPath("Assets/" + PathHelper.GetPathRelativeToAsset(file.FullName));
                importer.assetBundleName = assetBundleName;
            }
        }
    }

    private static void CollectAssetBundleFolder(DirectoryInfo curDir, List<DirectoryInfo> dirs)
    {
        if (curDir.Name.EndsWith("_ab"))
        {
            dirs.Add(curDir);
        }
        //遍历所有子目录
        foreach(var dir in curDir.GetDirectories())
        {
            CollectAssetBundleFolder(dir, dirs);
        }
    }

    private static bool CheckAssetBundleFolder(List<DirectoryInfo> dirs)
    {
        foreach(var dir in dirs)
        {
            //目录结构中出现了重复定义的_ab
            if(dir.FullName.IndexOf("_ab") != dir.FullName.Length - 3)
            {
                Debug.LogError("目录结构中出现了重复定义的_ab:" + dir.FullName);
                return false;
            }
        }
        return true;
    }

    [MenuItem("Build/SetAssetBundleName")]
    public static void SetAssetBundleName()
    {
        string resourcePath = Application.dataPath + "/" + "Resources";
        DirectoryInfo dirInfo = new DirectoryInfo(resourcePath);
        //收集打包的文件夹
        List<DirectoryInfo> assetBundleDirs = new List<DirectoryInfo>();
        CollectAssetBundleFolder(dirInfo, assetBundleDirs);
        if (!CheckAssetBundleFolder(assetBundleDirs))
        {
            return;
        }
        //设置打包标签
        foreach(var dir in assetBundleDirs)
        {
            SetAssetBundleName(dir, PathHelper.GetAssetBundleName(dir.FullName));
        }
        AssetDatabase.SaveAssets();
        Debug.Log("SetAssetBundleName done");
   }

    [MenuItem("Build/GenerateResourceInfo")]
    public static void GenerateResourceInfo()
    {
        var assetBundles = AssetDatabase.GetAllAssetBundleNames();
        foreach(var assetBundleName in assetBundles)
        {
            Debug.Log(assetBundleName);
        }
    }
}
