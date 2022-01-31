using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : new()
{
    private static T mInst;
    
    public static T Inst 
    {
        get 
        {
            if (mInst == null)
            {
                mInst = new T();
            }
            return mInst;
        }
    }
}
