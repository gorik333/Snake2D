using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameAnalytics
{

	public static void SendEventCounter( string evntName )
	{
		int i= Stats.Instance.GetParamInt( evntName );
		
		Stats.Instance.SetParam( evntName, i + 1 );

		if( i == 5 || i == 10 || i == 20 || i == 50 || i == 100 || i == 200 || i == 500 )
			Analytics.SendEvent( evntName + i.ToString() );
	}


	public static void SendDayCounter()
	{
		int day= Stats.Instance.DaysFromStart();

		int prev= Stats.Instance.GetParamInt( "prev_day", day );

		Stats.Instance.SetParam( "prev_day", day );

		int diff= day - prev;

		if( diff == 1 )
		{
			int cntr= 1 + Stats.Instance.GetParamInt( "day_counter", 0 );

			Stats.Instance.SetParam( "day_counter", cntr );

			if( cntr == 3 )
				Analytics.SendEvent( "users_3day" );

			if( cntr == 5 )
				Analytics.SendEvent( "users_5day" );
		}

		if( diff > 1 )
			Stats.Instance.SetParam( "day_counter", 0 );
	}

}
