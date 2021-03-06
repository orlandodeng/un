//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18063
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Text;


[InitializeOnLoad]
public class BuildDll  {
    static string m_dllName = "";

    [MenuItem("EZFun/build/build all &%#d")] // & - alt, %-ctrl, # - shift
    static void BuildAll()
    {
        BuildXgameDll();
        BuildLSharp();
        EnLua();
    }

    [MenuItem("EZFun/build/build xgame dll &%#d")] // & - alt, %-ctrl, # - shift
    static void BuildXgameDll()
    {
        string datapath = Application.dataPath;
        string[] strArray = datapath.Split('/');
        string parentPath = "";
        for (int i = 0; i < strArray.Length - 1; i++)
        {
            parentPath += strArray[i] + "/";
        }
        string folderName = strArray[strArray.Length - 2];
        string projectCofig = parentPath + "Assembly-CSharp.csproj";
//        m_dllName = "xgame" + Version.Instance.GetVersion(VersionType.App) + DateTimeToUnixTimestamp();
        ReplaceCofig(projectCofig);

        string path = parentPath + "/buildDll.bat";
        DelLog();
        System.Diagnostics.Process.Start(path, m_dllName);
    }

    [MenuItem("EZFun/build/build l# &%#d")] // & - alt, %-ctrl, # - shift
    static void BuildLSharp()
    {
        string datapath = Application.dataPath;
        string[] strArray = datapath.Split('/');
        string parentPath = "";
        for (int i = 0; i < strArray.Length - 1; i++)
        {
            parentPath += strArray[i] + "/";
        }
        string folderName = strArray[strArray.Length - 2];
        ReplaceLsharpCofig(parentPath + "LSharpCode/LSharpCode.csproj", folderName);

        string path = parentPath + "/buildL#.bat";
        DelLog();
        System.Diagnostics.Process.Start(path);
    }

    [MenuItem("EZFun/build/Encrypte Lua")]
    static void EnLua()
    {
        string inputPath = Application.dataPath + "/XGame/Script/Lua";
        ForeachFile(inputPath, (FileInfo f) =>
        {
            TableManager.InitCry();
            //var b = EZFunTools.ReadFile(f.FullName);
            //var cb = ResourceSys.cry.Encrypte(b);
            //File.WriteAllBytes(Application.streamingAssetsPath + "/Lua/" + f.Name, cb);
        });
    }

    static void ForeachFile(string path, Action<FileInfo> genFile)
    {
        if (!Directory.Exists(path))
            return;

        foreach (var fileStr in (new DirectoryInfo(path)).GetFiles())
        {
            if (fileStr.Name.EndsWith("meta"))
            {
                continue;
            }
            genFile(fileStr);
        }
        foreach (var dirStr in Directory.GetDirectories(path))
        {
            if (Directory.Exists(dirStr))
            {
                ForeachFile(dirStr, genFile);
            }
        }
    }

    static void ReplaceCofig(string path)
    {
        string text = string.Empty;
        using (StreamReader reader = new StreamReader(path, Encoding.Default))
        {
            text = reader.ReadToEnd();
            reader.Close();
        }
        //EZFunTools.SaveFile(path + "_bak", Encoding.Default.GetBytes(text));
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            ReplaceNode(ref text, "RootNamespace", m_dllName);
            ReplaceNode(ref text, "AssemblyName", m_dllName);

            text = text.Replace("<NoWarn>0169</NoWarn>", "<NoWarn>0169</NoWarn> \r\n <AllowUnsafeBlocks>true</AllowUnsafeBlocks>");
            text = text.Replace("<Compile Include=\"Assets\\XGame\\Public\\GameStartScrpits\\GameStart.cs\" />", "");

            writer.Write(text);
            writer.Close();
        }
    }

    static void ReplaceLsharpCofig(string path, string folderName)
    {
        string text = string.Empty;
        using (StreamReader reader = new StreamReader(path, Encoding.Default))
        {
            text = reader.ReadToEnd();
            reader.Close();
        }

        using (StreamWriter writer = new StreamWriter(path, false))
        {
            text = text.Replace("trunk", folderName);
            writer.Write(text);
            writer.Close();
        }
    }

    static void ReplaceNode(ref string text, string nodeName, string content)
    {
        string leftStr = "<" + nodeName + ">";
        string rightStr = "</" + nodeName + ">";
        int left = text.IndexOf(leftStr) + nodeName.Length + 2;
        int right = text.IndexOf(rightStr);

        string nodeCotent = text.Substring(left, right - left);
        string oldStr = leftStr + nodeCotent + rightStr;
        string newStr = leftStr + content + rightStr;
        text = text.Replace(oldStr, newStr);
    }

    static long DateTimeToUnixTimestamp()
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        return (long)(DateTime.Now - startTime).TotalSeconds;

    }

    static void DelFile(DirectoryInfo direc, string fileName)
    {
        var file = direc.GetFiles(fileName);
        if (file != null)
        {
            for (int i = 0; i < file.Length; i++)
            {
                file[i].Delete();
            }
        }
    }

    static void DelLog()
    {
        string path = Application.dataPath;
        DirectoryInfo direc = new DirectoryInfo(path);
        DirectoryInfo parDirec = direc.Parent;
        DelFile(parDirec, "buildtemp.log");
        DelFile(parDirec, "build.log");
        EditorApplication.update += Update;
    }

    static void Update()
    {
        string path = Application.dataPath;
        DirectoryInfo direc = new DirectoryInfo(path);
        DirectoryInfo parDirec = direc.Parent;
        var file = parDirec.GetFiles("build.log");
        if (file != null && file.Length > 0)
        {
            PrintLog(file[0].FullName);
        }
    }

    static void PrintLog(string fileName)
    {
        EditorApplication.update -= Update;
        StreamReader s = File.OpenText(fileName);
        string line = s.ReadLine();
        while (line != null)
        {
            if (line.Contains("warning CS"))
            {
                //				UnityEngine.Debug.LogWarning(line);
            }
            else if (line.Contains("error CS"))
            {
                UnityEngine.Debug.LogError(line);
            }
            line = s.ReadLine();
        }
        s.Close();
        File.Delete(fileName);
    }
}
