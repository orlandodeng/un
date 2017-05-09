//========================================================================
// Copyright(C): EZFun
// Created by : dhf at 2017/5/9 16:54:58
// Function : 行为类
//========================================================================

using UnityEngine;
using System;

// 行为状态
public enum UNBehaviorState
{
    None,
    Wait,
    Execute,
    Finish
}
public class UNBehavior:UNBaseBehavior
{
    public Action m_cb = null;
    public UNBehaviorType m_type;
    public UNBehaviorState m_state;
}