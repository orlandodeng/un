//========================================================================
// Copyright(C): EZFun
// Created by : dhf at 2017/5/9 16:54:58
// Function : 行为类
//========================================================================

using UnityEngine;
using System;

// 行为状态
public enum UNBehaviorStateType
{
    None,
    Wait,
    Execute,
    Finish
}
public class UNBehavior:UNBaseBehavior
{
    private UNBehaviorType m_type;
    private int m_priority;
    private UNBehaviorStateType m_curState;
    private Action<UNBehaviorStateType, UNBehaviorStateType> m_cbBeforeChange = null;
    private Action<UNBehaviorStateType, UNBehaviorStateType> m_cbAfterChange = null;
    private UNDictionary<UNBehaviorStateType, Action> m_stateCbs = new UNDictionary<UNBehaviorStateType, Action>();

    public UNBehavior(UNBehaviorType bType, 
        int priority = 0,
        UNBehaviorStateType bState = UNBehaviorStateType.Wait,
        Action<UNBehaviorStateType, UNBehaviorStateType> actionBeforeChange = null,
        Action<UNBehaviorStateType, UNBehaviorStateType> actionAfterChange = null)
    {
        ResetToBehavior(bType, priority, bState, actionBeforeChange, actionAfterChange);
    }

    public void ResetToBehavior(UNBehaviorType bType, 
        int priority = 0,
        UNBehaviorStateType bState = UNBehaviorStateType.Wait,
        Action<UNBehaviorStateType, UNBehaviorStateType> actionBeforeChange = null,
        Action<UNBehaviorStateType, UNBehaviorStateType> actionAfterChange = null)
    {
        m_type = bType;
        m_priority = priority;
        m_curState = bState;
        m_cbBeforeChange = actionBeforeChange;
        m_cbAfterChange = actionAfterChange;

        Init();
    }

    public override void Init()
    {
        base.Init();

        InitStateCbs();
    }

    private void InitStateCbs()
    {
        m_stateCbs.Add(UNBehaviorStateType.Wait, Wait);
        m_stateCbs.Add(UNBehaviorStateType.Execute, Execute);
        m_stateCbs.Add(UNBehaviorStateType.Finish, Finish);
    }

    public virtual void ChangeToState(UNBehaviorStateType state)
    {
        var oldState = m_curState;
        if (m_cbBeforeChange != null)
        {
            m_cbBeforeChange(oldState, m_curState);
        }
        m_stateCbs[state]();
        if (m_cbAfterChange != null)
        {
            m_cbAfterChange(oldState, m_curState);
        }
    }

    public UNBehaviorStateType GetCurState()
    {
        return m_curState;
    }

    protected void Wait()
    {
        m_curState = UNBehaviorStateType.Wait;
    }

    protected void Execute()
    {
        m_curState = UNBehaviorStateType.Execute;
    }

    protected void Finish()
    {
        m_curState = UNBehaviorStateType.Finish;
    }
}