using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OutTo : MonoBehaviour
{

	static OutTo _instance= null;

	public GameObject m_buttons;

	public GameObject m_core;

	public Text m_text;

	int m_currChannel= 0;

	List<string> m_listStrings= new List<string>();
    List<int> m_channels= new List<int>();

	//public static OutTo Instance { get { return _instance; } }


    OutTo()
    {
        _instance= this;
    }

	void Awake()
	{
		_Clear();

		GameObject last= GetLastButton();

		SetBtnListener( last, 0 );

		m_channels.Add( 0 );
	}

	public void _Clear()
	{
		m_text.text= "";
	}

	public void _Out( int channel, string str )
	{
		AddStr( channel, str );

		if( channel == m_currChannel )
			_SelectChannel( channel );
	}

	public void _SelectChannel( int channel )
	{
		m_currChannel= channel;
		m_text.text= GetStr( channel );
	}

	public void onClickShow()
	{
		m_core.SetActive( !m_core.activeSelf );
	}

	// INTERFACE 

	static public void Clear()
	{
		if( _instance != null )
			_instance._Clear();
	}

	static public void Out( string str )
	{
		Out( 0, str );
	}

	static public void Out( int channel, string str )
	{
		if( _instance != null )
			_instance._Out( channel, str );
	}

	static public void SelectChannel( int channel )
	{
		if( _instance != null )
			_instance._SelectChannel( channel );
	}

	/// CORE

	void AddStr( int channel, string str )
	{
		if( m_listStrings.Count < channel + 1 )
		{
			int c= channel + 1 - m_listStrings.Count;
			for(int i= 0; i< c; ++i)
				m_listStrings.Add( "" );
		}

		if( m_channels.Contains( channel ) == false )
		{
			m_channels.Add( channel );
			AddButton( channel );
		}

		m_listStrings[channel]+= str + "\n";
		//return m_listStrings[channel];
	}

	string GetStr( int channel )
	{
		if( channel < m_listStrings.Count )
			return m_listStrings[ channel ];
		return "";
	}

	List<int> GetChannels()
	{
		return m_channels;
	}

	/// buttons

	GameObject GetLastButton()
	{
		int childs= m_buttons.transform.childCount;
		return m_buttons.transform.GetChild( childs - 1 ).gameObject;
	}

	void AddButton( int channel )
	{
		GameObject last= GetLastButton();

		GameObject go= Instantiate( last, m_buttons.transform );

		SetBtnListener( go, channel );
	}

	void SetBtnListener( GameObject goBtn, int channel )
	{
		Button btn= goBtn.GetComponent<Button>();

		btn.onClick.AddListener( () => { SelectChannel(channel); } );

		Text txt= btn.GetComponentInChildren<Text>();

		txt.text= channel.ToString();
	}

}
