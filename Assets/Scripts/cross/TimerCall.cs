using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimerCall : MonoBehaviour
{

	static TimerCall _instance= null;

	void Awake()
	{
		_instance= this;
	}

	static public void RunLater( System.Action method, float waitSeconds )
	{
        if( waitSeconds < 0 || method == null || _instance == null )
		{
             return;
		}
        _instance.StartCoroutine( RunLaterCoroutine(method, waitSeconds) );
    }

    static IEnumerator RunLaterCoroutine( System.Action method, float waitSeconds )
	{
         yield return new WaitForSecondsRealtime(waitSeconds);
         method();
    }

}
