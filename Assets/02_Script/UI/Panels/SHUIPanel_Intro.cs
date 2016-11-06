using UnityEngine;
using System;
using System.Collections;

public class SHUIPanel_Intro : SHUIBasePanel
{
    #region Members : Inspector
    [Header("IntroInfo")]
    [SerializeField] private GameObject m_pButton = null;
    #endregion


    #region Members : Event
    private Action m_pEventToTouch = null;
    #endregion


    #region System Functions
    #endregion


    #region Virtual Functions
    public override void OnBeforeShow(params object[] pArgs)
    {
        if ((null == pArgs) || (1 > pArgs.Length))
            return;

        m_pEventToTouch = (Action)pArgs[0];
    }
    #endregion


    #region Interface Functions
    #endregion


    #region Utility Functions
    #endregion


    #region Event Handler
    public void OnClickToScreen()
    {
        if (null != m_pEventToTouch)
            m_pEventToTouch();
        
        m_pButton.SetActive(false);
    }
    #endregion
}
