using GitHubSearcher.Core.Models;
using GitHubSearcher.Core.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GitHubSearcher.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GitHubApiService _apiService;
        private readonly LoggingService _loggingService;

        public MainWindow()
        {
            InitializeComponent();

            // Instantiate our Core services
            _apiService = new GitHubApiService();
            _loggingService = new LoggingService();
        }

        /// <summary>
        /// Handles the click event for the Search button.
        /// </summary>
private async void SearchButton_Click(object sender, RoutedEventArgs e)
{
    string query = SearchBox.Text.Trim();

    if (string.IsNullOrWhiteSpace(query))
    {
        MessageBox.Show("Please enter a search query.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
    }

    SearchButton.IsEnabled = false;
    StatusText.Text = "Searching GitHub...";
    ResultsDataGrid.ItemsSource = null;

    try
    {
        string ignoreWord = IgnoreBox.Text.Trim();
        string language = LanguageBox.Text.Trim();
        string sort = ((ComboBoxItem)SortComboBox.SelectedItem).Tag?.ToString() ?? "";
        string order = ((ComboBoxItem)OrderComboBox.SelectedItem).Tag?.ToString() ?? "desc";
        int limit = int.Parse(((ComboBoxItem)LimitComboBox.SelectedItem).Content.ToString() ?? "30");

        // use a temporary HttpClient to get the raw string for saving
        using (var client = new System.Net.Http.HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "GitHubSearcher-UniversityProject");
            string url = _apiService.BuildRequestUrl(query, ignoreWord, language, sort, order, limit);
            
            // Fetch the raw JSON string
            string jsonString = await client.GetStringAsync(url);

            if (SaveJsonCheckBox.IsChecked == true)
            {
                _apiService.SaveRawJsonResponse(query, jsonString);
            }

            // Convert the string into C# objects
            var response = System.Text.Json.JsonSerializer.Deserialize<GitHubSearchResponse>(jsonString);

            if (response != null && response.Items != null)
            {
                var finalItems = response.Items.ToList();

                if (!string.IsNullOrWhiteSpace(ignoreWord))
                {
                    finalItems = finalItems
                        .Where(repo => !repo.Name.Contains(ignoreWord, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                ResultsDataGrid.ItemsSource = finalItems;
                StatusText.Text = $"Found {finalItems.Count} results.";
                await _loggingService.LogSearchAsync(query, finalItems.Count);
            }
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"An error occurred: {ex.Message}", "Search Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        StatusText.Text = "Search failed.";
    }
    finally
    {
        SearchButton.IsEnabled = true;
    }
}

        /// <summary>
        /// Opens the search log text file in the default text editor.
        /// </summary>
        private void ViewLogsButton_Click(object sender, RoutedEventArgs e)
        {
            string logFilePath = "search_logs.txt";

            if (File.Exists(logFilePath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = logFilePath,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show("No logs exist yet. Try making a successful search first!", "Logs Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ResultsDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ResultsDataGrid.SelectedItem is GitHubRepository selectedRepo)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = selectedRepo.Url,
                    UseShellExecute = true
                });
            }
        }
        private void ViewSavedButton_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SavedResults");
            if (Directory.Exists(folderPath))
            {
                Process.Start(new ProcessStartInfo { FileName = folderPath, UseShellExecute = true, Verb = "open" });
            }
            else
            {
                MessageBox.Show("The SavedResults folder hasn't been created yet. Search with the checkbox enabled!", "Folder Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}