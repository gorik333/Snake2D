using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Game : MonoBehaviour
{
	public static Game Instance { get; private set; }

	[SerializeField]
	private UIProc m_UI;

	[SerializeField]
	private GameUI m_gameUI;

	[SerializeField]
	private GameObject _camera;

	[SerializeField]
	private GameObject _snakePrefab;

	[SerializeField]
	private SkinData _skinData;

	[SerializeField]
	private SnakeSkin _snakeSkin;

	[SerializeField]
	private LevelGenerator _levelGenerator;

	private int _previousSnakeLength;
	private int _passedLevels;

	private const int DEFAULT_SNAKE_LENGTH = 3; // temp, default = 3

	private const float PASS_LEVEL_EXTRA_SPEED = 0.02f;
	private const float DEFAULT_SNAKE_SPEED = 1.2f;
	private const float MAX_SNAKE_SPEED = 2.8f;

	private const string PASSED_LEVELS_NAME = "PassedLevels";
	private const string PREVIOUS_SNAKE_LENGTH_NAME = "PreviousSnakeLength";

	private SnakeMovement _currentSnake;

	private Vector2 _prevPosition;
	private GameObject _prevNextCube;

	bool _useContinue;


	private void Awake()
	{
		Instance = this;
	}


	void Start()
	{
		Application.targetFrameRate = 60;

		ADs._AdShown += AdShown;
		ADs._AdHidden += AdHidden;

		Restart();
	}


	void Update()
	{
		ProgressBarCalc();

		if (_currentSnake != null)
		{
			if (_camera.transform.position.y <= _levelGenerator.GetLevelLength())
				_camera.transform.position = Vector3.Lerp( _camera.transform.position, new Vector3( 0, _currentSnake.transform.position.y, _currentSnake.transform.position.z ), 1f ); // 0.2 def
		}
	}


	private void ProgressBarCalc()
	{
		if (_currentSnake != null)
		{
			float finishPosition = _levelGenerator.GetLevelLength();

			m_gameUI.UpdateBar( _currentSnake.transform.position.y / finishPosition, Stats.Instance.level );
		}
	}


	public void Restart()
	{
		_useContinue = false;
		_levelGenerator.OnRestart( Stats.Instance.level - 1 );

		RespawnSnake();

		_camera.transform.position = Vector2.zero;

		m_gameUI.SetBarColor( _levelGenerator.GetColors().EndColor );

		Analytics.SendEvent( "LEVEL_START", "level", Stats.Instance.level );
		YaMetrica.SendEvent( "level_start", "level", Stats.Instance.level );
	}


	public void RespawnSnake()
	{
		Skin skinSnake = _skinData.GetSkin( Stats.Instance.skin );
		_snakeSkin.SetSkin( skinSnake, _levelGenerator.LevelColor.SnakeColor );

		RemoveOldSnake();
		SpawnSnake( Vector2.zero );
		SnakeSetUp();
	}


	private void RemoveOldSnake()
	{
		if (_currentSnake != null)
		{
			SnakeTail st = _currentSnake.GetComponent<SnakeTail>();
			_previousSnakeLength = st.SnakeLength;
			st?.Die();
			Stats.Instance.SetParam( PREVIOUS_SNAKE_LENGTH_NAME, _previousSnakeLength );
		}
	}


	private void SpawnSnake( Vector2 spawnPosition )
	{
		_currentSnake = Instantiate( _snakePrefab, spawnPosition, Quaternion.identity, transform ).GetComponent<SnakeMovement>();

		_levelGenerator.InitializeSnake( _currentSnake );
	}


	private void SnakeSetUp()
	{
		_previousSnakeLength = Stats.Instance.GetParamInt( PREVIOUS_SNAKE_LENGTH_NAME, DEFAULT_SNAKE_LENGTH );

		if (_previousSnakeLength < DEFAULT_SNAKE_LENGTH)
			_previousSnakeLength = DEFAULT_SNAKE_LENGTH;

		_currentSnake.InitializeSnake( _previousSnakeLength );
	}


	public void Continue()
	{
		_useContinue = true;

		Destroy( _prevNextCube );
		SpawnSnake( _prevPosition );
		SnakeSetUp();
	}

	private void StartGame()
	{
		_passedLevels = Stats.Instance.GetParamInt( PASSED_LEVELS_NAME, 0 );

		float moveSpeed = DEFAULT_SNAKE_SPEED + PASS_LEVEL_EXTRA_SPEED * _passedLevels;

		if (moveSpeed > MAX_SNAKE_SPEED)
			moveSpeed = MAX_SNAKE_SPEED;

		_currentSnake.StartSnakeMovement( moveSpeed );
	}


	// EVENTS
	public void onClick( bool pressed )
	{
		if (pressed && m_UI.IsMainMenuShown)
		{
			StartGame();

			m_UI.ShowMainMenu( false );
		}
		else if (pressed && !m_UI.IsMainMenuShown)
			_currentSnake.StartSnakeMovement( DEFAULT_SNAKE_SPEED );
	}


	public void onGameOver()
	{
		_prevNextCube = _levelGenerator.GetLastCube();
		_prevPosition = _currentSnake.transform.position;
		_camera.transform.position = new Vector2( 0, _prevPosition.y );

		m_UI.ShowGameOver( !_useContinue, Stats.Instance.scores );

		_passedLevels = 0;

		Stats.Instance.SetParam( PASSED_LEVELS_NAME, _passedLevels );
		Stats.Instance.SetParam( PREVIOUS_SNAKE_LENGTH_NAME, DEFAULT_SNAKE_LENGTH );

		SoundsMan.Play( "4pok" );

		Analytics.SendEvent( "LEVEL_FAIL", "level", Stats.Instance.level );
		YaMetrica.SendEvent( "level_finish", "level", Stats.Instance.level, "result", "lose" );

		Vibro.Heavy();
	}


	public void onFinish()
	{
		m_UI.ShowWin( Stats.Instance.level );

		_passedLevels++;

		Stats.Instance.SetParam( PASSED_LEVELS_NAME, _passedLevels );

		SoundsMan.Play( "4pok" );

		Analytics.SendEvent( "LEVEL_COMPL", "level", Stats.Instance.level );
		YaMetrica.SendEvent( "level_finish", "level", Stats.Instance.level, "result", "win" );

		//--  FINISH COINS EFFECT

		FinishLine fin= _levelGenerator.GetFinish();

		if( fin != null )
		{
			m_gameUI.TurnOnCoinCounter();

			int count= fin.MoveCoinsToUI( m_gameUI.GetCoinCounterPosition() );

			Stats.Instance.coins+= count;

			m_gameUI.ChangeCounterText( Stats.Instance.coins, 0.3f, 0.8f );
		}
	}

	void AdShown( bool rewarded )
	{
		UIProc.Instance.DebugOut( "AD SHOWN: " + rewarded );

		Vibro.Mute( true );
		SoundsMan.MuteSFX( true );
	}


	void AdHidden( bool rewarded )
	{
		UIProc.Instance.DebugOut( "AD HIDDEN: " + rewarded );

		Vibro.Mute( Stats.Instance.muteVibro );
		SoundsMan.MuteSFX( Stats.Instance.muteSFX );
	}
}
