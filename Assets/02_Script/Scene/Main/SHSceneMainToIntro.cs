using UnityEngine;
using System;
using System.Collections;

public class SHSceneMainToIntro : MonoBehaviour
{
    #region Members
    #endregion


    #region System Functions
    void Start() 
    {
        Single.AppInfo.CreateSingleton();
        Single.UI.Show("Panel - Intro", (Action)OnEventToNextScene);
	}
	
	void Update () 
    {

    }
    #endregion


    #region Virtual Functions
    #endregion


    #region Interface Functions
    #endregion


    #region Utility Functions
    #endregion


    #region Event Handler
    void OnEventToNextScene()
    {
        Single.Scene.GoTo(eSceneType.Login);
    }
    #endregion
}
