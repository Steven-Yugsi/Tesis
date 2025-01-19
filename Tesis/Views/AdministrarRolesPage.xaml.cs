using System;
using Xamarin.Forms;
using Tesis.ViewModels;

namespace Tesis.Views
{
    public partial class AdministrarRolesPage : ContentPage
    {
        // Declaramos el ViewModel como propiedad de la clase
        private AdministrarRolesViewModel viewModel;

        public AdministrarRolesPage()
        {
            InitializeComponent();

            // Inicializamos el ViewModel
            viewModel = new AdministrarRolesViewModel();
            BindingContext = viewModel;

            // Cargar roles desde Firebase al iniciar la página
            viewModel.CargarRolesAsync().ConfigureAwait(false);
        }

        private async void OnCrearRolesClicked(object sender, EventArgs e)
        {
            // Navegar a la página de CrearRoles
            await Navigation.PushAsync(new CrearRolesPage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Recargar roles cada vez que la página reaparece
            viewModel.CargarRolesCommand.Execute(null);
        }
    }
}
