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

public delegate bool UNBehaviorCanChangeToState();
public delegate void UNBehaviorChangeToState();
public delegate void UNBehaviorChangeStateCallBack(UNBehaviorType bType, UNBehaviorStateType fromState, UNBehaviorStateType toState);
public class UNBehavior:UNBaseBehavior
{
    private UNBehaviorType _m_type;
    public UNBehaviorType m_type
    {
        get
        {
            return _m_type;
        }
        set
        {
            _m_type = value;
        }
    }
    private int _m_priority;
    public int m_priority
    {
        get
        {
            return _m_priority;
        }
        set
        {
            _m_priority = value;
        }
    }
    private UNBehaviorStateType _m_curState;
    public UNBehaviorStateType m_curState
    {
        get
        {
            return _m_curState;
        }
        set
        {
            _m_curState = value;
        }
    }
    private UNList<UNBehaviorChangeStateCallBack> m_cbsBeforeChange = null;
    private UNList<UNBehaviorChangeStateCallBack> m_cbsAfterChange = null;
    private UNDictionary<UNBehaviorStateType, UNBehaviorCanChangeToState> m_stateCheckCbs = new UNDictionary<UNBehaviorStateType, UNBehaviorCanChangeToState>();
    private UNDictionary<UNBehaviorStateType, UNBehaviorChangeToState> m_stateCbs = new UNDictionary<UNBehaviorStateType, UNBehaviorChangeToState>();

    public UNBehavior(UNBehaviorType bType, 
        int priority = 0,
        UNBehaviorStateType bState = UNBehaviorStateType.Wait,
        UNList<UNBehaviorChangeStateCallBack> cbsBeforeChange = null,
        UNList<UNBehaviorChangeStateCallBack> cbsAfterChange = null)
    {
        ResetToBehavior(bType, priority, bState, cbsBeforeChange, cbsAfterChange);
    }

    public void ResetToBehavior(UNBehaviorType bType, 
        int priority = 0,
        UNBehaviorStateType bState = UNBehaviorStateType.Wait,
        UNList<UNBehaviorChangeStateCallBack> cbsBeforeChange = null,
        UNList<UNBehaviorChangeStateCallBack> cbsAfterChange = null)
    {
        m_type = bType;
        m_priority = priority;
        m_curState = bState;
        m_cbsBeforeChange = cbsBeforeChange;
        m_cbsAfterChange = cbsAfterChange;

        Init();
    }

    public override void Init()
    {
        base.Init();

        InitStateCheckCbs();
        InitStateCbs();
    }

    private void InitStateCheckCbs()
    {
        m_stateCheckCbs.Add(UNBehaviorStateType.Wait, CanWait);
        m_stateCheckCbs.Add(UNBehaviorStateType.Execute, CanExecute);
        m_stateCheckCbs.Add(UNBehaviorStateType.Finish, CanFinish);
    }

    private void InitStateCbs()
    {
        m_stateCbs.Add(UNBehaviorStateType.Wait, Wait);
        m_stateCbs.Add(UNBehaviorStateType.Execute, Execute);
        m_stateCbs.Add(UNBehaviorStateType.Finish, Finish);
    }

    public bool CanChangeToState(UNBehaviorStateType state)
    {
        return m_stateCheckCbs[state]();
    }

    protected virtual bool CanWait()
    {
        return true;
    }

    protected virtual bool CanExecute()
    {
        return true;
    }

    protected virtual bool CanFinish()
    {
        return true;
    }

    public virtual void ChangeToState(UNBehaviorStateType state)
    {
        var oldState = m_curState;
        if (m_cbsBeforeChange != null)
        {
            for (int i = 0; i < m_cbsBeforeChange.Count; ++i)
            {
                m_cbsBeforeChange[i](m_type, oldState, state);
            }
        }
        m_stateCbs[state]();
        if (m_cbsAfterChange != null)
        {
            for (int i = 0; i < m_cbsAfterChange.Count; ++i)
            {
                m_cbsAfterChange[i](m_type, oldState, state);
            }
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

    public override void Update()
    {
        base.Update();
    }
}