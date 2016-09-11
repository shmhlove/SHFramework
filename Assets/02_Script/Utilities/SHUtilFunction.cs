﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public static partial class SHUtil
{
    // --------------------------------------------------------------------
    // 형 변환 관련 ( Enum.Parse 엄청느립니다. 가급적 사용금지!!! )
    // String을 Enum으로
    public static T StringToEnum<T>(string strEnum, string strErrorLog = "")
    {
        if ((true == string.IsNullOrEmpty(strEnum)) || 
            (false == Enum.IsDefined(typeof(T), strEnum)))
        {
            UnityEngine.Debug.LogError(string.Format("{0} ( Enum:{1} )", strErrorLog, strEnum));
            return default(T);
        }

        return (T)Enum.Parse(typeof(T), strEnum);
    }
    // string을 DateTime으로
    public static DateTime GetDateTimeToString(string strDate, string strFormat)
    {
        return DateTime.ParseExact(strDate, strFormat, System.Globalization.CultureInfo.InstalledUICulture);
    }
    // DateTime을 string으로
    public static string GetStringToDateTime(DateTime pTime, string strFormat)
    {
        return pTime.ToString(strFormat, System.Globalization.CultureInfo.InstalledUICulture);
    }


    // --------------------------------------------------------------------
    // 컨테이너 관련
    // Foreach Array
    public static void ForeachToArray<T>(T[] pArray, Action<T> pLambda)
    {
        foreach (T tArray in pArray)
            pLambda(tArray);
    }
    // Foreach Enum
    public static void ForeachToEnum<T>(Action<T> pLambda)
    {
        foreach (T eEnum in Enum.GetValues(typeof(T)))
            pLambda(eEnum);
    }
    // Foreach List
    public static void ForeachToList<T>(List<T> pList, Action<T> pLambda)
    {        
        foreach (T tList in pList)
            pLambda(tList);
    }
    // Foreach Condition List
    public static bool ForeachToListOfBreak<T>(List<T> pList, bool bBreakCondition, Func<T, bool> pLambda)
    {
        foreach (T tList in pList)
        {
            if (bBreakCondition == pLambda(tList))
                return bBreakCondition;
        }
        return !bBreakCondition;
    }
    // Foreach Dictionary
    public static void ForeachToDic<TKey, TValue>(Dictionary<TKey, TValue> pDic, Action<TKey, TValue> pLambda)
    {
        foreach (KeyValuePair<TKey, TValue> kvp in pDic)
            pLambda(kvp.Key, kvp.Value);
    }
    // Foreach Condition Dictionary
    public static bool ForeachToDicOfBreak<TKey, TValue>(Dictionary<TKey, TValue> pDic, bool bBreakCondition, Func<TKey, TValue, bool> pLambda)
    {
        foreach (KeyValuePair<TKey, TValue> kvp in pDic)
        {
            if (bBreakCondition == pLambda(kvp.Key, kvp.Value))
                return bBreakCondition;
        }
        return !bBreakCondition;
    }
    // for Double
    public static void ForToDouble(int iMaxToFirst, int iMaxToSecond, Action<int, int> pLambda)
    {
        for (int iLoop1 = 0; iLoop1 < iMaxToFirst; ++iLoop1)
        {
            for (int iLoop2 = 0; iLoop2 < iMaxToSecond; ++iLoop2)
                pLambda(iLoop1, iLoop2);
        }
    }
    public static bool ForToDoubleOfBreak(int iMaxToFirst, int iMaxToSecond, bool bBreakCondition, Func<int, int, bool> pLambda)
    {
        for (int iLoop1 = 0; iLoop1 < iMaxToFirst; ++iLoop1)
        {
            for (int iLoop2 = 0; iLoop2 < iMaxToSecond; ++iLoop2)
            {
                if (bBreakCondition == pLambda(iLoop1, iLoop2))
                    return bBreakCondition;
            }
        }
        return !bBreakCondition;
    }
    // Inverse for Double
    public static void ForInverseToDouble(int iMaxToFirst, int iMaxToSecond, Action<int, int> pLambda)
    {
        for (int iLoop1 = iMaxToFirst; iLoop1 >= 0; --iLoop1)
        {
            for (int iLoop2 = iMaxToSecond; iLoop2 >= 0; --iLoop2)
                pLambda(iLoop1, iLoop2);
        }
    }


    //-------------------------------------------------------------------------
    // 디바이스 정보관련
    // UUID
    public static string GetUUID()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }


    //-------------------------------------------------------------------------
    // 유니티 에디터 관련
    // Missing컴포넌트 체크
    public static void CheckMissingComponent()
    {
#if UNITY_EDITOR
        var pObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        foreach (var pObject in pObjects)
        {
            if (null == pObject)
                continue;

            var pComponents = (pObject as GameObject).GetComponents<Component>();
            foreach (var pComponent in pComponents)
            {
                if (null == pComponent)
                    UnityEngine.Debug.Log(string.Format("<color=red>MissingComponent!!(GameObject{0})</color>", pObject.name));
            }
        }
#endif
    }
    // 유니티 에디터의 Pause를 Toggle합니다.
    public static void EditorPauseOfToggle(bool bToggle)
    {
#if UNITY_EDITOR
        EditorApplication.isPaused = bToggle;
#endif
    }
    public static void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        ProcessThreadCollection pThreads = Process.GetCurrentProcess().Threads;
        foreach (ProcessThread pThread in pThreads)
        {
            pThread.Dispose();
        }

        Process.GetCurrentProcess().Kill();
