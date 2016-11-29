using UnityEngine;
using System;
using System.Collections;

public class SHMonoWrapper : MonoBehaviour
{
    #region Members : Transform
    public Vector3     m_vPosition         = Vector3.zero;
    public Vector3     m_vLocalPosition    = Vector3.zero;
    public Vector3     m_vLocalScale       = Vector3.zero;
    public Quaternion  m_qRotate           = Quaternion.identity;
    public Quaternion  m_qLocalRotate      = Quaternion.identity;
    public bool?       m_eIsActive         = null;
    #endregion


    #region Members : Animation
    public Animation   m_pAnim             = null;
    public bool        m_bIsAnimPlaying    = false;
    #endregion


    #region System Functions
    public virtual void Awake()
    {

    }
    public virtual void Start()
    {
        StopAllCoroutines();
    }
    public virtual void OnDisable()
    {
        StopAllCoroutines();
    }
    #endregion


    #region Interface : Active
    public void SetActive(bool bIsActive)
    {
        if (IsActive() == bIsActive)
            return;
        
        gameObject.SetActive((m_eIsActive = bIsActive).Value);
    }
    public bool IsActive()
    {
        if (null == m_eIsActive)
            m_eIsActive = gameObject.activeInHierarchy;

        return m_eIsActive.Value;
    }
    #endregion


    #region Interface : Position
    public void SetPosition(Vector3 vPos)
    {
        gameObject.transform.position = m_vPosition = vPos;
    }
    public void SetPositionX(float fX)
    {
        Vector3 vPos = GetPosition();
        vPos.x = fX;
        SetPosition(vPos);
    }
    public void SetPositionY(float fY)
    {
        Vector3 vPos = GetPosition();
        vPos.y = fY;
        SetPosition(vPos);
    }
    public void SetLocalPosition(Vector3 vPos)
    {
        gameObject.transform.localPosition = m_vLocalPosition = vPos;
    }
    public void SetLocalPositionX(float fX)
    {
        Vector3 vPos = GetLocalPosition();
        vPos.x = fX;
        SetLocalPosition(vPos);
    }
    public void SetLocalPositionY(float fY)
    {
        Vector3 vPos = GetLocalPosition();
        vPos.y = fY;
        SetLocalPosition(vPos);
    }
    public Vector3 GetPosition()
    {
        return (Vector3.zero == m_vPosition) ?
               (m_vPosition = gameObject.transform.position) :
                m_vPosition;
    }
    public Vector3 GetLocalPosition()
    {
        return (Vector3.zero == m_vLocalPosition) ?
               (m_vLocalPosition = gameObject.transform.localPosition) :
                m_vLocalPosition;
    }
    #endregion


    #region Interface : Scale
    public void SetLocalScale(Vector3 vScale)
    {
        gameObject.transform.localScale = m_vLocalScale = vScale;
    }
    public void SetLocalScaleZ(float fScale)
    {
        Vector3 vScale = GetLocalScale();
        vScale.z = fScale;
        SetLocalScale(vScale);
    }
    public Vector3 GetLocalScale()
    {
        return (Vector3.zero == m_vLocalScale) ?
               (m_vLocalScale = gameObject.transform.localScale) :
                m_vLocalScale;
    }
    public bool IsZero2Scale()
    {
        Vector3 vScale = GetLocalScale();
        return ((0.0f == vScale.x) && (0.0f == vScale.y));
    }
    #endregion


