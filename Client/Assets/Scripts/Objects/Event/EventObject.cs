//========================================================================
// Copyright(C): EZFun
// Created by : dhf at 2017/4/20 20:36:21
// Function : 事件管理对象
//========================================================================

using UnityEngine;
using System;

public delegate void EventCallBack(params object[] pars);
public class EventObject:UNObject
{
    public UNDictionary<EventType, UNList<EventCallBack>> m_listeners;
    public UNDictionary<EventType, UNList<object[]>> m_triggers;

    public static new EventObject New()
    {
        var obj = ObjectManager.Instance.CreateObject<EventObject>();
        obj.Init();
        return obj;
    }

    public override void Init()
    {
        base.Init();

        m_listeners = UNDictionary<EventType, UNList<EventCallBack>>.New();
        m_triggers = UNDictionary<EventType, UNList<object[]>>.New();
    }

    public void AddEventListener(EventType type, EventCallBack cb)
    {
        UNList<EventCallBack> cbs = null;
        if (!m_listeners.TryGetValue(type, out cbs))
        {
            cbs = UNList<EventCallBack>.New();
            m_listeners.Add(type, cbs);
        }
        cbs.Add(cb);
    }

    public void DispatchEvent(EventType type, object[] pars)
    {
        UNList<object[]> objs = null;
        if (!m_triggers.TryGetValue(type, out objs))
        {
            objs = UNList<object[]>.New();
            m_triggers.Add(type, objs);
        }
        objs.Add(pars);
    }

    public void DispatchEventImmediate(EventType type, object[] pars)
    {
        if (!m_listeners.ContainsKey(type))
        {
            return;
        }
        var cbs = m_listeners[type];
        for (int i = 0; i < cbs.Count; ++i)
        {
            try
            {
                cbs[i](pars);
            }
            catch (System.Exception ex)
            {
                UNDebug.LogError(ex.ToString());
            }
        }
    }

    public new void Update()
    {
        for (int i = 0; i < m_triggers.Count; ++i)
        {
            var type = m_triggers.GetKey(i);
            if (!m_listeners.ContainsKey(type))
            {
                continue;
            }
            var pars = m_triggers.GetValue(i);
            for (int k = 0; k < pars.Count; ++k)
            {
                DispatchEventImmediate(type, pars[k]);
            }
        }
        m_triggers.Clear();
    }
}