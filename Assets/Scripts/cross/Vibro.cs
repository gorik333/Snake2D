using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Vibro
{

#if UNITY_ANDROID
	static float _interval= 0.05f;
#else
	static float _interval= 0.02f;
#endif

	static List<float> _time= new List<float>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };


	static bool IntCalc( int i )
	{
		float t= Time.time - _time[i];
		bool v= t >= _interval;
		if( v )
			_time[i]= Time.time;
		return v;
	}

	public static void Mute( bool mute )
	{
		Taptic.tapticOn= !mute;
	}

	public static void Light()
	{
		if( IntCalc(0) )
			Taptic.Light();
	}

	public static void Medium()
	{
		if( IntCalc(1) )
			Taptic.Medium();
	}

	public static void Heavy()
	{
		if( IntCalc(2) )
			Taptic.Heavy();
	}


}
