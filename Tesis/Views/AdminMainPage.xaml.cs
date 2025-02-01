using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tesis.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminMainPage : ContentPage
    {
        public AdminMainPage()
        {
            InitializeComponent();
          

        }
        private async void OnVerUsuariosClicked(object sender, EventArgs e)
        {
            // Llama al método de la página principal para abrir la lista de usuarios
            if (Application.Current.MainPage is FlyoutPage flyoutPage)
            {
                var flyoutDetailPage = flyoutPage.Detail as NavigationPage;
                await flyoutDetailPage.Navigation.PushAsync(new UserListPage());
            }
        }
    }
}