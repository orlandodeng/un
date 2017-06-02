//========================================================================
// Copyright(C): UN
// Created by : dhf at 2017/6/2 14:43:04
// Function : 
//========================================================================

using UnityEngine;
using System;

public class AssetsBundleManager:UNManager
{
    public static AssetsBundleManager Instance;
    public UNList<UNWWW> m_wwws = null;

    public override void Init()
    {
        base.Init();
        Instance = this;
        m_wwws = UNList<UNWWW>.New();

        InitAssetsBundle();
    }

    // 将AB加载到硬盘 将配置文件加载到内存
    public void InitAssetsBundle()
    {
        var path = "";
        var www = WWW.LoadFromCacheOrDownload(path, VersionManager.Instance.GetLocalVersion(VersionType.Resource));
        m_wwws.Add(UNWWW.New(www));
    }

    public object Load(string resName)
    {
        // resName->assetsbundleName->assetsbundle->object
        return null;
    }

    public override void Update()
    {
        base.Update();
        for(int i = m_wwws.Count - 1; i >= 0 ; --i)
        {
            if(m_wwws[i].IsNull())
            {
                m_wwws.RemoveAt(i);
                continue;
            }
            if(m_wwws[i].m_www.isDone)
            {
                if(m_wwws[i].m_cb != null)
                {
                    m_wwws[i].m_cb();
                }
                m_wwws[i].m_www.Dispose();
                m_wwws.RemoveAt(i);
            }
        }
    }
}