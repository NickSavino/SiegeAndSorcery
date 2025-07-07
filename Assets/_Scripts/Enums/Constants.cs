using UnityEngine;

/// <summary>
/// Enum for some program-wide constant values;
/// </summary>
public class Constants
{
    public Color Value { get; private set; }

    public string Name { get; private set; }
    private Constants(Color value) { Value = value; }

    private Constants(string name) { Name = name; }

    public static Color UNIT_ACTIVATED { get { return new Constants(Color.green).Value; } }
    public static Color UNIT_DEACTIVATED { get { return new Constants(Color.white).Value; } }

    public static string UI_SELECT_BOX { get { return new Constants("SelectBox").Name; } }





}