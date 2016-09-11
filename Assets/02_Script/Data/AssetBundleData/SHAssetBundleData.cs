using UnityEngine;

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class SHAssetBundle
{
    public string      m_pBundleName = string.Empty;
    public AssetBundle m_pBundle     = null;
}

public partial class SHAssetBundleData : SHBaseData
{
    public Dictionary<string, SHAssetBundle> m_dicBundles = new Dictionary<string, SHAssetBundle>();

    // 다양화 : 초기화
    public override void OnInitialize() { }
    // 다양화 : 마무리
    public override void OnFinalize()
    {
        foreach(var kvp in m_dicBundles)
        {
            kvp.Value.m_pBundle.Unload(false);
        }
        m_dicBundles.Clear();
    }
    // 다양화 : 업데이트
    public override void FrameMove() { }

    // 다양화 : 로드할 데이터 리스트 알려주기
    public override Dictionary<string, SHLoadData> GetLoadList(eSceneType eType)
    {
        return new Dictionary<string, SHLoadData>();;
    }

    // 다양화 : 패치할 데이터 리스트 알려주기
    public override Dictionary<string, SHLoadData> GetPatchList()
    {
        var dicLoadList = new Dictionary<string, SHLoadData>();
        
        // 서버정보파일(ServerConfiguration.json)에 URL이 없으면 패치하지 않는다.
        if (true == string.IsNullOrEmpty(SHPath.GetURLToBundleCDN()))
            return dicLoadList;

        var dicBundles  = Single.Table.GetAssetBundleInfo();
        foreach (var kvp in dicBundles)
        {
            if (true == IsExist(kvp.Key))
                continue;

            dicLoadList.Add(kvp.Key, CreatePatchInfo(kvp.Value));
        }

        return dicLoadList;
    }

    // 다양화 : 로더로 부터 호출될 로드함수
    public override void Load(SHLoadData pInfo, Action<string, SHLoadStartInfo> pStart, 
                                                Action<string, SHLoadEndInfo> pDone)
    {
    }

    // 다양화 : 로더로 부터 호출될 패치함수( 씬은 패치하려면 번들로 될것이므로 의미없음 )
    public override void Patch(SHLoadData pInfo, Action<string, SHLoadStartInfo> pStart,
                                                 Action<string, SHLoadEndInfo> pDone)
    {
        if (true == IsExist(pInfo.m_strName))
        {
            pStart(pInfo.m_strName, new SHLoadStartInfo());
            pDone(pInfo.m_strName, new SHLoadEndInfo(true, eLoadErrorCode.None));
            return;
        }

        WWW pAsync = Single.Coroutine.WWW((pWWW) =>
        {
            bool bIsSuccess = string.IsNullOrEmpty(pWWW.error);
            if (true == bIsSuccess)
            {
                AddBundleData(pInfo.m_strName, pWWW.assetBundle);
                pDone(pInfo.m_strName, new SHLoadEndInfo(true, eLoadErrorCode.None));
            }
            else
            {
                pDone(pInfo.m_strName, new SHLoadEndInfo(false, eLoadErrorCode.Patch_Bundle));
            }
            
        }, WWW.LoadFromCacheOrDownload(string.Format("{0}/{1}.unity3d", SHPath.GetURLToBundleCDNWithPlatform(), pInfo.m_strName.ToLower()),
                                       Single.Table.GetAssetBundleInfo(pInfo.m_strName).m_pHash128));

        pStart(pInfo.m_strName, new SHLoadStartInfo(pAsync));
    }

    // 유틸 : 번들추가
    void AddBundleData(string strBundleName, AssetBundle pBundle)
    {
        SHAssetBundle pBundleData   = new SHAssetBundle();
        pBundleData.m_pBundleName   = strBundleName;
        pBundleData.m_pBundle       = pBundle;
        m_dicBundles[strBundleName] = pBundleData;
    }

    // 인터페이스 : 로드정보 만들기
    public SHLoadData CreatePatchInfo(AssetBundleInfo pInfo)
    {
        return new SHLoadData()
        {
            m_eDataType = eDataType.BundleData,
            m_strName   = pInfo.m_strBundleName,
            m_pLoadFunc = Patch,
            m_pTriggerLoadCall = () =>
            {
                return Caching.ready;
            },
        };
    }

    // 인터페이스 : 번들이 있는가?
    public bool IsExist(string strBundleName)
    {
        return m_dicBundles.ContainsKey(strBundleName);
    }

    // 인터페이스 : 번들 데이터 얻기
    public SHAssetBundle GetBundleData(AssetBundleInfo pInfo)
    {
        if (null == pInfo)
            return null;

        if (false == IsExist(pInfo.m_strBundleName))
            return null;

        return m_dicBundles[pInfo.m_strBundleName];
    }
    public SHAssetBundle GetBundleData(string strBundleName)
    {
        if (false == IsExist(strBundleName))
            return null;

        return m_dicBundles[strBundleName];
    }
}