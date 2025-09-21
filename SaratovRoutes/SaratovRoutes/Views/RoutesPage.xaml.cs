using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaratovRoutes.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;


namespace SaratovRoutes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutesPage : ContentPage
    {
        public List<Route> allRoutes;
        private readonly string routesFilePath;
        //private readonly string[] cities = { "Вольск", "Саратов", "Энгельс", "Балашов", "Лох", "Маркс", "Хвалынск", "Петровск" };

        public RoutesPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            routesFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Routes.json");
            ViewRoutes();
            var addRoutePage = new AddRoutePage(this); // Передаем текущий экземпляр RoutesPage в AddRoutePage
            addRoutePage.RouteAdded += OnRouteAdded;
        }

        private void LoadCities()
        {
            var cities = allRoutes.Select(route => route.City).Distinct().ToList();
            CityPicker.ItemsSource = cities;
            
        }

        public void ViewRoutes()
        {
            // Загрузка встроенных маршрутов
            var embeddedRoutes = LoadEmbeddedRoutes();

            // Загрузка пользовательских маршрутов
            var userRoutes = GetUserRoutes();

            // Объединение списков маршрутов
            allRoutes = embeddedRoutes.Concat(userRoutes).ToList();

            // Изначально показываем все маршруты
            RouteView.ItemsSource = allRoutes;
            LoadCities();
        }

        private List<Route> LoadEmbeddedRoutes()
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(RoutesPage)).Assembly;
            string jsonFileName = "Routes.json";

            using (Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.{jsonFileName}"))
            {
                if (stream == null)
                {
                    return new List<Route>();
                }

                using (var reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<List<Route>>(json) ?? new List<Route>();
                }
            }
        }
        private List<Route> GetUserRoutes()
        {
            if (!File.Exists(routesFilePath))
            {
                return new List<Route>();
            }

            var json = File.ReadAllText(routesFilePath);
            return JsonConvert.DeserializeObject<List<Route>>(json) ?? new List<Route>();
        }

        public void UpdateRoutes()
        {
            ViewRoutes(); // Обновляем список маршрутов
        }

        private void OnCitySelected(object sender, EventArgs e)
        {
            FilterRoutes();
        }

        private void FilterRoutes()
        {
            string selectedCity = CityPicker.SelectedItem?.ToString();
            string searchText = RouteSearchBar.Text?.ToLower() ?? string.Empty;

            var filteredRoutes = allRoutes.Where(route =>
                (string.IsNullOrWhiteSpace(selectedCity) || route.City == selectedCity) &&
                (string.IsNullOrWhiteSpace(searchText) || route.Title.ToLower().Contains(searchText))
            ).ToList();

            RouteView.ItemsSource = filteredRoutes;
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
            ViewRoutes();
            FilterRoutes();
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

        private async void ButtonAdd_Clicked(object sender, EventArgs e)
        {
            AddRoutePage newPage = new AddRoutePage(this);
            await Navigation.PushAsync(newPage);
        }

        private void OnClearFilterClicked(object sender, EventArgs e)
        {
            CityPicker.SelectedItem = null;
            FilterRoutes();
        }

        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            FilterRoutes();
        }

        private void OnRouteAdded(object sender, EventArgs e)
        {
            // Обновляем список маршрутов
            ViewRoutes();
            Console.WriteLine("Route added event triggered.");
        }


    }
}