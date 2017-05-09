//========================================================================
// Copyright(C): EZFun
// Created by : dhf at 2017/4/20 20:38:58
// Function : 
//========================================================================

using UnityEngine;
using System;

public class UNComponent:UNBaseComponent
{
    // 拥有的行为
    private UNList<UNBehavior> m_behaviors = new UNList<UNBehavior>();
    // 行为转换规则
    private UNDictionary<UNList<UNBehaviorType>, UNBehaviorType> m_behaviorsTrans = new UNDictionary<UNList<UNBehaviorType>, UNBehaviorType>();
    // 待激活行为
    private UNList<UNBehavior> m_behaviorsWaitForActive = new UNList<UNBehavior>();
    // 当前行为
    public UNBehaviorType m_curBehavour;

    public override void Init()
    {
        base.Init();
    }
}