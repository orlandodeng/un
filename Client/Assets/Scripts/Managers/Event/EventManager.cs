//========================================================================
// Copyright(C): EZFun
// Created by : dhf at 2017/4/20 12:17:03
// Function : 事件管理
//========================================================================


public class EventManager : UNManager
{
    static public EventManager Instance;

    public override void Init()
    {
        base.Init();
        Instance = this;
    }

    public void AddEventListener(EventType type, EventCallBack cb)
    {
        m_event.AddEventListener(type, cb);
    }

    public void DispatchEvent(EventType type, object[] pars = null)
    {
        m_event.DispatchEvent(type, pars);
    }

    public void DispatchEventImmediate(EventType type, object[] pars = null)
    {
        m_event.DispatchEventImmediate(type, pars);
    }

    public override void Update()
    {
        m_event.Update();
    }
}