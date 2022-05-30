using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace AzureStaticWebApp.Common.Tests.Api.Data;

[Trait("Category", "Unit")]
public class VirtualListTests
{
    [Fact]
    [SuppressMessage("Roslynator", "RCS1047:Non-asynchronous method name should not end with 'Async'.", Justification = "Test name should match.")]
    public void ToVirtualListAsync()
    {
        // Handled by ListAsync integration tests.
        Assert.True(true);
    }
}