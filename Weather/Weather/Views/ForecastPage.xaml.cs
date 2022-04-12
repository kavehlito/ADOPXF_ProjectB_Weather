using System;
using System.Linq;
using System.Threading.Tasks;
using Weather.Models;
using Weather.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Weather.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ForecastPage : ContentPage
    {
        OpenWeatherService service;
        GroupedForecast groupedforecast;

        public ForecastPage()
        {
            InitializeComponent();
            service = new OpenWeatherService();
            groupedforecast = new GroupedForecast();
            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Code here will run right before the screen appears
            //You want to set the Title or set the City
            
            //This is making the first load of data
            MainThread.BeginInvokeOnMainThread(async () => { await LoadForecast(); });
        }

        private async Task LoadForecast()
        {
            //Here you load the forecast 

            await service.GetForecastAsync(Title);
            Task<Forecast> t1 = service.GetForecastAsync(Title);
            var title = Title;
            var items = t1.Result.Items;
            var groupedItems = items.OrderBy(f => f.DateTime.Date).GroupBy(f => f.DateTime.Date.ToString("dddd, MMMM d, yyyy"));

            GroupedDataList.ItemsSource = groupedItems;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        { 
            progBar.IsVisible = true;
            await progBar.ProgressTo(1, 4000, Easing.Linear);
            await LoadForecast();
            progBar.IsVisible = false;
        }
    }
}