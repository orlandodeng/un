//========================================================================
// Copyright(C): UN
// Created by : dhf at 2017/6/1 18:25:25
// Function : 
//========================================================================

using System;

public class UNPair<T1, T2>:UNObject
{
    private T1 m_first;
    public T1 First
    {
        get
        {
            return m_first;
        }
        set
        {
            m_first = value;
        }
    }
    private T2 m_second;
    public T2 Second
    {
        get
        {
            return m_second;
        }
        set
        {
            m_second = value;
        }
    }

    public UNPair<T1, T2> New(T1 t1, T2 t2)
    {
        return ObjectManager.Instance.CreateObject<UNPair<T1, T2>>();
    }

    public override void Release()
    {
        base.Release();
        m_first = default(T1);
        m_second = default(T2);
    }
}