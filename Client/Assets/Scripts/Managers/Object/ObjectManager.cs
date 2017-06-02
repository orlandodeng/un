//========================================================================
// Copyright(C): EZFun
// Created by : dhf at 2017/5/25 16:48:22
// Function : 管理自定义C#类型，控制mono堆的增长
//========================================================================

using System;

public class ObjectManager:UNManager
{
    public static ObjectManager Instance;
    private UNDictionary<Type, UNList<UNObject>> m_objs;
    private UNList<UNObject> m_objGetList;

    public ObjectManager()
    {
        Init();
    }

    public override void Init()
    {
        Instance = this;
        m_objs = new UNDictionary<Type, UNList<UNObject>>();
        base.Init();
    }

    protected override void AddEventListeners()
    {
    }

    public void AddEventListeners_()
    {
        base.AddEventListeners();
        EventManager.Instance.AddEventListener(EventType.MemoryWarning, OnMemoryWarning);
    }

    private void OnMemoryWarning(object[] pars)
    {
        ReleaseUnusedObjects();
    }

    public T CreateObject<T>()
        where T:UNObject,new()
    {
        if(m_objs.TryGetValue(typeof(T), out m_objGetList))
        {
            for (int i = 0; i < m_objGetList.Count; ++i)
            {
                if (m_objGetList[i].m_state == UNObjectStateType.Alive)
                {
                    continue;
                }
                m_objGetList[i].m_state = UNObjectStateType.Alive;
                return m_objGetList[i] as T;
            }
            T obj = new T();
            obj.m_state = UNObjectStateType.Alive;
            m_objGetList.Add(obj);
            return obj;
        }
        else
        {
            T obj = new T();
            obj.m_state = UNObjectStateType.Alive;
            var objList = new UNList<UNObject>();
            objList.Add(obj);
            m_objs.Add(typeof(T), objList);
            return obj;
        }
    }

    private void ReleaseUnusedObjects()
    {
        for (int i = 0; i < m_objs.Count; ++i)
        {
            var objs = m_objs.GetValue(i);
            for (int j = objs.Count - 1; j >= 0; --j)
            {
                if (objs[j].m_state == UNObjectStateType.Alive)
                {
                    continue;
                }
                objs[j].Release();
                objs[j] = null;
                objs.RemoveAt(j);
            }
        }
    }
}