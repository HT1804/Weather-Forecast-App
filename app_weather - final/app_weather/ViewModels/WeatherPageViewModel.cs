using app_weather.Models;
using app_weather.Pages;
using app_weather.Search;
using app_weather.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace app_weather.ViewModels
{
    public class WeatherViewModel : INotifyPropertyChanged
    {
        private IWeatherService _weatherService;
        private ISearchHistoryService _searchHistoryService;
        private double _latitude;
        private double _longitude;
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;
     

        public WeatherViewModel(IWeatherService weatherService, ISearchHistoryService searchHistoryService)
        {
            _weatherService = weatherService;
            _searchHistoryService = searchHistoryService;
            WeatherList = new ObservableCollection<List>();
            SearchHistoryList = new ObservableCollection<SearchHistoryItem>(_searchHistoryService.SearchHistoryList);
            // Subscribe to the ConnectivityChanged event
            Connectivity.ConnectivityChanged += OnConnectivityChanged;

            // Check connectivity when the app starts
            _ = CheckConnectivity();
        }

        public async Task<bool> CheckConnectivity()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                await GetLocation();
                await GetWeatherByLocation(_latitude, _longitude);
                return true;
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("No Internet", "Please check your internet connection and try again.", "OK");
                return false;
            }
        }
        private async void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            var access = e.NetworkAccess;
            if (access != NetworkAccess.Internet)
            {
                // Display no internet alert
                await Application.Current.MainPage.DisplayAlert("No Internet", "You have lost internet connection.", "OK");
            }
            else
            {
                // Internet connection is regained
                await Application.Current.MainPage.DisplayAlert("Internet Regained", "You are now connected to the internet.", "OK");
            }
        }
        public async Task GetLocation()
        {
            try
            {
                _isCheckingLocation = true;

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                CancellationToken cancellationToken = _cancelTokenSource.Token;

                Location location = await Geolocation.GetLocationAsync(request, _cancelTokenSource.Token);

#if IOS
                request.RequestFullAccuracy = true;
                location = await Geolocation.GetLocationAsync(request);
#endif

                if (location != null)
                {
                    _latitude = location.Latitude;
                    _longitude = location.Longitude;
                }
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation
                await Application.Current.MainPage.DisplayAlert("Cancelled", "Geolocation request was cancelled.", "OK");
            }
            catch (PermissionException)
            {
                // Handle permission exception
                await Application.Current.MainPage.DisplayAlert("Error", "Location permission is denied. Please enable it in the device settings.", "OK");
            }
            catch (Exception ex)
            {
                // Handle other errors
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to retrieve location: {ex.Message}", "OK");
            }
            finally
            {
                _isCheckingLocation = false;
            }
        }

        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
                _cancelTokenSource.Cancel();
        }

        public async Task GetWeatherByLocation(double latitude, double longitude)
        {
            try
            {
                var result = await _weatherService.GetWeather(latitude, longitude);
                await UpdateWeather(result);
            }
            catch (HttpRequestException ex)
            {
                // Handle network-related errors
                await Application.Current.MainPage.DisplayAlert("Network Error", "Failed to retrieve weather data due to a network error.", "OK");
            }
            catch (JsonException ex)
            {
                // Handle data parsing errors
                await Application.Current.MainPage.DisplayAlert("Data Parsing Error", "Failed to parse weather data.", "OK");
            }
            catch (Exception ex)
            {
                // Handle other types of errors
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to retrieve weather: {ex.Message}", "OK");
            }
        }
        private bool _isSearchHistoryEmpty;
        public bool IsSearchHistoryEmpty
        {
            get { return _isSearchHistoryEmpty; }
            set
            {
                _isSearchHistoryEmpty = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<SearchHistoryItem> _searchHistoryList;
        public ObservableCollection<SearchHistoryItem> SearchHistoryList
        {
            get { return _searchHistoryList; }
            set
            {
                _searchHistoryList = value;
                IsSearchHistoryEmpty = value.Count == 0;
                OnPropertyChanged(nameof(SearchHistoryList));
            }
        }
        public async Task GetWeatherByCity(string city)
        {
            bool isConnected = await CheckConnectivity();
            if (isConnected)
            {
                try
                {
                    var result = await _weatherService.GetWeatherByCity(city);
                    await UpdateWeather(result);

                    var searchItem = new SearchHistoryItem
                    {
                        Query = city,
                        Timestamp = DateTime.Now
                    };
                    _searchHistoryService.AddSearchHistoryItem(searchItem);

                    SearchHistoryList = new ObservableCollection<SearchHistoryItem>(_searchHistoryService.SearchHistoryList);

                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to retrieve weather: {ex.Message}", "OK");
                }
            }
        }

        private ObservableCollection<List> _weatherList;
        public ObservableCollection<List> WeatherList
        {
            get { return _weatherList; }
            set
            {
                _weatherList = value;
                OnPropertyChanged();
            }
        }

        private string _city;
        public string City
        {
            get { return _city; }
            set
            {
                _city = value;
                OnPropertyChanged();
            }
        }

        private string _date;
        public string Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        private string _weatherDescription;
        public string WeatherDescription
        {
            get { return _weatherDescription; }
            set
            {
                _weatherDescription = value;
                OnPropertyChanged();
            }
        }

        private string _temperature;
        public string Temperature
        {
            get { return _temperature; }
            set
            {
                _temperature = value;
                OnPropertyChanged();
            }
        }

        private string _humidity;
        public string Humidity
        {
            get { return _humidity; }
            set
            {
                _humidity = value;
                OnPropertyChanged();
            }
        }

        private string _wind;
        public string Wind
        {
            get { return _wind; }
            set
            {
                _wind = value;
                OnPropertyChanged();
            }
        }

        private string _weatherIcon;
        public string WeatherIcon
        {
            get { return _weatherIcon; }
            set
            {
                _weatherIcon = value;
                OnPropertyChanged();
            }
        }

        public async Task UpdateWeather(Root result)
        {
            if (result != null)
            {
                try
                {
                    WeatherList = new ObservableCollection<List>(result.list);

                    if (result.city != null)
                        City = result.city.name;

                    if (result.list != null && result.list.Count > 0)
                    {
                        var listItem = result.list[0];

                        {
                            long timestamp = listItem.dt;
                            int timeZoneOffsetSeconds = result.city.timezone;

                            DateTimeOffset dateTimeOffsetUtc = DateTimeOffset.FromUnixTimeSeconds(timestamp);
                            DateTimeOffset dateTimeOffsetLocal = dateTimeOffsetUtc.ToOffset(TimeSpan.FromSeconds(timeZoneOffsetSeconds));

                            string formattedDateTime = dateTimeOffsetLocal.ToString("dd-MM-yyyy HH:mm:ss");
                            Date = formattedDateTime;
                        }

                        if (listItem.weather != null && listItem.weather.Count > 0)
                            WeatherDescription = listItem.weather[0].description;

                        if (listItem.main != null)
                            Temperature = listItem.main.temperature + "°C";

                        if (listItem.main != null)
                            Humidity = listItem.main.humidity + "%";

                        if (listItem.wind != null)
                            Wind = listItem.wind.speed + "km/h";

                        if (listItem.weather != null && listItem.weather.Count > 0)
                            WeatherIcon = listItem.weather[0].customIcon;
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to parse weather data: {ex.Message}", "OK");
                }
            }
        }
        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
