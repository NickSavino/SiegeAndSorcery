public class STRUCTS_NAMES
{

    public string Value { get; private set; }
    private STRUCTS_NAMES(string value) { Value = value; }
    public static string UNIT_COLLIDER { get { return new STRUCTS_NAMES("UnitCollider").Value; } }
    public static string STRUCTURE_MANAGER { get { return new STRUCTS_NAMES("StructureManager").Value; } }

    public static string UNIT_PATH { get { return new STRUCTS_NAMES("UnitPath").Value; } }
    public static string ACTIVE_PATH { get { return new STRUCTS_NAMES("ActivePath").Value; } }
    public static string TEMP_PATH { get { return new STRUCTS_NAMES("TempPath").Value; } }
    public override string ToString()
    {
        return Value;
    }


}