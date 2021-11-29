using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FBMan
{

	static private FBMan _instance= null;

	static public FBMan Instance
	{
		get
		{
			if( _instance == null )
				_instance= new FBMan();
			return _instance;
		}
	}

	public bool IsInitialized { private set; get; }

#if USE_FB

	void Awake()
	{
		//var parameters = new Dictionary<string, object>();
		//string s= Facebook.Unity.AppEventParameterName.Level;
		//parameters[ Facebook.Unity.AppEventParameterName.Level ] = "1";
		//Facebook.Unity.FB.LogAppEvent(
		//	Facebook.Unity.AppEventName.AchievedLevel, parameters
		//);
		//s= s+ "3";
	}

	public void InitFB()
	{
		Awake();

		UIProc.Instance.DebugOut( "FBMan.InitFB - IsInitialized: " + Facebook.Unity.FB.IsInitialized );

		if( !Facebook.Unity.FB.IsInitialized )
			Facebook.Unity.FB.Init( OnFacebookInitComplete );
		else
			Facebook.Unity.FB.ActivateApp();
	}

	void OnFacebookInitComplete()
	{
		UIProc.Instance.DebugOut( "FBMan.OnFacebookInitComplete - IsLoggedIn: " + Facebook.Unity.FB.IsLoggedIn );

		if( Facebook.Unity.FB.IsInitialized )
			Facebook.Unity.FB.ActivateApp();

		if( Facebook.Unity.FB.IsLoggedIn )
			Login();
		else
			IsInitialized = true;
	}

	public static void SendEvent( string name )
	{
		if( Facebook.Unity.FB.IsInitialized )
			Facebook.Unity.FB.LogAppEvent( name );
	}

	public static void SendEvent( string name, Dictionary<string, object> parameters )
	{
		if( Facebook.Unity.FB.IsInitialized )
			Facebook.Unity.FB.LogAppEvent( name, default(float?), parameters );
	}

#else

	public void InitFB() { }

	public static void SendEvent( string name ) {}

	public static void SendEvent( string name, Dictionary<string, object> parameters ) {}

#endif

	public void Logout() { }

	public void Login() { }

}
