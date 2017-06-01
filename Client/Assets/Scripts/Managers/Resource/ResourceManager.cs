//========================================================================
// Copyright(C): EZFun
// Created by : dhf at 2017/5/9 17:37:35
// Function : 资源管理
// 管理Unity对象 
// 从assetsbundle读取 需要版本号
// 从resource读取
//========================================================================

using UnityEngine;
using System;

public class ResourceManager:UNManager
{
    public static ResourceManager Instance;
    private UNDictionary<UNPair<ResourceType, string>, UNList<UNResourceObject>> m_objs = null;

    public override void Init()
    {
        base.Init();
        Instance = this;
    }

    public object Load(ResourceType resType, string resName, ResourceCleanType resCleanType = ResourceCleanType.Count)
    {
        return null;
    }

    private void ReleaseUnusedResources()
    {
    }
}