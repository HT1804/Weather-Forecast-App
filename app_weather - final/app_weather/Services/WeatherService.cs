using app_weather.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace app_weather.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<Root> GetWeather(double latitude, double longitude)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&appid=7f127525312ab0cf40fae06126b1d7e3");
                return JsonConvert.DeserializeObject<Root>(response);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get weather data: " + ex.Message);
            }
        }

        public async Task<Root> GetWeatherByCity(string city)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"https://api.openweathermap.org/data/2.5/forecast?q={city}&units=metric&appid=7f127525312ab0cf40fae06126b1d7e3");
                return JsonConvert.DeserializeObject<Root>(response);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get weather data: " + ex.Message);
            }
        }
    }
}
