using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : new()
{
    private static T mInstance;
    
    public static T Instance 
    {
        get 
        {
            if (mInstance == null)
            {
                mInstance = new T();
            }
            return mInstance;
        }
    }
}
