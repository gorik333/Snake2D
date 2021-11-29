using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSkin : MonoBehaviour
{
	private Skin _currentSkin;
	private Color _defaulColor;

	private int _currentRepeat;
	private int _currentRepeatColor;

	private int _circleIndex = 0;

	public static SnakeSkin Instance { get; private set; }


	private void Awake()
	{
		Instance = this;
	}


	public void SetSkin( Skin skin, Color color )
	{
		_currentSkin = skin;
		_defaulColor = color;

		if (_currentSkin != null && _currentSkin.SnakeStyle == SnakeStyle.Default)
			_currentSkin = null;
	}


	public void AddFirstCircle( SpriteRenderer circle )
	{
		if (_currentSkin == null)
			circle.sharedMaterial.color = _defaulColor;
		else
		{
			circle.sharedMaterial.color = Color.white;

			if (_currentSkin.Head != null)
				circle.sprite = _currentSkin.Head;
			else if (_currentSkin.Head == null)
				CalcAddedCircleColor( circle );
		}
	}


	public void CalcAddedCircleColor( SpriteRenderer circle )
	{
		if (_currentSkin != null)
		{
			switch (_currentSkin.SnakeStyle)
			{
				case SnakeStyle.Default:
					return;
				case SnakeStyle.Gradient:
					GradientStyle( circle );
					break;
				case SnakeStyle.Repeat:
					RepeatStyle( circle );
					break;
				case SnakeStyle.Random:
					RandomStyle( circle );
					break;
			}
		}

		_circleIndex++;
	}


	public void RemoveCircle()
	{
		if (_currentSkin != null)
		{
			_circleIndex--;

			DecreaseRepeat();
		}
	}


	private void DecreaseRepeat()
	{
		_currentRepeat--;

		if (_currentRepeat < 0)
		{
			_currentRepeatColor--;

			if (_currentRepeatColor < 0)
				_currentRepeatColor = _currentSkin.SkinColor.Length - 1;

			_currentRepeat = _currentSkin.ColorRepeat - 1;
		}
	}


	private void GradientStyle( SpriteRenderer circle )
	{
		int colorIndex = _circleIndex / _currentSkin.GradientLength;

		var currentColor = _currentSkin.SkinColor[ colorIndex % _currentSkin.SkinColor.Length ];
		var nextColor = _currentSkin.SkinColor[ ( colorIndex + 1 ) % _currentSkin.SkinColor.Length ];

		circle.color = Color.Lerp( currentColor, nextColor, ( ( _circleIndex % _currentSkin.GradientLength ) + 1 ) / (float)_currentSkin.GradientLength );
		circle.sprite = _currentSkin.DefaultSprite;
	}


	private void RepeatStyle( SpriteRenderer circle )
	{
		circle.color = _currentSkin.SkinColor[ _currentRepeatColor ];
		circle.sprite = _currentSkin.DefaultSprite;

		_currentRepeat++;

		if (_currentRepeat >= _currentSkin.ColorRepeat)
		{
			_currentRepeatColor++;

			if (_currentRepeatColor > _currentSkin.SkinColor.Length - 1)
				_currentRepeatColor = 0;

			_currentRepeat = 0;
		}
	}


	private void RandomStyle( SpriteRenderer circle )
	{
		circle.color = _currentSkin.SkinColor[ Random.Range( 0, _currentSkin.SkinColor.Length ) ];
		circle.sprite = _currentSkin.DefaultSprite;
	}
}
