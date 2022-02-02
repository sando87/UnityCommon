using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class MyUtils
{
    // System =====================================
    public static T ToEnum<T>(this string value)
    {
        // 변환 오류인 경우 디폴트 값 리턴
        if (!System.Enum.IsDefined(typeof(T), value))
        {
            return default(T);
        }
        return (T)System.Enum.Parse(typeof(T), value, true);
    }
    public static int CountEnum<T>()
    {
        return System.Enum.GetValues(typeof(T)).Length;
    }
    static public int Sizeof<T>()
    {
        return Marshal.SizeOf(typeof(T));
    }
    static public byte[] Serialize(object obj)
    {
        try
        {
            int size = Marshal.SizeOf(obj);
            byte[] arr = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }
        catch (Exception ex)
        {
            LOG.warn(ex.Message);
        }
        return null;
    }
    static public T Deserialize<T>(ref byte[] data, int off = 0) where T : new()
    {
        try
        {
            T str = new T();
            int size = Marshal.SizeOf(str);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(data, off, ptr, size);
            str = (T)Marshal.PtrToStructure(ptr, str.GetType());
            Marshal.FreeHGlobal(ptr);
            return str;
        }
        catch (Exception ex)
        {
            LOG.warn(ex.Message);
        }
        return default(T);
    }
    public static byte[] HexStringToByteArray(string hex)
    {
        int hexFactor = 16; //string format is hex
        List<byte> rets = new List<byte>();
        for (int i = 0; i < hex.Length; i += 2)
        {
            rets.Add(Convert.ToByte(hex.Substring(i, 2), hexFactor));
        }
        return rets.ToArray();
    }
    public static string ByteArrayToHexString(byte[] data)
    {
        return BitConverter.ToString(data).Replace("-", string.Empty);
    }
    public static byte[] Encrypt(byte[] input)
    {
        byte[] output = new byte[input.Length];
        byte key = 0x4f;
        output[0] = (byte)(key ^ input[0]);
        for (int i = 1; i < input.Length; ++i)
        {
            output[i] = (byte)(output[i - 1] ^ (key ^ input[i]));
        }
        return output;
    }
    public static byte[] Decrypt(byte[] input)
    {
        byte[] output = new byte[input.Length];
        byte key = 0x4f;
        for (int i = input.Length - 1; i > 0; --i)
        {
            output[i] = (byte)((input[i] ^ input[i - 1]) ^ key);
        }
        output[0] = (byte)(key ^ input[0]);
        return output;
    }
    public static void SaveToFile(string data, string path, bool isEncrypt)
    {
        try
        {
            if (isEncrypt)
            {
                byte[] bytes = System.Text.Encoding.Default.GetBytes(data);
                byte[] encoded = Encrypt(bytes);
                File.WriteAllBytes(path, encoded);
            }
            else
            {
                File.WriteAllText(path, data);
            }
        }
        catch (IOException e)
        {
            LOG.warn(e.Message);
        }
    }
    public static bool LoadFromFile(string path, bool isEncrypt, out string data)
    {
        try
        {
            if (isEncrypt)
            {
                byte[] encoded = File.ReadAllBytes(path);
                byte[] decodedData = Decrypt(encoded);
                data = System.Text.Encoding.Default.GetString(decodedData);
                return true;
            }
            else
            {
                data = File.ReadAllText(path);
                return true;
            }
        }
        catch (IOException e)
        {
            LOG.warn(e.Message);
        }
        data = "";
        return false;
    }
    public static T LoadFromRegedit<T>(string key) where T : new()
    {
        try
        {
            if (PlayerPrefs.HasKey(key))
            {
                string hexStr = PlayerPrefs.GetString(key);
                byte[] bytes = MyUtils.HexStringToByteArray(hexStr);
                byte[] originData = MyUtils.Decrypt(bytes);
                string jsonStr = System.Text.Encoding.Default.GetString(originData);
                T ret = JsonUtility.FromJson<T>(jsonStr);
                return ret;
            }
            else
            {
                T ret = new T();
                return ret;
            }
        }
        catch (Exception ex) { LOG.warn(ex.Message); }
        return default;
    }
    public static void SaveToRegedit(string key, object obj)
    {
        try
        {
            string jsonStr = JsonUtility.ToJson(obj, true);
            byte[] oriData = System.Text.Encoding.Default.GetBytes(jsonStr);
            byte[] encryptData = MyUtils.Encrypt(oriData);
            string hexStr = MyUtils.ByteArrayToHexString(encryptData);
            PlayerPrefs.SetString(key, hexStr);
        }
        catch (Exception ex) { LOG.warn(ex.Message); }
    }
    public static T GetPrivatePropertyValue<T>(this object obj, string propName)
    {
        if (obj == null) throw new ArgumentNullException("obj");
        PropertyInfo pi = obj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (pi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Property {0} was not found in Type {1}", propName, obj.GetType().FullName));
        return (T)pi.GetValue(obj, null);
    }
    public static T GetPrivateFieldValue<T>(this object obj, string propName)
    {
        if (obj == null) throw new ArgumentNullException("obj");
        Type t = obj.GetType();
        FieldInfo fi = null;
        while (fi == null && t != null)
        {
            fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            t = t.BaseType;
        }
        if (fi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));
        return (T)fi.GetValue(obj);
    }
    public static void SetPrivatePropertyValue<T>(this object obj, string propName, T val)
    {
        Type t = obj.GetType();
        if (t.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) == null)
            throw new ArgumentOutOfRangeException("propName", string.Format("Property {0} was not found in Type {1}", propName, obj.GetType().FullName));
        t.InvokeMember(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, obj, new object[] { val });
    }
    public static void SetPrivateFieldValue<T>(this object obj, string propName, T val)
    {
        if (obj == null) throw new ArgumentNullException("obj");
        Type t = obj.GetType();
        FieldInfo fi = null;
        while (fi == null && t != null)
        {
            fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            t = t.BaseType;
        }
        if (fi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));
        fi.SetValue(obj, val);
    }
    public static void InvokePrivateMethod(this object obj, string methodName, object[] methodParam)
    {
        MethodInfo dynMethod = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (dynMethod == null) throw new ArgumentOutOfRangeException(methodName, string.Format("Method {0} was not found", methodName));
        dynMethod.Invoke(obj, methodParam);
    }

    // Unity Common =====================================

    // 동일한 스크린 좌표계의 rect 정보로 UI의 위치와 크기를 배치시킨다.
    public static void SetRect(this RectTransform ui, RectTransform targetUI)
    {
        ui.sizeDelta = targetUI.rect.size;
        ui.position = targetUI.position;
    }
    // world좌표계의 rect 위치로 UI의 위치와 크기를 배치시킨다.
    public static void SetRectFromWorldRect(this RectTransform ui, Rect worldRect)
    {
        // 실제 장비 해상도 기준의 스크린 좌표값
        Vector2 screenRectMin = Camera.main.WorldToScreenPoint(worldRect.min);
        Vector2 screenRectMax = Camera.main.WorldToScreenPoint(worldRect.max);
        Vector2 screenRectSize = screenRectMax - screenRectMin;
        // 유니티상에 설정된 레퍼런스 해상도 기준의 스크린 좌표값으로 변환 작업
        CanvasScaler scaler = ui.GetComponentInParent<CanvasScaler>();
        float wRatio = scaler.referenceResolution.x / Screen.width;
        float hRatio = scaler.referenceResolution.y / Screen.height;

        float ratio = wRatio * (1f - scaler.matchWidthOrHeight)
                    + hRatio * (scaler.matchWidthOrHeight);
        // 현재 UI RectTransform의 Inspector상에 들어가는 width, height 최종값
        Vector2 sizeDelta = screenRectSize * ratio;
        ui.sizeDelta = sizeDelta;
        ui.position = worldRect.center;
    }
    // world좌표계 너비를 실제 장비 기준 해상도 좌표계 너비로 변환
    public static Vector2 SizeWorldToScreen2D(Vector2 size)
    {
        // 실제 장비 해상도 기준의 스크린 좌표값
        Vector2 screenA = Camera.main.WorldToScreenPoint(new Vector2(0, 0));
        Vector2 screenB = Camera.main.WorldToScreenPoint(new Vector2(size.x, size.y));
        return screenB - screenA;
    }
    // 실제 장비 해상도를 inspector에 들어가는 값기준으로 변경(캔버스 스캐일링 된..)
    public static Vector2 SizeScreenToCavausScaled2D(Vector2 size, CanvasScaler scaler)
    {
        // 유니티상에 설정된 레퍼런스 해상도 기준의 좌표값으로 변환 작업
        float wRatio = scaler.referenceResolution.x / Screen.width;
        float hRatio = scaler.referenceResolution.y / Screen.height;
        float ratio = wRatio * (1f - scaler.matchWidthOrHeight)
                    + hRatio * (scaler.matchWidthOrHeight);
        // 현재 UI RectTransform의 Inspector상에 들어가는 width 최종값
        return size * ratio;
    }
    // world좌표계 너비를 unity inspector에 들어가는(캔버스 스캐일링 된) 좌표계 너비로 변환
    public static Vector2 SizeWorldToCavausScaled2D(Vector2 size, CanvasScaler scaler)
    {
        Vector2 screenSize = SizeWorldToScreen2D(size);
        return SizeScreenToCavausScaled2D(screenSize, scaler);
    }
    public static Vector3 Random(Vector3 pos, float range)
    {
        Vector3 ret = pos;
        ret.x += UnityEngine.Random.Range(-range, range);
        ret.y += UnityEngine.Random.Range(-range, range);
        return ret;
    }

    public static void DisableAllChilds(GameObject parent)
    {
        int cnt = parent.transform.childCount;
        for (int i = 0; i < cnt; ++i)
        {
            parent.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    public static T FindUIOnPosition<T>(Vector2 worldPos) where T : MonoBehaviour
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = worldPos;
        List<RaycastResult> rets = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, rets);
        foreach (RaycastResult ret in rets)
        {
            T recv = ret.gameObject.GetComponent<T>();
            if (recv != null)
                return recv;
        }
        return default(T);
    }
    public static T FindObjectOnPosition<T>(Vector2 worldPos) where T : MonoBehaviour
    {
        //Vector2 worldPos = Camera.main.ScreenToWorldPoint(transform.position);
        Collider2D[] mapObjects = Physics2D.OverlapPointAll(worldPos);
        foreach (Collider2D col in mapObjects)
        {
            T recv = col.GetComponent<T>();
            if (recv != null)
            {
                return recv;
            }
        }
        return default(T);
    }
    public static bool IsPointerOverUI()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return EventSystem.current.IsPointerOverGameObject(-1); //mouse click
