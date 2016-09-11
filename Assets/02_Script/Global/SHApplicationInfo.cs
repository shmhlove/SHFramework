﻿using UnityEngine;
using UnityEngine.EventSystems;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

public partial class SHApplicationInfo : SHSingleton<SHApplicationInfo>
{
    // 앱 종료 여부
    public bool                 m_bIsAppQuit        = false;

    // 배포제한 시간정보
    public SHReleaseTimer       m_pReleaseTime      = new SHReleaseTimer();

    // 컴포넌트(디버그) : 디버그용 정보출력 
    public GUIText              m_pText             = null;

    // 기타(디버그) : FPS 출력용 델타타임
    private float               m_fDeltaTime        = 0.0f;

    // 기타(디버그) : 로드 시도된 리소스 리스트
    private Dictionary<eSceneType, List<string>> m_dicRealLoadInfo = new Dictionary<eSceneType, List<string>>();

    // 다양화 : 생성시
    public override void OnInitialize()
    {
        SetDontDestroy();

        // 어플리케이션 정보설정
        SetApplicationInfo();

        // 디버그 기능
        StartCoroutine(PrintGameInfo());
        StartCoroutine(CheckReleaseTime());
    }

    // 인터페이스 : 어플리케이션 정보설정
    public void SetApplicationInfo()
    {
        var pClientInfo = Single.Table.GetTable<JsonClientConfiguration>();
        SetVSync(pClientInfo.GetVSyncCount());
        SetFrameRate(pClientInfo.GetFrameRate());
        SetCacheInfo(pClientInfo.GetCacheSize(), 30);
        SetSleepMode();
        SetCrittercism();
    }

    // 다양화 : 제거시
    public override void OnFinalize() { }

    // 시스템 : App Quit
    void OnApplicationQuit()
    {
        m_bIsAppQuit = true;
    }

    // 시스템 : App Pause
    void OnApplicationPause(bool bIsPause)
    {
    }

    // 시스템 : App Focus
    eBOOL m_eIsFocus = eBOOL.None;
    void OnApplicationFocus(bool bIsFocus)
    {
        // 초기 실행으로 인해 Focus가 true일때는 체크 무시
        if (m_eIsFocus == eBOOL.None)
        {
            m_eIsFocus = bIsFocus ? eBOOL.True : eBOOL.False;
            return;
        }

        // Focus가 true일때 아래 기능 동작할 수 있도록
        if (eBOOL.True != (m_eIsFocus = bIsFocus ? eBOOL.True : eBOOL.False))
            return;

        // 서비스상태 체크 후 RunGame이 아니면 인트로로 보낸다.
        CheckServiceState((eResult) => 
        {
            if (eServiceState.Run != eResult)
                Single.Scene.GoTo(eSceneType.Intro);
        });
    }

    // 시스템 : Net Disconnect
    void OnDisconnectedFromServer(NetworkDisconnection pInfo)
    {

    }

    // 시스템 : 업데이트
    void Update()
    {
        m_fDeltaTime += (Time.deltaTime - m_fDeltaTime) * 0.1f;
    }

    // 유틸 : VSync 설정
    void SetVSync(int iCount)
    {
        QualitySettings.vSyncCount = iCount;
    }

    // 유틸 : 프레임 레이트 설정
    void SetFrameRate(int iFrame)
    {
        Application.targetFrameRate = iFrame;
    }
    public int GetFrameRate()
    {
        return Application.targetFrameRate;
    }

    // 유틸 : SleepMode 설정
    void SetSleepMode()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    // 유틸 : 캐시크기 및 완료기간 설정
    void SetCacheInfo(long lSizeMB, int iExpirationMonth)
    {
        Caching.maximumAvailableDiskSpace   = lSizeMB * 1024 * 1024;
        Caching.expirationDelay             = 60 * 60 * 24 * iExpirationMonth;
    }

    // 유틸 : 크래시 래포트 설정
    void SetCrittercism()
    {
#if UNITY_ANDROID
        UnityEngine.Debug.LogFormat("Crittercism.DidCrashOnLastLoad = {0}", CrittercismAndroid.DidCrashOnLastLoad());
        CrittercismAndroid.Init("20fb64bf760d44589b6aefeb6bcb220700555300");
        CrittercismAndroid.SetLogUnhandledExceptionAsCrash(true);
#elif UNITY_IPHONE || UNITY_IOS
        UnityEngine.Debug.LogFormat("Crittercism.DidCrashOnLastLoad = {0}", CrittercismIOS.DidCrashOnLastLoad());
        CrittercismIOS.Init("7d02af2372694b93b84d75a999dd7dd400555300");
        CrittercismIOS.SetLogUnhandledExceptionAsCrash(true);
#endif
    }

    // 인터페이스 : 화면 회전모드 확인
    public bool IsLandscape()
    {
        return ((true == IsEditorMode()) ||
                (Screen.orientation == ScreenOrientation.Landscape) ||
                (Screen.orientation == ScreenOrientation.LandscapeLeft) ||
                (Screen.orientation == ScreenOrientation.LandscapeRight));
    }

    // 인터페이스 : 에디터 모드 체크
    public bool IsEditorMode()
    {
        return ((Application.platform == RuntimePlatform.WindowsEditor) ||
                (Application.platform == RuntimePlatform.OSXEditor));
    }

