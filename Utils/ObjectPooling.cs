using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : SingletonMono<ObjectPooling>
{
    // 요청한 객체가 부족할때 추가 할당해주는 객체의 개수
    private const int AllocCount = 10;
    // 나중에 객체가 반환될때 해당 객체가 어느 풀링그룹인지 확인할때 사용
    private const string PoolingGroup = "PoolingGroup";

    private Dictionary<string, Transform> mObjectPool = new Dictionary<string, Transform>();

    public GameObject Instantiate(string resourcesPath)
    {
        // 요청한 객체가 할당 가능하지 확인하고 불가능 하면 추가로 객체 생성한다.
        if(!IsAllocable(resourcesPath))
        {
            if(!AllocObjects(resourcesPath))
                return null;
        }

        // Pool에서 실제 요청한 객체 하나를 빼와서 반환해준다
        Transform obj = mObjectPool[resourcesPath].GetChild(0);
        obj.SetParent(null);
        obj.gameObject.SetActive(true);
        return obj.gameObject;
    }

    public GameObject Instantiate(string resourcesPath, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject obj = Instantiate(resourcesPath);
        if(obj == null)
        {
            return null;
        }
        
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.transform.SetParent(parent);
        return obj;
    }

    public GameObject InstantiateVFX(string prefabNameInVFXPath, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if(prefabNameInVFXPath.Length <= 0)
            return null;

        return Instantiate(Consts.VFXPath + prefabNameInVFXPath, position, rotation, parent);
    }

    // 요청한 객체가 할당 가능한지 여부 확인
    private bool IsAllocable(string path)
    {
        if(mObjectPool.ContainsKey(path))
        {
            return mObjectPool[path].childCount > 0;
        }
        return false;
    }
    // 요청한 객체가 Pool에 없을경우 10개의 객체를 미리 생성
    private bool AllocObjects(string resourcesPath)
    {
        // 프리팹 로딩
        GameObject prefab = ResourcesCache.Load<GameObject>(resourcesPath);
        if (prefab == null)
        {
            LOG.warn();
            return false;
        }

        // 최초 요청시에는 그룹 관리를 위한 부모 객체 생성
        if(!mObjectPool.ContainsKey(resourcesPath))
        {
            GameObject newParent = new GameObject();
            newParent.transform.SetParent(transform);
            newParent.name = resourcesPath;
            mObjectPool[resourcesPath] = newParent.transform;
        }

        // 그룹 즉 부모 객체 하위에 풀링할 실제 객체를 10개 미리 생성해 놓는다
        Transform parentTr = mObjectPool[resourcesPath];
        for (int i = 0; i < AllocCount; ++i)
        {
            GameObject newObj = GameObject.Instantiate(prefab, parentTr);
            newObj.gameObject.SetActive(false);
            newObj.SetValue(PoolingGroup, resourcesPath);
        }
        return true;
    }

    public void DestroyReturn(GameObject obj)
    {
        // 어느 그룹쪽으로 반환해야할지 정보를 가져옴
        object groupID = obj.GetValue(PoolingGroup);
        if(groupID == null)
        {
            //풀링 대상이 아니므로 그냥 삭제
            LOG.warn();
            Destroy(obj);
        }

        // 다 사용한 객체는 재활용을 위해 다시 Pool에 넣어준다
        string resourcesPath = groupID as string;
        Transform parentTr = mObjectPool[resourcesPath];
        obj.transform.SetParent(parentTr);
        obj.SetActive(false);
    }

    public void DestroyReturnAfter(GameObject obj, float delay)
    {
        if(delay <= 0)
        {
            DestroyReturn(obj);
        }
        else
        {
            this.ExDelayedCoroutine(delay, () => DestroyReturn(obj));
        }
    }

    // 인자로 받은 객체가 프리팹상태인지 아니면 게임상에 존재하는 object인지 확인
    private bool IsInstantiated(GameObject obj)
    {
        return obj.scene.rootCount > 0;
    }
}

public static class ObjectPoolingHelper
{
    public static GameObject ReturnAfter(this GameObject obj, float delay = 0)
    {
        if(obj == null) return null;
        ObjectPooling.Instance.DestroyReturnAfter(obj, delay);
        return obj;
    }
}