    #region Interface : Rotate
    public void SetRotate(Vector3 vRotate)
    {
        m_qRotate.eulerAngles = vRotate;
        SetRotate(m_qRotate);
    }
    public void SetRotate(Quaternion qRotate)
    {
        gameObject.transform.rotation = m_qRotate = qRotate;
    }
    public void SetLocalRotate(Vector3 vRotate)
    {
        m_qLocalRotate.eulerAngles = vRotate;
        SetLocalRotate(m_qLocalRotate);
    }
    public void SetLocalRotate(Quaternion qRotate)
    {
        gameObject.transform.localRotation = m_qLocalRotate = qRotate;
    }
    public void SetRoll(float fValue)
    {
        Quaternion  qRot = GetLocalRotate();
        Vector3     vRet = qRot.eulerAngles;
        vRet.z = fValue;
        SetLocalRotate(vRet);
    }
    public void AddRoll(float fValue)
    {
        SetRoll(GetRoll() + fValue);
    }
    public float GetRoll()
    {
        return GetLocalRotate().eulerAngles.z;
    }
    public void SetPitch(float fValue)
    {
        Quaternion  qRot = GetLocalRotate();
        Vector3     vRet = qRot.eulerAngles;
        vRet.x = fValue;
        SetLocalRotate(vRet);
    }
    public void AddPitch(float fValue)
    {
        SetPitch(GetPitch() + fValue);
    }
    public float GetPitch()
    {
        return GetLocalRotate().eulerAngles.x;
    }
    public void SetYaw(float fValue)
    {
        Quaternion  qRot = GetLocalRotate();
        Vector3     vRet = qRot.eulerAngles;
        vRet.y = fValue;
        SetLocalRotate(vRet);
    }
    public void AddYaw(float fValue)
    {
        SetYaw(GetYaw() + fValue);
    }
    public float GetYaw()
    {
        return GetLocalRotate().eulerAngles.y;
    }
    public Quaternion GetRotate()
    {
        return (Quaternion.identity == m_qRotate) ?
               (m_qRotate = gameObject.transform.rotation) :
                m_qRotate;
    }
    public Quaternion GetLocalRotate()
    {
        return (Quaternion.identity == m_qLocalRotate) ?
               (m_qLocalRotate = gameObject.transform.localRotation) :
                m_qLocalRotate;
    }
    #endregion


    #region Interface : Animation
    public void PlayAnim(eDirection eDir, GameObject pObject, AnimationClip pClip, Action pEndCallback)
    {
        if (null == pClip)
        {
            if (null != pEndCallback)
                pEndCallback();
            return;
        }

        var pAnim = CreateAnimation(pObject);
        if (null == pAnim.GetClip(pClip.name))
            pAnim.AddClip(pClip, pClip.name);

        if (1.0f == Time.timeScale)
        {
            var pState = pAnim[pClip.name];
            pState.normalizedTime = (eDirection.Front == eDir) ? 0.0f :  1.0f;
            pState.speed          = (eDirection.Front == eDir) ? 1.0f : -1.0f;

            pAnim.Stop();
            pAnim.Play(pClip.name);

            if (WrapMode.Loop != pState.wrapMode)
                StartCoroutine(CoroutinePlayAnim_WaitTime(pState.length, pEndCallback));
        }
        else
        {
            switch (eDir)
            {
                case eDirection.Front:
                    StartCoroutine(CoroutinePlayAnim_UnScaledForward(pObject, pAnim[pClip.name], pEndCallback));
                    break;
                case eDirection.Back:
                    StartCoroutine(CoroutinePlayAnim_UnScaledBackward(pObject, pAnim[pClip.name], pEndCallback));
                    break;
            }
        }
    }

    private IEnumerator CoroutinePlayAnim_WaitTime(float fSec, Action pCallback)
    {
        yield return new WaitForSeconds(fSec);

        if (null != pCallback)
            pCallback();
    }

    private IEnumerator CoroutinePlayAnim_UnScaledForward(GameObject pObject, AnimationState pState, Action pEndCallback)
    {
        m_bIsAnimPlaying = true;

        float fStart     = Time.unscaledTime;
        float fElapsed   = 0.0f;
        
        while (true)
        {
            if ((null == pObject) || (null == pState))
                break;
            
            if (false == pObject.activeInHierarchy)
                break;

            fElapsed = Time.unscaledTime - fStart;
            pState.clip.SampleAnimation(pObject, fElapsed);

            if (pState.length <= fElapsed)
            {
                fStart = Time.unscaledTime;
                if (WrapMode.Loop != pState.wrapMode)
                    break;
            }

            yield return null;
        }

        if ((null != pObject) || (null != pState))
            pState.clip.SampleAnimation(pObject, pState.length);

        if (null != pEndCallback)
            pEndCallback();

        m_bIsAnimPlaying = false;
    }
    private IEnumerator CoroutinePlayAnim_UnScaledBackward(GameObject pObject, AnimationState pState, Action pEndCallback)
    {
        m_bIsAnimPlaying = true;

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

        if (null != pEndCallback)
            pEndCallback();

        m_bIsAnimPlaying = false;
    }
    #endregion


    #region Utility Functions
    Animation CreateAnimation(GameObject pObject = null)
    {
        if (null != m_pAnim)
            return m_pAnim;

        if (null == pObject)
            pObject = gameObject;
        
        return (m_pAnim = SHGameObject.GetComponent<Animation>(pObject));
    }
    #endregion
}