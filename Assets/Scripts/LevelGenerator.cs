using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
	[SerializeField, Header( "Data" )]
	private LevelDataStorage _levelDataStorage;

	[SerializeField, Header( "Prefabs" )]
	private GameObject _cubeObstacle;

	[SerializeField]
	private GameObject _finishLine;

	[SerializeField]
	private GameObject _food;

	[SerializeField]
	private GameObject _oneLineObstacle;

	[SerializeField]
	private GameObject _twoLinesObstacle;

	[SerializeField]
	private GameObject _gates;

	[SerializeField, Header( "Spawn positions" )]
	private float[] _cubePositionsXOnLine;

	[SerializeField]
	private float[] _linePositionsXOnLine;

	private int _currentLine;
	private int _currentLinesToFullCubes;
	private int _currentLinesToGates;
	private int _lastSpawnedLine;

	private int _gatesSpawned;

	private int _countFullCubeLines;

	public const float CUBE_SIZE = 0.35f;

	public const int LEVEL_GENERATION_OFFSET = 6;

	private const int BLOCK_DISTANCE_BEFORE_FINISH = 2;
	private const int MAX_ATTEMPTS_TO_GENERATE_FOOD = 100;
	private const int MEDIUM = 30;
	private const int HARD = 70;

	private LevelDataConfiguration _levelData;
	private LevelDataColor _levelColor;
	private SnakeTail _currentSnake;

	private List<GameObject> _spawnedItems = new List<GameObject>();

	public static LevelGenerator Instance { get; private set; }

	FinishLine _finish;


	private void Awake()
	{
		Instance = this;
	}


	public GameObject GetLastCube()
	{

		return _currentSnake.GetComponent<SnakeCollision>().LastCube();
	}


	public float GetLevelLength()
	{

		return ( _levelData.Length - LEVEL_GENERATION_OFFSET ) * CUBE_SIZE;
	}


	private void ClearSpawnedItems()
	{
		for (int i = 0; i < _spawnedItems.Count; i++)
			Destroy( _spawnedItems[ i ] );

		_spawnedItems.Clear();

		_finish = null;
	}

	public FinishLine GetFinish() { return _finish; }

	private void SetUp()
	{
		Camera.main.backgroundColor = _levelColor.BackgroundColor;

		_currentLine = 0;
		_countFullCubeLines = 0;
		_currentLinesToFullCubes = 0;
		_currentLinesToGates = 0;
		_gatesSpawned = 0;
	}


	public void InitializeSnake( SnakeMovement snakeTail )
	{
		_currentSnake = snakeTail.GetComponent<SnakeTail>();
	}


	public void OnRestart( int level )
	{
		_levelData = _levelDataStorage.GetLevelConfiguration( level );
		_levelColor = _levelDataStorage.GetLevelColor( level );

		ClearSpawnedItems();
		SetUp();

		SpriteRenderer foodRender = _food.GetComponent<SpriteRenderer>();

		if (foodRender != null)
			foodRender.sharedMaterial.color = _levelColor.SnakeColor;
	}

	public LevelDataColor GetColors()
	{
		return _levelColor;
	}

	public void IncreaseCurrentLine()
	{
		_currentLine++;
		_currentLinesToFullCubes++;
		_currentLinesToGates++;

		GenerateGates();

		if (!GenerateLines())
			GenerateCubes();

		GenerateFood();
		GenerateFinishLine();
	}


	private void GenerateCubes()
	{
		int chance = Random.Range( 0, 100 );

		if (_levelData.FullLine > chance &&
			_currentLinesToFullCubes >= _levelData.LinesDistance && IsSpawnAllowed())
		{
			GenerateLineOfCubes();

			_countFullCubeLines++;
			_lastSpawnedLine = _currentLine;
			_currentLinesToFullCubes = 0;
			return;
		}

		chance = Random.Range( 0, 100 );
		if (_levelData.Cube2 > chance && IsSpawnAllowed())
		{
			GenerateRandomCubesOnLine( 2 );

			_lastSpawnedLine = _currentLine;
			return;
		}

		chance = Random.Range( 0, 100 );
		if (_levelData.Cube1 > chance && IsSpawnAllowed())
		{
			GenerateRandomCubesOnLine( 1 );

			_lastSpawnedLine = _currentLine;
			return;
		}
	}


	private void GenerateFood()
	{
		if (_currentLine > LEVEL_GENERATION_OFFSET && _currentLine + LEVEL_GENERATION_OFFSET < _levelData.Length - BLOCK_DISTANCE_BEFORE_FINISH)
		{
			if (_levelData.Food3 > Random.Range( 0, 100 ))
			{
				SpawnRandomFoodOnLine( 3 );

				return;
			}
			if (_levelData.Food2 > Random.Range( 0, 100 ))
			{
				SpawnRandomFoodOnLine( 2 );

				return;
			}
			if (_levelData.Food1 > Random.Range( 0, 100 ))
			{
				SpawnRandomFoodOnLine( 1 );

				return;
			}
		}
	}


	private void GenerateGates()
	{
		int chance = Random.Range( 0, 100 );

		if (_levelData.Gates > chance && IsSpawnAllowed() && _currentLinesToGates >= _levelData.GatesDistance)
		{
			SpawnGates();

			_lastSpawnedLine = _currentLine;
			_currentLinesToGates = 0;
		}
	}


	private bool GenerateLines()
	{
		bool isGenerated = false;

		if (IsSpawnAllowed())
		{
			for (int i = 0; i < _linePositionsXOnLine.Length; i++)
			{
				int chance = Random.Range( 0, 100 );

				if (_levelData.Line2 > chance && _currentLine - 1 != _lastSpawnedLine)
				{
					_lastSpawnedLine = _currentLine;

					isGenerated = true;
					SpawnLine( GetLineSpawnPosition( i ), _twoLinesObstacle );
				}
				else if (_levelData.Line1 > chance)
				{
					_lastSpawnedLine = _currentLine;

					isGenerated = true;
					SpawnLine( GetLineSpawnPosition( i ), _oneLineObstacle );
				}
			}
		}

		return isGenerated;
	}


	private void SpawnLine( Vector2 spawnPosition, GameObject prefab )
	{
		GameObject line = Instantiate( prefab, spawnPosition, Quaternion.identity, transform );

		var renderers = line.GetComponentsInChildren<SpriteRenderer>();

		for (int i = 0; i < renderers.Length; i++)
			renderers[ i ].color = _levelColor.LineObstacleColor;

		_spawnedItems.Add( line );
	}


	private void SpawnGates()
	{
		Vector2 spawnPosition = new Vector2( 0, ReachedDistance );
		GameObject gatesObject = Instantiate( _gates, spawnPosition, Quaternion.identity, transform );
		Gates[] gate = gatesObject.GetComponentsInChildren<Gates>();

		for (int i = 0; i < gate.Length; i++)
		{
			if (_gatesSpawned >= 2)
				gate[ i ].SetUpGate( _levelData.Plus, _levelData.Mul, _levelData.GateMaxNum, _levelColor.GatesColor, _levelColor.TextColor );
			else
				gate[ i ].SetUpGate( _levelData.Plus, _levelData.Mul, _levelData.GateMaxNum, _levelColor.GatesColor, _levelColor.TextColor, true );
		}

		_gatesSpawned++;

		_spawnedItems.Add( gatesObject );
	}


	private void GenerateFinishLine()
	{
		if (_currentLine + LEVEL_GENERATION_OFFSET == _levelData.Length)
		{
			Vector2 spawnPosition = new Vector2( 0, ReachedDistance );
			GameObject finishLine = Instantiate( _finishLine, spawnPosition, Quaternion.identity, transform );

			_finish = finishLine.GetComponent<FinishLine>();

			_spawnedItems.Add( finishLine );
		}
	}


	private void SpawnRandomFoodOnLine( int foodCount )
	{
		int attemptsToSpawn = 0;

		List<int> previousNumber = new List<int>();

		for (int i = 0; i < foodCount; i++)
		{
			int randomPositionX = Random.Range( 0, _cubePositionsXOnLine.Length );

			attemptsToSpawn++;

			if (attemptsToSpawn > MAX_ATTEMPTS_TO_GENERATE_FOOD)
				break;


			if (previousNumber.Contains( randomPositionX ))
			{
				i--;

				continue;
			}

			if (!IsSpawnFoodAllowed( randomPositionX ))
			{
				i--;

				continue;
			}


			SpawnFood( GetCubeAndFoodSpawnPosition( randomPositionX ) );

			previousNumber.Add( randomPositionX );
		}
	}


	private void SpawnFood( Vector2 spawnPosition )
	{
		_spawnedItems.Add( Instantiate( _food, spawnPosition, Quaternion.identity, transform ) );
	}


	private bool IsSpawnFoodAllowed( int positionIndex )
	{
		Vector2 spawnPosition = new Vector2( _cubePositionsXOnLine[ positionIndex ], ReachedDistance );

		RaycastHit2D hit = Physics2D.Raycast( spawnPosition, Vector2.zero );

		if (hit.collider != null)
		{
			CubeObstacle cubeObstacle = hit.collider.GetComponentInChildren<CubeObstacle>();
			Gates gates = hit.collider.GetComponentInChildren<Gates>();

			if (cubeObstacle != null || gates != null)
			{
				return false;
			}
		}

		return true;
	}


	private bool IsSpawnAllowed()
	{
		return !( _currentLine == _lastSpawnedLine || _currentLine - 1 == _lastSpawnedLine || _currentLine < LEVEL_GENERATION_OFFSET
				|| _currentLine + LEVEL_GENERATION_OFFSET >= _levelData.Length - BLOCK_DISTANCE_BEFORE_FINISH );
	}


	private void GenerateRandomCubesOnLine( int cubesCount )
	{
		int previousNumber = 0;

		for (int i = 0; i < cubesCount; i++)
		{
			int randomPositionX = Random.Range( 0, _cubePositionsXOnLine.Length );

			if (randomPositionX == previousNumber || randomPositionX + 1 == previousNumber || randomPositionX - 1 == previousNumber)
			{
				i--;

				continue;
			}

			int durability = Random.Range( 1, _levelData.CubeMaxNum );

			SpawnCube( GetCubeAndFoodSpawnPosition( randomPositionX ), durability );

			previousNumber = randomPositionX;
		}
	}


	private int GetDurability( int difficulty, int cubeMaxNum )
	{
		if (difficulty >= HARD)
			return cubeMaxNum / 1;
		else if (difficulty >= MEDIUM)
			return (int)( cubeMaxNum / 1.5f );
		else
			return cubeMaxNum / 2;
	}


	private void GenerateLineOfCubes()
	{
		List<int> durabilityValue = new List<int>();

		int cubeCount = _cubePositionsXOnLine.Length;
		int snakeLength = _currentSnake.SnakeLength - 1;
		int cubeMaxNum = _levelData.CubeMaxNum;
		int difficulty = _levelData.Difficulty;

		if (snakeLength > cubeMaxNum)
			snakeLength = cubeMaxNum;

		if (_countFullCubeLines == 0)
		{
			if (snakeLength >= 10)
				snakeLength = 10;

			difficulty = 0;
		}

		for (int i = 0; i < cubeCount; i++)
		{
			int durability = Random.Range( snakeLength, GetDurability( difficulty, cubeMaxNum ) );

			if (Random.Range( 0, 100 ) >= difficulty)
				durability = Random.Range( 1, snakeLength );

			durabilityValue.Add( durability );
		}

		int overDurabilityCounter = 0;

		for (int i = 0; i < durabilityValue.Count; i++)
		{
			if (durabilityValue[ i ] >= snakeLength)
				overDurabilityCounter++;

			if (durabilityValue[ i ] > cubeMaxNum)
				durabilityValue[ i ] = cubeMaxNum;
		}

		if (overDurabilityCounter >= cubeCount)
		{
			if (snakeLength + 1 < 1)
				durabilityValue[ Random.Range( 0, cubeCount ) ] = Random.Range( 0, snakeLength );
			else
				durabilityValue[ Random.Range( 0, cubeCount ) ] = Random.Range( 1, snakeLength + 1 );
		}

		if (_levelData.ForceFull)
		{
			for (int i = 0; i < cubeCount; i++)
			{
				if (durabilityValue[ i ] <= 0)
					durabilityValue[ i ] = 1;
			}
		}

		for (int i = 0; i < cubeCount; i++)
		{
			if (durabilityValue[ i ] > 0)
				SpawnCube( GetCubeAndFoodSpawnPosition( i ), durabilityValue[ i ] );
		}
	}


	private void SpawnCube( Vector2 spawnPosition, int durabilty )
	{
		GameObject cube = Instantiate( _cubeObstacle, spawnPosition, Quaternion.identity, transform );
		CubeObstacle cubeObstacle = cube.GetComponentInChildren<CubeObstacle>();

		cubeObstacle.SetUp( _levelColor, durabilty, _levelData.CubeMaxNum );

		_spawnedItems.Add( cube );
	}


	private Vector2 GetCubeAndFoodSpawnPosition( int positionIndex )
	{
		return new Vector2( _cubePositionsXOnLine[ positionIndex ], ReachedDistance );
	}


	private Vector2 GetLineSpawnPosition( int positionIndex )
	{
		return new Vector2( _linePositionsXOnLine[ positionIndex ], ReachedDistance );
	}


	public float GetReachedDistance( int extraLines )
	{
		return ( _currentLine + extraLines ) * CUBE_SIZE;
	}


	public float ReachedDistance => CUBE_SIZE * _currentLine;

	public LevelDataColor LevelColor { get => _levelColor; set => _levelColor = value; }
}
