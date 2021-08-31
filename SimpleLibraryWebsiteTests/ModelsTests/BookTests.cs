using SimpleLibraryWebsite.Models;
using Xunit;

namespace SimpleLibraryWebsiteTests.ModelsTests
{
    public class BookTests
    {
        [Fact]
        public void ShouldReturnTrueWhenBookHasNullProperties()
        {
            Book bookWithNullProperties = new();

            bool result = bookWithNullProperties.AnyFieldIsNullOrEmpty();

            Assert.True(result);
        }

        [Fact]

        public void ShouldReturnFalseWhenBookHasNotNullProperties()
        {
            Book bookWithNotNullProperties = new("Author", "Title", Genres.Scientific);

            bool result = bookWithNotNullProperties.AnyFieldIsNullOrEmpty();

            Assert.False(result);
        }
    }
}
