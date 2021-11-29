using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



#if APPLOVIN_ADS


public class ADs_Applovin
{
	static private bool _initialized= false;

	static private bool _initializedCore= false;

	static private ADType _adTypes;

	static private Action<int> _RewardedCompleted;

	static private bool _rewardedFinished= false;

	static private bool _bannerCreated= false;

	static private bool _showBanneAuto= true;

	static List<string> _rewardedLoaded= new List<string>();
	static List<string> _interstitialsLoaded= new List<string>();

	static bool _rewardedFailed= false;
	static bool _interstitialFailed= false;

///

#if UNITY_IOS
	static string _interstitialAdUnitId = "";
	static string _rewardedAdUnitId = "";
	static string _bannerAdUnitId = "";
#else
	static string _interstitialAdUnitId = "3834fedd2b54adbc";
	static string _rewardedAdUnitId = "aec4aedbda4747f7";
	static string _bannerAdUnitId = "66b9e8eeff8c0d3f";
#endif


	static public void OnApplicationPause( bool isPaused )
	{
	}

	static public void Init( ADType adType= ADType.All, bool showBannerAuto= true )
	{
		if( _initialized )
			return;

		_adTypes= adType;

		_initialized= true;

		_showBanneAuto= showBannerAuto;

			
        ///

		MaxSdkCallbacks.OnSdkInitializedEvent += OnSdkInitializedEvent;

		MaxSdk.SetSdkKey( "6AQkyPv9b4u7yTtMH9PT40gXg00uJOTsmBOf7hDxa_-FnNZvt_qTLnJAiKeb5-2_T8GsI_dGQKKKrtwZTlCzAR" );

		MaxSdk.InitializeSdk();

		///

		// Attach callback
	    MaxSdkCallbacks.OnInterstitialLoadedEvent += OnInterstitialLoaded;
	    MaxSdkCallbacks.OnInterstitialLoadFailedEvent += OnInterstitialFailed;
	    MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += InterstitialFailedToDisplay;
	    MaxSdkCallbacks.OnInterstitialHiddenEvent += OnInterstitialDismissed;
		MaxSdkCallbacks.OnInterstitialDisplayedEvent += OnInterstitialDisplayed;


		MaxSdkCallbacks.OnRewardedAdLoadedEvent += OnRewardedAdLoaded;
    	MaxSdkCallbacks.OnRewardedAdHiddenEvent += RewardedVideoAdClosedEvent;
    	MaxSdkCallbacks.OnRewardedAdLoadFailedEvent += OnRewardedAdFailed;
    	MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent += OnRewardedAdFailedToDisplay;
		MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += OnRewardedAdReceivedReward;
		MaxSdkCallbacks.OnRewardedAdDisplayedEvent += OnRewardedAdDisplayedEvent;

		//MaxSdkCallbacks.OnRewardedAdHiddenEvent

		MaxSdkCallbacks.OnBannerAdLoadedEvent += OnBannerAdLoadedEvent;
		MaxSdkCallbacks.OnBannerAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
	}
		
	static public void Cache( ADType adType )
	{
		if( adType == ADType.Interstitial )
		{
			MaxSdk.LoadInterstitial( _interstitialAdUnitId );
		}

		if( adType == ADType.Rewarded )
		{
			MaxSdk.LoadRewardedAd( _rewardedAdUnitId );
		}
	}

	public static bool HasInterstitial()
	{
		/*if( _interstitialFailed )
		{
			_interstitialFailed= false;
			Cache( ADType.Interstitial );
			return false;
		}*/
		return MaxSdk.IsInterstitialReady( _interstitialAdUnitId );
	}

	public static bool ShowInterstitial()
	{
		if( HasInterstitial() )
		{
			MaxSdk.ShowInterstitial( _interstitialAdUnitId );
			return true;
		}
		return false;
	}

	public static bool HasRewardedVideo()
	{
		/*if( _rewardedFailed )
		{
			_rewardedFailed= false;
			Cache( ADType.Rewarded );
			return false;
		}*/
		return MaxSdk.IsRewardedAdReady( _rewardedAdUnitId );
	}

	public static bool ShowRewardedVideo()
	{
		_rewardedFinished= false;

		if( HasRewardedVideo() )
		{
			MaxSdk.ShowRewardedAd( _rewardedAdUnitId );

			return true;
		}
		//_rewEvent(1);
		return false;
	}

	public static bool ShowBanner( bool top )
	{
		if( !_bannerCreated && _initializedCore )
		{
			_bannerCreated= true;

			MaxSdk.CreateBanner( _bannerAdUnitId, top ? MaxSdkBase.BannerPosition.TopCenter : MaxSdkBase.BannerPosition.BottomCenter );

			MaxSdk.ShowBanner( _bannerAdUnitId );
		}

		return true;
	}

