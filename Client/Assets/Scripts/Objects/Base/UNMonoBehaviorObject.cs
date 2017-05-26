//========================================================================
// Copyright(C): EZFun
// Created by : dhf at 2017/5/9 16:58:16
// Function : 
//========================================================================

using UnityEngine;
using System;

public class UNMonoBehaviorObject:UNBaseMonoBehaviorObject
{
    protected EventObject m_event;
    private UNObjectStateType _m_state;
    public UNObjectStateType m_state
    {
        get
        {
            return _m_state;
        }
        set
        {
            _m_state = value;
        }
    }

    public virtual void Init()
    {
        m_state = UNObjectStateType.Alive;
    }

    public virtual void Update()
    { }

    public virtual void Release()
    {
        // 将不用的对象置NULL
        m_state = UNObjectStateType.Dead;
    }

    public virtual void Reset()
    {
    }
}