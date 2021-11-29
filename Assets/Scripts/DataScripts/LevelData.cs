using UnityEngine;

public class LevelData : MonoBehaviour
{
    [SerializeField]
    private LevelDataObject _levelDataObject;


    public float OneCubeChance => _levelDataObject.OneCubeChance;

    public float TwoCubesChance => _levelDataObject.TwoCubeChance;

    public float CubeLineChance => _levelDataObject.CubeLineChance;

    public float OneFoodChance => _levelDataObject.OneFoodChance;

    public float TwoFoodChance => _levelDataObject.TwoFoodChance;

    public float ThreeFoodChance => _levelDataObject.ThreeFoodChance;

    public int LinesToFinish => _levelDataObject.LinesToFinish;

    public int LinesToCubeLine => _levelDataObject.LinesToCubeLine;

    public Color StartColor => _levelDataObject.StartColor;

    public Color MiddleColor => _levelDataObject.MiddleColor;

    public Color EndColor => _levelDataObject.EndColor;

    public int StartColorNumber => _levelDataObject.StartColorNumber;

    public int MiddleColorNumber => _levelDataObject.MiddleColorNumber;

    public int EndColorNumber => _levelDataObject.EndColorNumber;

    public Color BackgroundColor => _levelDataObject.BackgroundColor;
}
