using UnityEngine;

public enum SnakeStyle
{
	Gradient,
	Repeat, // красный белый красный белый
	Random, // красный белый белый красный красный белый
	Default // Standart snake color
}


[System.Serializable]
public class Skin
{
	public SnakeStyle SnakeStyle;

	public Sprite DefaultSprite;

	public Sprite Head;
	public Color[] SkinColor;
	public Sprite Tail;

	public int ColorRepeat;
	public int GradientLength;
}


public class SkinData : MonoBehaviour
{
	[SerializeField]
	private int _selectedSkin;

	[SerializeField]
	private Skin[] _skinData;


	public Skin GetSkin( int index )
	{
#if UNITY_EDITOR
		if (_selectedSkin >= 0)
			return _skinData[ _selectedSkin ];
#endif
		if (index == -1)
			return null;

		return _skinData[ index % _skinData.Length ];
	}
}
