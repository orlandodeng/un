//========================================================================
// Copyright(C): UN
// Created by : dhf at 2017/6/1 17:51:21
// Function : 资源数据
//========================================================================

using System;

public enum ResourceType
{
    None,
    Window,             // 界面
    Effect,             // 特效
    Font,               // 字体
    Sound,              // 音频
    Animation,          // 动画
    Model,              // 模型
    Texture,            // 纹理
}

public enum ResourceCleanType
{
    None,
    Immediate,      // 任一实例用完后
    Count,          // 引用计数清零后
    ChangeScene,    // 切换场景后
}