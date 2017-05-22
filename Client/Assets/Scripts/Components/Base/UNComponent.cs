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
    // 待激活行为
    private UNList<UNBehavior> m_behaviorsWaitForActive = new UNList<UNBehavior>();
    // 当前行为
    private UNBehavior m_curBehavior;
    // 固定ID
    protected int m_ID;

    public virtual void InitWithBehavior(UNBehaviorType bType)
    {
        Init();
        AddWaitBehavior(bType);
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

    protected override void Update()
    {
        base.Update();
        UpdateBehaviors();
    }

    protected void UpdateBehaviors()
    {
        // 当前行为wait 则进入execute
        // 当前行为execute 则loop
        // 当前行为finish 则遍历待激活行为 到行为规则里激活新行为
    }

    public void ActiveBehavior(UNBehaviorType bType)
    {
        if (m_curBehavior == null)
        {
            m_curBehavior = new UNBehavior(bType);
        }
        else
        {
            m_curBehavior.ResetToBehavior(bType);
        }
    }

    public void AddWaitBehavior(UNBehaviorType bType, int priority = 0)
    {
        AddWaitBehavior(new UNBehavior(bType, priority));
    }

    public void AddWaitBehavior(UNBehavior behavior)
    {
        m_behaviorsWaitForActive.Add(behavior);
    }

    public void ReplaceBehavior(UNBehaviorType bType, int priority = 0)
    {
        ClearBehavior();
        AddWaitBehavior(bType, priority);
    }

    public void ClearBehavior()
    {
        m_behaviorsWaitForActive.Clear();
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
            m_behaviorsTransRules.Add(toType, fromList);
        }
        if (fromList != null)
        {
            fromList.Add(fromType);
        }
    }
}