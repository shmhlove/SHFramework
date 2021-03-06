using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using SimpleJSON;

public class SHResourcesTableInfo
{
    #region Members
    public string           m_strName;             // 확장자가 없는 이름
    public string           m_strFileName;         // 확장자가 있는 이름
    public string           m_strExtension;        // 확장자
    public string           m_strSize;             // 파일크기
    //public string           m_strLastWriteTime;    // 마지막 수정날짜(리스팅 할때마다 변경되어서 그냥 제거시킴)
    public string           m_strHash;             // 해시
    public string           m_strPath;             // Resources폴더 이하 경로
    public eResourceType    m_eResourceType;       // 리소스 타입
    #endregion


    #region Interface Functions
    public void CopyTo(SHResourcesTableInfo pData)
    {
        if (null == pData)
            return;

        m_strName             = pData.m_strName;
        m_strFileName         = pData.m_strFileName;
        m_strExtension        = pData.m_strExtension;
        m_strSize             = pData.m_strSize;
        //m_strLastWriteTime  = pData.m_strLastWriteTime;
        m_strHash             = pData.m_strHash;
        m_strPath             = pData.m_strPath;
        m_eResourceType       = pData.m_eResourceType;
    }
    #endregion
}

public class JsonResourcesTable : SHBaseTable
{
    #region Members
    Dictionary<string, SHResourcesTableInfo> m_pData = new Dictionary<string, SHResourcesTableInfo>();
    #endregion


    #region System Functions
    public JsonResourcesTable()
    {
        m_strFileName = "ResourcesTable";
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

        int iMaxTable = pJson["ResourcesList"].Count;
        for (int iLoop = 0; iLoop < iMaxTable; ++iLoop)
        {
            JSONNode pDataNode         = pJson["ResourcesList"][iLoop];
            SHResourcesTableInfo pData = new SHResourcesTableInfo();
            pData.m_strName             = GetStrToJson(pDataNode, "s_Name");
            pData.m_strFileName         = GetStrToJson(pDataNode, "s_FileName");
            pData.m_strExtension        = GetStrToJson(pDataNode, "s_Extension");
            pData.m_strSize             = GetStrToJson(pDataNode, "s_Size");
            //pData.m_strLastWriteTime    = GetStrToJson(pDataNode, "s_LastWriteTime");
            pData.m_strHash             = GetStrToJson(pDataNode, "s_Hash");
            pData.m_strPath             = GetStrToJson(pDataNode, "s_Path");
            pData.m_eResourceType       = SHHard.GetResourceTypeToExtension(pData.m_strExtension);

            AddResources(pData.m_strName, pData);
        }

        return true;
    }

    public override ICollection GetData()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        return m_pData;
    }
    #endregion


    #region Interface Functions
    // 인터페이스 : 파일명으로 리소스 정보얻기
    public SHResourcesTableInfo GetResouceInfo(string strName)
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        strName = strName.ToLower().Trim();
        if (false == m_pData.ContainsKey(strName))
            return null;

        return m_pData[strName];
    }
    
    // 인터페이스 : 파일명으로 리소스 경로 얻기
    public string GetResoucesPath(string strName)
    {
        SHResourcesTableInfo pInfo = GetResouceInfo(strName);
        if (null == pInfo)
            return string.Empty;
    
        return pInfo.m_strPath;
    }
    
    // 인터페이스 : 타입에 해당하는 리소스 정보 리스트 얻기
    public List<SHResourcesTableInfo> GetResourceInfoByType(eResourceType eType)
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        var pList = new List<SHResourcesTableInfo>();
        SHUtils.ForToDic(m_pData, (pKey, pValue) =>
        {
            if (eType == pValue.m_eResourceType)
                pList.Add(pValue);
        });
        
        return pList;
    }

    // 인터페이스 : 리소스 정보 체크
    public bool IsContain(string strName)
    {
        return (null != GetResouceInfo(strName));
    }
    #endregion


    #region Utility Functions
    void AddResources(string strKey, SHResourcesTableInfo pData)
    {
        m_pData[strKey.ToLower().Trim()] = pData;
    }
    #endregion
}