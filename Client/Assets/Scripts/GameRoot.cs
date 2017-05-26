//========================================================================
// Copyright(C): UN
// Created by : dhf at 2017/4/19 15:47:00
// Function : 游戏流程管理
//========================================================================

using System;
using System.Reflection;
using UnityEngine;

public class GameRoot : MonoBehaviour
{
    private ObjectManager m_objMgr;
	private UNDictionary<Type, UNManager> m_managers;

	// Use this for initialization
	private void Start ()
	{
		if (!InitEnvironment ()) {
			DestroyEnvironment ();
			return;
		}

        m_managers = new UNDictionary<Type, UNManager>();
        m_objMgr = new ObjectManager();
        
		LoadModules ();

        m_objMgr.AddEventListeners_();
	}

	private bool InitEnvironment ()
	{
		return true;
	}

	private void LoadModules ()
	{
		LoadGameManagers ();
		LoadLogicSystems ();
	}

	private void LoadGameManager<T> ()
        where T:UNManager, new()
	{
		var mgr = new T ();
		m_managers.Add (typeof(T), mgr);
		mgr.Init ();
	}

	private void LoadGameManagers ()
	{
		LoadGameManager<EventManager>();
		LoadGameManager<TableManager>();
	}

	private void LoadLogicSystems ()
	{
	}
	
	// Update is called once per frame
	private void Update ()
	{
	}

	private void Finish ()
	{
		UnloadModules ();
		DestroyEnvironment ();
	}

	private void UnloadGameManager<T> ()
        where T:UNBaseManager
	{
		var mgr = m_managers [typeof(T)];
		if (mgr == null) {
			return;
		}
		mgr.Release ();
		m_managers.Remove (typeof(T));
	}

	private void UnloadGameManagers ()
	{
	}

	private void UnloadLogicSystems ()
	{
	}

	private void UnloadModules ()
	{
		UnloadLogicSystems ();
		UnloadGameManagers ();
	}

	private void DestroyEnvironment ()
	{
	}
}
