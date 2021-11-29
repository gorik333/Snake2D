using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SnakeTail : MonoBehaviour
{
	[SerializeField]
	private TextMeshPro _snakeLengthText;

	[SerializeField]
	private ParticleSystemController _particleSystemController;

	private int _snakeLengthNumber;

	private float _circleDiameter;

	[SerializeField]
	SnakeCircle _firstCircle;
	SnakeCircle _lastCircle;

	Curve _curve = new Curve();


	private void Awake()
	{
		_lastCircle = _firstCircle;

		SnakeSkin.Instance.AddFirstCircle( _lastCircle.GetComponent<SpriteRenderer>() );
	}


	private void Start()
	{
		_circleDiameter = GetComponent<CircleCollider2D>().radius * 2f;
	}


	private void UpdateSnakeLength()
	{
		_snakeLengthText.text = ( _snakeLengthNumber + 1 ).ToString();

		_curve.SetMaxLength( Mathf.Min( 4f, _snakeLengthNumber * _circleDiameter ) );
	}


	private void Update()
	{
		_curve.AddPoint( transform.position );

		UpdateSnake( _firstCircle );
	}


	public void AddSnakePart()
	{
		GameObject circle = Instantiate( _firstCircle.gameObject, _lastCircle.Pos, Quaternion.identity, transform.parent );
		SnakeCircle sc = circle.GetComponent<SnakeCircle>();
		SnakeSkin.Instance.CalcAddedCircleColor( sc.GetComponent<SpriteRenderer>() );

		sc.Next = null;
		sc.Prev = _lastCircle;
		_lastCircle.Next = sc;

		_snakeLengthNumber++;

		_lastCircle = sc;

		UpdateSnakeLength();
	}


	public void AddSnakePart( int count )
	{
		for (int i = 0; i < count; i++)
			AddSnakePart();
	}

	Vector3 MergePoint( Vector3 point, float distance )
	{
		Vector3 c = _curve.GetPoint( distance, point );
		Vector3 r = Vector3.Lerp( point, c, 0.2f );  // MAGIC Merge ;)
		return r;
	}

	void UpdateSnake( SnakeCircle first )
	{
		SnakeCircle curr = first.Next;

		float dist = 0;

		//_curve.ResetDistance();

		for (; curr != null;)
		{
			dist += _circleDiameter;

			SnakeCircle prev = curr.Prev;

			Vector3 dir = curr.Pos - prev.Pos;

			if (dir.magnitude > _circleDiameter)
			{
				curr.Pos = prev.Pos + dir.normalized * _circleDiameter;

				curr.Pos = MergePoint( curr.Pos, dist );
			}

			curr = curr.Next;
		}
	}

	public void RemoveSnakePart( int count = 1 )
	{
		if (_snakeLengthNumber > 0)
			_particleSystemController.SpawnParticles();

		for (int i = 0; i < count; i++)
		{
			_snakeLengthNumber--;

			if (!CheckIfDead())
			{
				if (_lastCircle != null)
				{
					SnakeCircle toKill = _lastCircle;

					_lastCircle = toKill.Prev;
					_lastCircle.Next = null;

					SnakeSkin.Instance.RemoveCircle();

					Destroy( toKill.gameObject );
				}
			}
			else
			{
				Die();

				Game.Instance.onGameOver();
				break;
			}
		}

		UpdateSnakeLength();
	}

	private bool CheckIfDead()
	{
		return _snakeLengthNumber < 0;
	}

	public void Die()
	{
		SnakeCircle curr = _firstCircle.Next;

		for (; curr != null;)
		{
			GameObject kill = curr.gameObject;
			curr = curr.Next;
			Destroy( kill );
		}

		Destroy( gameObject );
	}

	public int SnakeLength => _snakeLengthNumber;
}
