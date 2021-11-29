using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FinishLine : MonoBehaviour
{
	[SerializeField]
	private GameObject _prefabCoin;

	private List<GameObject> _coinList;

	private const float THROW_RADIUS = 0.4f;

	private const float MIN_RANDOM_MOVE_TIME = 0.2f;
	private const float MAX_RANDOM_MOVE_TIME = 0.5f;

	private SnakeMovement _currentSnake;

	bool _isFinished = false;


	private void Start()
	{
		_coinList = new List<GameObject>();
	}

	private void OnTriggerEnter2D( Collider2D other )
	{
		SnakeCircle circle = other.GetComponent<SnakeCircle>();

		_currentSnake = other.GetComponent<SnakeMovement>();

		if (_currentSnake != null)
		{
			_currentSnake.StartEndBoost();

			Invoke( "FireFinish", 4f );
		}

		if (circle != null && _isFinished == false)
		{
			SpawnCoin( circle.transform.position );

			if (circle.Next == null)
			{
				CancelInvoke( "FireFinish" );

				Invoke( "FireFinish", 0.5f );
			}
		}
	}

	private void FireFinish()
	{
		if (_isFinished)
			return;

		_isFinished = true;
		Game.Instance.onFinish();
	}

	private void SpawnCoin( Vector3 pos )
	{
		if (_coinList.Count >= 50 && Random.Range( 0, 3 ) != 0) // for optimization!
			return;

		GameObject coin = Instantiate( _prefabCoin, transform );

		_coinList.Add( coin );

		coin.transform.position = pos;
		coin.transform.localScale = Vector3.zero;

		Vector3 newPos = pos + new Vector3( Random.Range( -THROW_RADIUS, THROW_RADIUS ), Random.Range( -THROW_RADIUS, THROW_RADIUS ), 0 );

		EffectProc proc = EffectProc.GetProc( coin );

		proc.AddEffect( new Effect( Random.Range( 0f, 0.1f ), false ) );
		proc.AddEffect( new FxMoveTo( newPos, false, Random.Range( MIN_RANDOM_MOVE_TIME, MAX_RANDOM_MOVE_TIME ), true ) );
		proc.AddEffect( new FxScale( 0f, Random.Range( 0.8f, 1f ), 0.2f, true ) );

		if (_coinList.Count % 2 == 0)
			Vibro.Light();
	}


	public int MoveCoinsToUI( Vector3 newPos )
	{
		for (int i = 0; i < _coinList.Count; i++)
		{
			GameObject obj = _coinList[ i ];

			EffectProc proc = EffectProc.GetProc( obj );

			float delay = 0.1f + Random.Range( 0f, 0.1f ) + i / 50f;

			proc.AddEffect( new Effect( Mathf.Min( 1.2f, delay ), false ) );
			proc.AddEffect( new FxMoveTo( newPos, false, Random.Range( 0.2f, 0.4f ), true ) );
			proc.AddEffect( new FxScale( obj.transform.localScale.x, 0.5f, 0.2f, true ) );

			if (i % 4 == 0 && delay < 1.2f)
				proc.AddEffect( new FxVibro( 2, 0f ) );

			Destroy( _coinList[ i ], 0.75f );
		}

		return _coinList.Count;
	}
}
