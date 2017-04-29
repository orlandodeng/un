//========================================================================
// Copyright(C): UN
// Created by : dhf at 2017/4/19 17:46:59
// Function : 
//========================================================================

using System.Text;
using System.Collections.Generic;

public class UNString
{
    private static List<StringBuilder> m_sbs_inuse = new List<StringBuilder>();
    private static List<StringBuilder> m_sbs_unuse = new List<StringBuilder>();

    public static string LinkString(params object[] objects)
    {
        if (objects == null)
        {
            return "";
        }
        // 数量大于2时stringbuilder有GC优势
        // 数量大于6时stringbuilder有时间优势
        if (objects.Length > 2)
        {
            return LinkString1(objects);
        }
        else
        {
            return LinkString2(objects);
        }
    }

    private static string LinkString1(params object[] objects)
    {
        StringBuilder sb = null;
        if (m_sbs_unuse.Count > 0)
        {
            sb = m_sbs_unuse[0];
            sb.Remove(0, sb.Length);
            m_sbs_unuse.RemoveAt(0);
        }
        if (sb == null)
        {
            sb = new StringBuilder();
        }
        m_sbs_inuse.Add(sb);
        for (int i = 0; i < objects.Length; ++i)
        {
            sb.Append(objects[i]);
        }
        m_sbs_inuse.Remove(sb);
        m_sbs_unuse.Add(sb);

        return sb.ToString();
    }

    private static string LinkString2(params object[] objects)
    {
        string str = "";
        for (int i = 0; i < objects.Length; ++i)
        {
            str += objects[i];
        }
        return str;
    }
}