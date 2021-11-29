using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsMan : MonoBehaviour
{

	static Dictionary<string,AudioSource> m_clips= new Dictionary<string,AudioSource>();

	static bool m_sfxMute= false;

	static SoundsMan _instance;

	void Awake()
	{
		_instance= this;
	}

	void Start()
	{
		foreach(Transform child in transform)
		{
			AudioSource src= child.GetComponent<AudioSource>();

			if( src != null )
			{
				m_clips[ src.name ]= src;
			}
		}
	}

	static public void MuteSFX( bool mute )
	{
		m_sfxMute= mute;
	}

	static public bool Play( string name, float delay= 0f )
	{
		if( m_sfxMute )
			return false;

		AudioSource clip= null;
		if( m_clips.TryGetValue( name, out clip ) )
		{
			
			if( clip.isPlaying )
			{
				GameObject obj= Instantiate( clip.gameObject, _instance.gameObject.transform );
				Destroy( obj, delay + 2f );
				clip= obj.GetComponent<AudioSource>();
			}

			if( delay > 0f )
				clip.PlayDelayed( delay );
			else
				clip.Play();
			return true;
		}
		return false;
	}

	static public bool PlayPitch( string name, float pitch, float delay= 0f )
	{
		if( m_sfxMute )
			return false;

		AudioSource clip= null;
		if( m_clips.TryGetValue( name, out clip ) )
		{
			GameObject obj= Instantiate( clip.gameObject, _instance.gameObject.transform );
			Destroy( obj, delay + 2f );
			clip= obj.GetComponent<AudioSource>();

			clip.pitch= clip.pitch * pitch;
			if( delay > 0f )
				clip.PlayDelayed( delay );
			else
				clip.Play();
			return true;
		}
		return false;
	}

	static public bool Stop( string name )
	{
		if( m_sfxMute )
			return false;

		AudioSource clip= null;
		if( m_clips.TryGetValue( name, out clip ) )
		{
			if( clip.isPlaying )
			{
				clip.Stop();
			}
			return true;
		}
		return false;
	}

};