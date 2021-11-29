using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatFood : MonoBehaviour
{
	[SerializeField]
	private SnakeTail _snakeTail;


	private void OnTriggerEnter2D( Collider2D collision )
	{
		if (collision.GetComponent<Food>() != null)
		{
			Food food = collision.GetComponent<Food>();

			_snakeTail.AddSnakePart( food.GetFoodCount );

			Destroy( collision.gameObject );

			Vibro.Light();
		}
	}
}
