using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Text;
using System.IO;
using UnityEngine;
/// <summary>
/// 线程安全
/// </summary>
public class UNValue
{
    private UNValue()
	{
	}
	//private static byte[] keys = {0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF};
	//private static string keyUser = "12345678";
	//private static string keyDe ="12345678";
	public static bool m_isEncode = false;

    private static byte[] encryBytes = null;
    private static byte[] decryBytes = null;

    static UNValue()
    {
       
		System.Random random = new System.Random();
        encryBytes = new byte[256];
        decryBytes = new byte[256];
        for(int j= 0; j < 256; j ++)
        {
            encryBytes[j] = (byte)j;
            decryBytes[j] = (byte)j;
        }
        byte i = (byte)random.Next(256);
        byte temp = 0;
        for (int j = 0; j < 256; j++)
        {
            temp = encryBytes[i];
            encryBytes[i] = encryBytes[j];
            encryBytes[j] = temp;
            i = (byte)random.Next(256);
        }
       
        for (int j = 0; j < 256; j++)
        {
            decryBytes[encryBytes[j]] = (byte)j;
        }

    }
    [Obsolete]
    public static string DecryptDES(string decryptString)
    {
        if (string.IsNullOrEmpty(decryptString))
        {
            return "";
        }
        try
        {
            byte[] byts = StringToBytes(decryptString);
            for (int i = 0; i < byts.Length; i++)
            {
                byts[i] = decryBytes[byts[i]];
            }
            return BytesToString(byts);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.StackTrace);
            Debug.LogWarning(ex.Message);
            return decryptString;
        }
    }


    public static byte[] DecryptDES(byte[] bytes)
    {
        byte[] tempBytes = new byte[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            tempBytes[i] = (byte)(decryBytes[bytes[i]]);
        }
        return tempBytes;
    }

    public static byte[] EncryptDES(byte[] bytes)
    {
        byte[] tempBytes = new byte[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            tempBytes[i] = (byte)(encryBytes[bytes[i]]);
        }
        return tempBytes;
    }

	//int
	public static byte[] m_intBytes = new byte[4];
    public static byte[] DecryptIntDES(byte[] bytes)
    {
        for (int i = 0; i < 4; i++)
        {
            m_intBytes[i] = (byte)(decryBytes[bytes[i]]);
        }
        return m_intBytes;
    }
    public static byte[] EncryptIntDES(byte[] bytes)
    {
        for (int i = 0; i < 4; i++)
        {
            m_intBytes[i] = (byte)(encryBytes[bytes[i]]);
        }
        return m_intBytes;
    }

	//long
	public static byte[] m_longBytes = new byte[8];
    public static byte[] DecryptLongDES(byte[] bytes)
    {
        for (int i = 0; i < 8; i++)
        {
            m_longBytes[i] = (byte)(decryBytes[bytes[i]]);
        }
        return m_longBytes;
    }
    public static byte[] EncryptLongDES(byte[] bytes)
    {
        for (int i = 0; i < 8; i++)
        {
            m_longBytes[i] = (byte)(encryBytes[bytes[i]]);
        }
        return m_longBytes;
    }

    [Obsolete]
    public static string EncryptDES(string encryptString)	
	{
        if (string.IsNullOrEmpty(encryptString))
        {
            return "";
        }
        try
        {
            byte[] byts = StringToBytes(encryptString);//StringToBytes(encryptString, out count);
            for (int i = 0; i < byts.Length; i++)
            {
                byts[i] = (byte)(encryBytes[byts[i]] );
            }
            return BytesToString(byts); 
        }
        catch(Exception ex)
        {
            Debug.LogWarning(ex.StackTrace);
            Debug.LogWarning(ex.Message);
            return encryptString;
        }
	}

    

    private static string getStr(byte[] abc, int length = 0)
    {
        string str = "";
        if (length == 0)
        {
            length = abc.Length;
        }
        for (int i = 0; i < length; i++)
        {
            str += "[" + i + "= " + abc[i] + "]";
        }
        return str;
    }




    private static byte[] StringToBytes(string str)
    {
        int length = str.Length;
        byte[] outBytes1 = new byte[str.Length << 1];
        for (int i = 0; i < length; i++)
        {
            outBytes1[i << 1] = (byte)(str[i] >> 8);
            outBytes1[(i << 1) | 0x1] = (byte)(str[i] & 0x00ff);
        }
        return outBytes1;
    }

    private static string BytesToString(byte[] bytes)
    {
        int length = bytes.Length >> 1;
        char[] outChar = new char[length];
        //StringBuilder str = new StringBuilder(outChar.Length);
        for (int i = 0; i < length; i++)
        {
            outChar[i] = (char)(((char)(bytes[i * 2]) << 8) | bytes[(i * 2) + 1]);
            //str.Append(outChar[i]);
        }
        return new string(outChar);
    }


	//public static int GetIntValue(string src)
	//{
	//	return int.Parse(DecryptDES(src));
	//}

	public static void GetDeLs(List<UNValueInt> ls, ref List<int> returnLs)
	{
		returnLs.Clear ();
		for(int i = 0; i < ls.Count; i ++)
		{
			returnLs.Add(ls[i].m_value);
		}
	}
	
	public static void InitEnLs(List<int> ls, ref List<UNValueInt> returnLs)
	{
		returnLs.Clear ();
		for(int i = 0; i < ls.Count; i ++)
		{
			returnLs.Add(new UNValueInt(ls[i]));
		}
	}

	public static void GetDeLs(List<UNValueFloat> ls, ref List<float> returnLs)
	{
		returnLs.Clear ();
		for(int i = 0; i < ls.Count; i ++)
		{
			returnLs.Add(ls[i].m_value);
		}
	}
	
	public static void InitEnLs(List<float> ls, ref List<UNValueFloat> returnLs)
	{
		returnLs.Clear ();
		for(int i = 0; i < ls.Count; i ++)
		{
			returnLs.Add(new UNValueFloat(ls[i]));
		}
	}
}	