    // 유틸 : 해상도 비율값
    int GetRatioW(int iValue)
    {
        return (int)(iValue * (Screen.width / 1280.0f));
    }
    int GetRatioH(int iValue)
    {
        return (int)(iValue * (Screen.height / 720.0f));
    }

    // 인터페이스 : 실행 플랫폼
    public RuntimePlatform GetRuntimePlatform()
    {
        return Application.platform;
    }
    public string GetStrToRuntimePlatform()
    {
        return SHHard.GetStrToPlatform(GetRuntimePlatform());
    }

    // 인터페이스 : 현재 서비스 상태 체크
    public void CheckServiceState(Action<eServiceState> pCallback)
    {
        Single.Table.DownloadServerConfiguration(() => 
        {
            if (null == pCallback)
                return;

            pCallback(GetServiceState());
        });
    }

    public eServiceState GetServiceState()
    {
        var eState = Single.Table.GetServiceState();
        if (eServiceState.None == eState)
            return eServiceState.ConnectMarket;
        else
            return eState;
    }

    void OnGUI()
    {
        DrawAppInformation();
        ControlRenderFrame();
    }

    #region 에디터 테스트
    // 디버그 : 실시간 로드 리소스 리스트 파일로 출력
    [SHAttributeToShowFunc]
    public void SaveLoadResourceList()
    {
        string strBuff = string.Empty;
        foreach (var kvp in m_dicRealLoadInfo)
        {
            strBuff += string.Format("Scene : {0}\n", kvp.Key);
            foreach (var pInfo in kvp.Value)
            {
                strBuff += string.Format("\t{0}\n", pInfo);
            }
        }

        string strSavePath = string.Format("{0}/{1}", SHPath.GetPathToAssets(), "RealTimeLoadResource.txt");
        SHUtil.SaveFile(strBuff, strSavePath);
        System.Diagnostics.Process.Start(strSavePath);
    }
    [SHAttributeToShowFunc]
    public void ClearLoadResourceList()
    {
        m_dicRealLoadInfo.Clear();
    }
    #endregion

    #region 디버그 로그
    // 디버그 : 앱정보 출력
    void DrawAppInformation()
    {
        var pServerInfo = Single.Table.GetTable<JsonServerConfiguration>();
        if (false == pServerInfo.IsLoadTable())
            return;

        GUIStyle pStyle = new GUIStyle(GUI.skin.box);
        pStyle.fontSize = GetRatioW(20);

        GUI.Box(new Rect(0, (Screen.height - GetRatioH(30)), GetRatioW(250), GetRatioH(30)),
            string.Format("{0} : {1}", Single.Table.GetServiceMode(), GetRuntimePlatform()), pStyle);

        GUI.Box(new Rect((Screen.width * 0.5f) - (GetRatioW(120) * 0.5f), (Screen.height - GetRatioH(30)), GetRatioW(120), GetRatioH(30)),
            string.Format("Ver {0}", Single.Table.GetClientVersion()), pStyle);
    }

    // 디버그 : 렌더 프레임 제어
    void ControlRenderFrame()
    {
        //if (true == GUI.Button(new Rect(GetRatioW(150), 0, GetRatioW(150), GetRatioH(50)), string.Format("Up RenderFrame : {0}", GetFrameRate())))
        //    SetFrameRate(GetFrameRate() + 1);
        //if (true == GUI.Button(new Rect(GetRatioW(150), GetRatioH(50), GetRatioW(150), GetRatioH(50)), string.Format("Down RenderFrame : {0}", GetFrameRate())))
        //    SetFrameRate(GetFrameRate() - 1);
    }

    // 디버그 : 게임정보 출력
    IEnumerator PrintGameInfo()
    {
        if (null == m_pText)
            yield break;

        yield return new WaitForSeconds(1.0f);

        Profiler.BeginSample("CheckMemory");

        float fMemory       = Profiler.GetTotalAllocatedMemory() / 1024.0f / 1024.0f;
        m_pText.text        = string.Format("UsedMemory : {0:F2}MB\nFPS : {1:F2}", fMemory, (1.0f / m_fDeltaTime));
        m_pText.fontSize    = GetRatioW(20);
        m_pText.pixelOffset = new Vector2(0.0f, Screen.height * 0.7f);

        Profiler.EndSample();
        StartCoroutine(PrintGameInfo());
    }

    // 디버그 : 배포제한시간 체크
    IEnumerator CheckReleaseTime()
    {
        if (true == IsEditorMode())
            yield break;

        yield return new WaitForSeconds(1.0f);

        if (true == Single.Timer.IsPastTimeToLocal(m_pReleaseTime))
            SHUtil.GameQuit();
        else
            StartCoroutine(CheckReleaseTime());
    }

    // 디버그 : 실시간 로드 리소스 리스트
    public void SetLoadResource(string strInfo)
    {
        if (false == m_dicRealLoadInfo.ContainsKey(Single.Scene.GetActiveScene()))
            m_dicRealLoadInfo.Add(Single.Scene.GetActiveScene(), new List<string>());

        //// 콜스택 남기기
        //strInfo += string.Format("\n< CallStack >\n{0}", SHUtil.GetCallStack());

        m_dicRealLoadInfo[Single.Scene.GetActiveScene()].Add(strInfo);
    }
    #endregion
}