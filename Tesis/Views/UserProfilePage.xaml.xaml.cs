using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesis.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tesis.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserProfilePage : ContentPage
    {
       // private UserProfileViewModel viewModel;

        public UserProfilePage(string userId)
        {
            InitializeComponent();
            var viewModel = new UserProfileViewModel();
            BindingContext = viewModel;


            // Cargar datos del usuario (asegúrate de que LoadUserData sea un método asíncrono en tu ViewModel)
            _ = LoadUserDataAsync(viewModel, userId);
        }
        private async Task LoadUserDataAsync(UserProfileViewModel viewModel, string userId)
        {
            try
            {
                await viewModel.LoadUserData(userId);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron cargar los datos: {ex.Message}", "Aceptar");
            }
        }
    }
}