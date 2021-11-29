using UnityEngine;
using TMPro;

public enum GateType
{
	Division,
	Multiplication,
	Addition,
	Substraction
}

public class Gates : MonoBehaviour
{
	[SerializeField]
	private TextMeshPro _textMeshPro;

	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	private GateType _gateType;

	private int _number;

	private bool _isTriggered;

	/// <summary>
	/// max multiplication
	/// </summary>
	private const int MAX_MUL = 2;
	/// <summary>
	/// max division
	/// </summary>
	private const int MAX_DIV = 2;


	public void SetUpGate( float plus, float mul, int maxNum, Color gatesColor, Color textColor, bool isPositive = false )
	{
		GateType resultType;
		int resultNumber;

		if (plus > Random.Range( 0, 100 ) || isPositive)
		{
			if (mul > Random.Range( 0, 100 ))
			{
				resultNumber = MAX_MUL;
				resultType = GateType.Multiplication;
			}
			else
			{
				resultNumber = Random.Range( 1, maxNum );
				resultType = GateType.Addition;
			}
		}
		else
		{
			if (mul > Random.Range( 0, 100 ))
			{
				resultNumber = MAX_DIV;
				resultType = GateType.Division;
			}
			else
			{
				resultNumber = Random.Range( 1, maxNum );
				resultType = GateType.Substraction;
			}
		}

		_number = resultNumber;
		_gateType = resultType;

		_spriteRenderer.color = gatesColor;
		_textMeshPro.color = textColor;

		_textMeshPro.text = GetText( resultType, resultNumber );
	}


	private void OnTriggerEnter2D( Collider2D other )
	{
		if (other.GetComponent<SnakeTail>() != null && !IsTriggered)
		{
			SnakeTail snake = other.GetComponent<SnakeTail>();

			ChangeSnake( snake );

			DisableGates();

			Vibro.Medium();
		}
	}


	private void DisableGates()
	{
		Gates[] gates = transform.parent.gameObject.GetComponentsInChildren<Gates>();

		for (int i = 0; i < gates.Length; i++)
			gates[ i ].IsTriggered = true;

		Destroy( gameObject );
	}


	private void ChangeSnake( SnakeTail snake )
	{
		int snakeLength = snake.SnakeLength;
		int result = 0;

		switch (_gateType)
		{
			case GateType.Multiplication:
				result = ( snakeLength * _number ) + 1;
				break;
			case GateType.Addition:
				result = snakeLength + _number;
				break;
			case GateType.Division:
				result = snakeLength / _number;
				break;
			case GateType.Substraction:
				result = snakeLength - _number;
				break;
		}

		int addition = snakeLength - result;

		if (addition < 0)
			snake.AddSnakePart( Mathf.Abs( addition ) );
		else
			snake.RemoveSnakePart( addition );
	}


	private static string GetText( GateType type, int number )
	{
		switch (type)
		{
			case GateType.Division: return "÷" + number;
			case GateType.Multiplication: return "*" + number;
			case GateType.Addition: return "+" + number;
			case GateType.Substraction: return "-" + number;
		}

		return "";
	}

	public bool IsTriggered { get => _isTriggered; set => _isTriggered = value; }
}
