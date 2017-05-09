//========================================================================
// Copyright(C): EZFun
// Created by : dhf at 2017/4/20 20:36:21
// Function : 事件管理对象
//========================================================================

using UnityEngine;
using System;

public class UNEventObjecct:UNBaseObject
{
    public UNDictionary<UNEventType, UNList<UNEventCallBack>> m_listeners = new UNDictionary<UNEventType, UNList<UNEventCallBack>>();
    public UNDictionary<UNEventType, UNList<object[]>> m_triggers = new UNDictionary<UNEventType, UNList<object[]>>();

    public void AddEventListener(UNEventType type, UNEventCallBack cb)
    {
        UNList<UNEventCallBack> cbs = null;
        if (!m_listeners.TryGetValue(type, out cbs))
        {
            cbs = new UNList<UNEventCallBack>();
            m_listeners.Add(type, cbs);
        }
        cbs.Add(cb);
    }

    public void DispatchEvent(UNEventType type, object[] pars)
    {
        UNList<object[]> objs = null;
        if (!m_triggers.TryGetValue(type, out objs))
        {
            objs = new UNList<object[]>();
            m_triggers.Add(type, objs);
        }
        objs.Add(pars);
    }

    public void DispatchEventImmediate(UNEventType type, object[] pars)
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

    protected override void Update()
    {
        for (int i = 0; i < m_triggers.Count; ++i)
        {
            var type = m_triggers.GetKey(i);
            if (type == null)
            {
                m_triggers.Remove(type);
                continue;
            }
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