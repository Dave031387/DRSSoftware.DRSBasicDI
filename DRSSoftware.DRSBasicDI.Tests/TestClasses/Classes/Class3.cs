namespace DRSSoftware.DRSBasicDI.TestClasses.Classes;

using DRSSoftware.DRSBasicDI.Attributes;
using DRSSoftware.DRSBasicDI.TestClasses.Interfaces;

public class Class3([ResolvingKey("Test")] IClass2 class2) : IClass3
{
    private readonly IClass2 _class2 = class2;

    public string DoWork() => $"{_class2.DoWork()} called from Class3.DoWork";
}
