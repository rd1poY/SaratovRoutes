using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace SaratovRoutes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
          
            MapInit(); // Вызываем метод без передачи координат
        }

        public MapPage(string coordinatesString)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            MapInit(coordinatesString); // Вызываем метод с передачей координат
        }

        public void MapInit(string coordinatesString = null)
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

                function init() {{
                    var map = new ymaps.Map(""map"", {{
                        center: [51.533557, 46.034257],
                        zoom: 12
                    }});

                    // Добавляем пользовательскую кнопку для установки метки на заданных координатах
                    var addMarkerButton = new ymaps.control.Button({{
                        data: {{
                            content: 'Моё местоположение'
                        }},
                        options: {{
                            maxWidth: 150
                        }}
                    }});
                    map.controls.add(addMarkerButton, {{ float: 'left' }});

                    addMarkerButton.events.add('click', function (e) {{
                        var predefinedCoordinates = [51.505154, 45.924671];
                        addMarker(map, predefinedCoordinates);
                    }});

                    // Проверяем наличие координат
                    if ('{coordinatesString}' !== '') {{
                        buildRoute(map, '{coordinatesString}');
                    }}
                }}

                function buildRoute(map, coordinates) {{
                    var coordinatesArray = coordinates.split('|').map(function(coord) {{
                        return coord.split(',').map(function(value) {{
                            return parseFloat(value);
                        }});
                    }});

                    map.setCenter(coordinatesArray[0], 12);

                    var multiRoute = new ymaps.multiRouter.MultiRoute({{
                        referencePoints: coordinatesArray,
                        params: {{
                            routingMode: 'pedestrian'
                        }}
                    }}, {{
                        boundsAutoApply: true
                    }});
                    multiRoute.options.set({{
                        routeStrokeColor: '#0000ff',
                        routeStrokeWidth: 2
                    }});

                    map.geoObjects.add(multiRoute);
                }}

                function addMarker(map, coordinates) {{
                      map.setCenter(coordinates, 15);
                    var placemark = new ymaps.Placemark(coordinates, {{
                        hintContent: 'Метка',
                        balloonContent: 'Вы находитесь здесь'
                    }}, {{
                        preset: 'islands#redDotIcon'
                    }});
                    map.geoObjects.add(placemark);
                }}
            </script>
        </body>
        </html>
    ";
            CheckAndRequestLocationPermission();
            MapView.Source = new HtmlWebViewSource { Html = htmlContent };
        }

        private async void ButtonRoutes_Clicked(object sender, EventArgs e)
        {
            RoutesPage newPage = new RoutesPage();
            await Navigation.PushAsync((Page)newPage);
        }

        private async void ButtonWeather_Clicked(object sender, EventArgs e)
        {
            WeatherPage newPage = new WeatherPage();
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
        async void CheckAndRequestLocationPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status == PermissionStatus.Granted)
            {
                // Разрешение на доступ к геолокации получено, можно продолжить
            }
            else
            {
                // Разрешение на доступ к геолокации не получено
            }
        }

    }
}