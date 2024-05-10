using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace app_weather.Search
{
    internal class SearchHistoryService : INotifyPropertyChanged, ISearchHistoryService
    {
        private const string SearchHistoryFileName = "search_history.json";
        private ObservableCollection<SearchHistoryItem> _searchHistoryList;

        public SearchHistoryService()
        {
            _searchHistoryList = new ObservableCollection<SearchHistoryItem>();
            LoadSearchHistory();
        }

        public ObservableCollection<SearchHistoryItem> SearchHistoryList
        {
            get { return _searchHistoryList; }
            set
            {
                _searchHistoryList = value;
                OnPropertyChanged();
                SaveSearchHistory();
            }
        }

        public void AddSearchHistoryItem(SearchHistoryItem item)
        {
            _searchHistoryList.Insert(0, item);
            OnPropertyChanged(nameof(SearchHistoryList));
            SaveSearchHistory();
        }

        public void ClearSearchHistory()
        {
            _searchHistoryList.Clear();
            SaveSearchHistory();
        }

        public IEnumerable<SearchHistoryItem> LoadSearchHistory()
        {
            try
            {
                var searchHistoryFilePath = GetSearchHistoryFilePath();
                if (File.Exists(searchHistoryFilePath))
                {
                    var json = File.ReadAllText(searchHistoryFilePath);
                    var loadedSearchHistory = JsonSerializer.Deserialize<ObservableCollection<SearchHistoryItem>>(json);
                    _searchHistoryList = new ObservableCollection<SearchHistoryItem>(loadedSearchHistory.Reverse());
                }
                else
                {
                    _searchHistoryList = new ObservableCollection<SearchHistoryItem>();
                }
                OnPropertyChanged(nameof(SearchHistoryList));
                return _searchHistoryList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load search history: {ex.Message}");
                return null;
            }
        }

        private void SaveSearchHistory()
        {
            try
            {
                var searchHistoryFilePath = GetSearchHistoryFilePath();
                var json = JsonSerializer.Serialize(_searchHistoryList.Reverse());
                File.WriteAllText(searchHistoryFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save search history: {ex.Message}");
            }
        }

        private string GetSearchHistoryFilePath()
        {
            var appDataDirectory = FileSystem.AppDataDirectory;
            return Path.Combine(appDataDirectory, SearchHistoryFileName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
