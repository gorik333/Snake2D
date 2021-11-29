using UnityEngine;

[CreateAssetMenu(fileName = "LevelData.asset", menuName = "SnakeIO/Level Configuration")]
public class LevelDataObject : ScriptableObject
{
    [Header("Cube chances")]
    public float OneCubeChance = 17;

    public float TwoCubeChance = 11; 

    public float CubeLineChance = 1;

    [Header("Food chances")]
    public float OneFoodChance = 6;

    public float TwoFoodChance = 5;

    public float ThreeFoodChance = 2;

    [Header("Lines to certain action")]
    public int LinesToFinish = 50;

    public int LinesToCubeLine = 17;

    [Header("Block colors")]
    public Color StartColor;

    public Color MiddleColor;

    public Color EndColor;

    public int StartColorNumber = 50;

    public int MiddleColorNumber = 25;

    public int EndColorNumber = 1;

    [Header("Background color")]
    public Color BackgroundColor;
}
