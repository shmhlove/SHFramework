using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class SHUIManager : SHSingleton<SHUIManager>
{
    #region Util : SHUIPanel_Notice
    public void ShowNotice(eNoticeButton eType, string strTitle, string strMessage, 
        Action pOK = null, Action pCancel = null, Action pRetry = null)
    {
        Single.UI.Show("Panel - Notice", eType, strTitle, strMessage, pOK, pCancel, pRetry);
    }
    #endregion
}
