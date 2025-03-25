using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;

namespace Tesis.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashPage : ContentPage
    {
        public SplashPage()
        {
            InitializeComponent();
            StartAnimation();
        }
        private async void StartAnimation()
        {
            await Task.Delay(500);
            await this.FadeTo(1, 1000);

            await Circle.FadeTo(1, 800);
            await Circle.ScaleTo(4, 1200, Easing.CubicInOut);
            await Circle.FadeTo(0, 800);

            await Title.FadeTo(1, 1000, Easing.Linear);
            await Title.TranslateTo(0, -50, 800, Easing.CubicInOut);

            await Task.Delay(1200); 

            Application.Current.MainPage = new NavigationPage(new LoginPage());

        }
    }

   
}