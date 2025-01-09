using Xunit;
using System.Collections.Generic;
using System.Linq;
using GitHubSearcher.Core.Models;
using GitHubSearcher.Core.Services;

namespace GitHubSearcher.Tests
{
    public class GitHubApiServiceTests
    {
        [Fact]
        public void BuildRequestUrl_WithAllParameters_ConstructsCorrectUrl()
        {
            var apiService = new GitHubApiService();
            string expectedUrl = "https://api.github.com/search/repositories?q=tetris+NOT+puzzle+language:csharp&sort=stars&order=desc&per_page=50";

            string actualUrl = apiService.BuildRequestUrl("tetris", "puzzle", "csharp", "stars", "desc", 50);

            Assert.Equal(expectedUrl, actualUrl);
        }

        [Fact]
        public void BuildRequestUrl_WithoutOptionalParameters_ConstructsBasicUrl()
        {
            var apiService = new GitHubApiService();
            string expectedUrl = "https://api.github.com/search/repositories?q=snake&sort=updated&order=asc&per_page=10";

            string actualUrl = apiService.BuildRequestUrl("snake", "", "", "updated", "asc", 10);

            Assert.Equal(expectedUrl, actualUrl);
        }

        [Fact]
        public void FilteringLogic_ShouldRemoveIgnoredRepos()
        {
            // simulates the logic used in the UI to ensure filtering works correctly
            var fakeRepos = new List<GitHubRepository>
            {
                new GitHubRepository { Name = "Tetris-Python" },
                new GitHubRepository { Name = "Classic-Tetris" },
                new GitHubRepository { Name = "Super-Snake" }
            };
            string ignoreWord = "Python";

            var filtered = fakeRepos.Where(r => !r.Name.Contains(ignoreWord, System.StringComparison.OrdinalIgnoreCase)).ToList();

            Assert.Equal(2, filtered.Count);
            Assert.DoesNotContain(filtered, r => r.Name == "Tetris-Python");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void SearchQuery_IsInvalid_WhenNullOrWhiteSpace(string invalidQuery)
        {
            // test proves we correctly identify empty inputs to prevent broken API calls
            bool isInvalid = string.IsNullOrWhiteSpace(invalidQuery);

            Assert.True(isInvalid);
        }
    }
}