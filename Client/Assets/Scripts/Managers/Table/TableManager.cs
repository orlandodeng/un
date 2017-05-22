//========================================================================
// Copyright(C): EZFun
// Created by : dhf at 2017/5/9 17:39:55
// Function : 表格管理
//========================================================================

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using zlib;
using un_resource;

using TableType = System.Collections.Generic.Dictionary<object, global::ProtoBuf.IExtensible>;

public class TableManager:UNManager
{
	class MapDefine {
		public string[] keys;
	}

	class MapData {
		public Type type;
		public Dictionary<object, object> map = new Dictionary<object, object> ();
	}

	private Dictionary<Type, TableType> m_systemMap = new Dictionary<Type, TableType>();
	private Dictionary<Type, Dictionary<string, MapDefine>> m_mapDefines = new Dictionary<Type, Dictionary<string, MapDefine>>();
	private Dictionary<string, MapData> m_map = new Dictionary<string, MapData>();
	public static NetCrypte cry = new NetCrypte();

    static public string CryIV = "dd7fd4a156d28bade96f816db1d18609";
    static public string CryKey = "dd7fd4a156d28bade96f816db1d18609";

	public static TableManager Instance = null;
	public string m_basicPath = "";
	public string m_updatePath = "";
	static string StreamPath = "";
	private bool m_hasLoad = false;

	//add by janus
	//Map Edit need
	public static void InitCry()
	{
		cry.setIV  (CryIV);
		cry.setKey (CryKey);
		cry.isCrypte = true;
		cry.pkgLen = 4;
	}

    public void ClearMemory()
    {
        m_mapDefines.Clear();
        m_mapDefines.Clear();
    }

    // lua的number是double类型 要转型
    public static object changeKey(object key)
    {
        return key;
//        Type tkey = key.GetType();
//        object nkey = null;
//        if (tkey == typeof(int))
//        {
//            nkey = (double)(int)key;
//        }
//        else
//        {
//            nkey = key;
//        }
//        return nkey;
    }
	
	public override void Init()
	{
		if(!m_hasLoad)
		{
			//CDebug.Log ("resource init");
			InitCry();
			Instance = this;
			StreamPath = Application.streamingAssetsPath;
			m_basicPath = Application.streamingAssetsPath + "/Table/";
			m_updatePath = Application.persistentDataPath + "/Table/";
			m_hasLoad = true;
		}
	}

	public static byte[] Decompress(byte[] input)
	{	
		MemoryStream ms = new MemoryStream (input);
		var zos = new ZInputStream (ms);
		MemoryStream output = new MemoryStream ();
		byte[] temp = new byte[4096];
		int len = 0;
		while((len=zos.read(temp, 0, temp.Length)) > 0) {
			output.Write(temp, 0, len);
		};
		zos.Close ();
	
		return output.ToArray();
	}

    public static string DecompressStr(byte[] input)
    {
        byte[] b = Decompress(input);
        return System.Text.Encoding.UTF8.GetString(b);
    }

