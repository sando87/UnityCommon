using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;

#endif



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

public enum DamageKind
{
    Normal, Fire
}
public struct DamageProp
{
    public DamageKind type;
    public float damage;
    public DamageProp(float _damage) { this.damage = _damage; type = DamageKind.Normal; }
    public DamageProp(float _damage, DamageKind _type) { this.damage = _damage; this.type = _type; }
    
    // DamageProp형을 float형으로 암시적 형변환 가능 예) float damage = new DamageProp(_damage);
    public static implicit operator float(DamageProp info) => info.damage;

    // float형을 DamageProp로 암시적 형변환 가능 예) DamageProp info = 1.0f;
    public static implicit operator DamageProp(float damage) => new DamageProp(damage);
    
    public static DamageProp operator +(DamageProp a, DamageProp b)
        => new DamageProp(a.damage + b.damage, a.type);
    public static DamageProp operator +(DamageProp a, float _damage)
        => new DamageProp(a.damage + _damage, a.type);
        
    public static DamageProp operator -(DamageProp a, DamageProp b)
        => new DamageProp(a.damage - b.damage, a.type);
    public static DamageProp operator -(DamageProp a, float _damage)
        => new DamageProp(a.damage - _damage, a.type);

    public override string ToString() => $"{damage}";
}


// 유니티 Gameobject에 범용 데이터를 임시로 Get,Set 할수 있는 기능..
public static class GeneralValue
{
    private static Dictionary<int, Dictionary<string, object>> mValues = new Dictionary<int, Dictionary<string, object>>();

