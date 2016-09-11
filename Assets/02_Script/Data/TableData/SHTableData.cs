using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public partial class SHTableData : SHBaseData
{
    // 로컬 테이블 리스트
    private Dictionary<Type, SHBaseTable> m_dicTables = new Dictionary<Type, SHBaseTable>();
    public Dictionary<Type, SHBaseTable> Tables { get { return m_dicTables; } }

    // 다양화 : 초기화
    public override void OnInitialize()
    {
        m_dicTables.Clear();

        m_dicTables.Add(typeof(JsonClientConfiguration),          new JsonClientConfiguration());
        m_dicTables.Add(typeof(JsonServerConfiguration),          new JsonServerConfiguration());
        m_dicTables.Add(typeof(JsonPreLoadResourcesTable),        new JsonPreLoadResourcesTable());
        m_dicTables.Add(typeof(JsonResourcesTable),               new JsonResourcesTable());
        m_dicTables.Add(typeof(JsonAssetBundleInfo),              new JsonAssetBundleInfo());
    }

    // 다양화 : 마무리
    public override void OnFinalize()
    {
        m_dicTables.Clear();
    }

    // 다양화 : 업데이트
    public override void FrameMove()
    {
    }

    // 다양화 : 로드할 데이터 리스트 알려주기
    public override Dictionary<string, SHLoadData> GetLoadList(eSceneType eType)
    {
        var dicLoadList = new Dictionary<string, SHLoadData>();

        // 로컬 테이블 데이터
        SHUtil.ForeachToDic<Type, SHBaseTable>(m_dicTables,
        (pKey, pValue) =>
        {
            // 이미 로드된 데이터인지 체크
            if (true == pValue.IsLoadTable())
                return;

            dicLoadList.Add(pValue.m_strFileName, CreateLoadInfo(pValue.m_strFileName));
        });

        return dicLoadList; 
    }
    
    // 인터페이스 : 로드정보 만들기
    public SHLoadData CreateLoadInfo(string strName)
    {
        return new SHLoadData()
        {
            m_eDataType = eDataType.LocalTable,
            m_strName   = strName,
            m_pLoadFunc = Load
        };
    }
    
    // 다양화 : 패치할 데이터 리스트 알려주기
    public override Dictionary<string, SHLoadData> GetPatchList()
    {
        return new Dictionary<string, SHLoadData>();
    }

    // 다양화 : 로더로 부터 호출될 로드함수
    public override void Load(SHLoadData pInfo, Action<string, SHLoadStartInfo> pStart,
                                                Action<string, SHLoadEndInfo> pDone)
    {
        SHBaseTable pTable = GetTable(pInfo.m_strName);
        if (null == pTable)
        {
            Debug.LogError(string.Format("[TableData] 등록된 테이블이 아닙니다.!!({0})", pInfo.m_strName));
            pDone(pInfo.m_strName, new SHLoadEndInfo(false, eLoadErrorCode.Load_Table));
            return;
        }

        var pLoadOrder = GetLoadOrder(pTable);
        foreach(var pLambda in pLoadOrder)
        {
            bool? bIsSuccess = pLambda();
            if (null != bIsSuccess)
            {
                if (true == bIsSuccess.Value)
                    pDone(pInfo.m_strName, new SHLoadEndInfo(true, eLoadErrorCode.None));
                else
                    pDone(pInfo.m_strName, new SHLoadEndInfo(true, eLoadErrorCode.Load_Table));
                return;
            }
        }

        pDone(pInfo.m_strName, new SHLoadEndInfo(false, eLoadErrorCode.Load_Table));
    }

    // 다양화 : 로더로 부터 호출될 패치함수
    public override void Patch(SHLoadData pInfo, Action<string, SHLoadStartInfo> pStart,
                                                 Action<string, SHLoadEndInfo> pDone)
    { 
    }

    // 유틸 : 테이블 타입별 로드 순서 ( 앞선 타입의 로드에 성공하면 뒤 타입들은 로드명령 하지 않는다 )
    List<Func<bool?>> GetLoadOrder(SHBaseTable pTable)
    {
        var pLoadOrder = new List<Func<bool?>>();
        //if (true == Single.AppInfo.IsEditorMode())
        //{
        //    pLoadOrder.Add(() => { return pTable.LoadStatic();                        });
        //    pLoadOrder.Add(() => { return pTable.LoadXML(pTable.m_strFileName);       });
        //    pLoadOrder.Add(() => { return pTable.LoadBytes(pTable.m_strByteFileName); });
        //    pLoadOrder.Add(() => { return pTable.LoadJson(pTable.m_strFileName);      });
        //    pLoadOrder.Add(() => { return pTable.LoadDB(pTable.m_strFileName);        });
        //}
        //else
        {
            pLoadOrder.Add(() => { return pTable.LoadStatic();                        });
            pLoadOrder.Add(() => { return pTable.LoadBytes(pTable.m_strByteFileName); });
            pLoadOrder.Add(() => { return pTable.LoadXML(pTable.m_strFileName);       });
            pLoadOrder.Add(() => { return pTable.LoadJson(pTable.m_strFileName);      });
            pLoadOrder.Add(() => { return pTable.LoadDB(pTable.m_strFileName);        });
        }

        return pLoadOrder;
    }

    // 인터페이스 : 테이블 얻기
    public T GetTable<T>() where T : SHBaseTable
    {
        return GetTable(typeof(T)) as T;
    }
    public SHBaseTable GetTable(Type pType)
    {
        if (false == m_dicTables.ContainsKey(pType))
            return null;

        return m_dicTables[pType];
    }
    public SHBaseTable GetTable(string strFileName)
    {
        if (true == string.IsNullOrEmpty(strFileName))
            return null;

        return GetTable(GetTypeToFileName(strFileName));
    }

    // 인터페이스 : 데이터 얻기
    public ICollection GetData<T>()
    {
        return GetData(typeof(T));
    }
    public ICollection GetData(string strClassType)
    {
        return GetData(Type.GetType(strClassType));
    }
    public ICollection GetData(Type pType)
    {
        SHBaseTable pTable = GetTable(pType);
        if (null == pTable)
            return null;

        return pTable.GetData();
    }

    // 인터페이스 : 테이블 파일이름으로 클래스타입얻기
    public Type GetTypeToFileName(string strFileName)
    {
        strFileName = Path.GetFileNameWithoutExtension(strFileName);
        foreach (var kvp in m_dicTables)
        {
            if (true == kvp.Value.m_strFileName.Equals(strFileName))
                return kvp.Key;
        }

        return null;
    }
}