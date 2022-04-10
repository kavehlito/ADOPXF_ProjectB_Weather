using System;
using System.Linq;
using System.Net;
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

        /*
        #region Console Demo program
        //This is the method you replace with your async method renamed and NON static Main
        public async Task myMain1()
        {
            theConsole.WriteLine("Demo program output");

            //Write an output to the Console
            theConsole.Write("One ");
            theConsole.Write("Two ");
            theConsole.WriteLine("Three and end the line");

            //As theConsole.WriteLine return trips are quite slow in UWP, use instead of myConsoleString to build the the more complex output
            //string using several myConsoleString.AppendLine instead of several theConsole.WriteLine. 
            foreach (char c in "Hello World from my Console program")
            {
                theConsoleString.Append(c);
            }

            //Once the string is complete Write it to the Console
            theConsole.WriteLine(theConsoleString.ToString());

            theConsole.WriteLine("Wait for 2 seconds...");
            await Task.Delay(2000);

            //Finally, demonstrating getting some data async
            theConsole.WriteLine("Download from https://dotnet.microsoft.com/...");
            theConsoleString.Clear();
            using (var w = new WebClient())
            {
                string str = await w.DownloadStringTaskAsync("https://dotnet.microsoft.com/");
                theConsoleString.Append($"Nr of characters downloaded: {str.Length}");
            }
            theConsole.WriteLine(theConsoleString.ToString());
        }

        //If you have any event handlers, they could be placed here
        void myEventHandler(object sender, string message)
        {
            theConsole.WriteLine($"Event message: {message}"); //theConsole is a Captured Variable, don't use myConsoleString here
        }
        #endregion
        */

        public async Task MyMain()
        {
            //Register the event
            OpenWeatherService service = new OpenWeatherService();
            service.WeatherForecastAvailable += ReportWeatherDataAvailable;

            Task<Forecast> t1 = null, t2 = null, t3 = null, t4 = null;
            Exception exception = null;
            try
            {
                double latitude = 59.5086798659495;
                double longitude = 18.2654625932976;
                string city = "Märsta";

                //Create the two tasks and wait for comletion
                await service.GetForecastAsync(city);
                await service.GetForecastAsync(city);

                t1 = service.GetForecastAsync(city);
                t2 = service.GetForecastAsync(city);
                Task.WaitAll(t1, t2);

                t3 = service.GetForecastAsync(city);
                t4 = service.GetForecastAsync(city);
                Task.WaitAll(t3, t4);
                
            }
            catch (Exception ex)
            {
                //if exception write the message later
                exception = ex;
            }

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
            
            if (t2.Status == TaskStatus.RanToCompletion)
            {
                Forecast forecast = t2.Result;
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
                theConsole.WriteLine($"City weather service error");
                theConsole.WriteLine($"Error: {exception.Message}");
            }
            theConsoleString.Clear();
            theConsole.WriteLine("-----------------");
            await Task.Delay(1000);

            if (t3?.Status == TaskStatus.RanToCompletion)
            {
                Forecast forecast = t3.Result;
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
            theConsole.WriteLine("-----------------");
            await Task.Delay(1000);

            if (t4.Status == TaskStatus.RanToCompletion)
            {
                Forecast forecast = t4.Result;
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
                theConsole.WriteLine($"City weather service error");
                theConsole.WriteLine($"Error: {exception.Message}");
            }
            theConsoleString.Clear();
            await Task.Delay(1000);
        }
        void ReportWeatherDataAvailable(object sender, string message)
        {
            theConsole.WriteLine($"Event message from weather service: {message}");
        }
    }
}
