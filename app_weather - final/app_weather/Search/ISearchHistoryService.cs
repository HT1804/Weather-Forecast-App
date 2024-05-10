using System.Collections.ObjectModel;

namespace app_weather.Search
{
    public interface ISearchHistoryService
    {
        ObservableCollection<SearchHistoryItem> SearchHistoryList { get; }
        void AddSearchHistoryItem(SearchHistoryItem item);
        void ClearSearchHistory();
        IEnumerable<SearchHistoryItem> LoadSearchHistory();
    }
}
