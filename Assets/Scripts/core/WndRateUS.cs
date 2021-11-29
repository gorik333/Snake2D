
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class WndRateUS : Window
{
	public GameObject m_stars;

	public GameObject m_core;

	public Button m_btnRateUs;

	int m_starsRate= 0;



    void Start()
    {
    }

	public bool CanShow( bool force= false )
	{
#if UNITY_IOS
		return false;
#endif
		bool canShow= !Stats.Instance.HasParam( "rateus_is_showed" );

		return canShow && (force || Stats.Instance.level >= Stats.Instance.GetRemoteInt( "inapp_rate_level_android", 99 ));
	}

	public override void onShowWnd()
	{
		Stats.Instance.SetParam( "rateus_is_showed", 1 );
	}
    
	void EnableStars( int num )
	{
		for(int i= 0; i< m_stars.transform.childCount; ++i)
		{
			GameObject go= m_stars.transform.GetChild( i ).gameObject;

			Image img= go.GetComponent<Image>();

			Color c= Color.white;

			if( i >= num )
				c.a= 0.36f;

			img.color= c;
		}
	}

	public void onClickRate( int num )
	{
		m_starsRate= num;

		EnableStars( num );

		bool rateUs= num == 5;

		m_btnRateUs.interactable= true;
	}

	public void CloseMe()
	{
		Hide();
	}

	public void onClickClose()
	{
		Analytics.SendEvent( "RATE_WND_CLICK_CLOSE", "level", Stats.Instance.level );

		CloseMe();
	}

	public void onClickRateUS()
	{
		CloseMe();

		if( m_starsRate == 5 )
			Application.OpenURL( CoreConfig.GetMyURL() );

		Analytics.SendEvent( "RATE_WND_RATE_US", "level", Stats.Instance.level, "stars", m_starsRate );

		YaMetrica.SendEvent( "click_rate_us", "stars", m_starsRate );
	}

}
