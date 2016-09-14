using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using SimpleJSON;

public class AssetBundleInfo
{
    #region Value Members
    public string                                   m_strBundleName = string.Empty;
    public long                                     m_lBundleSize   = 0;
    public Hash128                                  m_pHash128;
    public Dictionary<string, SHResourcesTableInfo> m_dicResources  = new Dictionary<string, SHResourcesTableInfo>();
    #endregion


    #region System Functions
    public AssetBundleInfo() { }
    public AssetBundleInfo(AssetBundleInfo pCopy)
    {
        m_strBundleName = pCopy.m_strBundleName;
        m_lBundleSize   = pCopy.m_lBundleSize;
        m_pHash128      = pCopy.m_pHash128;
        m_dicResources  = new Dictionary<string, SHResourcesTableInfo>(pCopy.m_dicResources);
    }
    #endregion


    #region Interface Functions
    public void AddResourceInfo(SHResourcesTableInfo pInfo)
    {
        if (null == pInfo)
            return;

        m_dicResources[pInfo.m_strName] = pInfo;
    }

    public void DelResourceInfo(string strResourceName)
    {
        if (false == IsIncludeResource(strResourceName))
            return;

        m_dicResources.Remove(strResourceName);
    }

    public void CopyResourceInfo(Dictionary<string, SHResourcesTableInfo> dicCopy)
    {
        m_dicResources = new Dictionary<string, SHResourcesTableInfo>(dicCopy);
    }

    public SHResourcesTableInfo GetResourceInfo(string strResourceName)
    {
        if (false == IsIncludeResource(strResourceName))
            return null;

        return m_dicResources[strResourceName];
    }

    public bool IsIncludeResource(string strResourceName)
    {
        return m_dicResources.ContainsKey(strResourceName);
    }
    #endregion
}

public class JsonAssetBundleInfo : SHBaseTable
{
    #region Value Members
    Dictionary<string, AssetBundleInfo> m_pData = new Dictionary<string, AssetBundleInfo>();
    #endregion


    #region System Functions
    public JsonAssetBundleInfo()
    {
        m_strFileName = "AssetBundleInfo";
    }
    #endregion


    #region Virtual Functions
    public override void Initialize()
    {
        m_pData.Clear();
    }

    public override bool IsLoadTable()
    {
        return (0 != m_pData.Count);
    }

    public override bool? LoadJsonTable(JSONNode pJson, string strFileName)
    {
        if (null == pJson)
            return false;

        int iMaxTable = pJson["AssetBundleInfo"].Count;
        for (int iLoop = 0; iLoop < iMaxTable; ++iLoop)
        {
            JSONNode           pDataNode = pJson["AssetBundleInfo"][iLoop];
            AssetBundleInfo    pData     = new AssetBundleInfo();
            pData.m_strBundleName        = GetStrToJson(pDataNode, "s_BundleName");
            pData.m_lBundleSize          = (long)GetIntToJson(pDataNode, "s_BundleSize");
            pData.m_pHash128             = Hash128.Parse(GetStrToJson(pDataNode, "s_BundleHash"));
            
            int iMaxUnit = pDataNode["p_Resources"].Count;
            for(int iLoopUnit = 0; iLoopUnit < iMaxUnit; ++iLoopUnit)
            {
                JSONNode pUnitNode = pDataNode["p_Resources"][iLoopUnit];
                SHResourcesTableInfo pUnit = new SHResourcesTableInfo();
                pUnit.m_strName             = GetStrToJson(pUnitNode, "s_Name");
                pUnit.m_strFileName         = GetStrToJson(pUnitNode, "s_FileName");
                pUnit.m_strExtension        = GetStrToJson(pUnitNode, "s_Extension");
                pUnit.m_strSize             = GetStrToJson(pUnitNode, "s_Size");
                //pUnit.m_strLastWriteTime    = GetStrToJson(pUnitNode, "s_LastWriteTime");
                pUnit.m_strHash             = GetStrToJson(pUnitNode, "s_Hash");
                pUnit.m_strPath             = GetStrToJson(pUnitNode, "s_Path");
                pUnit.m_eResourceType       = SHHard.GetResourceTypeToExtension(pUnit.m_strExtension);

                pData.AddResourceInfo(pUnit);
            }

            AddData(pData.m_strBundleName, pData);
        }

        return true;
    }
    #endregion


