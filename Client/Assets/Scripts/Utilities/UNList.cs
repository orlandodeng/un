//========================================================================
// Copyright(C): UN
// Created by : dhf at 2017/4/19 17:46:59
// Function : 
//========================================================================

using System;
using System.Collections.Generic;

public class UNList<T>:UNObject
{
    private Action<T> m_addCB;
    private Action<T> m_removeCB;

    private List<T> _m_list = new List<T>();
    private List<T> m_list
    {
        get
        {
            NullCheck();
            return _m_list;
        }
        set
        {
            _m_list = value;
        }
    }

    private void NullCheck()
    {
        if(_m_list.IsNull())
        {
            _m_list = new List<T>();
        }
    }

    public int Count
    {
        get
        {
            return m_list.Count;
        }
    }

    public UNList()
    {
    }

    public static new UNList<T> New()
    {
        return ObjectManager.Instance.CreateObject<UNList<T>>();
    }

    public static UNList<T> New(Action<T> addCB = null, Action<T> removeCB = null)
    {
        var obj = New();
        obj.Init(addCB, removeCB);
        return obj;
    }

    public void Init(Action<T> addCB = null, Action<T> removeCB = null)
    {
        m_addCB = addCB;
        m_removeCB = removeCB;
    }

    public static explicit operator UNList<T>(List<T> list)
    {
        if (list.IsNull())
        {
            return null;
        }
        var el = new UNList<T>();
        for (int i = 0; i < list.Count; ++i)
        {
            el.Add(list[i]);
        }
        return el;
    }

    public void Add(T value)
    {
        m_list.Add(value);
        if(m_addCB != null)
        {
            m_addCB(value);
        }
    }

    public void Insert(int index, T value)
    {
        m_list.Insert(index, value);
        if (m_addCB != null)
        {
            m_addCB(value);
        }
    }

    public void Push(T value)
    {
        Add(value);
    }

    public void SetCallBack(Action<T> addCB, Action<T> removeCB)
    {
        m_addCB = addCB;
        m_removeCB = removeCB;
    }

    public bool Remove(T value)
    {
        if(m_removeCB != null)
        {
            m_removeCB(value);
            return m_list.Remove(value);
        }
        else
        {
            return m_list.Remove(value);
        }
    }

    public void RemoveAt(int index)
    {
        if (m_removeCB != null)
        {
            var v = m_list[index];
            m_list.RemoveAt(index);
            m_removeCB(v);
        }
        else
        {
            m_list.RemoveAt(index);
        }
    }

    public T Pop()
    {
        var value = m_list[0];
        RemoveAt(0);
        return value;
    }

    //public int RemoveAll(Predicate<T> match)
    //{
    //    return m_list.RemoveAll(match);
    //}

    public void Clear()
    {
        if (m_removeCB != null)
        {
            for (int i = 0; i < m_list.Count; ++i)
            {
                m_removeCB(m_list[i]);
            }
        }
        m_list.Clear();
    }

    public T this[int index]
    {
        get
        {
            return m_list[index];
        }
        set
        {
            m_list[index] = value;
        }
    }

    public bool Contains(T value)
    {
        return m_list.Contains(value);
    }

    public void Sort()
    {
        m_list.Sort();
    }

    public void Sort(IComparer<T> comparer)
    {
        m_list.Sort(comparer);
    }

    public void Sort(Comparison<T> comparison)
    {
        m_list.Sort(comparison);
    }

    public void Sort(int index, int count, IComparer<T> comparer)
    {
        m_list.Sort(index, count, comparer);
    }

    public T[] ToArray()
    {
        return m_list.ToArray();
    }

    public override void Release()
    {
        base.Release();
        Clear();
        m_addCB = null;
        m_removeCB = null;
    }
}