	public static void HideBanner( bool top )
	{
		_bannerCreated= false;

		MaxSdk.HideBanner( _bannerAdUnitId );

		MaxSdk.DestroyBanner( _bannerAdUnitId );
	}

	public static bool HasBanner()
	{
		return _bannerCreated; //?
	}

	////

	static void CheckFailed()
	{
		UIProc.Instance.DebugOut( "CHECK FAILED LOAD" );

		if( _interstitialFailed )
		{
			_interstitialFailed= false;
			Cache( ADType.Interstitial );
		}

		if( _rewardedFailed )
		{
			_rewardedFailed= false;
			Cache( ADType.Rewarded );
		}
	}

	////

	static void OnSdkInitializedEvent( MaxSdkBase.SdkConfiguration sdkConfiguration )
    {
		UIProc.Instance.DebugOut( "INIT AD SDK COMPLETED >> " + sdkConfiguration.ToString() );

		_initializedCore= true;

		MaxSdk.SetHasUserConsent(true);

		if( _adTypes == ADType.All || _adTypes == ADType.Rewarded )
        	Cache( ADType.Rewarded );

		if( _adTypes == ADType.All || _adTypes == ADType.Interstitial )
			Cache( ADType.Interstitial );

		if( _showBanneAuto && ( _adTypes == ADType.All || _adTypes == ADType.Banner ) )
			ShowBanner( false );

		//MaxSdk.ShowMediationDebugger();
    }

	// BANNERS

	static void OnBannerAdLoadFailedEvent( string adUnitId, int err )
	{
		UIProc.Instance.DebugOut( "Banner FAILED " + adUnitId + " >err: " + err );
	}

	static void OnBannerAdLoadedEvent( string adUnitId )
	{
		ADs.onBannerLoaded( "applovin" );
	}

	// INTERSTITIALS

	static void OnInterstitialLoaded( string adUnitId )
	{
		UIProc.Instance.DebugOut( "Interst Loaded " + adUnitId );
	}

	static void OnInterstitialDisplayed( string adUnitId )
	{
		UIProc.Instance.DebugOut( "Interst Show Success " + adUnitId );

		ADs.onInterstitialCompleted( "applovin" );

		ADs.onInterstitialShown();

		Cache( ADType.Interstitial );
	}

	static void OnInterstitialFailed( string adUnitId, int error )
	{
		UIProc.Instance.DebugOut( "Interst FAILED " + adUnitId + " >err: " + error );

		_interstitialFailed= true;

		TimerCall.RunLater( CheckFailed, 2f );
	}

	static void InterstitialFailedToDisplay( string adUnitId, int error )
	{
		UIProc.Instance.DebugOut( "Interst fail Display! " + adUnitId + " >err: " + error );

		Cache( ADType.Interstitial );
	}

	static void OnInterstitialDismissed( string adUnitId )
	{
		UIProc.Instance.DebugOut( "Interst Dismissed !! [!]" + adUnitId );

		ADs.onInterstitialHidden();

		Cache( ADType.Interstitial );
	}

	// REWARDED

	
	static void OnRewardedAdLoaded( string adUnitId )
	{
		UIProc.Instance.DebugOut( "REW Loaded " + adUnitId );
	}

	static void OnRewardedAdDisplayedEvent( string adUnitId )
	{
		UIProc.Instance.DebugOut( "REW SHOWN " + adUnitId );

		ADs.onRewardedShown();
	}

	static void OnRewardedAdReceivedReward( string adUnitId, MaxSdk.Reward reward )
	{
		UIProc.Instance.DebugOut( "REW " + adUnitId + " > " + reward.ToString() );

		_rewardedFinished= true;

		ADs.onRewardedCompleted( "applovin" );
	}
		
	static void RewardedVideoAdClosedEvent( string adUnitId )
	{
		UIProc.Instance.DebugOut( "REW Closed " + adUnitId );

		if( _rewardedFinished )
		{
			ADs.onRewardedClosed( "applovin", 0 );

			_rewardedFinished= false;
		}

		ADs.onRewardedHidden();

		Cache( ADType.Rewarded );
	}

	static void OnRewardedAdFailed( string adUnitId, int error )
	{
		UIProc.Instance.DebugOut( "REW FAILED ! " + adUnitId + " >err: " + error );

		_rewardedFailed= true;

		TimerCall.RunLater( CheckFailed, 2f );
	}

	static void OnRewardedAdFailedToDisplay( string adUnitId, int error )
	{
		UIProc.Instance.DebugOut( "FAILED TO DISPLAY " + adUnitId );

		Cache( ADType.Rewarded );
	}

}

#endif