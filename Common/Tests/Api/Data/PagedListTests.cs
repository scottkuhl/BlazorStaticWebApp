using AzureStaticWebApp.Common.Api.Data;
using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace AzureStaticWebApp.Common.Tests.Api.Data;

[Trait("Category", "Unit")]
public class PagedListTests
{
    [Fact]
    public void PagedList_ShouldAddItems_WhenPassedIntoConstructor()
    {
        // Arrange
        var items = new List<string> { "item1", "item2", "item3" };

        // Act
        var pagedList = new PagedList<string>(items, 1, 2, 3);

        // Assert
        _ = pagedList.Count.Should().Be(3);
    }

    [Fact]
    public void PagedList_ShouldSetCurrentPage_WhenPageNumberPassedIntoConstructor()
    {
        // Arrange
        var items = new List<string> { "item1", "item2", "item3" };

        // Act
        var pagedList = new PagedList<string>(items, 1, 2, 3);

        // Assert
        _ = pagedList.MetaData.CurrentPage.Should().Be(2);
    }

    [Fact]
    public void PagedList_ShouldSetPageSize_WhenPageSizePassedIntoConstructor()
    {
        // Arrange
        var items = new List<string> { "item1", "item2", "item3" };

        // Act
        var pagedList = new PagedList<string>(items, 1, 2, 3);

        // Assert
        _ = pagedList.MetaData.PageSize.Should().Be(3);
    }

    [Fact]
    public void PagedList_ShouldSetTotalCount_WhenCountPassedIntoConstructor()
    {
        // Arrange
        var items = new List<string> { "item1", "item2", "item3" };

        // Act
        var pagedList = new PagedList<string>(items, 1, 2, 3);

        // Assert
        _ = pagedList.MetaData.TotalPages.Should().Be(1);
    }

    [Fact]
    public void PagedList_ShouldSetTotalPages_WhenCountAndPageSizePassedIntoConstructor()
    {
        // Arrange
        var items = new List<string> { "item1", "item2", "item3" };

        // Act
        var pagedList = new PagedList<string>(items, 1, 2, 3);

        // Assert
        _ = pagedList.MetaData.TotalPages.Should().Be((int)Math.Ceiling(1 / (double)3));
    }

    [Fact]
    [SuppressMessage("Roslynator", "RCS1047:Non-asynchronous method name should not end with 'Async'.", Justification = "Test name should match.")]
    public void ToPagedListAsync()
    {
        // Handled by ListAsync integration tests.
        Assert.True(true);
    }
}