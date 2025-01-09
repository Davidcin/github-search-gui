using System;
using System.Text.Json.Serialization;

namespace GitHubSearcher.Core.Models
{
    /// <summary>
    /// Represents a single GitHub repository returned from the GitHub API.
    /// </summary>
    public class GitHubRepository
    {

        [JsonPropertyName("name")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("html_url")]
        public string Url { get; init; } = string.Empty;

        [JsonPropertyName("stargazers_count")]
        public int Stars { get; init; }

        [JsonPropertyName("forks_count")]
        public int Forks { get; init; }

        [JsonPropertyName("language")]
        public string? Language { get; init; }

        [JsonPropertyName("updated_at")]
        public DateTime LastUpdated { get; init; }

        [JsonPropertyName("open_issues_count")]
        public int OpenIssues { get; init; }
    }

    /// <summary>
    /// Represents the main wrapper response from the GitHub Search API.
    /// </summary>
    public class GitHubSearchResponse
    {
        [JsonPropertyName("total_count")]
        public int TotalCount { get; init; }

        [JsonPropertyName("items")]
        public GitHubRepository[] Items { get; init; } = Array.Empty<GitHubRepository>();
    }
}