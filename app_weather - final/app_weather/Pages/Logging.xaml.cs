using app_weather.Search;
using app_weather.Services;
using app_weather.ViewModels;
using System.Collections.ObjectModel;

namespace app_weather.Pages
{
    public partial class Logging : ContentPage
    {
        private WeatherViewModel _viewModel;
        private WeatherService _weatherService;
        private SearchHistoryService _searchHistoryService;
        public Logging()
        {
            InitializeComponent();

            
            _weatherService = new WeatherService();
            _searchHistoryService = new SearchHistoryService();
            _viewModel = new WeatherViewModel(_weatherService, _searchHistoryService);

            // Set the BindingContext to the WeatherViewModel instance
            BindingContext = _viewModel;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Retrieve the newest search input when the logging page appears
            _viewModel.SearchHistoryList = new ObservableCollection<SearchHistoryItem>(_searchHistoryService.LoadSearchHistory());
        }

    }
}