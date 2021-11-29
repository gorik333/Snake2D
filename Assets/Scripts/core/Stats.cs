
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.IO;



public class Stats
{
	private Dictionary<string,object>	_tempVars= new Dictionary<string,object>();

	private System.DateTime				_firstStart;

	private int 		_scores;

	private static Stats _instance;

	//>

	public bool SpendCoins( int need )
	{
		if( coins >= need )
		{
			coins-= need;
			return true;
		}
		return false;
	}


	public int coins
	{
		get { return PlayerPrefs.GetInt( "num_coins", 100 ); }
		set
		{
			PlayerPrefs.SetInt( "num_coins", value );
			PlayerPrefs.Save();
		}
	}

	public int level
	{
		get { return PlayerPrefs.GetInt( "curr_level", 1 ); }
		set
		{
			PlayerPrefs.SetInt( "curr_level", value );
			PlayerPrefs.Save();
		}
	}

	public int stage
	{
		get { return PlayerPrefs.GetInt( "curr_stage", 1 ); }
		set
		{
			PlayerPrefs.SetInt( "curr_stage", value );
			PlayerPrefs.Save();
		}
	}

	public int scores
	{
		get { return _scores; } // PlayerPrefs.GetInt( "scores", 0 ); }
		set
		{
			_scores= value;
			//PlayerPrefs.SetInt( "scores", value );
			//PlayerPrefs.Save();
		}
	}

	public int bestScores
	{
		get { return PlayerPrefs.GetInt( "best_scores", 0 ); }
		set
		{
			if( value > PlayerPrefs.GetInt( "best_scores", 0 ) )
			{
				PlayerPrefs.SetInt( "best_scores", value );
				PlayerPrefs.Save();
			}
		}
	}

	public int skin
	{
		get { return PlayerPrefs.GetInt( "skin", 0 ); } // -1
		set
		{
			PlayerPrefs.SetInt( "skin", value );
			PlayerPrefs.Save();
		}
	}



	public int adCounter
	{
		get { return PlayerPrefs.GetInt( "ad_counter", 0 ); }
		set
		{
			PlayerPrefs.SetInt( "ad_counter", value );
			PlayerPrefs.Save();
		}
	}

	public bool adsRemoved
	{
		get { return PlayerPrefs.GetInt( "ads_removed", 0 ) != 0; }
		set
		{
			PlayerPrefs.SetInt( "ads_removed", value ? 1 : 0 );
			PlayerPrefs.Save();
		}
	}

	public bool muteSFX
	{
		get { return PlayerPrefs.GetInt( "sfx_mute", 0 ) != 0; }
		set
		{
			PlayerPrefs.SetInt( "sfx_mute", value ? 1 : 0 );
			PlayerPrefs.Save();
		}
	}

	public bool muteVibro
	{
		get { return PlayerPrefs.GetInt( "vibro_mute", 0 ) != 0; }
		set
		{
			PlayerPrefs.SetInt( "vibro_mute", value ? 1 : 0 );
			PlayerPrefs.Save();
		}
	}

	//////////

	public int RewardedCount
	{
		get { return PlayerPrefs.GetInt( "rewarded_counter", 0 ); }
		set
		{
			PlayerPrefs.SetInt( "rewarded_counter", value );
			PlayerPrefs.Save();
		}
	}

	//////////
	///

	public bool HasTimer( string name )
	{
		if( PlayerPrefs.HasKey( "timer_" + name ) )
		{
			return GetTimer( name ) > 0;
		}
		return false;
	}

	public void SetTimer( string name, int seconds )
	{
		string s= TimeNow().AddSeconds( seconds ).ToString();
		PlayerPrefs.SetString( "timer_" + name, s );
		PlayerPrefs.Save();
	}

	public float GetTimer( string name )
	{
		string s= PlayerPrefs.GetString( "timer_" + name, TimeNow().ToLongTimeString() );
		System.DateTime dt= System.DateTime.Parse( s );
		System.TimeSpan span= dt - TimeNow();
		return (float)span.TotalSeconds;
	}

	System.DateTime TimeNow()
	{
		//return System.DateTime.Now;
		return TimeAntiCheat.Instance.Now();
	}

	//////////
	///


	public static Stats Instance
	{
		get 
		{
			if( _instance == null )
			{
				_instance= new Stats();
			}	
			return _instance;
		}
	}

