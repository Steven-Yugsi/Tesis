using Tesis.Views;
using Xamarin.Forms;

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
