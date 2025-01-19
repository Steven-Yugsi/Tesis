using Xamarin.Forms;
using Tesis.ViewModels;

namespace Tesis.Views
{
    public partial class UserListPage : ContentPage
    {
        private UserListViewModel viewModel;

        public UserListPage()
        {
            InitializeComponent();

            // Asigna el ViewModel al BindingContext
            viewModel = new UserListViewModel();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Ejecuta los comandos al mostrar la página
            viewModel.CargarUsuariosCommand.Execute(null);
        }

    }
}
