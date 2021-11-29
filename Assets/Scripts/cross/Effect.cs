using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Effect
{
	bool start= false;
	bool next= false;

	float timeStart;
	float effectTime= -1f; // -1 == infinite

	string targetObjName;

	GameObject target;
	GameObject subTarget;

	public string fxName= "fx";

	public bool isStarted { get { return start; } }

	public Effect( float fxTime )
	{
		effectTime= fxTime;
	}

	public Effect( float fxTime, bool _next )
	{
		next= _next;
		effectTime= fxTime;
	}

	public float GetCoeff()
	{
		if( effectTime == -1 )
			return 0;

		float c= GetLifeTime() / effectTime;

		c= Mathf.Clamp( c, 0f, 1f ) / 2f;

		float fs= Mathf.Sin( c * Mathf.PI );

		return fs;
	}

	public void Start()
	{
		start= true;
		timeStart= Time.time;

		onStart();
	}

	public bool IsEnd()
	{
		return GetLifeTime() >= effectTime;
	}

	public bool IsNext()
	{
		return next;
	}

	public float GetLifeTime()
	{
		if( !start )
			return 0;
		return Time.time - timeStart;
	}

	public void SetTargetObj( GameObject go )
	{
		target= go;
	}

	public GameObject GetTargetObj()
	{
		if( targetObjName != null && targetObjName.Length > 0 && target != null )
		{
			if( subTarget == null )
			{
				foreach(Transform child in target.transform)
				{
					if( child.name == targetObjName )
					{
						subTarget= child.gameObject;
						break;
					}
				}
			}
			return subTarget;
		}
		return target;
	}

	public void SetTargetObjName( string name )
	{
		targetObjName= name;
	}

	string GetTargetObjName()
	{
		return targetObjName;
	}

	public virtual void Update()
	{
	}

	public virtual void onStart()
	{
		
	}
}


public class FxMoveTo : Effect
{
	bool first= true;
	bool local= true;

	Vector3 toPos;
	Vector3 fromPos;

	public Vector3 endPos { get { return toPos; } }
	public Vector3 startPos { get { return fromPos; } }

	public FxMoveTo( Vector3 pos, bool localPos, float fxTime, bool _next= true ) : base( fxTime, _next )
	{
		toPos= pos;
		local= localPos;
	}

	public override void Update()
	{
		if( first )
		{
			first= false;

			if( local )
				fromPos= GetTargetObj().transform.localPosition;
			else
				fromPos= GetTargetObj().transform.position;
		}

		float c= GetCoeff();

		Vector3 pos= fromPos + (toPos - fromPos) * c;

		if( local )
			GetTargetObj().transform.localPosition= pos;
		else
			GetTargetObj().transform.position= pos;
	}

}

public class FxMove : Effect
{
	bool first= true;

	Vector3 fromPos;
	public Vector3 offset;

	public FxMove( Vector3 _offset, float fxTime, bool _next= true ) : base( fxTime, _next )
	{
		offset= _offset;
	}

	public override void Update()
	{
		if( first )
		{
			first= false;
			fromPos= GetTargetObj().transform.localPosition;
		}

		float c= GetCoeff();

		Vector3 pos= fromPos + offset * c;

		GetTargetObj().transform.localPosition= pos;
	}

}


public class FxMoveRigidTo : Effect
{
	bool first= true;

	Vector3 toPos;
	Vector3 fromPos;

	public Vector3 endPos { get { return toPos; } }
	public Vector3 startPos { get { return fromPos; } }

	public FxMoveRigidTo( Vector3 pos, float fxTime, bool _next= true ) : base( fxTime, _next )
	{
		toPos= pos;
	}

	public override void Update()
	{
		if( first )
		{
			first= false;

			fromPos= GetTargetObj().transform.position;
		}

		float c= GetCoeff();

		Vector3 pos= fromPos + (toPos - fromPos) * c;

		GetTargetObj().GetComponent<Rigidbody>().MovePosition( pos );

		//Debug.Log( "FX c= " + c + "  pos= " + pos.y );
	}

}


public class FxOpacity : Effect
{
	float alphaFrom, alphaTo;
	
	public FxOpacity( float a, float b, float fxTime, bool _next ) : base( fxTime, _next )
	{
		alphaFrom= a;
		alphaTo= b;
	}

	public override void onStart()
	{
		SetOpacity( alphaFrom );
	}

	public override void Update()
	{
		SetOpacity( Mathf.Lerp( alphaFrom, alphaTo, GetCoeff() ) );
	}

