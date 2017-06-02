//========================================================================
// Copyright(C): UN
// Created by : dhf at 2017/6/1 17:41:06
// Function : 版本数据
//========================================================================

using System;

// APP内部版本类型
public enum VersionType
{
    Package,                // 包体版本
    Code,                   // 代码版本
    Resource,               // 资源版本
}

// APP外部版本类型
public enum APPVersionType
{
    Public,                 // 外网版本
    Local,                  // 本地版本
}