    public static void SetValue(this GameObject obj, string name, object val)
    {
        int key = obj.GetInstanceID();
        if (!mValues.ContainsKey(key))
        {
            mValues[key] = new Dictionary<string, object>();
        }

        mValues[key][name] = val;
    }
    public static object GetValue(this GameObject obj, string name)
    {
        int instanceID = obj.GetInstanceID();
        if (mValues.ContainsKey(instanceID))
        {
            if (mValues[instanceID].ContainsKey(name))
            {
                return mValues[instanceID][name];
            }
        }
        return null;
    }
}


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
    public static string[] ToEnumStrings<T>()
    {
        return System.Enum.GetNames(typeof(T));
    }
    public static int CountEnum<T>()
    {
        return System.Enum.GetValues(typeof(T)).Length;
    }
    public static T RandomEnum<T>(bool exceptFirst = false)
    {
        int enumVal = UnityEngine.Random.Range(exceptFirst ? 1 : 0, CountEnum<T>());
        return (T) (object) enumVal;
    }
    public static bool IsPercentHit(int percent)
    {
        return (UnityEngine.Random.Range(0, 1000) % 100) < percent;
    }
    public static IEnumerable EnumForeach<T>()
    {
        foreach (T item in System.Enum.GetValues(typeof(T)))
            yield return item;
    }
    static public int Sizeof<T>()
    {
        return Marshal.SizeOf(typeof(T));
    }
    public static string[] GetFileNames(string folderName, string ext) // ext like this... ".png" ".json" ".asset" etc...
    {
        List<string> filenames = new List<string>();
        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(folderName);
        foreach (System.IO.FileInfo f in di.GetFiles())
        {
            if (f.Extension.ToLower().CompareTo(ext.ToLower()) == 0)
            {
                // String strInFileName = di.FullName + "\\" + f.Name;
                string name = f.Name.Replace(f.Extension, "");
                filenames.Add(name);
            }
        }
        return filenames.ToArray();
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

    // UI상의 RectTransform 영역을 World공간의 Rect로 변환.
    public static Rect ToWorldRect(this RectTransform uiRect, Camera worldCamera)
    {
        Rect ret = new Rect();
        Vector2 worldSize = ScreenRectToWorldRect(uiRect.rect.size, worldCamera);
        ret.size = worldSize;
        ret.center = uiRect.transform.position;
        return ret;
    }
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
    // 스크린 크기를 월드공간의 크기로 변환
    public static Vector2 ScreenRectToWorldRect(Vector2 size, Camera mainCamera)
    {
        Vector3 startPos = mainCamera.ScreenToWorldPoint(Vector3.zero);
        Vector3 endPos = mainCamera.ScreenToWorldPoint(new Vector3(size.x, size.y, 0));
        return new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);
    }
    public static Vector3 Random(Vector3 pos, float range)
    {
        Vector3 ret = pos;
        ret.x += UnityEngine.Random.Range(-range, range);
        ret.y += UnityEngine.Random.Range(-range, range);
        return ret;
    }
    public static Vector3 Random(Bounds bounds, float scaleRate = 1.0f)
    {
        Vector3 ret = bounds.center;
        ret.x += UnityEngine.Random.Range(-bounds.extents.x, bounds.extents.x) * scaleRate;
        ret.y += UnityEngine.Random.Range(-bounds.extents.y, bounds.extents.y) * scaleRate;
        ret.z += UnityEngine.Random.Range(-bounds.extents.z, bounds.extents.z) * scaleRate;
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

    public static bool IsSubType<Tsub, Tbase>()
    {
        return typeof(Tsub).IsSubclassOf(typeof(Tbase));
    }
    public static bool IsSubType(Type subTyp, Type baseType)
    {
        return subTyp.IsSubclassOf(baseType);
    }

    public static Rect LimitRectMovement(this Rect targetRect, Rect limitArea)
    {
        Rect retArea = targetRect;
        Vector2 cenPos = retArea.center;
        if (retArea.width < limitArea.width)
            cenPos.x = Mathf.Clamp(cenPos.x, limitArea.xMin + retArea.width * 0.5f, limitArea.xMax - retArea.width * 0.5f);
        else
            cenPos.x = limitArea.center.x;

        if (retArea.height < limitArea.height)
            cenPos.y = Mathf.Clamp(cenPos.y, limitArea.yMin + retArea.height * 0.5f, limitArea.yMax - retArea.height * 0.5f);
        else
            cenPos.y = limitArea.center.y;

        retArea.center = cenPos;
        return retArea;
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
    public static void ExLookAtPosition(this Transform tr, Vector3 position)
    {
        tr.transform.right = (position - tr.position).normalized;
        //tr.rotation = Quaternion.LookRotation((position - tr.position).normalized);
    }
    // public static void ExLookAtPosition(this Transform tr, Vector3 position, float degMaxStep)
    // {
    //     tr.rotation = Quaternion.RotateTowards(tr.rotation, Quaternion.LookRotation((position - tr.position).normalized), degMaxStep);
    // }
    public static void ExLookAtDirection(this Transform tr, Vector3 direction)
    {
        tr.transform.right = direction.normalized;
        //tr.rotation = Quaternion.LookRotation(direction.normalized);
    }
    // public static void ExLookAtDirection(this Transform tr, Vector3 direction, float degMaxStep)
    // {
    //     Vector3 r = tr.eulerAngles;
    //     transform.rotation = Quaternion.Euler(r.x, r.y, Mathf.LerpAngle(direction, target, i));
        
    //     Quaternion.RotateTowards(tr.rotation, Quaternion.LookRotation(direction.normalized), degMaxStep).
    //     tr.rotation = Quaternion.RotateTowards(tr.rotation, Quaternion.LookRotation(direction.normalized), degMaxStep);
    // }
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
        if(list.Count <= 1)
            return;

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
    public static void ExSortInCloseOrder(this List<Transform> list, Vector3 refPositioin)
    {
        list.Sort((a, b) => {
            return (a.position - refPositioin).sqrMagnitude > (b.position - refPositioin).sqrMagnitude ? 1 : -1;
        });
    }
    public static Coroutine ExRepeatCoroutine(this MonoBehaviour mono, float interval, Action func, int repeatCount = 0)
    {
        return mono.StartCoroutine(CoExRepeatCall(func, interval, repeatCount));
    }
    public static IEnumerator CoExRepeatCall(Action EventEnd, float interval, int repeatCount)
    {
        bool isInfiniteMode = repeatCount <= 0;
        int count = 0;
        while(isInfiniteMode || count < repeatCount)
        {
            EventEnd?.Invoke();
            yield return new WaitForSeconds(interval);
            count++;
        }
    }
    public static void ExDelayedCoroutine(this MonoBehaviour mono, float delay, Action func)
    {
        mono.StartCoroutine(CoExDelayCall(func, delay));
    }
    public static IEnumerator CoExDelayCall(Action EventEnd, float delay)
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
    
    public static bool IsIncludeLayer(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }

    public static void Snap(this Transform tr, float step = 0.1f)
    {
        float x = (int)((tr.position.x + (step * 0.5f)) / step) * step;
        float y = (int)((tr.position.y + (step * 0.5f)) / step) * step;
        // float z = (int)((tr.position.z + (step * 0.5f)) / step) * step;
        tr.transform.position = new Vector3(x, y, tr.position.z);
    }
    public static Vector3Int ToSnapIndex(Vector3 worldPos, float step = 0.1f)
    {
        int x = (int)((worldPos.x + (step * 0.5f)) / step);
        int y = (int)((worldPos.y + (step * 0.5f)) / step);
        int z = (int)((worldPos.z + (step * 0.5f)) / step);
        return new Vector3Int(x, y, z);
    }
    public static bool RaycastScreenToWorld(Camera worldCam, Vector2 mouseScreenPos, int layerMask, out RaycastHit hit)
    {
        Ray ray = worldCam.ScreenPointToRay(mouseScreenPos);
        if(Physics.Raycast(ray, out hit, 20, layerMask))
        {
            return true;
        }
        return false;
    }

    public static bool RaycastFromTo(Vector3 start, Vector3 end, out RaycastHit hit, int layerMask)
    {
        Vector3 dir = end - start;
        Ray ray = new Ray(start, dir);
        return Physics.Raycast(ray, out hit, dir.magnitude, layerMask);
    }
    public static RaycastHit[] RaycastAllFromTo(Vector3 start, Vector3 end, int layerMask)
    {
        Vector3 dir = end - start;
        Ray ray = new Ray(start, dir);
        return Physics.RaycastAll(ray, dir.magnitude, layerMask);
    }
    public static bool RaycastFromTo(Vector3 start, Vector3 end, int layerMask)
    {
        Vector3 dir = end - start;
        Ray ray = new Ray(start, dir);
        return Physics.Raycast(ray, dir.magnitude, layerMask);
    }

    // 실제 객체 transform에 의해 변형된 collider 전방방향의 world좌표 반환
    public static Vector3 Center(this BoxCollider box)
    {
        return box.transform.TransformPoint(box.center);
    }
    public static Vector3 Forward(this BoxCollider box, float sizeRate = 1.0f)
    {
        Vector3 localForwardPos = box.center + new Vector3(box.size.x * 0.5f * sizeRate, 0, 0);
        return box.transform.TransformPoint(localForwardPos);
    }
    public static Vector3 Back(this BoxCollider box, float sizeRate = 1.0f)
    {
        Vector3 localForwardPos = box.center + new Vector3(-box.size.x * 0.5f * sizeRate, 0, 0);
        return box.transform.TransformPoint(localForwardPos);
    }
    public static Vector3 Up(this BoxCollider box, float sizeRate = 1.0f)
    {
        Vector3 localForwardPos = box.center + new Vector3(0, box.size.y * 0.5f * sizeRate, 0);
        return box.transform.TransformPoint(localForwardPos);
    }
    public static Vector3 Down(this BoxCollider box, float sizeRate = 1.0f)
    {
        Vector3 localForwardPos = box.center + new Vector3(0, -box.size.y * 0.5f * sizeRate, 0);
        return box.transform.TransformPoint(localForwardPos);
    }
    public static Vector3 ForwardUp(this BoxCollider box, float sizeRateX = 1.0f, float sizeRateY = 1.0f)
    {
        Vector3 localForwardPos = box.center + new Vector3(box.size.x * 0.5f * sizeRateX, box.size.y * 0.5f * sizeRateY, 0);
        return box.transform.TransformPoint(localForwardPos);
    }
    public static Vector3 ForwardDown(this BoxCollider box, float sizeRateX = 1.0f, float sizeRateY = 1.0f)
    {
        Vector3 localForwardPos = box.center + new Vector3(box.size.x * 0.5f * sizeRateX, -box.size.y * 0.5f * sizeRateY, 0);
        return box.transform.TransformPoint(localForwardPos);
    }
    public static Vector3 BackDown(this BoxCollider box, float sizeRateX = 1.0f, float sizeRateY = 1.0f)
    {
        Vector3 localForwardPos = box.center + new Vector3(-box.size.x * 0.5f * sizeRateX, -box.size.y * 0.5f * sizeRateY, 0);
        return box.transform.TransformPoint(localForwardPos);
    }
    public static Vector3 BackUp(this BoxCollider box, float sizeRateX = 1.0f, float sizeRateY = 1.0f)
    {
        Vector3 localForwardPos = box.center + new Vector3(-box.size.x * 0.5f * sizeRateX, box.size.y * 0.5f * sizeRateY, 0);
        return box.transform.TransformPoint(localForwardPos);
    }

    public static Bounds GetWorldBounds2D(this BoxCollider box)
    {
        Vector2 corner1 = box.ForwardUp();
        Vector2 corner2 = box.BackUp();
        Vector2 corner3 = box.ForwardDown();
        Vector2 corner4 = box.BackDown();
        float minX = MyUtils.Min(corner1.x, corner2.x, corner3.x, corner4.x);
        float minY = MyUtils.Min(corner1.y, corner2.y, corner3.y, corner4.y);
        float maxX = MyUtils.Max(corner1.x, corner2.x, corner3.x, corner4.x);
        float maxY = MyUtils.Max(corner1.y, corner2.y, corner3.y, corner4.y);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, box.size.z);
        Bounds bounds = new Bounds(box.Center(), size);
        return bounds;
    }
    public static Vector3 Right(this Bounds bound, float sizeRate = 1.0f)
    {
        return bound.center += (Vector3.right * bound.extents.x * sizeRate);
    }
    public static Vector3 Left(this Bounds bound, float sizeRate = 1.0f)
    {
        return bound.center += (Vector3.left * bound.extents.x * sizeRate);
    }
    public static Vector3 Top(this Bounds bound, float sizeRate = 1.0f)
    {
        return bound.center += (Vector3.up * bound.extents.y * sizeRate);
    }
    public static Vector3 Bottom(this Bounds bound, float sizeRate = 1.0f)
    {
        return bound.center += (Vector3.down * bound.extents.y * sizeRate);
    }
    public static Vector3 RightTop(this Bounds bound, float sizeRateX = 1.0f, float sizeRateY = 1.0f)
    {
        return bound.center += new Vector3(bound.extents.x * sizeRateX, bound.extents.y * sizeRateY, 0);
    }
    public static Vector3 LeftTop(this Bounds bound, float sizeRateX = 1.0f, float sizeRateY = 1.0f)
    {
        return bound.center += new Vector3(-bound.extents.x * sizeRateX, bound.extents.y * sizeRateY, 0);
    }
    public static Vector3 RightBottom(this Bounds bound, float sizeRateX = 1.0f, float sizeRateY = 1.0f)
    {
        return bound.center += new Vector3(bound.extents.x * sizeRateX, -bound.extents.y * sizeRateY, 0);
    }
    public static Vector3 LeftBottom(this Bounds bound, float sizeRateX = 1.0f, float sizeRateY = 1.0f)
    {
        return bound.center += new Vector3(-bound.extents.x * sizeRateX, -bound.extents.y * sizeRateY, 0);
    }

    public static bool IsCooltimeOver(long prevTicks, float cooltime)
    {
        double interval = new System.TimeSpan(System.DateTime.Now.Ticks - prevTicks).TotalSeconds;
        return interval > cooltime;
    }

#if UNITY_EDITOR

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
    
    // prefab으로부터 guid정보를 가져온다
    public static string GetGUIDFromPrefab(UnityEngine.GameObject prefab)
    {
        UnityEngine.Object oriPrefab = GetOriginPrefab(prefab);
        if(oriPrefab == null)
            return "";
            
        string path = AssetDatabase.GetAssetPath(oriPrefab);
        string guid = AssetDatabase.AssetPathToGUID(path);
        return guid;
    }
    
    // 실제 ProjectView에 존재하는 원본 prefab파일에 접근한다
	public static UnityEngine.Object GetOriginPrefab(UnityEngine.Object selection)
	{
		if (selection == null)
			return null;

        // IsPersistent는 디스크에 파일 형태로 존재하는지 여부 파악(오리지널 프리팹을 확인하는 방법)
        if(EditorUtility.IsPersistent(selection))
            return selection;

        PrefabAssetType type = PrefabUtility.GetPrefabAssetType(selection);
		if (type == PrefabAssetType.Regular)
		{
            // 대응되는 원본 프리팹을 참조한다
			UnityEngine.Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(selection);
			return prefab;
		}

        PrefabStage prefabStage = PrefabStageUtility.GetPrefabStage(selection as GameObject);
        if(prefabStage != null)
        {
            return prefabStage.prefabContentsRoot;
        }

		return null;

        // There are totally 3 distinct type/state of prefab. And they must be handled differently.
        // Prefab Asset (The one that sits in Asset folder)
        // Prefab Instance (The one that dragged into scene to become blue game object, or nested inside another prefab)
        // Prefab Stage (Editing prefab intermediate in UnityEditor prefab edit mode)

        // For example: from getting assetPath alone, these 3 cases will need different API
        // Prefab Asset => use UnityEditor.AssetDatabase.GetAssetPath()
        // Prefab Instance => PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot()
        // Prefab Stage => PrefabStageUtility.GetCurrentPrefabStage then editorPrefabStage.prefabAssetPath
        
        // Finding root of game object for each case also need different API
        // Prefab Asset => use gameObject.transform.root.gameObject()
        // Prefab Instance => PrefabUtility.GetNearestPrefabInstanceRoot()
        // Prefab Stage => PrefabStageUtility.GetCurrentPrefabStage then editorPrefabStage.prefabContentsRoot
        // (See deprecate message in old PrefabUtility.FindPrefabRoot function, it is very useful.)

        // Im not sure which case you are on.
        // But for PrefabUitility API example, you could look into my demo on https://github.com/wappenull/UnityPrefabTester
        // It is editor script demo.
	}
    // guid값에서 실제 Asset 리소스 객체 가져옴
    public static string GetGUIDFromAsset(UnityEngine.Object asset)
    {
        // asset이 prefabView 모드나 scene에 instantiate된 객체일 경우 assetPath를 반환하지 않는다.
        // asset이 리소스일 경우만 동작
        string path = AssetDatabase.GetAssetPath(asset);
        string guid = AssetDatabase.AssetPathToGUID(path);
        return guid;
    }
    public static long GUIDToLong(string guid)
    {
        long halfA = guid.Substring(0, guid.Length / 2).GetHashCode();
        long halfB = guid.Substring(guid.Length / 2, guid.Length / 2).GetHashCode();
        long newID = halfA | (halfB << 32);
        newID &= ~((long)1 << 63);
        return newID;
    }
    

    public static string[] GetStateNames(this Animator animator, int layerIndex)
    {
        List<string> names = new List<string>();
        AnimatorController ac = animator.runtimeAnimatorController as AnimatorController;
        AnimatorStateMachine sm = ac.layers[layerIndex].stateMachine;
        ChildAnimatorState[] states = sm.states;
        foreach (ChildAnimatorState s in states)
        {
            names.Add(s.state.name);
        }
        return names.ToArray();
    }
    
#endif


    public static int Min(params int[] values)
    {
        int retVal = int.MaxValue;
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] < retVal)
                retVal = values[i];
        }
        return retVal;
    }
    public static int Max(params int[] values)
    {
        int retVal = int.MinValue;
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] > retVal)
                retVal = values[i];
        }
        return retVal;
    }
    public static float Min(params float[] values)
    {
        float retVal = float.MaxValue;
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] < retVal)
                retVal = values[i];
        }
        return retVal;
    }
    public static float Max(params float[] values)
    {
        float retVal = float.MinValue;
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] > retVal)
                retVal = values[i];
        }
        return retVal;
    }

    // 특정 타겟을 바라보는 방향으로 천천히 방향 전환, 유도탄 같은 궤적에서 사용(rotate 회전 lerp toward) - 3D용
    // Quaternion기반, 순수 Vector3 회전 기반, 2D기반(x,y축사용)
    public static IEnumerator CoRotateTowardsLerp(Transform me, Transform target, float rotateSpeed)
    {
        Vector3 lastTargetPos = Vector3.zero;
        while (true)
        {
            Vector3 curTargetPos = (target == null) ? lastTargetPos : target.position;
            Vector3 targetDirection = curTargetPos - me.transform.position;
            lastTargetPos = curTargetPos;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            me.transform.rotation = Quaternion.RotateTowards(me.transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
            yield return null;
        }
    }
    public static IEnumerator CoRotateTowardsVectorLerp(Transform me, Transform target, float rotateSpeed)
    {
        Vector3 lastTargetPos = Vector3.zero;
        while (true)
        {
            Vector3 curTargetPos = (target == null) ? lastTargetPos : target.position;
            Vector3 targetDirection = curTargetPos - me.transform.position;
            lastTargetPos = curTargetPos;
            float singleStep = rotateSpeed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(me.transform.forward, targetDirection, singleStep, 0.0f);
            Debug.DrawRay(me.transform.position, newDirection, Color.red);
            me.transform.rotation = Quaternion.LookRotation(newDirection);
            yield return null;
        }
    }

    public static IEnumerator CoRotateTowards2DLerp(Transform me, Transform target, float rotateSpeed, Vector3 offfset)
    {
        Vector3 lastTargetPos = Vector3.zero;
        while (true)
        {
            Vector3 curTargetPos = (target == null) ? lastTargetPos : (target.position + offfset);
            Vector3 targetDirection = curTargetPos - me.transform.position;
            lastTargetPos = curTargetPos;
            float singleStep = rotateSpeed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(me.transform.right, targetDirection, singleStep, 0.0f);

            Debug.DrawRay(me.transform.position, newDirection, Color.red);

            float degree = Vector3.SignedAngle(newDirection, Vector3.right, Vector3.back);
            me.transform.rotation = Quaternion.Euler(0, 0, degree);
            yield return null;
            rotateSpeed += 0.1f;// 시간이 지남에 따라 더빠르게 방향을 튼다(무한 회전 방지)
        }
    }
    // return : 0 ~ 180 degree
    public static float GetDegree(Vector3 dirA, Vector3 dirB)
    {
        return Vector3.Angle(dirA, dirB);
    }

    public static Vector3 Reflect(Vector3 enterVector, Vector3 normal)
    {
        return Vector3.Reflect(enterVector, normal);
    }
    public static bool IsOutOfRange<T>(this T[] list, int index)
    {
        return index < 0 || index >= list.Length;
    }
    public static bool IsOutOfRange<T>(this List<T> list, int index)
    {
        return index < 0 || index >= list.Count;
    }
    public static void SetMinimum(this ref int val, int minValue)
    {
        val = Mathf.Max(val, minValue);
        }
    public static void SetMaximum(this ref int val, int maxValue)
    {
        val = Mathf.Min(val, maxValue);
    }

    public static void FindChildAll(this Transform parent, string name, List<Transform> rets)
    {
        foreach(Transform child in parent)
        {
            if(child.name.Equals(name))
                rets.Add(child);
            
            if(child.childCount > 0)
                child.FindChildAll(name, rets);
        }
    }
    public static Vector3 RotateVector(Vector3 vec, Vector3 axis, float degree)
    {
        return Quaternion.AngleAxis(degree, axis.normalized) * vec;
    }
    public static Vector3[] CalcMultiDirections(Vector3 centerDir, Vector3 axis, int count, float stepDegree)
    {
        centerDir.Normalize();
        axis.Normalize();

        if (count <= 1)
            return new Vector3[1] { centerDir };

        List<Vector3> rets = new List<Vector3>();
        float totalDeg = stepDegree * (count - 1);
        Vector3 startDir = RotateVector(centerDir, axis, -totalDeg * 0.5f);
        rets.Add(startDir);
        for (int i = 1; i < count; ++i)
        {
            Vector3 dir = RotateVector(startDir, axis, stepDegree * i);
            rets.Add(dir);
        }
        return rets.ToArray();
    }
    public static bool IsCloseEnough(Vector3 posA, Vector3 posB, float distance)
    {
        return (posA - posB).sqrMagnitude <= (distance * distance);
    }
}

// MyUtils End =================================================





