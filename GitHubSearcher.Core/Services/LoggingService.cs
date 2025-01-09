using System;
using System.IO;
using System.Threading.Tasks;

namespace GitHubSearcher.Core.Services
{
    /// <summary>
    /// Handles logging application events and searches to a local text file.
    /// </summary>
    public class LoggingService
    {
        private const string LogFileName = "search_logs.txt";

        /// <summary>
        /// Asyncly logs a search query and the number of results
        /// </summary>
        public async Task LogSearchAsync(string query, int resultCount)
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Searched for '{query}' - Found {resultCount} results.{Environment.NewLine}";
            await File.AppendAllTextAsync(LogFileName, logEntry);
        }
    }
}