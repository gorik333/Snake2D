using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class wndMessage : Window
{
    
	public Text m_text;


	public void Show( string message )
	{
		base.Show( true );

		m_text.text= message;
	}

	public void onClickOK()
	{
		Hide();
	}

}
