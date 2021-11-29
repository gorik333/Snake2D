using UnityEngine;
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;




public static class TACFuncs
{
	#region [Native]
#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")]
	public static extern long TimeAntiCheat_SystemUptime();
#endif
	#endregion
}


public class TAC_Core
{
	virtual public DateTime Now() { return DateTime.Now; }

	virtual public void SessionStart() {}

	virtual public void SessionEnd() {}

	virtual public long TimeDiff() { return 0; }

	virtual public string GetDebugStr() { return "no str"; }

	virtual public long SystemUptime() { return 0; }
}


public class TAC_Android : TAC_Core
{
	private DateTime m_timeStartApp;
	private long	 m_diffTime= 0;

	string _str="";

	public TAC_Android()
	{
		m_timeStartApp= DateTime.Now.AddSeconds( -Time.realtimeSinceStartup );

		if( PlayerPrefs.HasKey( "last_os_lifetime" ) )
		{
			long prevTimeOS= PlayerPrefs.GetInt( "last_os_lifetime" );
			long currTimeOS= SystemUptime();

			long diff= currTimeOS - prevTimeOS;

			_str= " Diff: " + diff.ToString();
			_str+= "\n RealTime: " + Time.realtimeSinceStartup.ToString();

			if( diff > 0 )
			{
				m_timeStartApp= DateTime.Parse( PlayerPrefs.GetString("last_game_time") );
				m_timeStartApp= m_timeStartApp.AddSeconds( -Time.realtimeSinceStartup );
				m_diffTime= diff;
			}
			else
			{//reboot
				_str= "\nREBOOT DEVICE!";
			}
		}
	}

	override public string GetDebugStr(){ return _str; }

	override public DateTime Now()		{ return m_timeStartApp.AddSeconds( m_diffTime + Time.realtimeSinceStartup ); }

	override public long TimeDiff()		{ return m_diffTime; }

	override public void SessionStart() {}

	override public void SessionEnd()
	{
		PlayerPrefs.SetInt( "last_os_lifetime", (int)SystemUptime() );
		PlayerPrefs.SetString( "last_game_time", Now().ToString() );
	}

	override public long SystemUptime()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaObject jo = new AndroidJavaObject("android.os.SystemClock");
		long t= jo.CallStatic<long>("elapsedRealtime");
		return t / 1000;
#endif
		return 0;
	}
}



public class TAC_iOS : TAC_Android
{

	override public long SystemUptime()
	{
#if UNITY_IOS && !UNITY_EDITOR
		return TACFuncs.TimeAntiCheat_SystemUptime();
#endif
		return 0;
	}
}


public class TAC_Default : TAC_Android
{
	override public long SystemUptime() { return System.Environment.TickCount/1000; }
}


public class TimeAntiCheat : MonoBehaviour
{
	private TAC_Core _tacCore;

	private static TimeAntiCheat instance;

	public static TimeAntiCheat Instance
	{
		get
		{
			if( instance == null )
			{
				GameObject g = new GameObject("TimeAntiCheat_SINGLETON");
				instance = g.AddComponent<TimeAntiCheat>();
				DontDestroyOnLoad(g);
			}
			return instance;
		}
	}

	void Awake()
	{

#if UNITY_EDITOR
		_tacCore= new TAC_Default();
#else

#if UNITY_IOS
		_tacCore= new TAC_iOS();
#elif UNITY_ANDROID
		_tacCore= new TAC_Android();
#else
		_tacCore= new TAC_Default();
#endif

#endif
	}

	void OnApplicationPause( bool pause )
	{
		if( pause )
			SessionEnd();
		else
			SessionStart();
	}

	void OnApplicationQuit()
	{
		SessionEnd();
	}

	public string GetDebugStr()
	{
		return _tacCore.GetDebugStr();
	}

	public long SystemUptime()
	{
		return _tacCore.SystemUptime();
	}

	public DateTime Now()
	{
		return _tacCore.Now();
	}

	private void SessionStart()
	{
		_tacCore.SessionStart();
	}

	private void SessionEnd()
	{
		_tacCore.SessionEnd();
	}

	public long TimeDiff()
	{
		return _tacCore.TimeDiff();
	}

}
