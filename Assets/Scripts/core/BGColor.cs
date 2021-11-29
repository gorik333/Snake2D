using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BGColor : MonoBehaviour
{

	static BGColor _instance;

	public static BGColor Instance { get { return _instance; } }

	Material _mat;

	Color _c1;
	Color _c2;

	Color _cNew1;
	Color _cNew2;

	float _timeUpdate= 0f;
	float _lengthTime= 0f;

	BGColor()
	{
		_instance= this;
	}

	void Awake()
	{
		SpriteRenderer sr= GetComponent<SpriteRenderer>();
		if( sr )
			_mat= sr.material;
		else
		{
			Image img= GetComponent<Image>();

			_mat= img.material;
		}

		Color c1= GetC1(0);
		Color c2= GetC2(0);

		ChangeColor( c1, c2, 0f );
	}

	public void Init()
	{
		
	}

	public void RandColor()
	{
		int r= Random.Range( 0, 999 );
		Color c1= GetC1(r);
		Color c2= GetC2(r);

		ChangeColor( c1, c2, 2f );
	}

	public void SetColor( Color c1, Color c2 )
	{
		if( _mat != null )
		{
			_mat.SetColor( "_Color0", c1 );
			_mat.SetColor( "_Color1", c2 );
		}
	}

	public void ChangeColor( Color c1, Color c2, float time )
	{
		_cNew1= c1;
		_cNew2= c2;

		if( time == 0 )
		{
			_c1= _cNew1;
			_c2= _cNew2;
			SetColor( _c1, _c2 );
		}
		else
		{
			_timeUpdate= time;
			_lengthTime= time;
		}
	}

	void Update()
	{
		if( _timeUpdate > 0f )
		{
			float c= 1f - (_lengthTime == 0 ? 0 : Mathf.Clamp01( _timeUpdate / _lengthTime ));

			Color c1= Color.Lerp( _c1, _cNew1, c );
			Color c2= Color.Lerp( _c2, _cNew2, c );

			SetColor( c1, c2 );

			_timeUpdate-= Time.deltaTime;

			if( _timeUpdate <= 0f )
			{
				_c1= _cNew1;
				_c2= _cNew2;
			}
		}
	}

	//

	Color GetC1( int id )
	{
		return Color.red;
	}


	Color GetC2( int id )
	{
		return Color.black;
	}

	public static Color HexToColor( string hex )
	{
		hex = hex.Replace ("0x", "");//in case the string is formatted 0xFFFFFF
		hex = hex.Replace ("#", "");//in case the string is formatted #FFFFFF
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		byte a = 255;//assume fully visible unless specified in hex

		//Only use alpha if the string has enough characters
		if( hex.Length == 8 )
		{
			a = byte.Parse( hex.Substring(6,2), System.Globalization.NumberStyles.HexNumber );
		}
		return new Color32(r,g,b,a);
	}

}