    #region Interface Functions
    // 인터페이스 : 정보파일 다운로드
    public void DownloadByCDN(Action pComplate)
    {
        // 서버정보파일(ServerConfiguration.json)에 URL이 없으면 패치하지 않는다.
        if (true == string.IsNullOrEmpty(SHPath.GetURLToBundleCDN()))
        {
            pComplate();
            return;
        }

        Single.Coroutine.WWW((pWWW) =>
        {
            if (true == string.IsNullOrEmpty(pWWW.error))
            {
                SHJson pJson = new SHJson();
                pJson.SetJsonNode(pJson.GetJsonParseToByte(pWWW.bytes));
                LoadJsonTable(pJson.Node, m_strFileName);
                pComplate();
            }
            else
            {
                Debug.LogErrorFormat("Error!!! Download AssetBundleInfo.json : (Error : {0}, URL : {1}", pWWW.error, pWWW.url);
            }

        }, new WWW(string.Format("{0}/{1}.json", SHPath.GetURLToBundleCDNWithPlatform(), m_strFileName)));
    }

    // 인터페이스 : 컨테이너 얻기
    public override ICollection GetData()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        return m_pData;
    }

    // 인터페이스 : 컨테이너 얻기
    public Dictionary<string, AssetBundleInfo> GetContainer()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        return m_pData;
    }

    // 인터페이스 : 번들이름으로 정보얻기
    public AssetBundleInfo GetBundleInfo(string strBundleName)
    {
        if (false == m_pData.ContainsKey(strBundleName.ToLower()))
            return null;

        return m_pData[strBundleName.ToLower()];
    }

    // 인터페이스 : 리소스 이름으로 번들정보 얻기
    public AssetBundleInfo GetBundleInfoToResourceName(string strResourceName)
    {
        foreach(var kvp in m_pData)
        {
            if (true == kvp.Value.IsIncludeResource(strResourceName))
                return kvp.Value;
        }

        return null;
    }

    // 인터페이스 : 번들이름과 리소스 이름으로 리소스 정보얻기
    public SHResourcesTableInfo GetResourceInfo(string strBundleName, string strResourceName)
    {
        if (false == m_pData.ContainsKey(strBundleName))
            return null;
        if (false == m_pData[strBundleName].IsIncludeResource(strResourceName))
            return null;

        return m_pData[strBundleName].GetResourceInfo(strResourceName);
    }
    
    // 인터페이스 : 번들 리스트 얻기( 리소스 테이블과 비교 : 번들파일크기, 리소스 파일크기/해시코드 )
    public Dictionary<string, AssetBundleInfo> GetBundleListToCompare(JsonResourcesTable pResourceInfo)
    {
        var dicBundleInfo = GetContainer();
        if (null == pResourceInfo)
            return dicBundleInfo;

        var pResult = new Dictionary<string, AssetBundleInfo>();
        foreach (var kvpForBundle in dicBundleInfo)
        {
            // 체크 : 번들파일크기
            if (0 == kvpForBundle.Value.m_lBundleSize)
            {
                pResult.Add(kvpForBundle.Key, kvpForBundle.Value);
                continue;
            }

            // 체크 : 추가/제거/변경된 리소스(존재확인/파일크기/해시코드)
            foreach (var kvpForRes in kvpForBundle.Value.m_dicResources)
            {
                SHResourcesTableInfo pResourceUnit = pResourceInfo.GetResouceInfo(kvpForRes.Value.m_strName);

                // 제거된 리소스 확인
                if (null == pResourceUnit)
                {
                    AssetBundleInfo pCopyInfo = new AssetBundleInfo(kvpForBundle.Value);
                    pCopyInfo.DelResourceInfo(kvpForRes.Value.m_strName);
                    pResult.Add(kvpForBundle.Key, pCopyInfo);
                    break;
                }

                // 추가 및 변경된 리소스 확인
                if ((pResourceUnit.m_strSize != kvpForRes.Value.m_strSize) ||
                    (pResourceUnit.m_strHash != kvpForRes.Value.m_strHash))
                {
                    pResult.Add(kvpForBundle.Key, kvpForBundle.Value);
                    break;
                }
            }
        }

        return pResult;
    }

