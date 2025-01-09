using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GitHubSearcher.Core.Models;

namespace GitHubSearcher.Core.Services
{
    /// <summary>
    /// Handles all communication with the external GitHub REST API.
    /// </summary>
    public class GitHubApiService
    {
        private readonly HttpClient _httpClient;

        private const string BaseApiUrl = "https://api.github.com/search/repositories";
        private const string UserAgentName = "GitHubSearcher-UniversityProject";

        public GitHubApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgentName);
        }

        /// <summary>
        /// Queries the GitHub API for repositories based on the provided parameters.
        /// </summary>
        public async Task<GitHubSearchResponse?> SearchRepositoriesAsync(string query, string ignoreWord, string language, string sort, string order, int perPage)
        {
            string requestUrl = BuildRequestUrl(query, ignoreWord, language, sort, order, perPage);

            // if http request fails, throws an exception that UI layer handle and display.
            return await _httpClient.GetFromJsonAsync<GitHubSearchResponse>(requestUrl);
        }

        /// <summary>
        /// Constructs the properly formatted GitHub API query URL.
        /// </summary>
        public string BuildRequestUrl(string query, string ignoreWord, string language, string sort, string order, int perPage)
        {
            string finalQuery = query;

            if (!string.IsNullOrWhiteSpace(ignoreWord))
            {
                finalQuery += $"+NOT+{ignoreWord}";
            }

            if (!string.IsNullOrWhiteSpace(language))
            {
                finalQuery += $"+language:{language}";
            }

            return $"{BaseApiUrl}?q={finalQuery}&sort={sort}&order={order}&per_page={perPage}";
        }
        public void SaveRawJsonResponse(string query, string jsonContent)
        {
            string folderPath = "SavedResults";

            // create the folder if it doesn't exist
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            string fileName = $"{query.Replace(" ", "_")}-{timestamp}.json";
            string fullPath = Path.Combine(folderPath, fileName);

            File.WriteAllText(fullPath, jsonContent);
        }
    }
}