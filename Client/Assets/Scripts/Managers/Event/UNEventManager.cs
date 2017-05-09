//========================================================================
// Copyright(C): EZFun
// Created by : dhf at 2017/4/20 12:17:03
// Function : 事件管理类
//========================================================================


public delegate void UNEventCallBack(params object[] pars);
public class UNEventManager:UNBaseManager
{
    static public UNEventManager Instance;

    public override void Init()
    {
        base.Init();
        Instance = this;
    }

}