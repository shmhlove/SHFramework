using UnityEngine;
using System;
using System.Collections;

public class SHUIMassiveScrollView : MonoBehaviour 
{
    #region Members : Inspector
	[SerializeField] private AnimationClip  m_pAnimClipOnTouch;
	[SerializeField] private AnimationClip  m_pAnimClipOnHover;
    [SerializeField] private AnimationClip  m_pAnimClipOnIdle;
    [SerializeField] private AnimationClip  m_pAnimClipOnClick;
    [SerializeField] private GameObject     m_pAnimTarget;
    #endregion


    #region Members : ClipName
	private const string m_strTouchClipName = "UIButtonAnimationClipTouch";
	private const string m_strHoverClipName = "UIButtonAnimationClipHover";
    private const string m_strIdleClipName  = "UIButtonAnimationClipIdle";
    private const string m_strClickClipName = "UIButtonAnimationClipClick";
    #endregion


    #region Members : Info
    private Animation   m_mAnim          = null;
    private bool        m_bIsHover       = false;
    private bool        m_bIsAnimPlaying = false;
    private Vector3     m_vScale         = Vector3.one;
    #endregion


    #region System Functions
    void Awake()
    {
        m_vScale = GetTarget().transform.localScale;
        m_mAnim  = SHGameObject.GetComponent<Animation>(GetTarget());
    }
    void Start()
    {
        StopAllCoroutines();
        AddClip();
        
        if (true == UICamera.IsHighlighted(GetTarget()))
            OnHover(true);
        else
            Play(m_strIdleClipName);
    }
    void OnDisable()
    {
        GetTarget().transform.localScale = m_vScale;
        StopAllCoroutines();
    }
    #endregion


    #region Interface Functions
    #endregion


    #region Utility Functions
    GameObject GetTarget()
    {
        return (null == m_pAnimTarget) ? gameObject : m_pAnimTarget;
    }
    void AddClip()
    {
        AddClip(m_pAnimClipOnTouch, m_strTouchClipName);
        AddClip(m_pAnimClipOnHover, m_strHoverClipName);
        AddClip(m_pAnimClipOnIdle,  m_strIdleClipName);
        AddClip(m_pAnimClipOnClick, m_strClickClipName);
    }
    void AddClip(AnimationClip pClip, string strName)
    {
        if (null == m_mAnim)
            return;

        if (null == pClip)
            return;

        if (null != m_mAnim.GetClip(strName))
            return;

        m_mAnim.AddClip(pClip, strName);
    }
    bool Play(string strClipName, bool bForward = true, Action pOnPlayEnd = null)
    {
        if (null == m_mAnim)
            return false;

        if (null == m_mAnim.GetClip(strClipName))
            return false;
        
        StopAllCoroutines();
        StartCoroutine(CoroutinePlay(strClipName, bForward, pOnPlayEnd));
        return true;
    }
    void CheckHighlighted()
    {
        m_bIsHover = UICamera.IsHighlighted(gameObject);

        if (true == m_bIsHover)
            Play(m_strHoverClipName);
        else
            Play(m_strIdleClipName);
    }
    #endregion


    #region Coroutine Functions
    IEnumerator CoroutinePlay(string strClipName, bool bForward, Action pOnPlayEnd)
    {
        m_bIsAnimPlaying = true;

        if (null == m_mAnim.GetClip(strClipName))
            yield return null;
        
        if (1.0f == Time.timeScale)
        {
            var pState = m_mAnim[strClipName];
            pState.normalizedTime = (bForward == true) ? 0.0f :  1.0f;
            pState.speed          = (bForward == true) ? 1.0f : -1.0f;

            m_mAnim.Stop();
            m_mAnim.Play(strClipName);
            
            if (WrapMode.Loop == pState.wrapMode)
                yield return null;
            else
                yield return new WaitForSeconds(pState.length);
        }
        else
        {
            if (true == bForward)
                yield return StartCoroutine(CoroutinePlayForward(GetTarget(), m_mAnim[strClipName]));
            else
                yield return StartCoroutine(CoroutinePlayBackward(GetTarget(), m_mAnim[strClipName]));
        }

        m_bIsAnimPlaying = false;
        
        if (null != pOnPlayEnd)
            pOnPlayEnd();
    }
    IEnumerator CoroutinePlayForward(GameObject pObject, AnimationState pState)
    {
        float fStart    = Time.unscaledTime;
        float fElapsed  = 0.0f;
        
        while (true)
        {
            if ((null == pObject) || (null == pState))
                break;
            
            if (false == pObject.activeInHierarchy)
                break;

            fElapsed = Time.unscaledTime - fStart;
            pState.clip.SampleAnimation(pObject, fElapsed);

            if (fElapsed >= pState.length)
            {
                fStart = Time.unscaledTime;
                if (WrapMode.Loop != pState.wrapMode)
                    break;
            }

            yield return null;
        }

        if ((null != pObject) || (null != pState))
            pState.clip.SampleAnimation(pObject, pState.length);
    }
    IEnumerator CoroutinePlayBackward(GameObject pObject, AnimationState pState)
    {
        float fStart    = Time.unscaledTime;
        float fElapsed  = 0.0f;

        while (true)
        {
            if ((null == pObject) || (null == pState))
                break;

            if (false == pObject.activeInHierarchy)
                break;

            fElapsed = pState.length - (Time.unscaledTime - fStart);
            pState.clip.SampleAnimation(pObject, fElapsed);

            if (0.0f >= fElapsed)
            {
                fStart = Time.unscaledTime;
                if (WrapMode.Loop != pState.wrapMode)
                    break;
            }

            yield return null;
        }

        if ((null != pObject) || (null != pState))
            pState.clip.SampleAnimation(pObject, 0.0f);
    }
    #endregion


    #region Event Handler
    void OnPress(bool bIsPressed)
    {
        if (false == enabled)
            return;
        
        if (true == bIsPressed)
        {
            Play(m_strTouchClipName, pOnPlayEnd: CheckHighlighted);
        }
        else if (false == m_bIsAnimPlaying)
        {
            OnHover(UICamera.IsHighlighted(gameObject));
        }
    }
    void OnHover(bool bIsOver)
    {
        if (false == enabled)
            return;

        if (bIsOver == m_bIsHover)
            return;
        
        if (true == bIsOver)
        {
            Play(m_strHoverClipName, true);
        }
        else if (false == Play(m_strHoverClipName, false, () => Play(m_strIdleClipName)))
        {
            Play(m_strIdleClipName);
        }

        m_bIsHover = bIsOver;
    }
    void OnSelect(bool bSelected)
    {
        if (false == enabled)
            return;

        if ((false == bSelected) || 
            (UICamera.ControlScheme.Controller == UICamera.currentScheme))
        {
            OnHover(bSelected);
        }
    }
    void OnClick()
    {
        if (false == enabled)
            return;

        Play(m_strClickClipName, pOnPlayEnd: () => Play(m_strIdleClipName));
    }
    #endregion
}