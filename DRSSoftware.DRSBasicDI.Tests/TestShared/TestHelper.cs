namespace DRSSoftware.DRSBasicDI.TestShared;

using DRSSoftware.DRSBasicDI.Interfaces;

internal static class TestHelper
{
    internal static void AssertException<T>(Action action, string message) where T : Exception
    {
        action
            .Should()
            .ThrowExactly<T>()
            .WithMessage(message);
    }

    internal static void AssertException<T>(Action action, string inner, string outer)
        where T : Exception
    {
        action
            .Should()
            .ThrowExactly<T>()
            .WithMessage(outer)
            .WithInnerExceptionExactly<T>()
            .WithMessage(inner);
    }

    internal static void AssertException<TInner, TOuter>(Action action, string inner, string outer)
        where TInner : Exception where TOuter : Exception
    {
        action
            .Should()
            .ThrowExactly<TOuter>()
            .WithMessage(outer)
            .WithInnerExceptionExactly<TInner>()
            .WithMessage(inner);
    }

    internal static IDependency CreateScopedDependency<TDependency, TResolving>(string resolvingKey = EmptyKey)
        => new Dependency(typeof(TDependency), typeof(TResolving), DependencyLifetime.Scoped, resolvingKey);

    internal static IDependency CreateSingletonDependency<TDependency, TResolving>(string resolvingKey = EmptyKey)
        => new Dependency(typeof(TDependency), typeof(TResolving), DependencyLifetime.Singleton, resolvingKey);

    internal static IDependency CreateTransientDependency<TDependency, TResolving>(string resolvingKey = EmptyKey)
        => new Dependency(typeof(TDependency), typeof(TResolving), DependencyLifetime.Transient, resolvingKey);
}