using app_weather.Search;
using app_weather.Services;
using app_weather.ViewModels;

namespace app_weather.Pages
{
    public partial class WeatherPage : ContentPage
    {      
        private WeatherService _weatherService;
        private SearchHistoryService _searchHistoryService;
        private WeatherViewModel _viewModel;

        public WeatherPage()
        {
            InitializeComponent();
            _weatherService = new WeatherService();
            _searchHistoryService = new SearchHistoryService();
            _viewModel = new WeatherViewModel(_weatherService, _searchHistoryService);

            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CheckConnectivity();
        }

        private async void ImageButton_Refresh(object sender, EventArgs e)
        {
            await _viewModel.CheckConnectivity();
        }

        private async void ImageButton_SetWeather(object sender, EventArgs e)
        {
            var response = await DisplayPromptAsync(title: "", message: "", placeholder: "Search weather by city", accept: "Search", cancel: "Cancel");
            if (!string.IsNullOrEmpty(response))
            {
                await _viewModel.GetWeatherByCity(response);
                
            }
        }
    }
}
