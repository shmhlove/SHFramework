﻿using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHSceneHistory
{
    public eSceneType m_eTo;
    public eSceneType m_eFrom;

    public SHSceneHistory(eSceneType eTo, eSceneType eFrom)
    {
        m_eTo   = eTo;
        m_eFrom = eFrom;
    }
}

public class SHSceneManager : SHSingleton<SHSceneManager>
{
    // 씬 상태
    private eSceneType      m_eCurrentScene   = eSceneType.None;
    private eSceneType      m_eBeforeScene    = eSceneType.None;

    // 히스토리
    public List<SHSceneHistory> m_pHistory    = new List<SHSceneHistory>();

    // 이벤트
    private List<Action<eSceneType, eSceneType>> m_pEventToChangeScene = 
        new List<Action<eSceneType, eSceneType>>();

    // 다양화 : 초기화
    public override void OnInitialize()
    {
        SetDontDestroy();
    }

    // 다양화 : 종료
    public override void OnFinalize() { }

    // 인터페이스 : 씬 이동
    public void GoTo(eSceneType eChange)
    {
        // 알리아싱
        eSceneType eCurrent = GetCurrentScene();

        // 씬 변경시 처리해야 할 여러가지 작업들
        PerformanceToChangeScene(eCurrent, eChange);

        // 로드명령
		ExcuteGoTo(eChange); 
    }

    // 유틸 : 씬 이동 실행
    void ExcuteGoTo(eSceneType eType)
    {
        if (true == IsNeedLoading(eType))
            LoadScene(eSceneType.Loading, null); 
        else
            LoadScene(eType, null);
    }

    // 인터페이스 : 씬 로드 ( Add 방식 : SceneData 클래스에서 호출됨 )
    public AsyncOperation AddScene(string strSceneName, Action<bool> pComplate)
    {
        return SetLoadPostProcess(pComplate,
            SceneManager.LoadSceneAsync(strSceneName, LoadSceneMode.Additive));
    }

    // 인터페이스 : 씬 로드 ( Change 방식 : GoTo 명령시 호출됨 )
    public AsyncOperation LoadScene(eSceneType eType, Action<bool> pComplate)
    {
        return SetLoadPostProcess(pComplate,
            SceneManager.LoadSceneAsync(eType.ToString(), LoadSceneMode.Single));
    }

    // 유틸 : 씬 로드 후 처리를 위한 코루틴 등록
    AsyncOperation SetLoadPostProcess(Action<bool> pComplate, AsyncOperation pAsyncInfo)
    {
        Single.Coroutine.Async((bIsSuccess) =>
        {
            if (false == bIsSuccess)
                Debug.LogError(string.Format("씬 로드 실패!!(SceneType : {0})", GetCurrentScene()));

            if (null != pComplate)
                pComplate(bIsSuccess);
        },
        pAsyncInfo);

        return pAsyncInfo;
    }

    // 인터페이스 : 현재 씬 얻기
    public eSceneType GetCurrentScene()
    {
        if (eSceneType.None == m_eCurrentScene)
            return GetActiveScene();
        
        return m_eCurrentScene;
    }

    // 인터페이스 : 현재 씬 얻기
    public eSceneType GetActiveScene()
    {
        return SHHard.GetSceneTypeToString(SceneManager.GetActiveScene().name);
    }

    // 인터페이스 : 이전 씬 얻기
    public eSceneType GetBeforeScene()
    {
        return m_eBeforeScene;
    }

    // 인터페이스 : 현재 씬 인가?
    public bool IsCurrentScene(eSceneType eType)
    {
        return (GetCurrentScene() == eType);
    }

    // 인터페이스 : 이전 씬 인가?
    public bool IsBeforeScene(eSceneType eType)
    {
        int iLastIndex = m_pHistory.Count - 1;
        if (0 > iLastIndex)
            return false;

        return (m_pHistory[iLastIndex].m_eTo == eType);
    }

    // 인터페이스 : X씬을 거친적이 있는가?
    public bool IsPassedScene(eSceneType eType)
    {
        foreach(SHSceneHistory pHistory in m_pHistory)
        {
            if (eType == pHistory.m_eFrom)
                return true;
        }

        return false;
    }

    // 인터페이스 : 로딩이 필요한씬
    public bool IsNeedLoading(eSceneType eType)
    {
        return false;
    }

    // 인터페이스 : 콜백등록
    public void AddEventToChangeScene(Action<eSceneType, eSceneType> pAction)
    {
        if (null == pAction)
            return;

        if (true == IsAddEvent(pAction))
            return;

        m_pEventToChangeScene.Add(pAction);
    }

    // 인터페이스 : 콜백제거
    public void DelEventToChangeScene(Action<eSceneType, eSceneType> pAction)
    {
        if (false == IsAddEvent(pAction))
            return;

        m_pEventToChangeScene.Remove(pAction);
    }

    // 유틸 : 등록된 콜백인가?
    bool IsAddEvent(Action<eSceneType, eSceneType> pAction)
    {
        return m_pEventToChangeScene.Contains(pAction);
    }

    // 유틸 : 씬 변경될때 알려달라고 한 곳에 알려주자
    void SendCallback(eSceneType eCurrent, eSceneType eChange)
    {
        foreach (var pAction in m_pEventToChangeScene)
        {
            if (null == pAction)
                continue;

            pAction(eCurrent, eChange);
        }
    }

    // 유틸 : 씬 변경시 처리해야할 하드한 작업들
    void PerformanceToChangeScene(eSceneType eCurrent, eSceneType eChange)
    {
        // 전역 코루틴 모두 제거
        Single.Coroutine.DoDestroy();

        // 씬 변경 이벤트 콜
        SendCallback(eCurrent, eChange);

        // 히스토리 남기기
        SetHistory(eChange);
    }

    // 유틸 : 씬 변경 히스토리 기록
    void SetHistory(eSceneType eType)
    {
        m_pHistory.Add(new SHSceneHistory(m_eCurrentScene, eType));
        m_eBeforeScene  = m_eCurrentScene;
        m_eCurrentScene = eType;
    }
}