	private Stats()
	{
		Load();

		//RemoteSettings.Updated += new RemoteSettings.UpdatedEventHandler( RemoteSettingsUpdated );
		RemoteSettings.Completed += RemoteSettingsCompleted;
	}

	public void Load()
	{
		if( PlayerPrefs.HasKey( "first_start_day" ) == false )
		{
			_firstStart= TimeNow();
			PlayerPrefs.SetString( "first_start_day", _firstStart.ToLongDateString() );
			PlayerPrefs.Save();
		}

		_firstStart= System.DateTime.Parse( PlayerPrefs.GetString( "first_start_day" ) );
	}

	///

	public int DaysFromStart()
	{
		System.TimeSpan span= TimeNow().Subtract( _firstStart );
		return (int)span.TotalDays;
	}

	////////////
	/// TEMPLLARY VARS. not saved. valid only in active game session

	public void SetTempVar( string name, object v )
	{
		_tempVars[ name ]= v;
	}

	public bool HasTempVar( string name )
	{
		return _tempVars.ContainsKey( name );
	}

	public object GetTempVar( string name )
	{
		if( _tempVars.ContainsKey( name ) )
			return _tempVars[ name ];
		return null;
	}

	public int GetTempVarInt( string name, int default_int= 0 )
	{
		if( GetTempVar( name ) == null )
			return default_int;
		return (int)GetTempVar( name );
	}

	public float GetTempVarFloat( string name, float default_float= 0 )
	{
		if( GetTempVar( name ) == null )
			return default_float;
		return (float)GetTempVar( name );
	}

	//////////
	/// 

	public int ParamIncInt( string name, int addValue= 1 )
	{
		int v= GetParamInt( name ) + addValue;
		SetParam( name, v );
		return v;
	}

	public bool HasParam( string name )
	{
		return PlayerPrefs.HasKey( name );
	}

	public void SetParam( string name, int value )
	{
		PlayerPrefs.SetInt( name, value );
		PlayerPrefs.Save();
	}

	public void SetParam( string name, string value )
	{
		PlayerPrefs.SetString( name, value );
		PlayerPrefs.Save();
	}

	public void SetParam( string name, float value )
	{
		PlayerPrefs.SetFloat( name, value );
		PlayerPrefs.Save();
	}

	public int GetParamInt( string name, int defValue= 0 )
	{
		return PlayerPrefs.GetInt( name, defValue );
	}

	public float GetParamFloat( string name, float defValue= 0 )
	{
		return PlayerPrefs.GetFloat( name, defValue );
	}

	public string GetParamStr( string name, string defValue= "" )
	{
		return PlayerPrefs.GetString( name, defValue );
	}

	//////////
	///

	//////////
	/// REMOTE Variables. Uploaded from unity control center

	public int GetRemoteInt( string name, int defaultValue= 0 )
	{
		int i= RemoteSettings.GetInt( name, defaultValue );
		return i;
	}

	public float GetRemoteFloat( string name, float defaultValue= 0 )
	{
		return RemoteSettings.GetFloat( name, defaultValue );
	}

	public bool GetRemoteBool( string name, bool defaultValue= false )
	{
		bool b= RemoteSettings.GetBool( name, defaultValue );
		return b;
	}

	public string GetRemoteStr( string name, string defaultValue= "" )
	{
		return RemoteSettings.GetString( name, defaultValue );
	}

	public bool IsRemoteSettingsUpdated { get { return GetTempVarInt("remote_settings_updated",0) != 0; } }



	void RemoteSettingsUpdated()
	{
		SetTempVar( "remote_settings_updated", 1 );

		UIProc.Instance.DebugOut( "RemoteSettingsUpdated ++ !! --" );

		UIProc.Instance.DebugOut( ">>> " + Stats.Instance.GetRemoteInt( "inapp_rate_level_ios", -1 ) );
	}


	void RemoteSettingsCompleted( bool wasUpdatedFromServer, bool settingsChanged, int serverResponse )
	{
		if( serverResponse == 200 )
			SetTempVar( "remote_settings_updated", 1 );

		UIProc.Instance.DebugOut( "RemoteSettings COMPLETED ++ !! resp " + serverResponse + " Updt " + settingsChanged + " fromServ" + wasUpdatedFromServer );

		Analytics.SendEvent( "REMOTE_SETTINGS_UPDATED", "from_server", wasUpdatedFromServer, "settings_changed", settingsChanged, "response", serverResponse );
	}

}
