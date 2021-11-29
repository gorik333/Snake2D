using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class wndGameWin : Window
{

	public Text m_txtComplete;

	public GameObject m_instagram;


    void Start()
    {
	}

	public void Show( int level )
	{
		base.Show( true );

		m_txtComplete.text= "LEVEL " + level.ToString() + "\nCOMPLETED!";


		/// INSTAGRAM

		bool instAct= Stats.Instance.level >= 5;
		bool instPressed= Stats.Instance.HasParam( "instagram_pressed" );

		m_instagram.SetActive( instAct && !instPressed );

		m_instagram.SetActive( false );
	}

	public void onClickInstagram()
	{
		string defStr= "https://www.instagram.com/ifreeman13";

		string url= Stats.Instance.GetRemoteStr( "url_instargam", defStr );

		Application.OpenURL( url );

		Stats.Instance.SetParam( "instagram_pressed", 1 );
		m_instagram.SetActive( false );

		Analytics.SendEvent( "click_instargam" );
		YaMetrica.SendEvent( "click_instargam" );
	}

}
