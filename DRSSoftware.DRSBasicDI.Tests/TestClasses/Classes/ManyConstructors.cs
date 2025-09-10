namespace DRSSoftware.DRSBasicDI.TestClasses.Classes;

using DRSSoftware.DRSBasicDI.Attributes;

public class ManyConstructors
{
    public ManyConstructors()
    {
    }

    public ManyConstructors(int field1) => Field1 = field1;

    public ManyConstructors(string field2) => Field2 = field2;

    public ManyConstructors(string field2, bool field3)
    {
        Field2 = field2;
        Field3 = field3;
    }

    public ManyConstructors(int field1, bool field3)
    {
        Field1 = field1;
        Field3 = field3;
    }

    [DIConstructor()]
    public ManyConstructors(string field2, string field4)
    {
        Field2 = field2;
        Field4 = field4;
    }

    public ManyConstructors(int field1, string field4)
    {
        Field1 = field1;
        Field4 = field4;
    }

    public ManyConstructors(int field1, string field2, bool field3, string field4)
    {
        Field1 = field1;
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
    }

    public int Field1
    {
        get; set;
    }

    public string Field2 { get; set; } = "Field2";

    public bool Field3
    {
        get; set;
    }

    public string Field4 { get; set; } = "Field4";
}