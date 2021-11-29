using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



#if MOPUB_ADS


public class ADs_MoPub
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
    static private readonly string[] _bannerAdUnits = { "349ff4e39b3145f3bd7edbaeb00f1923" };
    static private readonly string[] _interstitialAdUnits = { "925d7b03210a4039a7793771be752b14" };
    static private readonly string[] _rewardedVideoAdUnits = { "6c247221667d48eeabdc77282da7df24" };
    static private readonly string[] _rewardedRichMediaAdUnits = { };

	//TEST
	//static private readonly string[] _bannerAdUnits = { "0ac59b0996d947309c33f59d6676399f" };
    //static private readonly string[] _interstitialAdUnits = { "4f117153f5c24fa6a3a92b818a5eb630" };
    //static private readonly string[] _rewardedVideoAdUnits = { "8f000bd5e00246de9c789eed39ff6096" };
    //static private readonly string[] _rewardedRichMediaAdUnits = { };
#elif UNITY_ANDROID || UNITY_EDITOR
    //static private readonly string[] _bannerAdUnits = { "b195f8dd8ded45fe847ad89ed1d016da" };
    //static private readonly string[] _interstitialAdUnits = { "24534e1901884e398f1253216226017e" };
    //static private readonly string[] _rewardedVideoAdUnits = { "920b6145fb1546cf8b5cf2ac34638bb7" };
    //static private readonly string[] _rewardedRichMediaAdUnits = { "a96ae2ef41d44822af45c6328c4e1eb1" };

	static private readonly string[] _bannerAdUnits = { "b58f90d4b5284e718ed6540c1816050c" };
    static private readonly string[] _interstitialAdUnits = { "07ed61c035b64747a88f1bee836b87b0" };
    static private readonly string[] _rewardedVideoAdUnits = { "697c10060ea14ddaa2a513feaa5a2b28" };
#endif

