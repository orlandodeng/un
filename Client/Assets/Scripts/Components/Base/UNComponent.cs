//========================================================================
// Copyright(C): EZFun
// Created by : dhf at 2017/4/20 20:38:58
// Function : 
//========================================================================

using System;
using un_resource;

public class UNComponent:UNBaseComponent
{
    // 行为转换规则 value[i] -> key
    private UNDictionary<UNBehaviorType, UNList<UNBehaviorType>> m_behaviorsTransRules = new UNDictionary<UNBehaviorType, UNList<UNBehaviorType>>();
    // 待执行行为
    private UNList<UNBehavior> m_behaviorsWaitForExecute = new UNList<UNBehavior>();
    // 当前行为
    private UNBehavior m_curBehavior;
    // 固定ID
    protected int m_ID;
    // 行为 切换 回调
    private UNList<UNBehaviorChangeStateCallBack> m_cbsBeforeBehaviorChange = new UNList<UNBehaviorChangeStateCallBack>();
    private UNList<UNBehaviorChangeStateCallBack> m_cbsAfterBehaviorChange = new UNList<UNBehaviorChangeStateCallBack>();

    public virtual void InitWithBehavior(UNBehaviorType bType,
        int priority = 0,
        UNBehaviorStateType bState = UNBehaviorStateType.Wait,
        UNBehaviorChangeStateCallBack cbBeforeBehaviorChange = null,
        UNBehaviorChangeStateCallBack cbAfterBehaviorChange = null)
    {
        Init();
        AddWaitBehavior(bType, priority, bState, cbBeforeBehaviorChange, cbAfterBehaviorChange);
    }

    public override void Init()
    {
        base.Init();
        InitBehaviorsTrans();
    }

    // 初始化行为转换规则
    public void InitBehaviorsTrans()
    {
        var tableData = TableManager.Instance.GetEntry<ResBehaviorList>(m_ID) as ResBehavior;
        if (tableData == null)
        {
            UNDebug.LogError("no this behavior " + m_ID);
            return;
        }
        for (int i = 0; i < tableData.trans.Count; ++i)
        {
            var data = tableData.trans[i];
            var fromType = (UNBehaviorType)data.from;
            var toType = (UNBehaviorType)data.to;
            AddBehaviorsTransRule(fromType, toType);
        }
    }

    public override void Update()
    {
        base.Update();
        UpdateBehaviors();
    }

    protected void UpdateBehaviors()
    {
        if (m_curBehavior == null)
        {
            UpdateWaitBehavior();
        }
        else
        {
            if (m_curBehavior.m_curState == UNBehaviorStateType.Finish)
            {
                UpdateNewBehavior();
            }
            else
            {
                UpdateCurBehavior();
            }
        }
    }

    // update 等待 行为
    private void UpdateWaitBehavior()
    {
        var priority = -1;
        UNBehavior behavior = null;
        for (int i = 0; i < m_behaviorsWaitForExecute.Count; ++i)
        {
            if (!m_behaviorsWaitForExecute[i].CanChangeToState(UNBehaviorStateType.Execute))
            {
                continue;
            }
            if (priority < m_behaviorsWaitForExecute[i].m_priority)
            {
                behavior = m_behaviorsWaitForExecute[i];
                priority = m_behaviorsWaitForExecute[i].m_priority;
            }
        }
        m_behaviorsWaitForExecute.Clear();
        if (behavior == null)
        {
            return;
        }
        m_curBehavior = behavior;
        m_curBehavior.ChangeToState(UNBehaviorStateType.Execute);
    }

    // update 当前 行为
    private void UpdateCurBehavior()
    {
        if (m_curBehavior == null)
        {
            return;
        }
        if (m_curBehavior.CanChangeToState(UNBehaviorStateType.Finish))
        {
            m_curBehavior.ChangeToState(UNBehaviorStateType.Finish);
            return;
        }
        m_curBehavior.Update();
    }

    // update 新 行为
    private void UpdateNewBehavior()
    {
        if (m_curBehavior == null)
        {
            return;
        }
        for (int i = 0; i < m_behaviorsTransRules.Count; ++i)
        {
            var k = m_behaviorsTransRules.GetKey(i);
            var v = m_behaviorsTransRules.GetValue(i);
            for (int j = 0; j < v.Count; ++j)
            {
                if (m_curBehavior.m_type != v[j])
                {
                    continue;
                }
                AddWaitBehavior(k);
                return;
            }
        }
    }

    public void AddBehaviorImmediate(UNBehaviorType bType,
        int priority = 0,
        UNBehaviorStateType bState = UNBehaviorStateType.Wait,
        UNBehaviorChangeStateCallBack cbBeforeChange = null,
        UNBehaviorChangeStateCallBack cbAfterChange = null)
    {
        m_cbsBeforeBehaviorChange.Add(cbBeforeChange);
        m_cbsAfterBehaviorChange.Add(cbAfterChange);
        if (m_curBehavior == null)
        {
            m_curBehavior = new UNBehavior(bType, priority, bState, m_cbsBeforeBehaviorChange, m_cbsAfterBehaviorChange);
        }
        else
        {
            m_curBehavior.ChangeToState(UNBehaviorStateType.Finish);
            m_curBehavior.ResetToBehavior(bType, priority, bState, m_cbsBeforeBehaviorChange, m_cbsAfterBehaviorChange);
        }
        m_curBehavior.ChangeToState(UNBehaviorStateType.Execute);
    }

    public void AddWaitBehavior(UNBehaviorType bType,
        int priority = 0,
        UNBehaviorStateType bState = UNBehaviorStateType.Wait,
        UNBehaviorChangeStateCallBack cbBeforeChange = null,
        UNBehaviorChangeStateCallBack cbAfterChange = null)
    {
        m_cbsBeforeBehaviorChange.Add(cbBeforeChange);
        m_cbsAfterBehaviorChange.Add(cbAfterChange);
        AddWaitBehavior(new UNBehavior(bType, priority, bState, m_cbsBeforeBehaviorChange, m_cbsAfterBehaviorChange));
    }

    public void AddWaitBehavior(UNBehavior behavior)
    {
        m_behaviorsWaitForExecute.Add(behavior);
    }

    public void ReplaceBehavior(UNBehaviorType bType, int priority = 0)
    {
        ClearBehavior();
        AddWaitBehavior(bType, priority);
    }

    public void ClearBehavior()
    {
        m_behaviorsWaitForExecute.Clear();
    }

    public void AddBehaviorsTransRule(UNBehaviorType fromType, UNBehaviorType toType)
    {
        UNList<UNBehaviorType> fromList = null;
        if (m_behaviorsTransRules.ContainsKey(toType))
        {
            if (!m_behaviorsTransRules[toType].Contains(fromType))
            {
                fromList = m_behaviorsTransRules[toType];
            }
        }
        else
        {
            fromList = new UNList<UNBehaviorType>();
        }
        if (fromList != null)
        {
            fromList.Add(fromType);
        }
        AddBehaviorsTransRule(fromList, toType);
    }

    public void AddBehaviorsTransRule(UNList<UNBehaviorType> fromTypes, UNBehaviorType toType)
    {
        m_behaviorsTransRules.Add(toType, fromTypes);
    }
}