using System;
using UnityEngine;

/// <summary>
/// Resources폴더에서 동적으로 프리팹 로딩올때 해당 에셋 캐쉬
/// </summary>

static public class ResourcesCache
{
    static private Dictionary<string, UnityEngine.Object> mObjects = new Dictionary<string, UnityEngine.Object>();

    static public T Load<T>(string fullPath) where T : UnityEngine.Object
    {
        // 캐시여부 확인하여 이전에 가져온적이 있다면 캐싱
        if(mObjects.ContainsKey(fullPath))
        {
            return mObjects[fullPath] as T;
        }
        else
        {
            // 이전에 가져온적이 없으면 Resources폴더에서 로딩해온다.
            T asset = Resources.Load<T>(fullPath);
            if(asset == null)
            {
                Debug.LogError("No Asset : " + fullPath);
                return null;
            }

            mObjects[fullPath] = asset;
            return asset;
        }
    }
}