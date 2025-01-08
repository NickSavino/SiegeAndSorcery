public class STRUCTS_NAMES
{

    public string Value { get; private set; }
    private STRUCTS_NAMES(string value) { Value = value; }
    public static string UNIT_COLLIDER { get { return new STRUCTS_NAMES("UnitCollider").Value; } }
    public static string STRUCTURE_MANAGER { get { return new STRUCTS_NAMES("StructureManager").Value; } }
    public override string ToString()
    {
        return Value;
    }


}