public class UNValueListInt
{
	private List<UNValueInt> _m_value = new List<UNValueInt>();
	private List<int> _m_value_int = new List<int>();
	public List<int> m_value 
	{
		get
		{
			UNValue.GetDeLs(_m_value, ref _m_value_int);
			return _m_value_int;
		}
		set
		{
			UNValue.InitEnLs(value, ref _m_value);
		}
	}

	public UNValueListInt()
	{
	}

	public UNValueListInt(List<int> value)
	{
		UNValue.InitEnLs(value, ref _m_value);
	}
}

public class UNValueListFloat
{
	private List<UNValueFloat> _m_value = new List<UNValueFloat>();
	private List<float> _m_value_float = new List<float>();
	public List<float> m_value 
	{
		get
		{
			UNValue.GetDeLs(_m_value, ref _m_value_float);
			return _m_value_float;
		}
		set
		{
			UNValue.InitEnLs(value, ref _m_value);
		}
	}
	
	public UNValueListFloat()
	{
	}
	
	public UNValueListFloat(List<float> value)
	{
		UNValue.InitEnLs(value, ref _m_value);
	}
}

//public class EvalueData<T>
//{
//    private byte[] _m_value = null;

//    public EvalueData()
//    {
//        _m_value = EValue.EncryptDES(BitConverter.GetBytes(0.0));
//    }

//    public EvalueData(double value)
//    {
//        if (typeof(T) == typeof(int) ||
//            typeof(T) == typeof(long))
//        {
//            _m_value = EValue.EncryptDES(BitConverter.GetBytes((long)value));
//        }
//        else if (typeof(T) == typeof(float) ||
//            typeof(T) == typeof(double))
//        {
//            _m_value = EValue.EncryptDES(BitConverter.GetBytes(value));
//        }
//    }

//    public EvalueData(T value)
//    {
//        if (typeof(T) == typeof(int) ||
//            typeof(T) == typeof(long))
//        {
//            var v = (long)System.Convert.ChangeType(value, typeof(long));
//            _m_value = EValue.EncryptDES(BitConverter.GetBytes(v));
//        }
//        else if (typeof(T) == typeof(float) ||
//            typeof(T) == typeof(double))
//        {
//            var v = (double)System.Convert.ChangeType(value, typeof(double));
//            _m_value = EValue.EncryptDES(BitConverter.GetBytes(v));
//        }
//    }

//    public EZFunNumber<T> m_value
//    {
//        get
//        {
//            if (typeof(T) == typeof(int) ||
//                typeof(T) == typeof(long))
//            {
//                var r = BitConverter.ToInt64(EValue.DecryptDES(_m_value), 0);
//                var rt = (T)System.Convert.ChangeType(r, typeof(T));
//                return new EZFunNumber<T>(rt);
//            }
//            else if (typeof(T) == typeof(float) ||
//                typeof(T) == typeof(double))
//            {
//                var r = BitConverter.ToDouble(EValue.DecryptDES(_m_value), 0);
//                var rt = (T)System.Convert.ChangeType(r, typeof(T));
//                return new EZFunNumber<T>(rt);
//            }
//            return null;
//        }
//        set
//        {
//            if (typeof(T) == typeof(int) ||
//                typeof(T) == typeof(long))
//            {
//                var v = (long)System.Convert.ChangeType(value.m_value, typeof(long));
//                _m_value = EValue.EncryptDES(BitConverter.GetBytes(v));
//            }
//            else if (typeof(T) == typeof(float) ||
//                typeof(T) == typeof(double))
//            {
//                var v = (double)System.Convert.ChangeType(value.m_value, typeof(double));
//                _m_value = EValue.EncryptDES(BitConverter.GetBytes(v));
//            }
//        }
//    }
//}


