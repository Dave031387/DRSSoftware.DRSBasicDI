namespace DRSSoftware.DRSBasicDI.TestClasses.Classes;

using DRSSoftware.DRSBasicDI.Extensions;
using DRSSoftware.DRSBasicDI.TestClasses.Interfaces;

public class GenericClass1<T1, T2> : IGenericClass1<T1, T2>
{
    public string DoWork(T1 arg1, T2 arg2)
    {
        string type1 = typeof(T1).GetFriendlyName();
        string type2 = typeof(T2).GetFriendlyName();

        return $"GenericClass1<{type1}, {type2}>.DoWork\n  arg1={arg1}\n  arg2={arg2}";
    }
}