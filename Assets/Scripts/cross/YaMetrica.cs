using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class YaMetrica : MonoBehaviour
{

	void Start()
	{
		//Init();
	}

    public static void Init( bool firstStart )
    {
		return;
		//string apiKey = "0bb95b47-5cd3-421b-8129-f5519e3eb111"; //test
		string apiKey = "b0d2158b-960d-4df9-aabc-8cacc0c7c5e4"; //release

		YandexAppMetricaConfig yconf= new YandexAppMetricaConfig( apiKey );

		yconf.SessionTimeout= 300;
		yconf.CrashReporting= true;
		yconf.LocationTracking= true;
		yconf.StatisticsSending= true;
		yconf.HandleFirstActivationAsUpdate= !firstStart;

        AppMetrica.Instance.ActivateWithConfiguration( yconf );

		AppMetrica.Instance.SetUserProfileID( "" );
    }

	public static void SendEvent( string eventName )
	{
		AppMetrica.Instance.ReportEvent( eventName );
	}

	public static void SendEvent( string eventName, Dictionary<string, object> parameters )
	{
		if( parameters != null && parameters.ContainsKey( "day" ) == false )
			parameters.Add( "day", Stats.Instance.DaysFromStart() );

		AppMetrica.Instance.ReportEvent( eventName, parameters );
	}

	///

	public static void SendEvent( string eventName, string param, object value )
	{
		SendEvent( eventName, new Dictionary<string,object>() { { param, value } } );
	}

	public static void SendEvent( string eventName, string param1, object value1, string param2, object value2 )
	{
		SendEvent( eventName, new Dictionary<string,object>() { { param1, value1 }, { param2, value2 } } );
	}

	public static void SendEvent( string eventName, string param1, object value1, string param2, object value2, string param3, object value3 )
	{
		SendEvent( eventName, new Dictionary<string,object>() { { param1, value1 }, { param2, value2 }, { param3, value3 } } );
	}

	public static void SendEvent( string eventName, string param1, object value1, string param2, object value2, string param3, object value3, string param4, object value4 )
	{
		SendEvent( eventName, new Dictionary<string,object>() { { param1, value1 }, { param2, value2 }, { param3, value3 }, { param4, value4 } } );
	}

}
