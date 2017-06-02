//========================================================================
// Copyright(C): UN
// Created by : dhf at 2017/6/1 18:18:52
// Function : 取代UnityEngine.GameObject
//========================================================================

using UnityEngine;

public class UNResourceObjectKey:UNObject
{
    public string m_name = null;
    public ResourceType m_type = ResourceType.None;

    public static new UNResourceObjectKey New()
    {
        return ObjectManager.Instance.CreateObject<UNResourceObjectKey>();
    }

    public static UNResourceObjectKey New(string name, ResourceType type)
    {
        var obj = ObjectManager.Instance.CreateObject<UNResourceObjectKey>();
        obj.m_name = name;
        obj.m_type = type;
        return obj;
    }
}

public class UNResourceObject : UNObject
{
    public Object m_obj = null;
    public UNResourceObjectKey m_key = null;
    public ResourceCleanType m_cleanType = ResourceCleanType.None;
    public UNList<UNMonoBehaviorObject> m_scripts = null;

    public static UNResourceObject New(Object go, string name, ResourceType type = ResourceType.None, ResourceCleanType cType = ResourceCleanType.Immediate)
    {
        var obj = ObjectManager.Instance.CreateObject<UNResourceObject>();
        obj.m_obj = go;
        obj.m_key = UNResourceObjectKey.New(name, type);
        obj.m_cleanType = cType;
        return obj;
    }

    public override void Init()
    {
        base.Init();
        if (m_scripts.IsNull())
        {
            m_scripts = UNList<UNMonoBehaviorObject>.New();
        }
    }

    public override void Release()
    {
        base.Release();
        m_key.Release();
        m_scripts.Release();
        m_cleanType = ResourceCleanType.None;
    }

    public override void Reset()
    {
        base.Reset();
        m_scripts.Reset();
    }
}