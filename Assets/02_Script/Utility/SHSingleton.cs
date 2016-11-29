using UnityEngine;

using System;
using System.Collections.Generic;

public static class Single
{
    // 데이터
    public static SHDataManager             Data                { get { return SHDataManager.Instance; } }
    public static SHResourceData            Resource            { get { return Data.ResourcesData; } }
    public static SHAssetBundleData         AssetBundle         { get { return Data.AssetBundleData; } }
    public static SHTableData               Table               { get { return Data.TableData; } }
    public static SHUserData                User                { get { return Data.UserData; } }
    
    // 씬
    public static SHSceneManager            Scene               { get { return SHSceneManager.Instance; } }

    // UI
    public static SHUIManager               UI                  { get { return SHUIManager.Instance; } }
    
    // 사운드
    
    // 렌더유틸
    public static SHRenderTextureManager    RenderTexture       { get { return SHRenderTextureManager.Instance; } }

    // 유틸리티
    public static SHApplicationInfo         AppInfo             { get { return SHApplicationInfo.Instance; } }
    public static SHEventUtil               Event               { get { return SHEventUtil.Instance; } }
    public static SHCoroutine               Coroutine           { get { return SHCoroutine.Instance; } }
    public static SHTimer                   Timer               { get { return SHTimer.Instance; } }
}

public abstract class SHSingleton<T> : SHMonoWrapper where T : SHSingleton<T>
{
    #region Members
    private static T        m_pInstance     = null;
    public static T         Instance        { get { return GetInstance(); } }
    public static bool      IsExists        { get { return (null != m_pInstance); } }
    #endregion


    #region Virtual Functions
    // 다양화 : 초기화( 게임오브젝트에 붙은경우 Awake시, 직접 생성인 경우 Instance에 접근하는 순간 호출 됨 )
    public abstract void OnInitialize();

    // 다양화 : 종료( DontDestory가 설정된경우 어플이 종료될때, 아닌 경우에는 씬이 변경될때, 혹은 DoDestory로 명시적으로 제거할때 호출 됨 )
    public abstract void OnFinalize();
    #endregion


    #region System Functions
    // 시스템 : 생성(하이어라키에 올라간 싱글턴)
    public override void Awake()
    {
        Initialize(this as T);
    }

    // 시스템 : 제거
    public void OnDestroy()
    {
        Destroyed();
    }

    // 시스템 : 어플종료
    private void OnApplicationQuit()
    {
        Destroyed();
    }
    #endregion


    #region Interface Functions
    // 인터페이스 : 객체얻기
    private static object m_pLocker = new object();
    public static T GetInstance()
    {
        lock (m_pLocker)
        {
            if (null == m_pInstance)
            {
                if (null == (m_pInstance = SHGameObject.FindObjectOfType<T>()))
                    Initialize(SHGameObject.CreateEmptyObject(typeof(T).ToString()).AddComponent<T>());
            }

            return m_pInstance;
        }
    }

    // 인터페이스 : 아무런 동작없이 객체만 생성시키기
    public void CreateSingleton() { }

    // 인터페이스 : 씬이 제거되어도 싱글턴을 제거하지 않습니다.
    public void SetDontDestroy()
    {
        if (null == m_pInstance)
            return;

#if UNITY_EDITOR
        if (false == Application.isPlaying)
            return;
#endif

        DontDestroyOnLoad(m_pInstance.SetParent("SHSingletons(DontDestroy)"));
    }

    // 인터페이스 : 명시적으로 싱글턴 제거
    public void DoDestroy()
    {
        SHGameObject.DestoryObject(gameObject);
    }
    #endregion


    #region Utility Functions
    // 유틸 : 싱글턴 제거
    void Destroyed()
    {
        if (null == m_pInstance)
            return;

        m_pInstance.OnFinalize();
        m_pInstance = null;
    }

    // 유틸 : 객체 초기화
    static void Initialize(T pInstance)
    {
        if (null == pInstance)
            return;

        // 싱글턴 생성시 Awake에서 호출되고, Instance에서 호출되므로 같으면 무시
        if ((null != m_pInstance) && (m_pInstance == pInstance))
            return;

        // 인스턴스 중복체크
        T pDuplication = SHGameObject.GetDuplication(pInstance);
        if (null != pDuplication)
        {
            m_pInstance = pDuplication;
            SHGameObject.DestoryObject(pInstance.gameObject);
            return;
        }

        m_pInstance = pInstance;
        m_pInstance.SetParent("SHSingletons(Destroy)");
        m_pInstance.OnInitialize();
    }

    // 유틸 : 싱글턴 부모설정
    GameObject SetParent(string strRootName)
    {
        return SHGameObject.SetParent(gameObject, strRootName);
    }
    #endregion
}