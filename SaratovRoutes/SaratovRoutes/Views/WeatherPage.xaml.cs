using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SaratovRoutes;
using Xamarin.Essentials;

namespace SaratovRoutes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WeatherPage : ContentPage
    {
        string[] cities = new string[]
        {
             "Вольск", "Саратов", "Энгельс", "Балашов","Лох",  "Маркс", "Хвалынск","Петровск"
        };

        const string API = "a6dda19cad21e5d32048c72a567f55d9";

        private async void CityPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCity = cityPicker.SelectedItem.ToString();
            await FetchWeatherData(selectedCity);
        }
        string apiKey = "a6dda19cad21e5d32048c72a567f55d9";
        private async Task FetchWeatherData(string city)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Нет подключения", "Пожалуйста, проверьте ваше интернет-соединение.", "ОК");
                //await Navigation.PopAsync();
                return;
            }

            string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={apiKey}&units=metric&lang=ru";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        WeatherForecast forecast = JsonConvert.DeserializeObject<WeatherForecast>(json);
                        DisplayWeatherForecast(forecast);
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Не удалось получить данные о погоде", "ОК");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", "Произошла ошибка при загрузке данных: " + ex.Message, "ОК");
                }
            }
        }

        private void DisplayWeatherForecast(WeatherForecast forecast)
        {
            var currentWeather = forecast.List[0];
            temperatureLabel.Text = $"Температура: {currentWeather.Main.Temp} °C";
            feelsLikeLabel.Text = $"Ощущается как: {currentWeather.Main.Feels_Like} °C";
            humidityLabel.Text = $"Влажность: {currentWeather.Main.Humidity}%";
            weatherDescriptionLabel.Text = $"Состояние: {currentWeather.Weather[0].Description}";
            DescriptionLabel.Text = "";
            weatherImage.Source = GetWeatherImage(currentWeather.Weather[0].Icon);

            //forecastLabel.Text = "Прогноз на 5 дней:";
            //foreach (var item in forecast.List)
            //{
            //  forecastLabel.Text += $"\n{item.DtTxt}: {item.Main.Temp} °C, {item.Weather[0].Description}";
            //}
        }
        private async void ButtonMap_Clicked(object sender, EventArgs e)
        {
            MapPage newPage = new MapPage();
            await Navigation.PushAsync(newPage);
        }
        private async void ButtonRoutes_Clicked(object sender, EventArgs e)
        {
            RoutesPage newPage = new RoutesPage();
            await Navigation.PushAsync((Page)newPage);
        }
        private async void OnButtonPressed(object sender, EventArgs e)
        {
            if (sender is VisualElement button)
            {
                await button.ScaleTo(0.9, 50, Easing.CubicIn);
            }
        }

        private async void OnButtonReleased(object sender, EventArgs e)
        {
            if (sender is VisualElement button)
            {
                await button.ScaleTo(1, 50, Easing.CubicOut);
            }
        }

        public WeatherPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            cityPicker.ItemsSource = cities;
            cityPicker.SelectedIndexChanged += CityPicker_SelectedIndexChanged;
        }

        private string GetWeatherImage(string icon)
        {
            return $"https://openweathermap.org/img/wn/{icon}@2x.png";
        }
    }

    public class WeatherForecast
    {
        public List<WeatherList> List { get; set; }
    }

    public class WeatherList
    {
        public Main Main { get; set; }
        public Weather[] Weather { get; set; }
        public string DtTxt { get; set; }
    }

    public class WeatherData
    {
        public Main Main { get; set; }
        public Weather[] Weather { get; set; }
    }
    public class Main
    {
        public float Temp { get; set; }
        public float Feels_Like { get; set; }
        public int Humidity { get; set; }
    }

    public class Weather
    {
        public string Description { get; set; }
        public string Icon { get; set; }
    }

}


