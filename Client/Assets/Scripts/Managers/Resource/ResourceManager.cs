//========================================================================
// Copyright(C): EZFun
// Created by : dhf at 2017/5/9 17:37:35
// Function : 资源管理
// 管理Unity对象 
// 从assetsbundle读取 需要版本号
// 从resource读取
//========================================================================

using UnityEngine;
using System.IO;

public class ResourceManager:UNManager
{
    public static ResourceManager Instance;
    private UNDictionary<UNResourceObjectKey, UNList<UNResourceObject>> m_objs = null;

    public override void Init()
    {
        base.Init();
        Instance = this;
        m_objs = UNDictionary<UNResourceObjectKey, UNList<UNResourceObject>>.New();
    }

    public UNResourceObject Load(string resName, ResourceType resType, ResourceCleanType resCleanType = ResourceCleanType.Count)
    {
        var resObj = GetFromObjects(resName, resType, resCleanType);
        if(resObj != null)
        {
            return resObj;
        }
        // 取更新文件 用assetsbundle
        var path = GetPath_Update(resType);
        if(resObj != null)
        {
            return resObj;
        }
        // 取非更新文件 用resource.load
        path = GetPath(resType);
        var obj = Resources.Load(path);
        var ins = Object.Instantiate(obj);
        obj = null;
        resObj = UNResourceObject.New(ins, resName, resType, resCleanType);
        AddToObjects(resObj);
        return resObj;
    }

    private UNResourceObject GetFromObjects(string resName, ResourceType resType, ResourceCleanType resCleanType = ResourceCleanType.Count)
    {
        for(int i = 0; i < m_objs.Count; ++i)
        {
            var key = m_objs.GetKey(i);
            if(key.m_type != resType || key.m_name != resName)
            {
                continue;
            }
            var value = m_objs[key];
            for(int j = 0; j < value.Count; ++i)
            {
                var obj = value[j];
                if(obj.m_state == UNObjectStateType.Alive)
                {
                    continue;
                }
                obj.Init();
                return obj;
            }
        }
        return null;
    }

    private void AddToObjects(Object obj, string resName, ResourceType resType, ResourceCleanType resCleanType = ResourceCleanType.Count)
    {
        var robj = UNResourceObject.New(obj, resName, resType, resCleanType);
        AddToObjects(robj);
    }

    private void AddToObjects(UNResourceObject resObj)
    {
        var list = UNList<UNResourceObject>.New();
        list.Add(resObj);
        m_objs.Add(resObj.m_key, list);
    }

    private void RemoveFromObjects(string resName, ResourceType resType)
    {

    }
    // 取路径 非更新
    private string GetPath(ResourceType type)
    {
        string path = "";
        if(type == ResourceType.None)
        {
            return path;
        }
        path = UNString.LinkString(type.ToString(), Path.DirectorySeparatorChar);
        return path;
    }

    // 取路径 更新 根据版本号
    private string GetPath_Update(ResourceType type)
    {
        return "";
    }

    private void ReleaseUnusedResources()
    {
        for(int i = m_objs.Count - 1; i >= 0 ; --i)
        {
            var key = m_objs.GetKey(i);
            var value = m_objs[key];
            if(value.Count <= 0)
            {
                m_objs.Remove(key);
            }
        }
    }
}