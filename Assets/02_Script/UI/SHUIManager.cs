using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using DicPanels = System.Collections.Generic.Dictionary<string, SHUIBasePanel>;

public partial class SHUIManager : SHSingleton<SHUIManager>
{
    #region Members
    [ReadOnlyField][SerializeField] private DicPanels m_dicPanels = new DicPanels();
    #endregion

    #region Virtual Functions
    public override void OnInitialize()
    {
        SetDontDestroy();
        Single.Scene.AddEventToChangeScene(OnEventToChangeScene);
    }
    public override void OnFinalize()
    {
        
    }
    #endregion


    #region System Functions
    #endregion


    #region Interface Functions
    public void AddPanel(SHUIBasePanel pPanel)
    {
        if (null == pPanel)
        {
            Debug.LogError("AddPanel() - Panel Is null!!");
            return;
        }

        if (true == m_dicPanels.ContainsKey(pPanel.name))
            m_dicPanels[pPanel.name] = pPanel;
        else
            m_dicPanels.Add(pPanel.name, pPanel);
    }
    public void Show(string strName, params object[] pArgs)
    {
        var pPanel = GetPanel(strName);
        if (null == pPanel)
        {
            Debug.LogErrorFormat("Show() - No Exist Panel(Name : {0})", strName);
            return;
        }

        pPanel.Initialize();
        pPanel.Show(pArgs);
    }
    public void Close(string strName)
    {
        var pPanel = GetPanel(strName);
        if (null == pPanel)
        {
            Debug.LogErrorFormat("Close() - No Exist Panel(Name : {0})", strName);
            return;
        }

        pPanel.Close();
    }
    #endregion


    #region Utility Functions
    SHUIBasePanel GetPanel(string strName)
    {
        if (false == m_dicPanels.ContainsKey(strName))
        {
            AddPanel(Single.Resource.GetObjectComponent<SHUIBasePanel>(strName));
        }

        if (false == m_dicPanels.ContainsKey(strName))
        {
            return null;
        }
        
        return m_dicPanels[strName];
    }
    void DestoryPanel(Dictionary<string, SHUIBasePanel> dicPanels)
    {
        if (null == dicPanels)
            return;

        SHUtil.ForToDic(new DicPanels(dicPanels), (pKey, pValue) =>
        {
            DestroyPanel(pValue);
            m_dicPanels.Remove(pKey);
        });
    }
    void DestroyPanel(SHUIBasePanel pPanel)
    {
        if (null == pPanel)
            return;

        SHGameObject.DestoryObject(pPanel);
    }
    #endregion


    #region Event Handler
    public void OnEventToChangeScene(eSceneType eCurrentScene, eSceneType eNextScene)
    {
        var pDestroyPanels = new DicPanels();
        SHUtil.ForToDic(m_dicPanels, (pKey, pValue) =>
        {
            if (eObjectDestoryType.ChangeScene != pValue.m_eDestroyType)
                return;

            pDestroyPanels.Add(pKey, pValue);
        });

        DestoryPanel(pDestroyPanels);
    }
    #endregion
}
