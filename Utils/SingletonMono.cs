using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전역 싱글톤으로 사용할 MonoBehaviour 가 있다면 이 클래스를 상속하여 사용
/// 선언예) public class SomethingManager : SingletonMono<SomethingManager> {}
/// 사용법) SomethingManager.Inst.DoSomething();
/// </summary>

public class SingletonMono<T> : MonoBehaviour
{
    public static T Inst { get; private set; }

    void Awake()
    {
        if(Inst == null)
        {
            Inst = GetComponent<T>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 싱글톤 객체가 두번 생성되었으니 해제
            Destroy(gameObject);
        }
    }

}
