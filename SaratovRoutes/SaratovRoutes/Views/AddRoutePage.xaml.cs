using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SaratovRoutes.Models;

namespace SaratovRoutes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddRoutePage : ContentPage
    {
        private readonly string routesFilePath;
        private readonly RoutesPage _routesPage;
        public event EventHandler RouteAdded;

        public AddRoutePage(RoutesPage routesPage)
        {
            InitializeComponent();
            _routesPage = routesPage;
            routesFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Routes.json");
            MapInit();
        }

        private void MapInit()
        {
            string htmlContent = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <title>Карта с Яндекс.Карты API</title>
                <script src=""https://api-maps.yandex.ru/2.1/?apikey=b9ab7df0-cc81-43ff-ad44-a9f227e9ea53&lang=ru_RU"" type=""text/javascript""></script>
                <style>
                    html, body {{
                        margin: 0;
                        padding: 0;
                        height: 100%;
                        width: 100%;
                    }}
                    #map {{
                        width: 100%;
                        height: 100%;
                    }}
                </style>
            </head>
            <body>
                <div id=""map""></div>
                <script>
                    ymaps.ready(init);
                    var coordinatesArray = [];
                    var placemarksArray = [];
                    var map;
                    function init() {{
                        map = new ymaps.Map(""map"", {{
                            center: [51.505154, 45.924671],
                            zoom: 12
                        }});
                        map.events.add('click', function (e) {{
                            var coords = e.get('coords');
                            coordinatesArray.push(coords);
                            var placemark = new ymaps.Placemark(coords, {{
                                balloonContent: 'Место клика: ' + coords,
                                iconContent: coordinatesArray.length
                            }}, {{
                                preset: 'islands#blueCircleIcon'
                            }});
                            map.geoObjects.add(placemark);
                            placemarksArray.push(placemark);
                            updateHiddenInput();
                            if (coordinatesArray.length === 1) {{
                                getCityName(coords);
                            }}
                        }});
                    }}
                    function updateHiddenInput() {{
                        var coordsString = coordinatesArray.map(function(coord) {{
                            return coord.join(',');
                        }}).join('|');
                        document.getElementById('coordsInput').value = coordsString;
                    }}
                    function getCityName(coords) {{
                        ymaps.geocode(coords).then(function (res) {{
                            var geoObject = res.geoObjects.get(0);
                            var city = geoObject.getLocalities().length ? geoObject.getLocalities()[0] : 'Город не найден';
                            document.getElementById('cityNameInput').value = city;
                        }});
                    }}
                    function removeLastPoint() {{
                        if (coordinatesArray.length > 0) {{
                            coordinatesArray.pop();
                            var lastPlacemark = placemarksArray.pop();
                            map.geoObjects.remove(lastPlacemark);
                            updateHiddenInput();
                        }}
                    }}
                </script>
                <input type=""hidden"" id=""coordsInput"" />
                <input type=""hidden"" id=""cityNameInput"" />
            </body>
            </html>";

            MapView.Source = new HtmlWebViewSource { Html = htmlContent };
        }

        private async void OnAddRouteClicked(object sender, EventArgs e)
        {
            var coords = await MapView.EvaluateJavaScriptAsync("document.getElementById('coordsInput').value;");
            var cityName = await MapView.EvaluateJavaScriptAsync("document.getElementById('cityNameInput').value;");

            if (!string.IsNullOrEmpty(coords))
            {
                string routeName = RouteName.Text;
                string routeDescription = RouteDescription.Text;
                string routeTime = RouteTime.Text;

                var newRoute = new Route
                {
                    Id = GetNewRouteId(),
                    time = routeTime,
                    Title = routeName,
                    Type = "Пмаршрут",
                    City = cityName,
                    ImgRoutes = "user_route.jpg",
                    ImgRoute = "user_route.jpg",
                    Description = routeDescription,
                    Coordinates = coords
                };

                AddRouteToFile(newRoute);
                RouteAdded?.Invoke(this, EventArgs.Empty);

                _routesPage.UpdateRoutes();

                await Navigation.PopAsync();
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
        private int GetNewRouteId()
        {
            var routes = GetRoutesFromFile();
            int maxUserRouteId = routes
                .Where(r => r.Id >= 1000)
                .DefaultIfEmpty(new Route { Id = 999 })
                .Max(r => r.Id);
            return maxUserRouteId + 1;
        }

        private List<Route> GetRoutesFromFile()
        {
            if (!File.Exists(routesFilePath))
            {
                return new List<Route>();
            }

            var json = File.ReadAllText(routesFilePath);
            return JsonConvert.DeserializeObject<List<Route>>(json);
        }

        private void AddRouteToFile(Route newRoute)
        {
            var routes = GetRoutesFromFile();
            routes.Add(newRoute);

            var json = JsonConvert.SerializeObject(routes, Formatting.Indented);
            File.WriteAllText(routesFilePath, json);
        }

        private void OnRemoveLastPointClicked(object sender, EventArgs e)
        {
            MapView.EvaluateJavaScriptAsync("removeLastPoint();");
        }
    }
}