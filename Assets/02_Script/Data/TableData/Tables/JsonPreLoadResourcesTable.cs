using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using SimpleJSON;

public class JsonPreLoadResourcesTable : SHBaseTable
{
    #region Value Members
    Dictionary<eSceneType, List<string>> m_pData = new Dictionary<eSceneType, List<string>>();
    #endregion


    #region System Functions
    public JsonPreLoadResourcesTable()
    {
        m_strFileName = "PreLoadResourcesTable";
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

        int iMaxNodeCount = pJson["PreLoadResourcesList"].Count;
        for (int iNodeLoop = 0; iNodeLoop < iMaxNodeCount; ++iNodeLoop)
        {
            JSONNode pDataNode = pJson["PreLoadResourcesList"][iNodeLoop];
            foreach (eSceneType eType in Enum.GetValues(typeof(eSceneType)))
            {
                string strSceneType = eType.ToString();
                int iMaxNode        = pDataNode[strSceneType].Count;
                for (int iDataLoop = 0; iDataLoop < iMaxNode; ++iDataLoop)
                {
                    AddData(eType, pDataNode[strSceneType][iDataLoop].Value);
                }
            }
        }

        return true;
    }
    #endregion


    #region Interface Functions
    public override ICollection GetData()
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        return m_pData;
    }
    public List<string> GetData(eSceneType eType)
    {
        if (false == IsLoadTable())
            LoadJson(m_strFileName);

        if (false == m_pData.ContainsKey(eType))
            return new List<string>();

        return m_pData[eType];
    }
    #endregion


    #region Utility Functions
    void AddData(eSceneType eType, string strData)
    {
        if (false == m_pData.ContainsKey(eType))
            m_pData.Add(eType, new List<string>());

        strData = strData.ToLower();
        m_pData[eType].Add(strData);
    }
    #endregion
}