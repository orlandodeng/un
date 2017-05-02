//========================================================================
// Copyright(C): EZFun
// Created by : dhf at 2017/4/20 14:26:11
// Function : 
//========================================================================

using UnityEngine;
using System;

public class UNDebug
{
    public static readonly bool SHOWLOG = true;
    public static void Log(string str)
    {
        if(!SHOWLOG)
        {
            return;
        }
        Debug.Log(str);
    }

    public static void LogWarning(string str)
    {
        if(!SHOWLOG)
        {
            return;
        }
        Debug.LogWarning(str);
    }

    public static void LogError(string str)
    {
        if(!SHOWLOG)
        {
            return;
        }
        Debug.LogError(str);
    }
}