//========================================================================
// Copyright(C): EZFun
// Created by : dhf at 2017/5/25 16:02:51
// Function : 
//========================================================================

using UnityEngine;
using System;

public class MemoryManager:UNManager
{
    public void AllocMemory()
    {
    }

    public bool CheckEnoughMemory()
    {
        return GetLeftMemory() > 0;
    }

    public long GetLeftMemory()
    {
        return UNConstants.MAX_MEMORY_SIZE - GetUsedMemory();
    }

    public long GetUsedMemory()
    {
        return 0;
    }

    public void MemoryWarning()
    {
        EventManager.Instance.DispatchEventImmediate(EventType.MemoryWarning);
    }

    public void UpdateMemory()
    {
        if (GetLeftMemory() > UNConstants.WARN_MEMORY_SIZE)
        {
            return;
        }
        MemoryWarning();
    }

    public override void Update()
    {
        base.Update();

        UpdateMemory();
    }
}