	void SetOpacity( float f )
	{
		Graphic img= GetTargetObj().GetComponent<Graphic>();
		if( img != null )
		{
			Color c= img.color;
			c.a= f;
			img.color= c;
		}
	}
}


public class FxTurnActive : Effect
{
	bool _activate;

	public FxTurnActive( bool activate, float fxTime, bool _next ) : base( fxTime, _next )
	{
		_activate= activate;
	}

	public override void Update()
	{
		if( IsEnd() )
		{
			GetTargetObj().gameObject.SetActive( _activate );
		}
	}
}

public class FxShaderParamFloat : Effect
{
	float _from, _to;

	string _param;

	Material _material;

	public FxShaderParamFloat( Material mat, string param, float from, float to, float fxTime, bool _next ) : base( fxTime, _next )
	{
		_to= to;
		_from= from;

		_param= param;
		_material= mat;
	}

	public override void Update()
	{
		float c= GetCoeff();
		float v= Mathf.Lerp( _from, _to, c );

		_material.SetFloat( _param, v );
	}
}

public class FxMaterialColor : Effect
{
	Color _from, _to;

	Material _material;

	public FxMaterialColor( Material mat, Color from, Color to, float fxTime, bool _next ) : base( fxTime, _next )
	{
		_to= to;
		_from= from;
		_material= mat;
	}

	public override void Update()
	{
		float c= GetCoeff();
		Color v= Color.Lerp( _from, _to, c );

		_material.color= v;
	}
}


public class FxScale : Effect
{
	Vector3 _from, _to;

	public FxScale( float fromScale, float toScale, float fxTime, bool _next= true ) : base( fxTime, _next )
	{
		_to= new Vector3( toScale, toScale, toScale );
		_from= new Vector3( fromScale, fromScale, fromScale );
	}

	public FxScale( Vector3 fromScale, Vector3 toScale, float fxTime, bool _next= true ) : base( fxTime, _next )
	{
		_to= toScale;
		_from= fromScale;
	}

	public override void Update()
	{
		float c= GetCoeff();
		
		Vector3 scale= _from + (_to - _from) * c;

		GetTargetObj().transform.localScale= scale;

		//Debug.Log( "fxSCALE: " + scale );
	}
}


public class FxScaleTo : Effect
{
	bool _first;
	Vector3 _from, _to;

	public FxScaleTo( Vector3 toScale, float fxTime, bool _next= true ) : base( fxTime, _next )
	{
		_to= toScale;
		_first= true;
	}

	public override void Update()
	{
		if( _first )
		{
			_first= false;

			_from= GetTargetObj().transform.localScale;
		}

		float c= GetCoeff();
		
		Vector3 scale= _from + (_to - _from) * c;

		GetTargetObj().transform.localScale= scale;
	}

}


public class FxRotateX : Effect
{
	float _from, _to;

	public FxRotateX( float from, float to, float fxTime, bool _next= true ) : base( fxTime, _next )
	{
		_to= to;
		_from= from;
	}

	public override void Update()
	{
		Vector3 r= GetTargetObj().transform.localRotation.eulerAngles;

		if( !IsEnd() )
		{
			float c= GetCoeff();

			r.x= _from + (_to - _from) * c;

			Quaternion q= GetTargetObj().transform.localRotation;

			q.eulerAngles= r;

			GetTargetObj().transform.localRotation= q;
		}
		else
		{
			r.x= _to;

			Quaternion q= GetTargetObj().transform.localRotation;

			q.eulerAngles= r;

			GetTargetObj().transform.localRotation= q;
		}
	}

}

public class FxRotateY : Effect
{
	float _from, _to;

	public FxRotateY( float from, float to, float fxTime, bool _next= true ) : base( fxTime, _next )
	{
		_to= to;
		_from= from;
	}

	public override void Update()
	{
		Vector3 r= GetTargetObj().transform.localRotation.eulerAngles;

		if( !IsEnd() )
		{
			float c= GetCoeff();

			r.y= _from + (_to - _from) * c;

			Quaternion q= GetTargetObj().transform.localRotation;

			q.eulerAngles= r;

			GetTargetObj().transform.localRotation= q;
		}
		else
		{
			r.y= _to;

			Quaternion q= GetTargetObj().transform.localRotation;

			q.eulerAngles= r;

			GetTargetObj().transform.localRotation= q;
		}
	}

}


public class FxRotateZ : Effect
{
	float _from, _to;

