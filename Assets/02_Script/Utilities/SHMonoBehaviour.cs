using UnityEngine;
using System;
using System.Collections;

public class SHMonoBehaviour : MonoBehaviour
{
    #region Members
    private Vector3     m_vPosition         = Vector3.zero;
    private Vector3     m_vLocalPosition    = Vector3.zero;

    private Vector3     m_vLocalScale       = Vector3.zero;

    private Quaternion  m_qRotate           = Quaternion.identity;
    private Quaternion  m_qLocalRotate      = Quaternion.identity;

    private eBOOL       m_eIsActive         = eBOOL.None;
    #endregion


    #region Interface : Active
    public void SetActive(bool bIsActive)
    {
        if (bIsActive == IsActive())
            return;

        m_eIsActive = (true == bIsActive) ? eBOOL.True : eBOOL.False;
        gameObject.SetActive(bIsActive);
    }
    public bool IsActive()
    {
        if (eBOOL.None == m_eIsActive)
            m_eIsActive = (true == gameObject.activeInHierarchy) ? eBOOL.True : eBOOL.False;

        return (eBOOL.True == m_eIsActive);
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
    public void SetRotate(Quaternion qRotate)
    {
        gameObject.transform.rotation = m_qRotate = qRotate;
    }
    public void SetLocalRotate(Quaternion qRotate)
    {
        gameObject.transform.localRotation = m_qLocalRotate = qRotate;
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
    public IEnumerator CoroutineToPlayAnim(GameObject pTarget, AnimationClip pClip, Action pEndCallback)
    {
        if (null == pClip)
        {
            pEndCallback();
            yield break;
        }

        if (null == pTarget)
            pTarget = gameObject;

        var pAnimation = SHGameObject.GetComponent<Animation>(pTarget);
        if (null == pAnimation)
        {
            pEndCallback();
            yield break;
        }
        
        if (null == pAnimation.GetClip(pClip.name))
            pAnimation.AddClip(pClip, pClip.name);

        yield return StartCoroutine(
            CoroutineToPlayAnim(pTarget, pAnimation[pClip.name], pEndCallback));
    }
    public IEnumerator CoroutineToPlayAnim(GameObject pTarget, AnimationState pAnimState, Action pEndCallback)
    {
        float fStartTime = Time.unscaledTime;

        if (null == pTarget)
            pTarget = gameObject;

        while (null != pAnimState)
        {
            float fElapsed = Time.unscaledTime - fStartTime;
            pAnimState.clip.SampleAnimation(pTarget, Time.unscaledTime - fStartTime);

            if (fElapsed >= pAnimState.length)
            {
                fStartTime = Time.unscaledTime;
                if (WrapMode.Loop != pAnimState.wrapMode)
                    break;
            }

            yield return null;
        }

        if (null != pAnimState)
            pAnimState.clip.SampleAnimation(pTarget, pAnimState.length);

        pEndCallback();
    }
    #endregion
}