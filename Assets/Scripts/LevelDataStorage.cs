using UnityEngine;

[System.Serializable]
public struct LevelDataConfiguration
{
	[Header( "Main settings" )]
	public int Length;
	public int LinesDistance;   // минимальное расстояние между полными линиями
	public int GatesDistance;   // минимальное расстояние между воротами

	public int CubeMaxNum;

	[Header( "Cube chances" )]
	public float Cube1;
	public float Cube2;
	public float FullLine;
	public bool ForceFull;

	[Header( "Gates chances" )]
	public float Gates;
	public int GateMaxNum;
	public float Plus;
	public float Mul;

	[Header( "Line chances" )]
	public float Line1;
	public float Line2;

	[Header( "Food chances" )]
	public float Food1;
	public float Food2;
	public float Food3;

	[Range( 0, 100 )]
	public int Difficulty;      // 0= easy, 100= hard
}

[System.Serializable]
public struct LevelDataColor
{
	[Header( "Block colors" )]
	public Color StartColor;

	public Color MiddleColor;

	public Color EndColor;

	public Color TextColor;

	public Color SnakeColor;

	[Header( "Background color" )]
	public Color BackgroundColor;

	[Header( "Line obstacle color" )]
	public Color LineObstacleColor;

	[Header( "Gates color" )]
	public Color GatesColor;
}


public class LevelDataStorage : MonoBehaviour
{
	[SerializeField]
	bool m_killGates = false;

	[SerializeField]
	int m_selectedLevel = -1;

	[SerializeField]
	int m_selectedSkin = -1;

	[SerializeField]
	private LevelDataConfiguration[] _levelDataConfiguration;

	[SerializeField]
	public LevelDataColor[] _levelDataColor;


	public LevelDataConfiguration GetLevelConfiguration( int level )
	{
#if UNITY_EDITOR
		if (m_selectedLevel >= 0)
		{
			LevelDataConfiguration r = _levelDataConfiguration[ m_selectedLevel ];

			if( m_killGates )
				r.Gates= 0;

			return r;
		}
#endif
		int first = 3;  //!

		int indx = level;

		if (level >= _levelDataConfiguration.Length)
		{
			int p = level - first;

			indx = p % ( _levelDataConfiguration.Length - first );

			indx += first;
		}

		LevelDataConfiguration rs = _levelDataConfiguration[ indx ];

		if( level > 10 )
		{
			rs.Length+= (level-10) / 2;

			if( rs.Length > 120 )
				rs.Length= 120;
		}

		if( m_killGates )
			rs.Gates= 0;

		return rs;
	}

	public LevelDataColor GetLevelColor( int index )
	{
#if UNITY_EDITOR
		if (m_selectedSkin >= 0)
			return _levelDataColor[m_selectedSkin];
#endif
		return _levelDataColor[index % _levelDataColor.Length];
	}
}
