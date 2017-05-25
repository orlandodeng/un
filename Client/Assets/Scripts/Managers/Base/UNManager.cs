//========================================================================
// Copyright(C): EZFun
// Created by : dhf at 2017/5/9 16:55:38
// Function : 
//========================================================================

using UnityEngine;
using System;

public class UNManager:UNBaseManager
{
    public override void Init()
    {
        base.Init();
        AddEventListeners();
    }

    protected virtual void AddEventListeners()
    {
    }
}