public class UNValueInt
{
    private byte[] _m_value = new byte[4];//= BitConverter.GetBytes(0);

    public UNValueInt()
    {
        lock (UNValue.m_intBytes)
        {
            Array.Copy(UNValue.EncryptIntDES(BitConverter.GetBytes(0)), _m_value, 4);
        }
    }

    public UNValueInt(int value)
    {
        lock (UNValue.m_intBytes)
        {
            Array.Copy(UNValue.EncryptIntDES(BitConverter.GetBytes(value)), _m_value, 4);
        }
    }

    public int m_value
    {
        get
        {
            //这么弄是为了少点内存啊
            lock (UNValue.m_intBytes)
            {
                return BitConverter.ToInt32(UNValue.DecryptIntDES(_m_value), 0);
            }
        }
        set
        {
            lock (UNValue.m_intBytes)
            {
                Array.Copy(UNValue.EncryptIntDES(BitConverter.GetBytes(value)), _m_value, 4);
            }
        }
    }
}

public class UNValueFloat
{
    private byte[] _m_value = new byte[4];//= BitConverter.GetBytes(0);

	public UNValueFloat()
	{
        _m_value = UNValue.EncryptDES(BitConverter.GetBytes(0f));
	}

	public UNValueFloat(float value)
	{
        _m_value = UNValue.EncryptDES(BitConverter.GetBytes(value));
	}

	public float m_value
	{
        get
        {
            //这么弄是为了少点内存啊
            return BitConverter.ToSingle(UNValue.DecryptDES(_m_value), 0);
        }
        set
        {
            _m_value = UNValue.EncryptDES(BitConverter.GetBytes(value));
        }
	}
}




public class UNValueLong
{
    private byte[] _m_value = new byte[8];

    public UNValueLong()
    {
        lock (UNValue.m_longBytes)
        {
            Array.Copy(UNValue.EncryptLongDES(BitConverter.GetBytes(0L)), _m_value, 8);
        }
    }

    public UNValueLong(long value)
    {
        lock (UNValue.m_longBytes)
        {
            Array.Copy(UNValue.EncryptLongDES(BitConverter.GetBytes(value)), _m_value, 8);
        }
    }

    public long m_value
    {
        get
        {
            //这么弄是为了少点内存啊
            lock (UNValue.m_longBytes)
            {
                return BitConverter.ToInt64(UNValue.DecryptLongDES(_m_value), 0);
            }
        }
        set
        {
            lock (UNValue.m_longBytes)
            {
                Array.Copy(UNValue.EncryptLongDES(BitConverter.GetBytes(value)), _m_value, 8);
            }
        }
    }
}


public class UNValueBool
{
      private byte[] _m_value = null;

    public UNValueBool()
    {
        _m_value = UNValue.EncryptDES(BitConverter.GetBytes(false));
    }

    public UNValueBool(double value)
    {
        _m_value = UNValue.EncryptDES(BitConverter.GetBytes(value));
    }

    public bool m_value
    {
        get
        {
            //这么弄是为了少点内存啊
            return BitConverter.ToBoolean(UNValue.DecryptDES(_m_value), 0);
        }
        set
        {
            _m_value = UNValue.EncryptDES(BitConverter.GetBytes(value));
        }
    }
}


public class UNValueDouble
{
    private byte[] _m_value = null;

    public UNValueDouble()
    {
        _m_value = UNValue.EncryptDES(BitConverter.GetBytes(0.0));
    }

    public UNValueDouble(double value)
    {
        _m_value = UNValue.EncryptDES(BitConverter.GetBytes(value));
    }

    public double m_value
    {
        get
        {
            //这么弄是为了少点内存啊
            return BitConverter.ToDouble(UNValue.DecryptDES(_m_value), 0);
        }
        set
        {
            _m_value = UNValue.EncryptDES(BitConverter.GetBytes(value));
        }
    }
}

public class UNValueVector2
{
    private UNValueFloat _m_x = new UNValueFloat();
    private UNValueFloat _m_Y = new UNValueFloat();

    public UNValueVector2(float _x, float _y)
    {
        _m_x.m_value = _x;
        _m_Y.m_value = _y;
    }

    public float x
    {
        get{return _m_x.m_value;}
        set{_m_x.m_value = value;}
    }

    public float y
    {
        get{return _m_Y.m_value;}
        set{_m_Y.m_value = value;}
    }
}