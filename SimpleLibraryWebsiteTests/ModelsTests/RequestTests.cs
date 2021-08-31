using SimpleLibraryWebsite.Models;
using Xunit;

namespace SimpleLibraryWebsiteTests.ModelsTests
{
    public class RequestTests
    {
        [Fact]
        public void ShouldReturnTrueWhenRequestHasNullProperties()
        {
            Request requestWithNullProperties = new();

            bool result = requestWithNullProperties.AnyFieldIsNullOrEmpty();

            Assert.True(result);
        }

        [Fact]
        public void ShouldReturnFalseWhenRequestHasNotNullProperties()
        {
            Request requestWithNullProperties = new("0", "Title", "Author", Genres.Scientific);

            bool result = requestWithNullProperties.AnyFieldIsNullOrEmpty();

            Assert.False(result);
        }
    }
}
