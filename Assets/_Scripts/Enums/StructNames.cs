public class STRUCTS_NAMES
{

    public string Value { get; private set; }
    private STRUCTS_NAMES(string value) { Value = value; }
    public static string UNIT_COLLIDER { get { return new STRUCTS_NAMES("UnitCollider").Value; } }

    public override string ToString()
    {
        return Value;
    }


}