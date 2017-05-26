//========================================================================
// Copyright(C): EZFun
// Created by : dhf at 2017/4/20 20:49:05
// Function : 
//========================================================================

using UnityEngine;
using System;

public enum UNObjectStateType
{
    None,
    Alive,
    Dead,
}

public class UNObject:UNBaseObject
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

    public UNObject()
    {
    }

    public static UNObject New()
    {
        var obj = ObjectManager.Instance.CreateObject<UNObject>();
        obj.Init();
        return obj;
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