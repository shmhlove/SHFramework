using UnityEngine;
using System;
using System.Collections;

public enum eNoticeButton
{
    One,
    Two,
    Three,
}

public class SHUIPanel_Notice : SHUIBasePanel
{
    #region Members : Inspector
    [Header("NoticeInfo")]
    [SerializeField] private UILabel       m_pTitle     = null;
    [SerializeField] private UILabel       m_pMessage   = null;
    [SerializeField] private GameObject    m_pOneButton = null;
    [SerializeField] private GameObject    m_pTwoButton = null;
    [SerializeField] private GameObject    m_pThrButton = null;
    #endregion


    #region Members : Event
    private Action m_pEventToOK     = null;
    private Action m_pEventToCancel = null;
    private Action m_pEventToRetry  = null;
    #endregion


    #region System Functions
    #endregion


    #region Virtual Functions
    public override void OnBeforeShow(params object[] pArgs)
    {
        if ((null == pArgs) || (3 > pArgs.Length))
            return;

        SetActiveButton((eNoticeButton)pArgs[0]);
        SetTitle((string)pArgs[1]);
        SetMessage((string)pArgs[2]);

        if (4 <= pArgs.Length)  m_pEventToOK     = (Action)pArgs[3];
        if (5 <= pArgs.Length)  m_pEventToCancel = (Action)pArgs[4];
        if (6 <= pArgs.Length)  m_pEventToRetry  = (Action)pArgs[5];
    }
    #endregion


    #region Interface Functions
    #endregion


    #region Utility Functions
    void SetActiveButton(eNoticeButton eType)
    {
        m_pOneButton.SetActive(eNoticeButton.One   == eType);
        m_pTwoButton.SetActive(eNoticeButton.Two   == eType);
        m_pThrButton.SetActive(eNoticeButton.Three == eType);
    }
    void SetTitle(string strTitle)
    {
        m_pTitle.text = strTitle;
    }
    void SetMessage(string strMessage)
    {
        m_pMessage.text = strMessage;
    }
    #endregion


        #region Event Handler
    public void OnClickToOK()
    {
        if (null != m_pEventToOK)
            m_pEventToOK();

        Close();
    }
    public void OnClickToCancel()
    {
        if (null != m_pEventToCancel)
            m_pEventToCancel();

        Close();
    }
    public void OnClickToRetry()
    {
        if (null != m_pEventToRetry)
            m_pEventToRetry();

        Close();
    }
    #endregion
}
