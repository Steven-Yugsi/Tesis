using System;
using Xamarin.Forms;
using Firebase.Database;
using System.Threading.Tasks;
using Tesis.Views;
using Firebase.Database.Query;
using Xamarin.Essentials;

namespace Tesis
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new LoginPage());
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }

}
