
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class RemoteParams
{
	/*
	private Dictionary<string,object>	_tempVars= new Dictionary<string,object>();

	private static RemoteParams _instance;


	//////////
	///


	public static RemoteParams Instance
	{
		get 
		{
			if( _instance == null )
			{
				_instance= new RemoteParams();
			}	
			return _instance;
		}
	}

	private RemoteParams()
	{
		Load();

		//RemoteSettings.Updated += new RemoteSettings.UpdatedEventHandler( RemoteSettingsUpdated );
		RemoteSettings.Completed += RemoteSettingsCompleted;
	}

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

	void RemoteSettingsCompleted( bool wasUpdatedFromServer, bool settingsChanged, int serverResponse )
	{
		if( serverResponse == 200 )
			SetTempVar( "remote_settings_updated", 1 );

		UIProc.Instance.DebugOut( "RemoteSettings COMPLETED ++ !! resp " + serverResponse + " Updt " + settingsChanged + " fromServ" + wasUpdatedFromServer );

		Analytics.SendEvent( "REMOTE_SETTINGS_UPDATED", "from_server", wasUpdatedFromServer, "settings_changed", settingsChanged, "response", serverResponse );
	}
	*/
}
