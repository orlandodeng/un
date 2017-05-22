//========================================================================
// Copyright(C): EZFun
// Created by : dhf at 2017/4/20 20:38:58
// Function : 
//========================================================================

using System;
using un_resource;

public class UNComponent:UNBaseComponent
{
    // 拥有的行为
    private UNList<UNBehavior> m_behaviors = new UNList<UNBehavior>();
    // 行为转换规则 value[i] -> key
    private UNDictionary<UNBehaviorType, UNList<UNBehaviorType>> m_behaviorsTransRules = new UNDictionary<UNBehaviorType, UNList<UNBehaviorType>>();
    // 待激活行为
    private UNList<UNBehavior> m_behaviorsWaitForActive = new UNList<UNBehavior>();
    // 当前行为
    public UNBehaviorType m_curBehavour;
    // 固定ID
    public long m_ID;

    public void InitWithBehavior(UNBehaviorType bType)
    {
        Init();
        m_curBehavour = bType;
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
            UNList<UNBehaviorType> fromList = null;
            var data = tableData.trans[i];
            var fromType = (UNBehaviorType)data.from;
            var toType = (UNBehaviorType)data.to;
            if (m_behaviorsTransRules.ContainsKey(toType))
            {
                if(!m_behaviorsTransRules[toType].Contains(fromType))
                {
                    fromList = m_behaviorsTransRules[toType];
                }
                else
                {
                    continue;
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
}