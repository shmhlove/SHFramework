using UnityEngine;
using System.Collections;

public static partial class SHMath
{
    // 소수점 자르기
    public static float Round(float fValue, int iOmit)
    {
        float fOmit = 1.0f;
        for (int iLoop = 0; iLoop < iOmit; ++iLoop)
            fOmit *= 10.0f;

        return Mathf.Round(fValue * fOmit) / fOmit;
    }

    // 비율 구하기
    public static float Percent(float fMin, float fMax, float fCurrent)
    {
        float fMaxGap = Mathf.Clamp(fMax - fMin, 0.0f, fMax);
        float fCurrentGap = Mathf.Clamp(fCurrent - fMin, 0.0f, fMaxGap);

        return Percent(fMaxGap, fCurrentGap);
    }
    public static int Percent(int iMax, int iCurrent)
    {
        return (int)(Percent((float)iMax, (float)iCurrent) * 100.0f);
    }
    public static float Percent(float fMax, float fCurrent)
    {
        return Round(Divide(fCurrent, fMax), 2);
    }

    // Value 스왑
    public static void Swap(ref float fValue1, ref float fValue2)
    {
        float fValue = fValue1;
        fValue1 = fValue2;
        fValue2 = fValue;
    }

    // 벡터 각 요소들 곱하기
    public static Vector3 Vector3ToMul(Vector3 v3Arg1, Vector3 v3Arg2)
    {
        return new Vector3((v3Arg1.x * v3Arg2.x), (v3Arg1.y * v3Arg2.y), (v3Arg1.z * v3Arg2.z));
    }

    // 벡터From에서 벡터To로의 방향
    public static Vector3 GetDirection(Vector3 vFrom, Vector3 vTo)
    {
        return (vTo - vFrom).normalized;
    }

    // 반사벡터 구하기
    public static Vector3 GetReflect(Vector3 vFrom, Vector3 vTo, Vector3 vNormal)
    {
        return Vector3.Reflect(GetDirection(vFrom, vTo), vNormal).normalized;
    }

    // 두 지점 사이의 X비율에 해당하는 값 구하기
    public static float Lerp(float fMin, float fMax, float fRatio)
    {
        if (fMax < fMin)
            Swap(ref fMin, ref fMax);

        //Mathf.Lerp(fMax, fMin, fRatio)
        return fMin + ((fMax - fMin) * fRatio);
    }
    public static Vector3 Lerp(Vector3 vMin, Vector3 vMax, float fRatio)
    {
        //Vector3.Lerp(vMax, vMin, fRatio)
        return vMin + ((vMax - vMin) * fRatio);
    }

    // 범위 컨버팅 : -1 ~ 1 범위값을 0 ~ 1로 컨버팅
    public static float GetRangeToConvert(float fValue)
    {
        fValue = Mathf.Clamp(fValue, -1.0f, 1.0f);
        return 0.5f + (fValue * 0.5f);
    }

    // 범위체크 : Min과 Max사이에 Value가 속하는가?
    public static bool IsInToRange(float fMin, float fMax, float fValue)
    {
        return (fMin <= fValue && fValue < fMax);
    }

    // 부호얻기(float)
    public static float Sign(float fVal)
    {
        return Mathf.Sign(fVal);
    }
    // 부호얻기(Vector3)
    public static Vector3 Sign(Vector3 vVal)
    {
        return new Vector3(Sign(vVal.x), Sign(vVal.y), Sign(vVal.z));
    }
    // 0 ~ 360 -> 0 ~ 180, 0 ~ -180 변환
    public static float GetHalfAngle(float fAngle)
    {
        return fAngle > 180 ? fAngle - 360 : fAngle;
    }
    // 안전하게 나누기
    public static float Divide(float fNumerator, float fDenominator)
    {
        if (0.0f == fDenominator)
            return 0.0f;

        if (0.0f == fNumerator)
            return 0.0f;

        return (fNumerator / fDenominator);
    }
    // 모듈러
    public static int Modulus(int iNum, int iDiv)
    {
        return iNum % iDiv;
    }

    // Nan체크 : Float
    public static bool IsNan(float f)
    {
        return float.IsNaN(f);
    }

    // Nan체크 : Quaternion
    public static bool IsNan(Quaternion q)
    {
        return float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w);
    }
}