///

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

			
        var anyAdUnitId = _interstitialAdUnits[0];

        MoPub.InitializeSdk( new MoPub.SdkConfiguration
		{
            AdUnitId = anyAdUnitId,

            // Set desired log level here to override default level of MPLogLevelNone
            LogLevel = MoPubBase.LogLevel.MPLogLevelDebug,

            // Uncomment the following line to allow supported SDK networks to collect user information on the basis
            // of legitimate interest.
            //AllowLegitimateInterest = true,

            // Specify the mediated networks you are using here:
            MediatedNetworks = new MoPub.MediatedNetwork[]
            {
				new MoPub.SupportedNetwork.AppLovin
                {
                    // Network adapter configuration settings (initialization).
                    NetworkConfiguration = new Dictionary<string,object> { { "sdk_key", "9m-B8oO0dPizI7yVtZtwO4PcGF6Ap-dzlc8tPXu1ya-EhxkZUaUueuUw8b5xaivJJ1AkZ2lVdkkKeRu81M1gEV" } },
                },
			},
        });

		if( adType == ADType.All || adType == ADType.Banner )
        	MoPub.LoadBannerPluginsForAdUnits( _bannerAdUnits );

		if( adType == ADType.All || adType == ADType.Interstitial )
        	MoPub.LoadInterstitialPluginsForAdUnits( _interstitialAdUnits );

		if( adType == ADType.All || adType == ADType.Rewarded )
        	MoPub.LoadRewardedVideoPluginsForAdUnits( _rewardedVideoAdUnits );

        //MoPub.LoadRewardedVideoPluginsForAdUnits( _rewardedRichMediaAdUnits );

		///

		MoPubManager.OnSdkInitializedEvent += OnSdkInitializedEvent;

		MoPubManager.OnAdFailedEvent+= BannerAdFailedEvent;
		MoPubManager.OnAdLoadedEvent+= BannerAdLoadedEvent;

		MoPubManager.OnInterstitialLoadedEvent += InterstitialAdLoadedEvent;
		MoPubManager.OnInterstitialShownEvent += InterstitialAdShowSucceededEvent;
		MoPubManager.OnInterstitialFailedEvent += InterstitialAdFailedEvent;
		MoPubManager.OnInterstitialExpiredEvent += InterstitialAdExpiredEvent;
		MoPubManager.OnInterstitialDismissedEvent += InterstitialAdDismissedEvent;

		MoPubManager.OnRewardedVideoLoadedEvent += RewardedVideoAdLoadedEvent;
		MoPubManager.OnRewardedVideoReceivedRewardEvent += RewardedVideoAdRewardedEvent;
		MoPubManager.OnRewardedVideoClosedEvent += RewardedVideoAdClosedEvent;
		MoPubManager.OnRewardedVideoFailedEvent += RewardedVideoAdFailedEvent;
		MoPubManager.OnRewardedVideoExpiredEvent+= RewardedVideoAdExpiredEvent;

		MoPubManager.OnRewardedVideoClickedEvent+= RewardedVideoAdClickedEvent;
		MoPubManager.OnRewardedVideoLeavingApplicationEvent+= RewardedVideoAdLeavingEvent;
	}

	static public void Cache( ADType adType )
	{
		if( adType == ADType.Interstitial )
		{
			foreach(var adUnit in _interstitialAdUnits)
				MoPub.RequestInterstitialAd( adUnit );
		}

		if( adType == ADType.Rewarded )
		{
			foreach(var adUnit in _rewardedVideoAdUnits)
				MoPub.RequestRewardedVideo( adUnit );
		}
	}

	public static bool HasInterstitial()
	{
		if( _interstitialFailed )
		{
			_interstitialFailed= false;
			Cache( ADType.Interstitial );
			return false;
		}
		return _interstitialsLoaded.Count > 0;
	}

	public static bool ShowInterstitial()
	{
		if( HasInterstitial() )
		{
			MoPub.ShowInterstitialAd( _interstitialsLoaded[0] );

			_interstitialsLoaded.RemoveAt( 0 );

			return true;
		}
		return false;
	}

	public static bool HasRewardedVideo()
	{
		if( _rewardedFailed )
		{
			_rewardedFailed= false;
			Cache( ADType.Rewarded );
			return false;
		}
		return _rewardedLoaded.Count > 0;
	}

	public static bool ShowRewardedVideo()
	{
		_rewardedFinished= false;

		if( HasRewardedVideo() )
		{
			MoPub.ShowRewardedVideo( _rewardedLoaded[0] );

			_rewardedLoaded.RemoveAt( 0 );

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

			foreach(var bannerAdUnit in _bannerAdUnits)
				MoPub.CreateBanner( bannerAdUnit, top ? MoPubBase.AdPosition.TopCenter : MoPubBase.AdPosition.BottomCenter );
		}

		return true;
	}

	public static void HideBanner( bool top )
	{
		_bannerCreated= false;

		foreach(var bannerAdUnit in _bannerAdUnits)
			MoPub.DestroyBanner( bannerAdUnit );
	}

	public static bool HasBanner()
	{
		return _bannerCreated; //?
	}

	////

	static void OnSdkInitializedEvent( string adUnitId )
    {
		_initializedCore= true;

		UIProc.Instance.DebugOut( "INIT AD SDK COMPLETED >> " + adUnitId );

        Cache( ADType.Rewarded );
		Cache( ADType.Interstitial );

		if( _showBanneAuto && ( _adTypes == ADType.All || _adTypes == ADType.Banner ) )
			ShowBanner( false );
    }


	static void BannerAdFailedEvent( string adUnitId, string str )
	{
		UIProc.Instance.DebugOut( "Banner FAILED " + adUnitId + " " + str );
	}

	static void BannerAdLoadedEvent( string adUnitId, float height )
	{
		ADs.onBannerLoaded( "mopub" );
	}


	static void InterstitialAdLoadedEvent( string adUnitId )
	{
		UIProc.Instance.DebugOut( "Interst Loaded " + adUnitId );

		_interstitialsLoaded.Add( adUnitId );
	}

	static void InterstitialAdShowSucceededEvent( string adUnitId )
	{
		UIProc.Instance.DebugOut( "Interst Show Success " + adUnitId );

		ADs.onInterstitialCompleted( "mopub" );

		Cache( ADType.Interstitial );
	}

	static void InterstitialAdFailedEvent( string adUnitId, string str )
	{
		UIProc.Instance.DebugOut( "Interst FAILED " + adUnitId + " > " + str );

		_interstitialFailed= true;
	}

	static void InterstitialAdExpiredEvent( string adUnitId )
	{
		UIProc.Instance.DebugOut( "Interst Expired " + adUnitId );

		_interstitialsLoaded.Remove( adUnitId );
		Cache( ADType.Interstitial );
	}

	static void InterstitialAdDismissedEvent( string adUnitId )
	{
		UIProc.Instance.DebugOut( "Interst Dismissed !! [!]" + adUnitId );

		_interstitialsLoaded.Remove( adUnitId );
		Cache( ADType.Interstitial );
	}


	static void RewardedVideoAdLoadedEvent( string adUnitId )
	{
		UIProc.Instance.DebugOut( "REW Loaded " + adUnitId );

		_rewardedLoaded.Add( adUnitId );
	}

	static void RewardedVideoAdRewardedEvent( string adUnitId, string str, float f )
	{
		UIProc.Instance.DebugOut( "REW " + adUnitId + " > " + str + " = " + f.ToString() + " ? " + adUnitId );

		_rewardedFinished= true;

		ADs.onRewardedCompleted( "mopub" );
	}
		
	static void RewardedVideoAdClosedEvent( string adUnitId )
	{
		UIProc.Instance.DebugOut( "REW Closed " + adUnitId );

		if( _rewardedFinished )
		{
			ADs.onRewardedClosed( "mopub", 0 );

			_rewardedFinished= false;
		}

		Cache( ADType.Rewarded );
	}

	static void RewardedVideoAdExpiredEvent( string adUnitId )
	{
		UIProc.Instance.DebugOut( "REW EXPIRED " + adUnitId );

		_rewardedLoaded.Remove( adUnitId );
		Cache( ADType.Rewarded );
	}

	static void RewardedVideoAdFailedEvent( string adUnitId, string  str )
	{
		UIProc.Instance.DebugOut( "REW FAILED ! " + adUnitId + " " + str );

		_rewardedFailed= true;
	}

	static void RewardedVideoAdClickedEvent( string adUnitId )
	{
	}

	static void RewardedVideoAdLeavingEvent( string adUnitId )
	{
	}

}

#endif