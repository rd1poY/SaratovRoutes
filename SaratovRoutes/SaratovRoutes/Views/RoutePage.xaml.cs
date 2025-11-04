using Newtonsoft.Json;
using SaratovRoutes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;

namespace SaratovRoutes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutePage : ContentPage
    {
        private Route _route;
        private string _routesFilePath;

        public RoutePage(Route route)
        {
            InitializeComponent();
            _route = route;
            BindingContext = route;
            _routesFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Routes.json");
            ImagesCarousel.ItemsSource = new List<string> { _route.ImgRoute, _route.ImgRoutes };
        }

        private async void ButtonMapOpen_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MapPage(_route.Coordinates));
        }

        private async void ButtonDelete_Clicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Удаление", "Вы уверены, что хотите удалить этот маршрут?", "Да", "Нет");
            if (confirm)
            {
                DeleteRouteFromFile(_route.Id);
                await Navigation.PopAsync();
            }
        }

        private void DeleteRouteFromFile(int routeId)
        {
            if (File.Exists(_routesFilePath))
            {
                var json = File.ReadAllText(_routesFilePath);
                var routes = JsonConvert.DeserializeObject<List<Route>>(json);

                var routeToDelete = routes.FirstOrDefault(r => r.Id == routeId);
                if (routeToDelete != null)
                {
                    routes.Remove(routeToDelete);
                    json = JsonConvert.SerializeObject(routes, Formatting.Indented);
                    File.WriteAllText(_routesFilePath, json);
                }
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

        private void ButtonModerate_Clicked(object sender, EventArgs e)
        {
            var email = "LitvinovArtem73@yandex.ru";
            var subject = Uri.EscapeUriString("Отправка маршрута на модерацию");
            var body = Uri.EscapeUriString($"Если хотите чтобы в маршруте использовались ваши изображения приложите их к письму\n Маршрут: {_route.Title}\nГород: {_route.City}\nВремя: {_route.time}\nОписание: {_route.Description}\nКоординаты: {_route.Coordinates}");

            var mailtoUri = $"mailto:{email}?subject={subject}&body={body}";

            // Открываем почтовый клиент с предзаполненным шаблоном письма
            Device.OpenUri(new Uri(mailtoUri));
        }
    }
}