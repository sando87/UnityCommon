using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newWaitForSeconds
{
    private static Dictionary<int, YieldInstruction> mTable = new Dictionary<int, YieldInstruction>();
    
    // new WaitforSeconds 객체를 매번 생성하지않고 재사용하도록 관리한다.
    public static YieldInstruction Cache(float second)
    {
        int sec = (int)(second * 1000.0f); // 정밀도는 1ms단위까지 관리한다.
        if (mTable.ContainsKey(sec))
        {
            return mTable[sec];
        }
        mTable[sec] = new WaitForSeconds(second);
        return mTable[sec];
    }
}