public class TAGS_STRUCTS
{

    public string Value { get; private set; }
    private TAGS_STRUCTS(string value) { Value = value; }
    public static string INVALID_PLACEMENT { get { return new TAGS_STRUCTS("InvalidStruct").Value; } }


    public override string ToString()
    {
        return Value;
    }


}