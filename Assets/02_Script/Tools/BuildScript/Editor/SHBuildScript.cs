﻿using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class SHBuildScript
{
    #region Members
    static string[] SCENES    = FindEnabledEditorScenes();
    #endregion

    #region KOR Android Build
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 인터페이스 : 한국 AOS App빌드 ( 전체 패킹 )
    [MenuItem("SHTools/CI/Korea Android To Full AppBuild")]
    static void KOR_AndroidToOnlyAppBuild()                             {   OnlyAppBuild(eNationType.Korea, BuildTarget.Android, eServiceMode.DevQA);   }

    // 인터페이스 : 한국 AOS App빌드 + 번들(ALL) 패킹
    [MenuItem("SHTools/CI/Korea Android To AppBuild With All AssetBundles")]
    static void KOR_AndroidToAppBuildWithAllAssetBundlesPacking()       {   AppBuildWithBundlePacking(eNationType.Korea, BuildTarget.Android, eServiceMode.DevQA, eBundlePackType.All);  }

    // 인터페이스 : 한국 AOS App빌드 + 번들(Update) 패킹
    [MenuItem("SHTools/CI/Korea Android To AppBuild With Update AssetBundles")]
    static void KOR_AndroidToAppBuildWithUpdateAssetBundlesPacking()    {   AppBuildWithBundlePacking(eNationType.Korea, BuildTarget.Android, eServiceMode.DevQA, eBundlePackType.Update);   }

    // 인터페이스 : 한국 AOS 번들(ALL) 패킹
    [MenuItem("SHTools/CI/Korea Android To Only AssetBundles Of All")]
    static void KOR_AndroidToOnlyAssetBundlesOfAll()                    {   OnlyBundlePacking(eNationType.Korea, BuildTarget.Android, eBundlePackType.All);  }

    // 인터페이스 : 한국 AOS 번들(Update) 패킹
    [MenuItem("SHTools/CI/Korea Android To Only AssetBundles Of Update")]
    static void KOR_AndroidToOnlyAssetBundlesOfUpdate()                 {   OnlyBundlePacking(eNationType.Korea, BuildTarget.Android, eBundlePackType.Update);   }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #endregion

    #region KOR IOS Build
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 인터페이스 : 한국 IOS App빌드 ( 전체 패킹 )
    [MenuItem("SHTools/CI/Korea IPhone To Full AppBuild")]
    static void KOR_IPhoneToOnlyAppBuild()                              {   OnlyAppBuild(eNationType.Korea, BuildTarget.iOS, eServiceMode.DevQA);   }

    // 인터페이스 : 한국 IOS App빌드 + 번들(ALL) 패킹
    [MenuItem("SHTools/CI/Korea IPhone To AppBuild With All AssetBundles")]
    static void KOR_IPhoneToAppBuildWithAllAssetBundlesPacking()        {   AppBuildWithBundlePacking(eNationType.Korea, BuildTarget.iOS, eServiceMode.DevQA, eBundlePackType.All);  }

    // 인터페이스 : 한국 IOS App빌드 + 번들(Update) 패킹
    [MenuItem("SHTools/CI/Korea IPhone To AppBuild With Update AssetBundles")]
    static void KOR_IPhoneToAppBuildWithUpdateAssetBundlesPacking()     {   AppBuildWithBundlePacking(eNationType.Korea, BuildTarget.iOS, eServiceMode.DevQA, eBundlePackType.Update);   }

    // 인터페이스 : 한국 IOS 번들(ALL) 패킹
    [MenuItem("SHTools/CI/Korea IPhone To Only AssetBundles Of All")]
    static void KOR_IPhoneToOnlyAssetBundlesOfAll()                     {   OnlyBundlePacking(eNationType.Korea, BuildTarget.iOS, eBundlePackType.All);  }

    // 인터페이스 : 한국 IOS 번들(Update) 패킹
    [MenuItem("SHTools/CI/Korea IPhone To Only AssetBundles Of Update")]
    static void KOR_IPhoneToOnlyAssetBundlesOfUpdate()                  {   OnlyBundlePacking(eNationType.Korea, BuildTarget.iOS, eBundlePackType.Update);   }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #endregion

    #region Utility Functions
    // 유틸 : Only App Build
    static void OnlyAppBuild(eNationType eNation, BuildTarget eTarget, eServiceMode eMode)
    {
        // 국가 별 설정 처리
        SetNationInfo(eNation, eMode);

        // 빌드타겟 별 설정 처리
        SetBuildTargetInfo(eTarget);

        // App Build
        BuildApplication(SCENES, eTarget, BuildOptions.None);
    }

    // 유틸 : App Build + BundlePacking
    static void AppBuildWithBundlePacking(eNationType eNation, BuildTarget eTarget, eServiceMode eMode, eBundlePackType ePackType)
    {
        // 국가 별 설정 처리
        SetNationInfo(eNation, eMode);

        // 빌드타겟 별 설정 처리
        SetBuildTargetInfo(eTarget);

        // Asset Bundle Packing
        PackingAssetBundles(eTarget, ePackType, true);

        // App Build
        BuildApplication(SCENES, eTarget, BuildOptions.None);
    }

    // 유틸 : Only Bundle Packing
    static void OnlyBundlePacking(eNationType eNation, BuildTarget eTarget, eBundlePackType ePackType)
    {
        // 빌드타겟 별 설정 처리
        SetBuildTargetInfo(eTarget);

        // Asset Bundle Packing
        PackingAssetBundles(eTarget, ePackType, false);
    }

    // 유틸 : 국가별 설정 처리
    static void SetNationInfo(eNationType eNation, eServiceMode eMode)
    {
        switch (eNation)
        {
            case eNationType.Korea:
                // ClinetConfiguration파일 업데이트( CDN 주소 )
                WriteClientConfiguration(GetURLToConfigurationCDNOfKorea(eMode), eMode);
                break;
        }
    }

    // 유틸 : 빌드 타켓별 설정
    static void SetBuildTargetInfo(BuildTarget eTarget)
    {
        switch(eTarget)
        {
            case BuildTarget.Android:
                // 텍스쳐 압축모드
                EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ETC;
                break;
            case BuildTarget.iOS:
                // 텍스쳐 압축모드
                EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.PVRTC;
                break;
        }
    }

    // 유틸 : 빌드에 묶을 씬 찾기
    static string[] FindEnabledEditorScenes()
    {
        var pScenes = new List<string>();
        SHUtil.ForToArray (EditorBuildSettings.scenes, (pScene) =>
        {
            if (false == pScene.enabled) 
                return;

            pScenes.Add(pScene.path);
        });
        return pScenes.ToArray();
    }

    // 유틸 : App 빌드
    static void BuildApplication(string[] strScenes, BuildTarget eTarget, BuildOptions eOptions)
    {
        Debug.LogFormat("** Build Start({0}) -> {1}", eTarget, DateTime.Now.ToString("yyyy-MM-dd [ HH:mm:ss ]"));
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(eTarget);

            string strFileName = (BuildTarget.Android == eTarget) ? string.Format("{0}.apk", Single.AppInfo.GetAppName()) : Single.AppInfo.GetAppName();
            string strResult = BuildPipeline.BuildPlayer(strScenes, strFileName, eTarget, eOptions);
            if (0 < strResult.Length)
                throw new Exception("BuildPlayer failure: " + strResult);
        }
        Debug.LogFormat("** Build End({0}) -> {1}", eTarget, DateTime.Now.ToString("yyyy-MM-dd [ HH:mm:ss ]"));
    }

    // 유틸 : PackingAssetBundles 패킹
    static void PackingAssetBundles(BuildTarget eTarget, eBundlePackType eType, bool bIsDelOriginal)
    {
        Debug.LogFormat("** AssetBundles Packing Start({0}) -> {1}", eTarget, DateTime.Now.ToString("yyyy-MM-dd [ HH:mm:ss ]"));
        {
            SHAssetBundleMaker.PackingAssetBundle(eTarget, eType, bIsDelOriginal);
            AssetDatabase.Refresh();
        }
        Debug.LogFormat("** AssetBundles Packing End({0}) -> {1}", eTarget, DateTime.Now.ToString("yyyy-MM-dd [ HH:mm:ss ]"));
    }
    
    // 유틸 : ClientConfiguration파일 업데이트
    static void WriteClientConfiguration(string strConfigurationCDN, eServiceMode eMode)
    {
        var pConfigFile = new JsonClientConfiguration();
        pConfigFile.LoadJson(pConfigFile.m_strFileName);
        pConfigFile.m_strConfigurationCDN   = strConfigurationCDN;

        if (eServiceMode.None != eMode)
            pConfigFile.m_strServiceMode = eMode.ToString();
        else
            pConfigFile.m_strServiceMode = string.Empty;

        pConfigFile.SaveJsonFile(SHPath.GetPathToJson());
    }

    // 유틸 : 한국 ServerConfiguration CDN 주소
    static string GetURLToConfigurationCDNOfKorea(eServiceMode eMode)
    {
        return string.Format("{0}/{1}", "http://blueasa.synology.me/home/shmhlove/KOR", eMode);
    }
    #endregion
}