using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CubeObstacle : MonoBehaviour
{
	[SerializeField]
	private TextMeshPro _currentDurabilityText;

	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private ParticleSystemController _particleSystemController;

	private int _currentDurabilityNumber;
	private int _upperBound;

	private Color _startColor;
	private Color _middleColor;
	private Color _endColor;


	public void SetUp( LevelDataColor levelDataColor, int durability, int upperBound )
	{
		_startColor = levelDataColor.StartColor;
		_middleColor = levelDataColor.MiddleColor;
		_endColor = levelDataColor.EndColor;

		_currentDurabilityText.color = levelDataColor.TextColor;

		_currentDurabilityNumber = durability;
		_upperBound = upperBound;

		SetStartColor();

		UpdateDurabilityInfo();
	}


	private void SetStartColor()
	{
		Color newColor;

		float middleBound = _upperBound / 2f;

		if( _currentDurabilityNumber >= middleBound )
			newColor= Color.Lerp( _startColor, _middleColor, (_upperBound - _currentDurabilityNumber) / middleBound );
		else
			newColor= Color.Lerp( _endColor, _middleColor, _currentDurabilityNumber / middleBound );

		_spriteRenderer.color = newColor;
	}


	private void CalcCurrentColor()
	{
		Color newColor;

		int middleBound = _upperBound / 2;

		if (_currentDurabilityNumber > middleBound)
		{
			var result = _startColor - _middleColor;

			newColor = _spriteRenderer.color - ( result / middleBound );
		}
		else
		{
			var result = _middleColor - _endColor;

			newColor = _spriteRenderer.color - ( result / middleBound );
		}

		_spriteRenderer.color = newColor;
	}


	private void UpdateDurabilityInfo()
	{
		_currentDurabilityText.text = _currentDurabilityNumber.ToString();
	}


	private void CheckIfDestroy()
	{
		if (_currentDurabilityNumber <= 0)
		{
			_particleSystemController.SpawnParticles( _endColor );

			Destroy( transform.parent.gameObject );
		}
	}


	public void TakeDamage()
	{
		_currentDurabilityNumber--;

		UpdateDurabilityInfo();
		CheckIfDestroy();
		CalcCurrentColor();

		EffectProc proc= EffectProc.GetProc( _spriteRenderer.gameObject );

		if( proc.HasEffects() == false )
		{
			const float sc= 0.385f; //curr scale

			proc.AddEffect( new FxScale( sc, sc * 0.92f, 0.05f, false ) );
			proc.AddEffect( new FxScale( sc * 0.92f, sc, 0.05f, false ) );

			Vibro.Light();
		}
	}


	public int CurrentDurability => _currentDurabilityNumber;

	public int UppedBound => _upperBound;
}
