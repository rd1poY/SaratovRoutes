using Newtonsoft.Json;
using SaratovRoutes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SaratovRoutes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AttrationsPage : ContentPage
    {
        public AttrationsPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            AttrationsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Attration.json");
            ViewAttrations();
        }
        public List<Attration> allAttrations;
        private readonly string AttrationsFilePath;
        //private readonly string[] cities = { "Вольск", "Саратов", "Энгельс", "Балашов", "Лох", "Маркс", "Хвалынск", "Петровск" };

    

        private void LoadCities()
        {
            var cities = allAttrations.Select(route => route.City).Distinct().ToList();
            CityPicker.ItemsSource = cities;

        }

        public void ViewAttrations()
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(RoutesPage)).Assembly;
            string jsonFileName = "Attration.json";  // Укажите имя вашего JSON файла

            using (Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.{jsonFileName}"))
            {
                using (var reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();

                    
                    var items = JsonConvert.DeserializeObject<List<Attration>>(json);

                    AttrationView.ItemsSource = items;
                }
            }
           LoadCities();
        }

     

        public void ViewRoutes()
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(RoutesPage)).Assembly;
            string jsonFileName = "Routes.json"; // Укажите имя вашего JSON файла

            using (Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.{jsonFileName}"))
            {
                using (var reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();

                    // Десериализация JSON в массив объектов Route
                    var items = JsonConvert.DeserializeObject<List<Route>>(json);

                    //RouteView.ItemsSource = items;
                }
            }
        }
        public void UpdateAttrations()
        {
            ViewAttrations(); // Обновляем список маршрутов
        }

        private void OnCitySelected(object sender, EventArgs e)
        {
           FilterAttrations();
        }

        private void FilterAttrations()
        {
            string selectedCity = CityPicker.SelectedItem?.ToString();
          string searchText = AttrationsearchBar.Text?.ToLower() ?? string.Empty;

            var filteredAttrations = allAttrations.Where(attration =>
                (string.IsNullOrWhiteSpace(selectedCity) || attration.City == selectedCity) &&
                (string.IsNullOrWhiteSpace(searchText) || attration.Title.ToLower().Contains(searchText))
            ).ToList();

            AttrationView.ItemsSource = filteredAttrations;
        }

        private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Route selectedRoute)
            {
               await Navigation.PushAsync(new RoutePage(selectedRoute));
            }
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

        protected override void OnAppearing()
        {
            ViewAttrations();
          FilterAttrations();
            base.OnAppearing();
        }

        private async void ButtonMap_Clicked(object sender, EventArgs e)
        {
            MapPage newPage = new MapPage();
            await Navigation.PushAsync(newPage);
        }

        private async void ButtonWeather_Clicked(object sender, EventArgs e)
        {
            WeatherPage newPage = new WeatherPage();
            await Navigation.PushAsync(newPage);
        }

       

        private void OnClearFilterClicked(object sender, EventArgs e)
        {
            CityPicker.SelectedItem = null;
            FilterAttrations();
        }
        private async void ButtonRoutes_Clicked(object sender, EventArgs e)
        {
            RoutesPage newPage = new RoutesPage();
            await Navigation.PushAsync((Page)newPage);
        }

        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
         FilterAttrations();
        }

       


    }
}
