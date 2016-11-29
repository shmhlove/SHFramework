using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class SHReleaseTimer
{
    [Range(2016, 2999)] public int iYear    = 2999;
    [Range(1, 12)]      public int iMonth   = 12;
    [Range(1, 31)]      public int iDay     = 31;
    [Range(1, 24)]      public int i24Hour  = 23;
}

public class SHTimer : SHSingleton<SHTimer>
{
    #region Members
    Dictionary<string, DateTime> m_dicDeltaTimer = new Dictionary<string, DateTime>();
    #endregion


    #region Virtual Functions
    public override void OnInitialize()
    {
        SetDontDestroy();
    }

    public override void OnFinalize()
    {

    }
    #endregion


    #region 델타타임 기능
    public void StartDeltaTime(string strKey)
    {
        if (false == m_dicDeltaTimer.ContainsKey(strKey))
            m_dicDeltaTimer.Add(strKey, DateTime.Now);

        m_dicDeltaTimer[strKey] = DateTime.Now;
    }

    public DateTime GetDeltaTime(string strKey)
    {
        if (false == m_dicDeltaTimer.ContainsKey(strKey))
            StartDeltaTime(strKey);

        return m_dicDeltaTimer[strKey];
    }

    public float GetDeltaTimeToSecond(string strKey)
    {
        return ((float)(DateTime.Now - GetDeltaTime(strKey)).TotalMilliseconds / 1000.0f);
    }
    #endregion


    #region 배포제한 기능
    //----------------------------------------------------------------------------
    // 타임서버 사용.
    // 현재 타임서버 사용은 타임서버가 많이 느린 문제로 사용 지양하는 게 좋을 듯
    //public bool IsPastTimeToServer(CKReleaseTimer pTime)
    //{
    //    return IsPastTime(GetNowTimeFromTimeServer(), pTime);
    //}

    // 로컬타임 사용.
    public bool IsPastTimeToLocal(SHReleaseTimer pTime)
    {
        return IsPastTime(DateTime.Now, pTime);
    }

    public bool IsPastTime(DateTime pNowTime, SHReleaseTimer pTime)
    {
        if (null == pTime)
            return false;

        // 년체크
        if (pTime.iYear < pNowTime.Year)
            return true;
        if (pTime.iYear > pNowTime.Year)
            return false;

        // 월체크
        if (pTime.iMonth < pNowTime.Month)
            return true;
        if (pTime.iMonth > pNowTime.Month)
            return false;

        // 일체크
        if (pTime.iDay < pNowTime.Day)
            return true;
        if (pTime.iDay > pNowTime.Day)
            return false;

        // 시간체크
        if (pTime.i24Hour <= pNowTime.Hour)
            return true;
        if (pTime.i24Hour > pNowTime.Hour)
            return false;

        return false;
    }

    // private InternetTime.SNTPClient m_cSNTPClient = null;
    // public DateTime GetNowTimeFromTimeServer()
    // {
    //     if (null == m_cSNTPClient)
    //     {
    //         try
    //         {
    //             m_cSNTPClient = new InternetTime.SNTPClient("time.nuri.net");
    //             m_cSNTPClient.Connect(false);
    //         }
    //         catch (Exception e)
    //         {
    //             Debug.LogError("ERROR : {0}", e.Message);
    //             return new DateTime();
    //         }
    // 
    //         Debug.Log(m_cSNTPClient.ToString());
    //     }
    // 
    //     return m_cSNTPClient.DestinationTimestamp;
    // }
    #endregion
}
