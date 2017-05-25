//========================================================================
// Copyright(C): EZFun
// Created by : dhf at 2017/4/20 14:26:11
// Function : 
//========================================================================

using UnityEngine;
using System;

public class UNDebug
{
    public static void Log(string str)
    {
        if(!UNConstants.SHOW_LOG)
        {
            return;
        }
        Debug.Log(str);
    }

    public static void LogWarning(string str)
    {
        if(!UNConstants.SHOW_LOG)
        {
            return;
        }
        Debug.LogWarning(str);
    }

    public static void LogError(string str)
    {
        if(!UNConstants.SHOW_LOG)
        {
            return;
        }
        Debug.LogError(str);
    }
}