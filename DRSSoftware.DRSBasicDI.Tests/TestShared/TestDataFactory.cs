namespace DRSSoftware.DRSBasicDI.TestShared;

using DRSSoftware.DRSBasicDI.TestClasses.Classes;

public static class TestDataFactory
{
    public static TheoryData<Type, string> GetArrayTypes() => new()
    {
        { typeof(int[]), "int[]" },
        { typeof(char[,,]), "char[,,]" },
        { typeof(object[,]), "object[,]" },
        { typeof(Class1[,,,]), "Class1[,,,]" },
        { typeof(GenericClass1<int, string>[]), "GenericClass1<int, string>[]" },
        { typeof(byte?[,]), "byte?[,]" }
    };

    public static TheoryData<Type, string> GetGenericTypes() => new()
    {
        { typeof(IEnumerable<Class1A>), "IEnumerable<Class1A>" },
        { typeof(GenericClass1<string, Class2>), "GenericClass1<string, Class2>" },
        { typeof(IEnumerable<GenericClass1<int?, char[]>>), "IEnumerable<GenericClass1<int?, char[]>>" }
    };

    public static TheoryData<Type, string> GetNullableTypes() => new()
    {
        { typeof(char?), "char?" },
        { typeof(int?), "int?" },
        { typeof(GenericClass1<float?, char>), "GenericClass1<float?, char>" },
        { typeof(ulong?), "ulong?" }
    };

    public static TheoryData<Type, string> GetPredefinedTypes() => new()
    {
        { typeof(bool), "bool" },
        { typeof(byte), "byte" },
        { typeof(char), "char" },
        { typeof(decimal), "decimal" },
        { typeof(double), "double" },
        { typeof(float), "float" },
        { typeof(int), "int" },
        { typeof(long), "long" },
        { typeof(object), "object" },
        { typeof(sbyte), "sbyte" },
        { typeof(short), "short" },
        { typeof(string), "string" },
        { typeof(uint), "uint" },
        { typeof(ulong), "ulong" },
        { typeof(ushort), "ushort" },
        { typeof(void), "void" }
    };
}