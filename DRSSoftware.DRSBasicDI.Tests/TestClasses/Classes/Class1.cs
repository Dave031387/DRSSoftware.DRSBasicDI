namespace DRSSoftware.DRSBasicDI.TestClasses.Classes;

using DRSSoftware.DRSBasicDI.TestClasses.Interfaces;

public class Class1 : IClass1
{
    private static readonly bool _isInitialized;
    private readonly int _value;

    static Class1() => _isInitialized = true;

    public Class1()
    {
    }

    public Class1(string builtBy)
    {
        BuiltBy = builtBy;
    }

    private Class1(int value, string builtBy)
    {
        _value = value;
        BuiltBy = builtBy;
    }

    public static Class1 Instance => new(42, "Static");

    public string BuiltBy { get; init; } = "Normal";

    public virtual string DoWork() => "Class1.DoWork";
}