    public static string DeCompressByte64(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return "";
        }
        else
        {
            return DecompressStr(Convert.FromBase64String(str));
        }
    }

	public T Load<T>()
		where T : ProtoBuf.IExtensible
	{
		Type t = typeof(T);

		//CDebug.Log ("load table :" + t.Name);
		string plocal = m_basicPath + t.Name + ".bytes";
		string ulocal = m_updatePath + t.Name + ".bytes";
        byte[] b = null;
		if (File.Exists(ulocal))
		{
			b = ReadFileStream(ulocal);
        }
        else
        {
			b = ReadFile(plocal);
        }

        byte[] zdata = cry.Decrypte(b);
        var data = Decompress(zdata);
        MemoryStream ms = new MemoryStream(data);
        T res = ProtoBuf.Serializer.Deserialize<T>(ms);
		//CDebug.Log ("load table1 :" + t.Name);
        return res;
    }

	public byte[] Encrypte(byte[] inByte)
	{
		byte[] outByte = cry.Encrypte(inByte);
		return outByte;
	}

	public static byte[] Decrypte(byte[] inByte)
	{
		byte[] outByte = cry.Decrypte(inByte);
		return outByte;
	}

    public static byte[] DecryDecompress(byte[] b)
    {
        byte[] zdata = cry.Decrypte(b);
        var data = Decompress(zdata);
        return data;
    }

	static public TableType InitTable(ProtoBuf.IExtensible table)
	{
		TableType dictTable = new TableType();
		Type t = table.GetType ();
		System.Reflection.PropertyInfo p = t.GetProperty ("list");
		IList ol = (IList) p.GetValue(table, null);
		foreach(ProtoBuf.IExtensible obj in ol) 
		{
			Type objType = obj.GetType();
			System.Reflection.PropertyInfo objProperty = objType.GetProperty("ID");
            object key = objProperty.GetValue(obj, null);
            object nkey = changeKey(key);

			dictTable[nkey] = obj;
		}
		return dictTable;
	}

	public TableType GetTable<T>()
		where T : ProtoBuf.IExtensible
	{
		if (m_systemMap.ContainsKey(typeof(T)))
		{
			return m_systemMap [typeof(T)];
		}
		T table = Load<T>();
		TableType dictTable = InitTable (table);
		m_systemMap [typeof(T)] = dictTable;
		if (m_mapDefines.ContainsKey (typeof(T))) {
			Dictionary<string, MapDefine> maps = m_mapDefines[typeof(T)];
			foreach(KeyValuePair<string, MapDefine> entry in maps) {
				_GenMap(typeof(T), entry.Key, entry.Value);
			}
		}
		return dictTable;
	}

    public delegate void Foreach_Lua_delegate(object value, object o);
    public void ForeachTable_Lua<T, T1>(object cb)
		where T : ProtoBuf.IExtensible 
    {
        var table = GetTable<T>();
        if(table == null)
        {
            return;
        }

        var wbl = Type.GetType("WindowBaseLua");
        var to = wbl.GetField("m_luaMgr");
        var obje = to.GetValue(null);
        var lsm = Type.GetType("LuaScriptMgr");
        var method = lsm.GetMethod("CallLuaFunction");
        object[] obA = method.Invoke(obje, new object[] {"register_func", new[] { cb } }) as object[];
        int luaIndex = int.Parse(obA[0].ToString());
        var lcb = Type.GetType("CLuaCallBack");
        var luaCallBack = Activator.CreateInstance(lcb, new object[] { luaIndex });
        var method1 = lcb.GetMethod("GetCallBack");
        var t = method1.Invoke(luaCallBack, new object[] { typeof(Foreach_Lua_delegate) }) as Foreach_Lua_delegate;
        foreach (var kv in table)
        {
            var value = (T1)kv.Value;
            t(value, null);
        }
    }

    public void RemoveTable<T>()
    {
        m_systemMap.Remove(typeof(T));
    }

	public ProtoBuf.IExtensible GetEntry<T>(object key)
		where T : ProtoBuf.IExtensible
	{
		TableType table = GetTable<T> ();
        object nkey = changeKey(key);
        
		if(table.ContainsKey(nkey))
		{
			return table [nkey];
		}
		return null;
	}


    void _GenMap(Type type, string mapName, MapDefine mapDefine) {
		TableType table = m_systemMap [type];
		MapData mapData = new MapData ();
		mapData.type = type;
		Dictionary<object, object> mapDic = mapData.map;
		m_map [mapName] = mapData;

		foreach (object key in table.Keys) {
			Dictionary<object, object> dic = mapDic;
			object entry = table[key];
			Type entryType = entry.GetType();
			for (int i=0; i<mapDefine.keys.Length; ++i) {
				string cellName = mapDefine.keys[i];
				System.Reflection.PropertyInfo objProperty = entryType.GetProperty(cellName);
				object cellValue = objProperty.GetValue(entry, null);
				if (i == mapDefine.keys.Length - 1) {
					List<object> list = null;
					if (dic.ContainsKey(cellValue)) {
						list = (List<object>)dic[cellValue];
					} else {
						list = new List<object>();
						dic[cellValue] = list;
					}
					list.Add(key);
				} else {
					if (!dic.ContainsKey(cellValue)) {
						dic[cellValue] = new Dictionary<object, object>();
					}
					dic = (Dictionary<object, object>)dic[cellValue];
				}
			}
		}
	}

	public void GenMap<T>(string mapName, string[] keys) 
		where T : ProtoBuf.IExtensible {
		Type type = typeof (T);
		MapDefine mapDefine = new MapDefine ();
		mapDefine.keys = keys;
		GetTable<T>();

		if (!m_mapDefines.ContainsKey (type)) {
			m_mapDefines[type] = new Dictionary<string, MapDefine>();
		}
		m_mapDefines [type] [mapName] = mapDefine;
		if (m_systemMap.ContainsKey(type)) {
			_GenMap(type, mapName, mapDefine);
		}
	}
	public void GenMap<T>(string mapName, string key) 
		where T : ProtoBuf.IExtensible {
		string[] keys = {key};
		GenMap<T> (mapName, keys);
	}
	public void GenMap<T>(string mapName, string key1, string key2) 
		where T : ProtoBuf.IExtensible {
		string[] keys = {key1, key2};
		GenMap<T> (mapName, keys);
	}
	public void GenMap<T>(string mapName, string key1, string key2, string key3)
		where T : ProtoBuf.IExtensible {
		string[] keys = {key1, key2, key3};
		GenMap<T> (mapName, keys);
	}

	public List<object> GetListFromMap(string mapName, object[] keys) {
		MapData mapData = m_map [mapName];
		Dictionary<object, object> dic = mapData.map;
		for (int i=0; i<keys.Length; ++i) {
			object cellValue = keys[i];
			if (!dic.ContainsKey(cellValue)) {
				return null;
			}
			if (i == keys.Length - 1) {
				List<object> list = (List<object>)dic[cellValue];
				return list;
			} else {
				dic = (Dictionary<object, object>)dic[cellValue];
			}
		}
		return null;
	}

	public List<object> GetListFromMap(string mapName, object key1) {
		object[] keys = {key1};
		return GetListFromMap (mapName, keys);
	}
	public List<object> GetListFromMap(string mapName, object key1, object key2) {
		object[] keys = {key1, key2};
		return GetListFromMap (mapName, keys);
	}
	public List<object> GetListFromMap(string mapName, object key1, object key2, object key3) {
		object[] keys = {key1, key2, key3};
		return GetListFromMap (mapName, keys);
	}

	public ProtoBuf.IExtensible GetEntryFromMap(string mapName, object[] keys) {
		List<object> list = GetListFromMap (mapName, keys);
		if (list == null) {
			return null;
		}
		MapData mapData = m_map [mapName];
		return m_systemMap [mapData.type] [list [0]];
	}
	public ProtoBuf.IExtensible GetEntryFromMap(string mapName, object key1) {
		object[] keys = {key1};
		return GetEntryFromMap(mapName, keys);
	}
	public ProtoBuf.IExtensible GetEntryFromMap(string mapName, object key1, object key2) {
		object[] keys = {key1, key2};
		return GetEntryFromMap(mapName, keys);
	}

	public ArrayList GetKeysFromMap(string mapName, object[] keys = null) {
		MapData mapData = m_map [mapName];

		Dictionary<object, object> dic = mapData.map;
		if (keys != null) {
			for (int i=0; i<keys.Length; ++i) {
				object cellValue = keys[i];
				if (!dic.ContainsKey(cellValue)) {
					return null;
				}
				dic = (Dictionary<object, object>)dic[cellValue];
			}
		}
		var array = new ArrayList();
		foreach (object key in dic.Keys) {
			array.Add(key);
		}
		return array;
	}

	public ArrayList GetKeysFromMap(string mapName, object key1) {
		object[] keys = {key1};
		return GetKeysFromMap (mapName, keys);
	}
	public ArrayList GetKeysFromMap(string mapName, object key1, object key2) {
		object[] keys = {key1, key2};
		return GetKeysFromMap (mapName, keys);
	}

	// ??streamassets??????
	static byte[] ReadFile(string path)
	{
		byte[] b = null;
		if(Application.platform == RuntimePlatform.Android && path.Contains(StreamPath))
		{
			WWW www = new WWW(path);
			while (!www.isDone) ;
			if(string.IsNullOrEmpty(www.error))
			{
				b = www.bytes;
			}
			www.Dispose();
		}
		else
		{
			b = ReadFileStream(path);
		}
		
		return b;
	}
	
	static byte[] ReadFileStream(string path)
	{
		byte[] b = null;
		using (Stream file = File.OpenRead(path))
		{
			b = new byte[(int)file.Length];
			file.Read(b, 0, b.Length);
			file.Close();
			file.Dispose();
		}
		return b;
	}
}