#endif
    }

    //-------------------------------------------------------------------------
    // 디렉토리 체크
    public static void Search(string strPath, Action<FileInfo> pCallback)
    {
#if UNITY_EDITOR
        DirectoryInfo pDirInfo = new DirectoryInfo(strPath);
        SearchFiles(pDirInfo, pCallback);
        SearchDirs(pDirInfo, pCallback);
#endif
    }
    static void SearchDirs(DirectoryInfo pDirInfo, Action<FileInfo> pCallback)
    {
#if UNITY_EDITOR
        if (false == pDirInfo.Exists)
            return;

        DirectoryInfo[] pDirs = pDirInfo.GetDirectories();
        foreach (DirectoryInfo pDir in pDirs)
        {
            SearchFiles(pDir, pCallback);
            SearchDirs(pDir, pCallback);
        }
#endif
    }
    static void SearchFiles(DirectoryInfo pDirInfo, Action<FileInfo> pCallback)
    {
#if UNITY_EDITOR
        FileInfo[] pFiles = pDirInfo.GetFiles();
        foreach (FileInfo pFile in pFiles)
        {
            pCallback(pFile);
        }
#endif
    }


    //-------------------------------------------------------------------------
    // 파일저장
    public static void SaveFile(string strBuff, string strSavePath)
    {
        SHUtil.CreateDirectory(strSavePath);

        var pFile    = new FileStream(strSavePath, FileMode.Create, FileAccess.Write);
        var pWriter  = new StreamWriter(pFile);
        pWriter.WriteLine(strBuff);
        pWriter.Close();
        pFile.Close();

// ICloud에 백업 안되게 파일에 대해 SetNoBackupFlag를 해주자!!
#if UNITY_IPHONE
        UnityEngine.iOS.Device.SetNoBackupFlag(strSavePath);
#endif

        UnityEngine.Debug.Log(string.Format("{0} File 저장", strSavePath));
    }

    public static string ReadFile(string strReadPath)
    {
        var pFile   = new FileStream(strReadPath, FileMode.Open, FileAccess.Read);
        var pReader = new StreamReader(pFile);
        string strBuff = pReader.ReadToEnd();
        pReader.Close();
        pFile.Close();

        return strBuff;
    }

    public static void SaveByte(byte[] pBytes, string strSavePath)
    {
        SHUtil.CreateDirectory(strSavePath);

        var pFile       = new FileStream(strSavePath, FileMode.Create, FileAccess.Write);
        var pWriter     = new BinaryWriter(pFile);
        pWriter.Write(pBytes);
        pWriter.Close();
        pFile.Close();

// ICloud에 백업 안되게 파일에 대해 SetNoBackupFlag를 해주자!!
#if UNITY_IPHONE
        UnityEngine.iOS.Device.SetNoBackupFlag(strSavePath);
#endif

        UnityEngine.Debug.Log(string.Format("{0} Byte 저장", strSavePath));
    }
    public static void DeleteFile(string strFilePath)
    {
        if (false == File.Exists(strFilePath))
            return;

        FileInfo pFile = new FileInfo(strFilePath);
        pFile.Attributes = FileAttributes.Normal;
        File.Delete(strFilePath);
        
    }
    public static void CopyFile(string strSource, string strDest)
    {
        if (false == File.Exists(strSource))
            return;

        SHUtil.CreateDirectory(strDest);

        File.Copy(strSource, strDest, true);

// ICloud에 백업 안되게 파일에 대해 SetNoBackupFlag를 해주자!!
#if UNITY_IPHONE
        UnityEngine.iOS.Device.SetNoBackupFlag(strDest);
#endif
    }
    public static void CreateDirectory(string strPath)
    {
        if (false == string.IsNullOrEmpty(Path.GetExtension(strPath)))
            strPath = Path.GetDirectoryName(strPath);

        DirectoryInfo pDirectoryInfo = new DirectoryInfo(strPath);
        if (true == pDirectoryInfo.Exists)
            return;

        pDirectoryInfo.Create();
    }
    public static void DeleteDirectory(string strPath)
    {
        DirectoryInfo pDirInfo = new DirectoryInfo(strPath);
        if (false == pDirInfo.Exists)
            return;

        FileInfo[] pFiles = pDirInfo.GetFiles("*.*", SearchOption.AllDirectories);
        foreach (FileInfo pFile in pFiles)
        {
            if (false == pFile.Exists)
                continue;

            pFile.Attributes = FileAttributes.Normal;
        }

        Directory.Delete(strPath, true);
    }


    //-------------------------------------------------------------------------
    // 탐색기 열기
    public static void OpenInFileBrowser(string strPath)
    {
        if (true == string.IsNullOrEmpty(strPath))
            return;

#if UNITY_EDITOR_WIN
        SHUtil.OpenInWinFileBrowser(strPath);
#elif UNITY_EDITOR_OSX
        SHUtil.OpenInMacFileBrowser(strPath);
#endif
    }
    public static void OpenInMacFileBrowser(string strPath)
     {
        strPath = strPath.Replace("\\", "/");

        if (false == strPath.StartsWith("\""))
            strPath = "\"" + strPath;

        if (false == strPath.EndsWith("\""))
            strPath = strPath + "\"";

        System.Diagnostics.Process.Start("open", ((true == Directory.Exists(strPath)) ? "" : "-R ") + strPath);
    }
    public static void OpenInWinFileBrowser(string strPath)
    {
        strPath = strPath.Replace("/", "\\");
        System.Diagnostics.Process.Start("explorer.exe", ((true == Directory.Exists(strPath)) ? "/root," : "/select,") + strPath);
    }


    //-------------------------------------------------------------------------
    // 기타
    // Action 함수를 예외처리 후 콜해준다.
    public static void SafeActionCall(Action pAction)
    {
        if (null == pAction)
            return;

        pAction();
    }
    // WWW.error 메시지로 에러코드 얻기
    public static int GetWWWErrorCode(string strErrorMsg)
    {
        if (true == string.IsNullOrEmpty(strErrorMsg))
            return 0;

        int      iErrorCode = 0;
        string[] strSplit   = strErrorMsg.Split(new char[]{ ' ' });
        int.TryParse(strSplit[0], out iErrorCode);
        return iErrorCode;
    }
    // 콜스택 얻기
    public static string GetCallStack()
    {
        var pCallStack      = new StackTrace();
        var strCallStack    = string.Empty;
        foreach (var pFrame in pCallStack.GetFrames())
        {
            strCallStack += string.Format("{0}({1}) : {2}\n",
                pFrame.GetMethod(), pFrame.GetFileLineNumber(), pFrame.GetFileName());
        }

        return strCallStack;
    }
}


public class SHPair<T1, T2>
{
    public T1 Value1;
    public T2 Value2;

    public SHPair()
    {
        Initialize();
    }

    public SHPair(T1 _Value1, T2 _Value2)
    {
        Value1 = _Value1;
        Value2 = _Value2;
    }

    public void Initialize()
    {
        Value1 = default(T1);
        Value2 = default(T2);
    }
}