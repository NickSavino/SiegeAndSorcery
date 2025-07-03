using UnityEngine;

public class Constants
{
    public Color Value { get; private set; }
    private Constants(Color value) { Value = value; }
    public static Color UNIT_ACTIVATED { get { return new Constants(Color.green).Value; } }
    public static Color UNIT_DEACTIVATED { get { return new Constants(Color.white).Value; } }
}