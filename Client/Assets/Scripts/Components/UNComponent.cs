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
    public UNDictionary<UNBehaviorType, UNBaseBehavior> m_behaviours;
    // 行为转换规则
    public UNDictionary<UNList<UNBehaviorType>, UNBehaviorType> m_behavioursTrans;
}