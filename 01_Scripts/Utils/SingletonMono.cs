using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전역 싱글톤으로 사용할 MonoBehaviour 가 있다면 이 클래스를 상속하여 사용
/// 선언예) public class SomethingManager : SingletonMono<SomethingManager> {}
/// 사용법) SomethingManager.Inst.DoSomething();
/// </summary>

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T mInstance;
    public static T Instance
    { 
        get
        {
            if (mInstance == null)
            {
                //Scene상에 이미 매니저가 존재하고 있을 경우 참조(참고로 Disable된 상태의 객체는 검색 안됨)
                mInstance = FindObjectOfType<T>();

                if (mInstance == null)
                {
                    //Prefab상태로 있을경우 로딩후 객체화 하여 참조
                    T[] prefabs = Resources.LoadAll<T>("");
                    if (prefabs != null && prefabs.Length > 0)
                        mInstance = Instantiate(prefabs[0]);
                }

                if (mInstance == null)
                {
                    //Scene상에 없을경우 객채 생성 후 참조
                    mInstance = new GameObject().AddComponent<T>();
                    mInstance.name = typeof(T).Name;
                }

                DontDestroyOnLoad(mInstance.gameObject);
            }
            return mInstance;
        }
    }

    protected virtual void Awake()
    {
        T current = this as T;
        if(mInstance == null)
        {
            mInstance = current;
            DontDestroyOnLoad(gameObject);
        }
        else if(mInstance != current)
        {
            // 싱글톤 객체가 두번 생성되었으니 해제
            Destroy(gameObject);
        }
    }

}
