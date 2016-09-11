using UnityEngine;
using System.Collections;

public class SHUIManager : SHSingleton<SHUIManager>
{
    #region Virtual Functions
    public override void OnInitialize()
    {
        SetDontDestroy();
    }
    public override void OnFinalize()
    {

    }
    #endregion
}
