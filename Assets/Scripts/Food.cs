using UnityEngine;
using TMPro;

public class Food : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro _textMeshPro;

    private int _foodCount;


    private void Start()
    {
        SetUp();
    }


    private void SetUp()
    {
        _foodCount = Random.Range(1, 5);

        _textMeshPro.text = _foodCount.ToString();
    }


    public int GetFoodCount => _foodCount;
}