	public FxRotateZ( float from, float to, float fxTime, bool _next= true ) : base( fxTime, _next )
	{
		_to= to;
		_from= from;
	}

	public override void Update()
	{
		Vector3 r= GetTargetObj().transform.localRotation.eulerAngles;

		if( !IsEnd() )
		{
			float c= GetCoeff();

			r.z= _from + (_to - _from) * c;

			Quaternion q= GetTargetObj().transform.localRotation;

			q.eulerAngles= r;

			GetTargetObj().transform.localRotation= q;
		}
		else
		{
			r.z= _to;

			Quaternion q= GetTargetObj().transform.localRotation;

			q.eulerAngles= r;

			GetTargetObj().transform.localRotation= q;
		}
	}

}


public class FxRotateXYZ : Effect
{
	Vector3 _from, _to;

	public FxRotateXYZ( Vector3 from, Vector3 to, float fxTime, bool _next= true ) : base( fxTime, _next )
	{
		_to= to;
		_from= from;
	}

	public override void Update()
	{
		if( !IsEnd() )
		{
			float c= GetCoeff();

			Vector3 r= _from + (_to - _from) * c;

			Quaternion q= GetTargetObj().transform.localRotation;

			q.eulerAngles= r;

			GetTargetObj().transform.localRotation= q;
		}
		else
		{
			Quaternion q= GetTargetObj().transform.localRotation;

			q.eulerAngles= _to;

			GetTargetObj().transform.localRotation= q;
		}
	}
}


public class FxRotateQ : Effect
{
	Quaternion _from, _to;

	public FxRotateQ( Quaternion from, Quaternion to, float fxTime, bool _next= true ) : base( fxTime, _next )
	{
		_to= to;
		_from= from;
	}

	public override void Update()
	{
		if( !IsEnd() )
		{
			float c= GetCoeff();

			GetTargetObj().transform.localRotation= Quaternion.Lerp( _from, _to, c );
		}
		else
		{
			GetTargetObj().transform.localRotation= _to;
		}
	}
}


public class FxChangeTexture : Effect
{
	Texture2D _texture;

	SpriteRenderer _renderer;

	public FxChangeTexture( SpriteRenderer renderer, Texture2D texture, float fxTime, bool _next= true ) : base( fxTime, _next )
	{
		_texture= texture;
		_renderer= renderer;
	}

	public override void Update()
	{
		if( IsEnd() )
		{
			SpriteRenderer renderer= _renderer;//GetTargetObj().GetComponent<SpriteRenderer>();

			if( renderer != null )
				renderer.sprite = Sprite.Create( _texture, renderer.sprite.rect, new Vector2(0.5f, 0.5f) );
		}
	}

}


public class FxGravity : Effect
{
	Vector3 mVelocity= new Vector3( 0, 0.2f, 0 );

	public FxGravity() : base( 2f, true )
	{
	}

	public FxGravity( Vector3 velocity ) : base( 2f, true )
	{
		mVelocity= velocity;
	}

	public override void Update()
	{
		if( !IsEnd() )
		{
			mVelocity+= new Vector3( 0, -0.5f, 0 ) * Time.deltaTime;

			GetTargetObj().transform.position+= mVelocity;
		}
	}
}



public class FxScore : Effect
{
	int _from, _to;

	public FxScore( int from, int to, float time ) : base( time, true )
	{
		_from= from;
		_to= to;
	}

	public override void Update()
	{
		Text txt= GetTargetObj().GetComponent<Text>();

		if( txt != null )
		{
			int i= _from + (int)((_to - _from) * GetCoeff());
			txt.text= i.ToString();
		}
	}

}


public class FxVibro : Effect
{
	int _type;

	public FxVibro( int type, float time ) : base( time, true )
	{
		_type= type;
	}

	public override void Update()
	{
		if( IsEnd() )
		{
			switch( _type )
			{
			case 0 : Vibro.Light();		break;
			case 1 : Vibro.Medium();	break;
			case 2 : Vibro.Heavy();		break;
			}
		}
	}

}

/*
public class FxGravity : Effect
{
	public FxGravity() : base( Random.Range(420,680)/100f, true )
	{
		_force= new Vector3( (float)Random.Range(-200,200)/100f, 4f, 0f );
	}
		
	public override void Update()
	{
		GameObject go= GetTargetObj();

		{
			Vector3 _gravity= new Vector3( 0, -19f, 0 );

			_force+= _gravity * Time.deltaTime;

			go.transform.position+= _force;
		}
	}

	Vector3 _force;
};
*/