#elif UNITY_ANDROID || UNITY_IPHONE
        return Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId); //finger touch 
#endif
    }
    public static Vector2 GetPointerScreenPosition()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return Input.mousePosition;
#elif UNITY_ANDROID || UNITY_IPHONE
        return Input.touchCount > 0 ? Input.GetTouch(0).position : Vector2.zero;
#endif
    }

    //SerializedProperty 에서 실제 객체 TYPE의 오브젝트로 반환
    public static object GetValueFromProp(SerializedProperty property, string path)
    {
        System.Type parentType = property.serializedObject.targetObject.GetType();
        System.Reflection.FieldInfo fi = parentType.GetField(path);
        return fi.GetValue(property.serializedObject.targetObject);
    }
    //SerializedProperty 에서 실제 객체 TYPE의 오브젝트값을 세팅
    public static void SetValueToProp(SerializedProperty property, object value)
    {
        System.Type parentType = property.serializedObject.targetObject.GetType();
        System.Reflection.FieldInfo fi = parentType.GetField(property.propertyPath);//this FieldInfo contains the type.
        fi.SetValue(property.serializedObject.targetObject, value);
    }
    ////SerializedProperty 에서 실제 객체의 Type을 반환
    public static System.Type GetTypeFromProp(SerializedProperty property)
    {
        System.Type parentType = property.serializedObject.targetObject.GetType();
        System.Reflection.FieldInfo fi = parentType.GetField(property.propertyPath);
        return fi.FieldType;
    }
    // guid값에서 실제 Asset 리소스 객체 가져옴
    public static UnityEngine.Object GetAssetFromGUID(string guid, Type assetType)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        return AssetDatabase.LoadAssetAtPath(path, assetType);
    }
    // guid값에서 실제 Asset 리소스 객체 가져옴
    public static string GetGUIDFromAsset(UnityEngine.Object asset)
    {
        string path = AssetDatabase.GetAssetPath(asset);
        string guid = AssetDatabase.AssetPathToGUID(path);
        return guid;
    }


    // Extentions =====================================
    public static void ExSetPosition2D(this Transform tr, Vector2 val)
    {
        tr.position = new Vector3(val.x, val.y, tr.position.z);
    }
    public static void ExSetLocalPosition2D(this Transform tr, Vector2 val)
    {
        tr.localPosition = new Vector3(val.x, val.y, tr.localPosition.z);
    }
    public static Vector2 ExToVector2(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.y);
    }
    public static Vector3 ExToVector3(this Vector2 vec, float z = 0)
    {
        return new Vector3(vec.x, vec.y, z);
    }
    public static void ExSetAlpha(this Text text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
    public static void ExSetAlpha(this SpriteRenderer sr, float alpha)
    {
        Color color = sr.color;
        color.a = alpha;
        sr.color = color;
    }
    public static void ExSetAlpha(this Image img, float alpha)
    {
        Color color = img.color;
        color.a = alpha;
        img.color = color;
    }
    public static void ExSetAlpha(this TextMesh text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
    public static void ExSortRandomly<T>(this List<T> list)
    {
        System.Random ran = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int idx = ran.Next(n + 1);
            T val = list[idx];
            list[idx] = list[n];
            list[n] = val;
        }
    }
    public static void ExInvoke(this MonoBehaviour mono, float delay, Action func)
    {
        mono.StartCoroutine(CoExInvoke(func, delay));
    }
    public static IEnumerator CoExInvoke(Action EventEnd, float delay)
    {
        yield return new WaitForSeconds(delay);
        EventEnd?.Invoke();
    }
    public static string SecondsToMinSec(int playTime)
    {
        TimeSpan t = new TimeSpan(0, 0, playTime);
        return String.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
    }
    public static string ToCommaEveryThree(int amount)
    {
        // 123456789 -> 123,456,789 로 표기
        return String.Format("{0:#,0}", amount);
    }
    public static string CurrentStyleToInt(string money)
    {
        // 123,456,789 -> 123456789 로 표기
        return money.Replace(",", "");
    }
}

// Util Class ================================================
class RandomSequence
{
    // 0 ~ maxValue사이의 랜덤값을 겹치지 않게 반환

    private int[] mNumbers = null;
    private int mIndex = 0;
    public RandomSequence(int maxValue)
    {
        System.Random ran = new System.Random();
        List<int> values = new List<int>();
        for (int i = 0; i <= maxValue; ++i)
            values.Add(i);
        values.Sort((a, b) => { return ran.Next(-1, 1); });
        mNumbers = values.ToArray();
        mIndex = 0;
    }
    public int GetNext()
    {
        mIndex = (mIndex + 1) % mNumbers.Length;
        return mNumbers[mIndex];
    }
}
