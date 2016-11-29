using UnityEngine;
using System;
using System.Collections;

public class SHUIPanel_Login : SHUIBasePanel
{
    #region Members : Inspector
    #endregion


    #region Members : Event
    #endregion


    #region System Functions
    #endregion


    #region Virtual Functions
    public override void OnBeforeShow(params object[] pArgs)
    {
        if ((null == pArgs) || (1 > pArgs.Length))
            return;
        
    }
    #endregion


    #region Interface Functions
    #endregion


    #region Utility Functions

    #endregion


    #region Event Handler
    public void OnClickToGoggle()
    {
        Single.UI.ShowNotice_NoMake();
    }
    public void OnClickToFacebook()
    {
        Single.UI.ShowNotice_NoMake();
    }
    public void OnClickToGuast()
    {
        Single.UI.ShowNotice_NoMake();
    }
    #endregion
}
