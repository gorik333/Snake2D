using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EffectProc : MonoBehaviour
{
	List<Effect> listEffects = new List<Effect>();

	public static EffectProc GetProc( GameObject go )
	{
		EffectProc proc= go.GetComponent<EffectProc>();

		if( proc != null )
			return proc;

		return go.AddComponent<EffectProc>();
	}
	
	public Effect AddEffect( Effect fx )
	{
		listEffects.Add( fx );

		fx.SetTargetObj( this.gameObject );
		//fx.Start();

		return fx;
	}

	public Effect BackEffect()
	{
		if( listEffects.Count > 0 )
			return listEffects[ listEffects.Count - 1 ];
		return null;
	}

	public void ClearEffects()
	{
		listEffects.Clear();
	}

	public bool HasEffects()
	{
		return listEffects.Count > 0;
	}
	
	void UpdateEffects()
	{
		for(int i= 0; i< listEffects.Count; ++i)
		{
			Effect fx= listEffects[i];

			if( !fx.isStarted )
				fx.Start();

			fx.Update();

			if( fx.IsEnd() )
			{
				listEffects.RemoveAt( i );
				--i;
			}
			else
			{
				if( !fx.IsNext() )
					break;
			}
		}
	}

	void Update()
	{
		UpdateEffects();
	}

}
