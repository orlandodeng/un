//========================================================================
// Copyright(C): UN
// Created by : dhf at 2017/6/1 20:47:15
// Function : 
//========================================================================

using UnityEngine;
using System.IO;

public static class UNUtilities
{
    // 只读路径
    public static string StreamingAssetsPath
    {
        get
        {
            return Application.streamingAssetsPath;
        }
        private set { }
    }
    // 读写路径
    public static string PersistentDataPath
    {
        get
        {
            return Application.persistentDataPath;
        }
        private set { }
    }
    // 只读路径 这个属性返回的是程序的数据文件所在文件夹的路径，例如在Editor中就是项目的Assets文件夹的路径，通过这个路径可以访问项目中任何文件夹中的资源，但是在移动端它是完全没用。
    public static string DataPath
    {
        get
        {
            return Application.dataPath;
        }
        private set { }
    }
    // 读取streamassets目录中的文件
    public static byte[] ReadFile(string path)
    {
        byte[] b = null;
        if (Application.platform == RuntimePlatform.Android && path.Contains(StreamingAssetsPath))
        {
            WWW www = new WWW(path);
            while (!www.isDone) ;
            if (string.IsNullOrEmpty(www.error))
            {
                b = www.bytes;
            }
            www.Dispose();
        }
        else
        {
            b = ReadFileStream(path);
        }

        return b;
    }

    public static byte[] ReadFileStream(string path)
    {
        byte[] b = null;
        using (Stream file = File.OpenRead(path))
        {
            b = new byte[(int)file.Length];
            file.Read(b, 0, b.Length);
            file.Close();
            file.Dispose();
        }
        return b;
    }
}
public static class NullUtil
{
    public static bool IsNull(this System.Object o)
    {
        return o == null;
    }
    public static bool IsNotNull(this System.Object o)
    {
        return o != null;
    }
    public static bool IsNull(this UnityEngine.Object o)
    {
        return o == null;
    }
    public static bool IsNotNull(this UnityEngine.Object o)
    {
        return o != null;
    }
}