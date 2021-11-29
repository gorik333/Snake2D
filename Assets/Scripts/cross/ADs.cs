using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum ADType
{
	All,
	Banner,
	Interstitial,
	Rewarded,
	None,
}

public class ADs
{

	static private bool _initialized= false;

	static private Action<int> _RewardedCompleted;

	static public Action<bool> _AdShown;
	static public Action<bool> _AdHidden;


	static private ADs _instance= null;

	static private ADs Instance
	{
		get
		{
			if( _instance == null )
				_instance= new ADs();
			return _instance;
		}
	}


	public static void OnApplicationPause( bool pause )
	{
		ADs_Applovin.OnApplicationPause( pause );
	}

	static public void Init( ADType adType= ADType.All, bool showBannerAuto= true )
	{
		_initialized= true;

		ADs_Applovin.Init( adType, showBannerAuto );
	}

	static public void Cache( ADType adType )
	{
	}

	public static bool HasInterstitial()
	{
		if( Application.isEditor )
			return true;
		return ADs_Applovin.HasInterstitial();
	}

	public static bool ShowInterstitial()
	{
		if( Application.isEditor )
			return true;

		if( Stats.Instance.adsRemoved )
			return false;
		
		return ADs_Applovin.ShowInterstitial();
	}

	public static bool HasRewardedVideo()
	{
		if( Application.isEditor )
			return true;//UnityEngine.Random.Range(0,2) == 0;

		return ADs_Applovin.HasRewardedVideo();
	}

	public static bool ShowRewardedVideo( Action<int> _rewEvent )
	{
		if( HasRewardedVideo() )
			_RewardedCompleted= _rewEvent;
		else
			return false;

		if( Application.isEditor )
		{
			_rewEvent.Invoke(0);
			return true;
		}

		return ADs_Applovin.ShowRewardedVideo();
	}

	public static bool ShowBanner( bool top )
	{
		if( Stats.Instance.adsRemoved )
			return false;
		return ADs_Applovin.ShowBanner( top );
	}

	public static void HideBanner( bool top )
	{
		ADs_Applovin.HideBanner( top );
	}

	public static bool HasBanner()
	{
		bool has= ADs_Applovin.HasBanner();
		return has;
	}


	/// /////////////////////


	static public void onBannerLoaded( string adSource )
	{
		UIProc.Instance.DebugOut( "onBannerLoaded" );

		int c= Stats.Instance.ParamIncInt( "ad_impr_banner" );

		Analytics.SendEvent( "AdImpression", "ad_type", "banner", "counter", c );

		YaMetrica.SendEvent( "video_ads_watch", "ad_type", "banner" );
	}

	static public void onInterstitialCompleted( string adSource )
	{
		UIProc.Instance.DebugOut( "onInterstitialCompleted" );

		int c= Stats.Instance.ParamIncInt( "ad_impr_interstitial" );

		Analytics.SendEvent( "AdImpression", "ad_type", "interstitial", "counter", c );

		YaMetrica.SendEvent( "video_ads_watch", "ad_type", "interstitial" );
	}

	static public void onRewardedCompleted( string adSource )
	{
		UIProc.Instance.DebugOut( "onRewardedCompleted" );

		int c= Stats.Instance.ParamIncInt( "ad_impr_rewarded" );

		Analytics.SendEvent( "AdImpression", "ad_type", "rewarded", "counter", c );


		if( Stats.Instance.GetRemoteBool( "ad_interstitial_rewarded_interval", true ) )
		{
			Stats.Instance.SetTempVar( "last_show_interstitial_time", (int)Time.unscaledTime );
		}

		//_RewardedCompleted( 0 );

		//FBMan.SendPurchase( 0.01f );
	}

	static public void onRewardedClosed( string adSource, int value )
	{
		UIProc.Instance.DebugOut( "onRewardedClosed" );

		_RewardedCompleted( value );
	}

	static public void onRewardedShown()
	{
		_AdShown.Invoke( true );
	}

	static public void onRewardedHidden()
	{
		_AdHidden.Invoke( true );
	}

	/// /////////////////////


	static public void onInterstitialShown()
	{
		_AdShown.Invoke( false );
	}

	static public void onInterstitialHidden()
	{
		_AdHidden.Invoke( false );
	}


	public static bool ShowInterstitialAuto()
	{
		if( Stats.Instance.adsRemoved )
			return false;

		bool show= false;

		if( Stats.Instance.adCounter == Stats.Instance.GetRemoteInt("ad_interstitial_show_at_level",-1) )
		{
			show= ShowInterstitial();

			Analytics.SendEvent( "SHOW_INTERSTITIAL", "has", show?1:0 );
		}
		else
		if( Stats.Instance.adCounter >= Stats.Instance.GetRemoteInt("ad_interstitial_start_at",2) )
		{
			string dayParam= "ad_interstitial_interval_day" + Stats.Instance.DaysFromStart().ToString();

			int defInterval= Stats.Instance.GetRemoteInt( "ad_interstitial_interval_day_x", 38 );

			int showInterval= Stats.Instance.GetRemoteInt( dayParam, defInterval );

			show= ShowInterstitialInterval( showInterval );
		}

		++Stats.Instance.adCounter;

		return show;
	}

	public static bool ShowInterstitialInterval( int seconds= 30 )
	{
		if( Stats.Instance.adsRemoved )
			return false;

		int currTime= (int)Time.unscaledTime;
		int lastShow= Stats.Instance.GetTempVarInt( "last_show_interstitial_time", currTime - seconds );

		if( currTime - lastShow >= seconds )
		{
			bool show= ShowInterstitial();

			if( show )
				Stats.Instance.SetTempVar( "last_show_interstitial_time", currTime );

			Analytics.SendEvent( "SHOW_INTERSTITIAL", "has", show?1:0 );

			return show;
		}

		return false;
	}

}
