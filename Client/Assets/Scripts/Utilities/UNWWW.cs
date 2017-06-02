//========================================================================
// Copyright(C): UN
// Created by : dhf at 2017/6/2 15:14:01
// Function : 
//========================================================================

using UnityEngine;
using System;

public class UNWWW:UNObject
{
    public WWW m_www;
    public Action m_cb;

    public static new UNWWW New()
    {
        return ObjectManager.Instance.CreateObject<UNWWW>();
    }

    public static UNWWW New(WWW w)
    {
        if(w == null)
        {
            return null;
        }
        var obj = ObjectManager.Instance.CreateObject<UNWWW>();
        obj.m_www = w;
        return obj;
    }
}