#if UNITY_EDITOR
    // 인터페이스 : 번들정보 업데이트( Streaming기준으로 추가/변경/제거된 번들목록을 갱신한다. )
    public Dictionary<string, AssetBundleInfo> UpdateAssetBundlesMakeInfoByStreamingPath(string strCDN, BuildTarget eTarget)
    {
        // Download 경로
        string strDownloadPath = string.Format("{0}/{1}/{2}.json", strCDN, SHHard.GetStrToPlatform(eTarget), m_strFileName);
        
        // CDN에 있는 AssetBundleInfo.Json 다운로드
        JsonAssetBundleInfo pCDNInfo = new JsonAssetBundleInfo();
        pCDNInfo.LoadJsonTable((new SHJson()).LoadWWW(strDownloadPath), m_strFileName);
        
        // 로컬 Streaming에 저장된 AssetBundleInfo 로드
        JsonAssetBundleInfo pStreamingInfo = new JsonAssetBundleInfo();
        pStreamingInfo.LoadJsonTable((new SHJson()).LoadToStreamingForLocal(m_strFileName), m_strFileName);
        
        // StreamingPath기준으로 목록 갱신
        foreach (var kvpToStreaming in pStreamingInfo.GetContainer())
        {
            var pCDNBundleInfo = pCDNInfo.GetBundleInfo(kvpToStreaming.Key);

            // 추가 : 번들자체가 CDN에는 없고, Streaming에는 있는 경우
            if (null == pCDNBundleInfo)
            {
                kvpToStreaming.Value.m_lBundleSize  = 0;
                kvpToStreaming.Value.m_pHash128     = Hash128.Parse("0");
                continue;
            }

            // 변경 : 번들자체가 CDN과 Streaming이 다르다면 비교할 수 있게 CDN내용을 복사
            kvpToStreaming.Value.m_lBundleSize = pCDNBundleInfo.m_lBundleSize;
            kvpToStreaming.Value.m_pHash128    = pCDNBundleInfo.m_pHash128;

            // 리소스 업데이트 체크
            foreach (var kvpToStreamingRes in kvpToStreaming.Value.m_dicResources)
            {
                var pCDNResInfo = pCDNBundleInfo.GetResourceInfo(kvpToStreamingRes.Key);

                // 추가 : 리소스가 CDN에는 없고, Streaming에는 있는 경우
                if (null == pCDNResInfo)
                {
                    kvpToStreamingRes.Value.m_strSize = "0";
                    kvpToStreamingRes.Value.m_strHash = "0";
                }
                // 변경 : 리소스가 CDN과 Streaming이 다르다면 비교할 수 있게 CDN내용을 복사
                else
                {
                    kvpToStreamingRes.Value.CopyTo(pCDNResInfo);
                }
            }

            // 추가 : 리소스가 CDN에는 있고, Streaming 에는 없는 경우 비교할 수 있게 CDN내용 추가
            foreach(var kvpToCDNRes in pCDNBundleInfo.m_dicResources)
            {
                if (true == kvpToStreaming.Value.IsIncludeResource(kvpToCDNRes.Key))
                    continue;

                kvpToStreaming.Value.AddResourceInfo(kvpToCDNRes.Value);
            }
        }

        return pStreamingInfo.GetContainer();
    }
#endif

    // 인터페이스 : Dic데이터를 Json파일로 저장
    public void SaveJsonFileByDic(string strSaveFilePath)
    {
        SaveJsonFileByDic(m_pData, strSaveFilePath);
    }
    public void SaveJsonFileByDic(Dictionary<string, AssetBundleInfo> dicData, string strSaveFilePath)
    {
        SHResourcesLister.SaveToAssetBundleInfo(dicData, strSaveFilePath);
    }
    #endregion


    #region Utility Functions
    // 유틸 : 데이터 하나 저장
    void AddData(string strKey, AssetBundleInfo pData)
    {
        m_pData[strKey.ToLower()] = pData;
    }

    // 유틸 : 데이터 덮기
    public void SetData(Dictionary<string, AssetBundleInfo> pData)
    {
        m_pData = new Dictionary<string, AssetBundleInfo>(pData);
    }
    #endregion
}