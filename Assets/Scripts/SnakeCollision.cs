using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SnakeCollision : MonoBehaviour
{
	[SerializeField]
	private SnakeTail _snakeTail;

	private List<CubeObstacle> _cubeObstacle;

	private const float REDUCE_HP_DEFAULT = 0.08f;
	private const float HP_DEPENDENCE_MULTIPLIER = 2.8f;

	private float _reduceHPDelay;
	private float _currentReduceHPDelay;

	private GameObject _lastCube;


	private void Start()
	{
		_cubeObstacle = new List<CubeObstacle>();
	}


	private void Update()
	{
		_currentReduceHPDelay += Time.deltaTime;

		if (_cubeObstacle.Count > 0)
		{
			if (_currentReduceHPDelay >= _reduceHPDelay)
				DamageBoth();
		}
	}


	private void DamageBoth()
	{
		CalcReduceHPDelay();

		_cubeObstacle[ 0 ].TakeDamage();
		_snakeTail.RemoveSnakePart();

		_currentReduceHPDelay = 0;
	}


	private void OnCollisionStay2D( Collision2D collision )
	{
		if (collision.gameObject.GetComponent<CubeObstacle>() != null && IsPassedTheCube( collision.transform ))
		{
			CubeObstacle cubeObstacle = collision.gameObject.GetComponent<CubeObstacle>();

			if (!_cubeObstacle.Contains( cubeObstacle ))
			{
				_cubeObstacle.Add( cubeObstacle );
			}
		}
	}


	private void OnCollisionExit2D( Collision2D collision )
	{
		if (collision.gameObject.GetComponent<CubeObstacle>() != null)
		{
			CubeObstacle cubeObstacle = collision.gameObject.GetComponent<CubeObstacle>();

			if (_cubeObstacle.Contains( cubeObstacle ))
			{
				_cubeObstacle.Remove( cubeObstacle );
				_lastCube = cubeObstacle.transform.parent.gameObject;
			}
		}
	}


	private void CalcReduceHPDelay()
	{
		int currentDurability = Mathf.Min( _cubeObstacle[ 0 ].CurrentDurability - 1, GetComponent<SnakeTail>().SnakeLength );
		int upperBound = _cubeObstacle[ 0 ].UppedBound;

		float result = (float)upperBound / 100 * currentDurability;

		_reduceHPDelay = REDUCE_HP_DEFAULT - ( REDUCE_HP_DEFAULT / 100 * result ) * HP_DEPENDENCE_MULTIPLIER;
	}


	private bool IsPassedTheCube( Transform cube )
	{
		if (cube.position.y - 0.06f < transform.position.y) // snake and collider position
		{

			return false;
		}

		return true;
	}


	public GameObject LastCube()
	{

		return _lastCube;
	}
}
