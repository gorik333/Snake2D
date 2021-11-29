using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;


public class NativeRateInApp : MonoBehaviour
{

#region [Native]
	#if UNITY_IOS && !UNITY_EDITOR
		[DllImport("__Internal")]
	static extern bool NativeRateInApp_Show();
#endif
	#endregion


	////////////////////


	static public bool ShowRate()
	{
		if( CanRateInApp() )
		{
#if UNITY_IOS && !UNITY_EDITOR
			return NativeRateInApp_Show();
#endif
		}
		return false;
	}

	static public bool CanRateInApp()
	{
#if UNITY_IOS && !UNITY_EDITOR
		string [] nums= UnityEngine.iOS.Device.systemVersion.Split( '.' );

		int n1= int.Parse( nums[0] );
		int n2= int.Parse( nums[1] );

		if( n1 > 10 || ( n1 == 10 && n2 >= 3 ) )
		{
			return true;
		}
#endif
		return false;
	}

}
