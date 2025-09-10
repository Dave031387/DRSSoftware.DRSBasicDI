namespace DRSSoftware.DRSBasicDI.TestShared;

using FluentAssertions;

internal static class TestHelper
{
    internal static void AssertException<T>(Action action, string message) where T : Exception
    {
        action
            .Should()
            .ThrowExactly<T>()
            .WithMessage(message);
    }
}
