using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json; //Requires nuget package System.Net.Http.Json
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text.Json;

using Weather.Models;

namespace Weather.Services
{
    //You replace this class witth your own Service from Project Part A
    public class OpenWeatherService
    {
        ConcurrentDictionary<(string, string), Forecast> _Cityforecastcache = new ConcurrentDictionary<(string, string), Forecast>();
        ConcurrentDictionary<(double, double, string), Forecast> _Coordsforecastcache = new ConcurrentDictionary<(double, double, string), Forecast>();

        HttpClient httpClient = new HttpClient();
        readonly string apiKey = "06108d4761e05b311b258326f90ec128"; // Your API Key


        public event EventHandler<string> WeatherForecastAvailable;

        protected virtual void OnWeatherAvailable(string e)
        {
            WeatherForecastAvailable?.Invoke(this, e);
        }

        public async Task<Forecast> GetForecastAsync(string city)
        {

            Forecast forecast;

            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            if (!_Cityforecastcache.TryGetValue((city, date), out forecast))
            {
                //https://openweathermap.org/current
                var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&units=metric&lang={language}&appid={apiKey}";
                forecast = await ReadWebApiAsync(uri);

                _Cityforecastcache.TryAdd((city, date), forecast);

                OnWeatherAvailable($"New weather forecast for {city} available");
            }
            else OnWeatherAvailable($"Cached weather forecast for {city} available");

            return forecast;

        }
        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {

            Forecast forecast;

            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            if (!_Coordsforecastcache.TryGetValue((latitude, longitude, date), out forecast))
            {
                //https://openweathermap.org/current
                var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";


                forecast = await ReadWebApiAsync(uri);
                _Coordsforecastcache.TryAdd((latitude, longitude, date), forecast);
                OnWeatherAvailable($"New weather forecast for ({latitude}, {longitude}) available");

            }
            else OnWeatherAvailable($"Cached weather forecast for ({latitude}, {longitude}) available");


            return forecast;
        }
        private async Task<Forecast> ReadWebApiAsync(string uri)
        {
            // part of your read web api code here
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();
            
            /*var webClient = new WebClient();
            var json = await webClient.DownloadStringTaskAsync(uri);
            WeatherApiData wd = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherApiData>(json);*/

            // part of your data transformation to Forecast here
            Forecast forecast = new Forecast();

                forecast.City = wd.city.name;
                forecast.Items = wd.list.Select(item => new ForecastItem
                {
                    DateTime = UnixTimeStampToDateTime(item.dt),
                    Temperature = item.main.temp,
                    WindSpeed = item.wind.speed,
                    Description = item.weather.Select(desc => desc.description).FirstOrDefault().ToString(),
                    Icon = item.weather.Select(icon => icon.icon).FirstOrDefault().ToString()
                }).ToList();
                forecast.Items.ForEach(x => x.Icon = $"http://openweathermap.org/img/wn/{x.Icon}@2x.png");
            //generate an event with different message if cached data

            return forecast;
        }
        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}
