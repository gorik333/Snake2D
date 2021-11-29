using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SnakeCircle : MonoBehaviour
{

	public SnakeCircle Next;
	public SnakeCircle Prev;

	public Vector3 Pos
	{
		get { return transform.position; }

		set { transform.position= value; }
	}

	public void Move( Vector3 dir )
	{
		Pos= Pos + dir;
	}

	public void SetColor( Color col )
	{
		SpriteRenderer sr= GetComponent<SpriteRenderer>();

		if( sr != null )
			sr.sharedMaterial.color= col;
	}

}
