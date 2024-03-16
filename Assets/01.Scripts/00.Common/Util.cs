using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class Util
{
    public static StringBuilder Sb = new StringBuilder();

    //Memo: CSVReader Value
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };
    public static string SPECIALCHARS = @"[^0-9a-zA-Z가-힣]";


    public static List<Dictionary<string, object>> CSVReadFromResourcesFolder(string file)
    {
        var list = new List<Dictionary<string, object>>();

        TextAsset data = Resources.Load(file) as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1)
            return list;

        var header = Regex.Split(lines[0], SPLIT_RE);

        for (int i = 0; i < header.Length; i++)
        {
            header[i] = header[i].Split('|')[0];
        }

        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "")
                continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                value = value.Replace("<br>", "\n");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                    finalvalue = n;
                else if (float.TryParse(value, out f))
                    finalvalue = f;

                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        return list;
    }

    public static T Parse<T>(string value) where T : struct
    {
        try
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return default(T);
        }
    }

    public static T Parse<T>(string value, bool ignoreCase, T defaultValue) where T : struct
    {
        try
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }
        catch
        {
            return defaultValue;
        }
    }

    public static T TryParse<T>(string value) where T : struct
    {
        try
        {
            if (Enum.TryParse(value, out T result))
                return result;
            else
                return default(T);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return default(T);
        }
    }

    public static float FloatParse(string _value)
    {
        return float.Parse(_value, CultureInfo.InvariantCulture);

    }

    public static int ParseToInt<T>(string value) where T : struct
    {
        try
        {
            return (int)Enum.Parse(typeof(T), value, true);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return 0;
        }
    }

    public static bool TryParseToInt<T>(string value, out int number) where T : struct
    {
        try
        {
            number = (int)Enum.Parse(typeof(T), value, true);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        number = 0;
        return false;
    }

    public static int ParseToInt(string _value)
    {
        return int.Parse(_value, CultureInfo.InvariantCulture);
    }

    public static double ParseToDouble(string _value)
    {
        return double.Parse(_value, CultureInfo.InvariantCulture);
    }

    public static DateTime ParseToDateTime(string _value)
    {
        return DateTime.Parse(_value, CultureInfo.InvariantCulture);
    }

    public static Vector3 ConvertWorldToCanvasPosition(RectTransform rect, Vector3 worldPos)
    {
        Vector3 canvasPos = default;

        Vector3 viewportPos = Camera.main.WorldToViewportPoint(worldPos);
        RectTransform canvasRect = rect;
        canvasPos = new Vector3(
            ((viewportPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((viewportPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

        return canvasPos;
    }

    public static T DeepCopy<T>(T obj)
    {
        using (var stream = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            stream.Position = 0;

            return (T)formatter.Deserialize(stream);
        }
    }
}
