using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class SHCoroutine : SHSingleton<SHCoroutine>
{
    #region Virtual Functions
    public override void OnInitialize() { }
    public override void OnFinalize() { }
    #endregion


    #region Interface Functions
    // yield return null : 다음 Update까지 대기
    //-----------------------------------------------
    public void NextUpdate(Action pAction)
    {
        if (null == pAction)
            return;

        StartCoroutine(InvokeToNextUpdate(pAction));
    }
    IEnumerator InvokeToNextUpdate(Action pAction)
    {
        yield return null;
        pAction.Invoke();
    }


    // yield return new WaitForFixedUpdate() : 다음 FixedUpdate까지 대기
    //-----------------------------------------------
    public void NextFixed(Action pAction)
    {
        if (null == pAction)
            return;

        StartCoroutine(InvokeToNextFixed(pAction));
    }
    IEnumerator InvokeToNextFixed(Action pAction)
    {
        yield return new WaitForFixedUpdate();
        pAction.Invoke();
    }
    

    // yield return new WaitForEndOfFrame() : 렌더링 작업이 끝날 때 까지 대기
    //-----------------------------------------------
    public void NextFrame(Action pAction)
    {
        if (null == pAction)
            return;

        StartCoroutine(InvokeToNextFrame(pAction));
    }
    IEnumerator InvokeToNextFrame(Action pAction)
    {
        yield return new WaitForEndOfFrame();
        pAction.Invoke();
    }
    

    // yield return new WaitForSeconds : 지정한 시간까지 대기
    //-----------------------------------------------
    public void WaitTime(Action pAction, float fDelay)
    {
        if (null == pAction)
            return;

        StartCoroutine(InvokeToWaitTime(pAction, fDelay));
    }
    IEnumerator InvokeToWaitTime(Action pAction, float fDelay)
    {
        yield return new WaitForSeconds(fDelay);
        pAction.Invoke();
    }
    

    //yield return new WWW(string) : 웹 통신 작업이 끝날 때까지 대기
    //-----------------------------------------------
    public WWW WWW(Action<WWW> pAction, WWW pWWW)
    {
        StartCoroutine(InvokeToWWW(pAction, pWWW));
        return pWWW;
    }
    IEnumerator InvokeToWWW(Action<WWW> pAction, WWW pWWW)
    {
        yield return pWWW;

        if (null != pAction)
            pAction.Invoke(pWWW);
    }
    public WWW WWWOfSync(WWW pWWW)
    {
        InvokeToWWW(null, pWWW);
        while (false == pWWW.isDone);
        return pWWW;
    }
    

    //yield return new AsyncOperation : 비동기 작업이 끝날 때 까지 대기 (씬로딩)
    //-----------------------------------------------
    public AsyncOperation Async(Action<bool> pAction, AsyncOperation pAsync)
    {
        StartCoroutine(InvokeToAsync(pAction, pAsync));
        return pAsync;
    }
    IEnumerator InvokeToAsync(Action<bool> pAction, AsyncOperation pAsync)
    {
        yield return pAsync;

        if (null != pAction)
            pAction.Invoke(pAsync.isDone);
    }
    

    //yield return StartCoroutine(IEnumerator) : 다른 코루틴이 끝날 때 까지 대기
    //-----------------------------------------------
    public void Routine(Action pAction, IEnumerator pRoutine)
    {
        if (null == pAction)
            return;

        StartCoroutine(InvokeToRoutine(pAction, pRoutine));
    }
    IEnumerator InvokeToRoutine(Action pAction, IEnumerator pRoutine)
    {
        yield return StartCoroutine(pRoutine);
        pAction.Invoke();
    }
    #endregion
}