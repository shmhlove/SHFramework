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

    // �پ�ȭ : �ʱ�ȭ
    public override void OnInitialize() { }
    // �پ�ȭ : ������
    public override void OnFinalize()
    {
        foreach(var kvp in m_dicBundles)
        {
            kvp.Value.m_pBundle.Unload(false);
        }
        m_dicBundles.Clear();
    }
    // �پ�ȭ : ������Ʈ
    public override void FrameMove() { }

    // �پ�ȭ : �ε��� ������ ����Ʈ �˷��ֱ�
    public override Dictionary<string, SHLoadData> GetLoadList(eSceneType eType)
    {
        return new Dictionary<string, SHLoadData>();;
    }

    // �پ�ȭ : ��ġ�� ������ ����Ʈ �˷��ֱ�
    public override Dictionary<string, SHLoadData> GetPatchList()
    {
        var dicLoadList = new Dictionary<string, SHLoadData>();
        
        // ������������(ServerConfiguration.json)�� URL�� ������ ��ġ���� �ʴ´�.
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

    // �پ�ȭ : �δ��� ���� ȣ��� �ε��Լ�
    public override void Load(SHLoadData pInfo, Action<string, SHLoadStartInfo> pStart, 
                                                Action<string, SHLoadEndInfo> pDone)
    {
    }

    // �پ�ȭ : �δ��� ���� ȣ��� ��ġ�Լ�( ���� ��ġ�Ϸ��� ����� �ɰ��̹Ƿ� �ǹ̾��� )
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

    // ��ƿ : �����߰�
    void AddBundleData(string strBundleName, AssetBundle pBundle)
    {
        SHAssetBundle pBundleData   = new SHAssetBundle();
        pBundleData.m_pBundleName   = strBundleName;
        pBundleData.m_pBundle       = pBundle;
        m_dicBundles[strBundleName] = pBundleData;
    }

    // �������̽� : �ε����� �����
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

    // �������̽� : ������ �ִ°�?
    public bool IsExist(string strBundleName)
    {
        return m_dicBundles.ContainsKey(strBundleName);
    }

    // �������̽� : ���� ������ ���
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