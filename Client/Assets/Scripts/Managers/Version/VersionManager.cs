//========================================================================
// Copyright(C): UN
// Created by : dhf at 2017/6/1 17:40:09
// Function : 版本管理
//========================================================================

using System;

public class VersionManager:UNManager
{
    static public VersionManager Instance;

    public override void Init()
    {
        base.Init();
        Instance = this;
    }

    public int GetPublicVersion(VersionType version)
    {
        return 0;
    }

    public int GetLocalVersion(VersionType version)
    {
        return 0;
    }
}