using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using SimpleJSON;

public class JsonClientConfiguration : SHBaseTable
{
    public string        m_strConfigurationCDN  = string.Empty;
    public string        m_strServiceMode       = string.Empty;
    public string        m_strVersion           = string.Empty;
    public int           m_iVSyncCount          = 0;
    public int           m_iFrameRate           = 60;
    public int           m_iCacheSize           = 200;

    public JsonClientConfiguration()
    {
        m_strFileName = "ClientConfiguration";
    }

    public override void Initialize()
    {
        m_strConfigurationCDN   = string.Empty;
        m_strServiceMode        = string.Empty;
        m_strVersion            = string.Empty;
        m_iVSyncCount           = 0;
        m_iFrameRate            = 60;
        m_iCacheSize            = 200;
    }

    public override bool IsLoadTable()
    {
        return (false == string.IsNullOrEmpty(m_strVersion));
    }

    public override bool? LoadJsonTable(JSONNode pJson, string strFileName)
    {
        if (null == pJson)
            return false;

        JSONNode pDataNode      = pJson["ClientConfiguration"];

        m_strConfigurationCDN   = GetStrToJson(pDataNode, "ServerConfigurationCDN");
        m_strServiceMode        = GetStrToJson(pDataNode, "ServiceMode");
        m_strVersion            = GetStrToJson(pDataNode, "Version");
        m_iVSyncCount           = GetIntToJson(pDataNode, "VSyncCount");
        m_iFrameRate            = GetIntToJson(pDataNode, "FrameRate");
        m_iCacheSize            = GetIntToJson(pDataNode, "CacheSize(MB)");
        
        return true;
    }

    // 인터페이스 : 정보를 Json파일로 저장
    public void SaveJsonFile(string strSavePath)
    {
        string strNewLine = "\r\n";
        string strBuff = "{" + strNewLine;

        strBuff += string.Format("\t\"{0}\": {1}", "ClientConfiguration", strNewLine);
        strBuff += "\t{" + strNewLine;
        {
            strBuff += string.Format("\t\t\"ServerConfigurationCDN\": \"{0}\",{1}",
                m_strConfigurationCDN,
                strNewLine);

            strBuff += string.Format("\t\t\"ServiceMode\": \"{0}\",{1}",
                m_strServiceMode,
                strNewLine);

            strBuff += string.Format("\t\t\"Version\": \"{0}\",{1}",
                m_strVersion,
                strNewLine);

            strBuff += string.Format("\t\t\"VSyncCount\": {0},{1}",
                m_iVSyncCount,
                strNewLine);

            strBuff += string.Format("\t\t\"FrameRate\": {0},{1}",
                m_iFrameRate,
                strNewLine);

            strBuff += string.Format("\t\t\"CacheSize(MB)\": {0}{1}",
                m_iCacheSize,
                strNewLine);
        }
        strBuff += "\t}";
        strBuff += string.Format("{0}", strNewLine);
        strBuff += "}";

        // 저장
        SHUtil.SaveFile(strBuff, string.Format("{0}/{1}.json", strSavePath, m_strFileName));
    }

    // 인터페이스 : ConfigurationCDN URL얻기
    public string GetConfigurationCDN()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        return m_strConfigurationCDN;
    }

    // 인터페이스 : 서비스 모드 얻기
    public string GetServiceMode()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        return m_strServiceMode;
    }

    // 인터페이스 : 클라이언트 버전얻기
    public string GetVersion()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);
        
        return m_strVersion;
    }
    public int GetVersionToOrder(eOrderNum eOrder)
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        string [] strSplit = m_strFileName.Split(new char[]{'.'});
        if ((int)eOrder > strSplit.Length)
            return 0;

        return int.Parse(strSplit[((int)eOrder) - 1]);
    }

    // 인터페이스 : VSync 카운트 얻기
    public int GetVSyncCount()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        return m_iVSyncCount;
    }

    // 인터페이스 : 프레임 레이트 얻기
    public int GetFrameRate()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        return m_iFrameRate;
    }

    // 인터페이스 : 캐쉬크기 얻기
    public int GetCacheSize()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        return m_iCacheSize;
    }
}