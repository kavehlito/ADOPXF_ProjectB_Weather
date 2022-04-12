using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Models;
using Weather.Services;

namespace Weather.Consoles
{
    //Your can move your Console application Main here. Rename Main to myMain and make it NOT static and async
    class Program
    {
        #region used by the Console
        Views.ConsolePage theConsole;
        StringBuilder theConsoleString;
        public Program(Views.ConsolePage myConsole)
        {
            //used for the Console
            theConsole = myConsole;
            theConsoleString = new StringBuilder();
        }
        #endregion


        public async Task MyMain()
        {
            //Register the event
            OpenWeatherService service = new OpenWeatherService();
            service.WeatherForecastAvailable += ReportWeatherDataAvailable;

            Task<Forecast> t1 = null;
            Exception exception = null;

            string city = "Märsta";

            //Create the two tasks and wait for comletion
            await service.GetForecastAsync(city);

            theConsole.WriteLine("-----------------");

            if (t1?.Status == TaskStatus.RanToCompletion)
            {
                Forecast forecast = t1.Result;
                theConsole.WriteLine($"Weather forecast for {forecast.City}");
                var GroupedList = forecast.Items.GroupBy(item => item.DateTime.Date);
                foreach (var group in GroupedList)
                {
                    theConsoleString.AppendLine(group.Key.Date.ToShortDateString());
                    foreach (var item in group)
                    {
                        theConsoleString.AppendLine($"   - {item.DateTime.ToShortTimeString()}: {item.Description}, temperature: {item.Temperature} degC, wind: {item.WindSpeed} m/s");
                    }
                }
                theConsole.WriteLine(theConsoleString.ToString());
            }
            else
            {
                theConsole.WriteLine($"Geolocation weather service error.");
                theConsole.WriteLine($"Error: {exception.Message}");
            }
            theConsoleString.Clear();
            await Task.Delay(1000);
            theConsole.WriteLine("-----------------");

        }
        void ReportWeatherDataAvailable(object sender, string message)
        {
            theConsole.WriteLine($"Event message from weather service: {message}");
        }
    }
}
