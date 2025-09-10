namespace DRSSoftware.DRSBasicDI.TestClasses.Classes;

using DRSSoftware.DRSBasicDI.TestClasses.Interfaces;

public abstract class AbstractClass1 : IClass1
{
    public string BuiltBy
    {
        get;
        init;
    } = "Abstract";

    public abstract string DoWork();
}
