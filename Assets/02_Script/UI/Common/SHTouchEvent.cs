using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SHTouchEvent : MonoBehaviour
{
    #region Value Members
    public BoxCollider  m_pCollider          = null;
    public Transform    m_pStickObject       = null;
    public float        m_fStickRadius       = 0.1f;
    public bool         m_bIsCenterOnPress   = true;

    public Vector3      m_vCenterPos         = Vector3.zero;
    public Vector3      m_vTouchPos          = Vector2.zero;
    public Vector3      m_vBeforePos         = Vector2.zero;
    public Vector3      m_vCurrentPos        = Vector2.zero;

    public SHEvent eventTouch                = new SHEvent(); // Press시
    public SHEvent eventTouchPos             = new SHEvent(); // Drag시 처음 터치한 위치로 부터의 상대위치
    public SHEvent eventTouchDirection       = new SHEvent(); // Drag시 처음 터치한 위치로 부터의 방향
    public SHEvent eventStickPos             = new SHEvent(); // Drag시 StickObject 위치를 이용한 상대위치
    public SHEvent eventStickDirection       = new SHEvent(); // Drag시 StickObject 위치를 이용한 방향
    public SHEvent eventTouchEnd             = new SHEvent(); // Release시
    #endregion


    #region System Functions
    void Awake()
    {
        m_pCollider = gameObject.GetComponent<BoxCollider>();
        if (null == m_pCollider)
            m_pCollider = gameObject.AddComponent<BoxCollider>();
    }
    void Start()
    {
        m_vCenterPos    =
        m_vTouchPos     = 
        m_vBeforePos    = 
        m_vCurrentPos   = transform.position;
    }
    #endregion


    #region Interface Functions
    public void SetSize(Vector2 vSize)
    {
        m_pCollider.size = vSize;
    }
    #endregion


    #region Utility Functions
    void SetPressToOn()
    {
        m_vTouchPos = GetPosToTouch();
        
        if (true == m_bIsCenterOnPress)
            m_vCenterPos = m_vTouchPos;
        else
            m_vCenterPos = transform.position;

        m_vBeforePos    = m_vTouchPos;
        m_vCurrentPos   = m_vTouchPos;

        SetStickPos(m_vTouchPos);
        SendEventToTouch();
    }

    void SetPressToOff()
    {
        m_vBeforePos    = Vector3.zero;
        m_vCurrentPos   = Vector3.zero;

        SendEventToEndTouch();
    }

    Vector3 GetPosToTouch()
    {
        Ray pRay = UICamera.currentCamera.ScreenPointToRay(UICamera.lastTouchPosition);
        Vector3 vTouchPos = pRay.GetPoint(0.0f);
        vTouchPos.z = 0.0f;

        return vTouchPos;
    }

    void SetStickPos(Vector3 vPos)
    {
        if (null == m_pStickObject)
            return;

        Vector3 vLocation = (vPos - m_vCenterPos);
        if (m_fStickRadius < vLocation.magnitude)
            vPos = m_vCenterPos + (SHMath.GetDirection(m_vCenterPos, vPos) * m_fStickRadius);

        m_pStickObject.position = vPos;
    }

    Vector3 GetStickPos()
    {
        if (null == m_pStickObject)
            return Vector3.zero;

        return m_pStickObject.position;
    }

    void SendEventToTouch()
    {
        eventTouch.Callback(this);
    }

    void SendEventToDrag()
    {
        eventTouchPos.Callback<Vector2>(this, (m_vCurrentPos - m_vTouchPos));
        eventTouchDirection.Callback<Vector2>(this, (m_vCurrentPos - m_vTouchPos).normalized);
        eventStickPos.Callback<Vector2>(this, (GetStickPos() - m_vCenterPos));
        eventStickDirection.Callback<Vector2>(this, (GetStickPos() - m_vCenterPos).normalized);
    }

    void SendEventToEndTouch()
    {
        eventTouchEnd.Callback(this);
    }
    #endregion


    #region Event Handler
    void OnPress(bool bIsOn)
    {
        if (null == m_pStickObject)
            return;

        if (true == bIsOn)
            SetPressToOn();
        else
            SetPressToOff();
    }

    void OnDrag(Vector2 vDelta)
    {
        if (null == m_pStickObject)
            return;

        m_vBeforePos = m_vCurrentPos;
        m_vCurrentPos = GetPosToTouch();

        SetStickPos(m_vCurrentPos);
        SendEventToDrag();
    }
    #endregion
}
