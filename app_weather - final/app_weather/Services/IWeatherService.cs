using app_weather.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_weather.Services
{
    public interface IWeatherService
    {
        Task<Root> GetWeather(double latitude, double longitude);

        Task<Root> GetWeatherByCity(string city);

    }
}
