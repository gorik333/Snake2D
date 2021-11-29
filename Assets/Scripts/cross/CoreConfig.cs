using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CoreConfig
{

	//CORE

	static public string BundleIOS= "";

	static public string BundleAndroid= "block.breaker.game";

	static public string AppID_IOS= "";


	//FLURRY

	static public string FlurryIOS= "";

	static public string FlurryAndroid= "ZYSNHDZTX82W5QP8J3K6";


	//APPSFLYER

	static public string AppsFlyerKey= "r9vNC83N8nYpCzYGigyjUh";


	//TENJIN

	static public string TenjinKEY= "";





	//FUNCS

	public static string GetMyURL()
	{
#if UNITY_ANDROID
		return "https://play.google.com/store/apps/details?id=" + BundleAndroid;
#else
		return "https://itunes.apple.com/id" + AppID_IOS;
#endif
	}

}
