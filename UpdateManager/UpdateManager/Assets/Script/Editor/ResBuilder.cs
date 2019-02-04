using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;

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

    [MenuItem("Build/Step/1.SetAssetBundleName")]
    public static void SetAssetBundleName()
    {
        string resourcePath = Application.dataPath + "/" + "Resources";
        DirectoryInfo dirInfo = new DirectoryInfo(resourcePath);
        //清理所有打包标签
        SetAssetBundleName(dirInfo, string.Empty);
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
        Debug.Log("1.SetAssetBundleName done");
   }

    [MenuItem("Build/Step/2.GenerateResourceInfo")]
    public static void GenerateResourceInfo()
    {
        //旧的ResourceInfo文件
        var resourceInfoPath = PathHelper.GetPathRelativeToProject(Application.dataPath + "/Resources/ResourcesInfo.asset");
        var resourceInfos = AssetDatabase.LoadAssetAtPath<ResourceInfo>(resourceInfoPath);
        if(resourceInfos == null)
        {
            resourceInfos = ScriptableObject.CreateInstance<ResourceInfo>();
            AssetDatabase.CreateAsset(resourceInfos, resourceInfoPath);
        }
        //中间数据
        var bundleDataDic = new Dictionary<string, BundleData>();
        foreach(var bundleData in resourceInfos.resources)
        {
            bundleDataDic[bundleData.bundleName] = bundleData;
        }

        var assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
        //是否有assetBundle被删除
        List<BundleData> removedBundleData = new List<BundleData>();
        foreach(var bundleData in bundleDataDic.Values)
        {
            if (!assetBundleNames.Contains(bundleData.bundleName)){
                removedBundleData.Add(bundleData);
            }
        }
        foreach(var removeItem in removedBundleData)
        {
            bundleDataDic.Remove(removeItem.bundleName);
        }
        foreach (var assetBundleName in assetBundleNames)
        {
            BundleData bundleData;
            //新增的assetBundle
            if(!bundleDataDic.TryGetValue(assetBundleName, out bundleData))
            {
                bundleData = new BundleData();
                bundleData.bundleName = assetBundleName;
                bundleDataDic.Add(assetBundleName, bundleData);
            }
            var assets = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
            bundleData.allAssets = new List<string>(assets);
        }
        resourceInfos.resources = new List<BundleData>(bundleDataDic.Values);
        EditorUtility.SetDirty(resourceInfos);
        AssetDatabase.SaveAssets();
        Debug.Log("2.GenerateResourceInfo done");
    }

    [MenuItem("Build/Step/3.GenerateAssetBundles")]
    public static void GenerateAssetBundles()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath);
        if (!dir.Exists)
        {
            dir.Create();
        }
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        Debug.Log("3.GenerateAssetBundles done");
    }

    private static string GetFileHash(string path)
    {
        var hash = SHA1.Create();
        var stream = new FileStream(path, FileMode.Open);
        byte[] hashBytes = hash.ComputeHash(stream);
        stream.Close();
        return BitConverter.ToString(hashBytes).Replace("-", "");
    }

    public static long GetFileSize(string path)
    {
        FileInfo fileInfo = new FileInfo(path);
        return fileInfo.Length;
    }

    [MenuItem("Build/Step/4.CalcAssetBundlesInfo")]
    public static void CalcAssetBundlesInfo()
    {
        var resourceInfoPath = PathHelper.GetPathRelativeToProject(Application.dataPath + "/Resources/ResourcesInfo.asset");
        var resourceInfos = AssetDatabase.LoadAssetAtPath<ResourceInfo>(resourceInfoPath);
        foreach(var bundleData in resourceInfos.resources)
        {
            var oldHash = bundleData.hashCode;
            var fileHash = GetFileHash(Application.streamingAssetsPath + "/" + bundleData.bundleName);
            if (fileHash != oldHash)
            {
                bundleData.version++;
            }
            bundleData.hashCode = fileHash;
            bundleData.size = GetFileSize(Application.streamingAssetsPath + "/" + bundleData.bundleName);
        }
        EditorUtility.SetDirty(resourceInfos);
        AssetDatabase.SaveAssets();
        //复制一份ResourceInfo到StreamAsset文件夹
        File.Copy(Application.dataPath + "/Resources/ResourcesInfo.asset",  Application.streamingAssetsPath + "/ResourcesInfo.asset", true);
        Debug.Log("4.CalcAssetBundlesInfo done");
    }

    [MenuItem("Build/BuildAllStep")]
    public static void BuildAllStep()
    {
        SetAssetBundleName();
        GenerateResourceInfo();
        GenerateAssetBundles();
        CalcAssetBundlesInfo();
    }
}
