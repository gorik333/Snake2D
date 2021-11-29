using System.Collections;
using System.Collections.Generic;
using System.Linq;using TMPro;using UnityEngine;

public class SnakeMovement : MonoBehaviour
{
	[SerializeField]
	private SnakeTail _snakeTail;

	[SerializeField]
	private Rigidbody2D _snakeRB;

	[SerializeField]
	private float _sensitivity = 12;

	private const int EXTRA_LINES_START_OFFSET = 5;

	private const float EXTRA_BOOST_SPEED = 0.15f;
	private const float BOOST_DURATION = 1.75f;

	private Camera _mainCamera;

	private Vector2 _lastTouchPosition;

	private float _sidewayMoveSpeed;
	private float _snakeMoveSpeed;


	private void Start()
	{
		_mainCamera = Camera.main;
	}


	public void InitializeSnake( int startLength )
	{
		for (int i = 0; i < startLength; i++) _snakeTail.AddSnakePart();

		StopSnakeMovement();
	}


	public void StopSnakeMovement()
	{
		_snakeRB.constraints = RigidbodyConstraints2D.FreezePosition;

		_snakeMoveSpeed = 0f;
	}


	public void StartSnakeMovement( float moveSpeed )
	{
		_snakeRB.constraints = RigidbodyConstraints2D.None;
		_snakeRB.constraints = RigidbodyConstraints2D.FreezeRotation;

		_snakeMoveSpeed = moveSpeed;
	}


	public void StartEndBoost()
	{
		StartCoroutine( EndBoost() );
	}


	private IEnumerator EndBoost()
	{
		int i = 0;
		float boostdelay = BOOST_DURATION / _snakeTail.SnakeLength;

		while (i < _snakeTail.SnakeLength)
		{
			_snakeMoveSpeed += EXTRA_BOOST_SPEED;

			i++;

			yield return new WaitForSeconds( boostdelay );
		}
	}


	private void Update()
	{
		if (_snakeMoveSpeed != 0)
		{
			ControlSnake();

			CheckPassedLine();

#if UNITY_EDITOR
			if (Input.GetKey( KeyCode.A )) // temp
			{
				_snakeTail.AddSnakePart();
			}
			if (Input.GetKeyDown( KeyCode.D ))
			{
				_snakeTail.RemoveSnakePart();
			}
			if (Input.GetKey( KeyCode.W ))
			{
				_snakeTail.RemoveSnakePart();
			}
			if (Input.GetKeyDown( KeyCode.S ))
			{
				_snakeTail.AddSnakePart();
			}
#endif
		}
	}


	private void FixedUpdate()
	{
		if (_snakeMoveSpeed != 0)
		{
			MoveSnake();
		}
	}


	private void ControlSnake()
	{
		if (Input.GetMouseButtonDown( 0 ))
		{
			_lastTouchPosition = _mainCamera.ScreenToViewportPoint( Input.mousePosition );
		}
		else if (Input.GetMouseButtonUp( 0 ))
		{
			_sidewayMoveSpeed = 0;
		}
		else if (Input.GetMouseButton( 0 ))
		{
			Vector2 delta = (Vector2)_mainCamera.ScreenToViewportPoint( Input.mousePosition ) - _lastTouchPosition;
			_sidewayMoveSpeed += delta.x * _sensitivity;
			_lastTouchPosition = _mainCamera.ScreenToViewportPoint( Input.mousePosition );
		}
	}


	private void MoveSnake()
	{
		if (Mathf.Abs( _sidewayMoveSpeed ) > 4)
			_sidewayMoveSpeed = 4 * Mathf.Sign( _sidewayMoveSpeed );

		_snakeRB.velocity = new Vector2( _sidewayMoveSpeed * 5, _snakeMoveSpeed );

		_sidewayMoveSpeed = 0;
	}

	/// <summary>
	/// Every CubeObstacleSize, snake pass the line
	/// </summary>
	private void CheckPassedLine()
	{
		if (transform.position.y > LevelGenerator.Instance.GetReachedDistance( -EXTRA_LINES_START_OFFSET ))
		{
			LevelGenerator.Instance.IncreaseCurrentLine();
		}
	}


	public float SnakeMoveSpeed { get => _snakeMoveSpeed; set => _snakeMoveSpeed = value; }
}


class Curve
{
	float m_maxLength = 1f;

	float m_length = 0;

	LinkedList<Vector3> m_points = new LinkedList<Vector3>();


	public void AddPoint( Vector3 p )
	{
		if (m_points.Count > 0 && ( m_points.First.Value - p ).magnitude < 0.001f)
			return;

		m_points.AddFirst( p );

		if (m_points.Count > 1)
			m_length += ( p - m_points.First.Next.Value ).magnitude;

		for (int i = 0; i < 10; ++i)
		{
			if (m_length > m_maxLength && m_points.Count > 1)
			{
				LinkedListNode<Vector3> last = m_points.Last;

				m_length -= ( last.Value - last.Previous.Value ).magnitude;

				m_points.RemoveLast();
			}
			else
				break;
		}
	}

	public Vector3 GetPoint( float distance, Vector3 def )
	{
		if (m_points.Count < 2)
			return def;

		LinkedListNode<Vector3> node = m_points.First.Next;

		float curr = 0;

		for (; node != null;)
		{
			Vector3 p0 = node.Previous.Value;
			Vector3 p1 = node.Value;

			Vector3 dir = p1 - p0;

			float d = dir.magnitude;

			if (curr <= distance && distance < ( curr + d ))
			{
				float p = ( distance - curr ) / d;
				return Vector3.Lerp( p0, p1, p );
			}

			curr += d;

			node = node.Next;
		}

		return def;
	}

	public void SetMaxLength( float len ) { m_maxLength = len; }

	public int NumPoints { get { return m_points.Count; } }

#if UNITY_EDITOR
	public Vector3 GetPoint( int ind )
	{
		List<Vector3> list = m_points.ToList();
		return list[ ind ];
	}
#endif

};