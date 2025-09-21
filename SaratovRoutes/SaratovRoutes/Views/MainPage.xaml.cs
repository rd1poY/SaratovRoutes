using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using SaratovRoutes.Views;

namespace SaratovRoutes
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<string> yourImageCollection;

        public ObservableCollection<string> YourImageCollection
        {
            get { return yourImageCollection; }
            set
            {
                if (yourImageCollection != value)
                {
                    yourImageCollection = value;
                    OnPropertyChanged(nameof(YourImageCollection));
                }
            }
        }
        public MainPage()
        {
            InitializeComponent();
            YourImageCollection = new ObservableCollection<string>
        {
            "Carousel1.png",
            "Carousel2.png",
            "Carousel3.png"
        };
            BindingContext = this;
        }
        private async void ButtonGuest_Clicked(object sender, EventArgs e)
        {

            RoutesPage newPage = new RoutesPage();
            await Navigation.PushAsync((Page)newPage);
        }

    }
}
