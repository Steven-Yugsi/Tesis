using Xamarin.Forms;
using Tesis.ViewModels;
using System;
using Tesis.Models;

namespace Tesis.Views
{
    public partial class UserListPage : ContentPage
    {
        private UserListViewModel viewModel;
        private bool _isFirstLoad = true;


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

           
            if (_isFirstLoad)
            {
                // Evitar disparar el evento por la asignación de TipoPerfil
                viewModel.CargarUsuariosCommand.Execute(null);
                _isFirstLoad = false; // Cambiar a falso para futuras cargas
            }
        }
        private async void OnTipoPerfilChanged(object sender, EventArgs e)
        {
            // Verificar si es la primera carga
            if (_isFirstLoad)
                return;

            // Validar que el sender sea un Picker
            var picker = sender as Picker;
            if (picker == null || picker.SelectedItem == null)
                return;

            var usuarioSeleccionado = picker.BindingContext as MUsuarios;
            var selectedPerfil = picker.SelectedItem as string;

            if (usuarioSeleccionado != null && !string.IsNullOrEmpty(selectedPerfil))
            {
                await (BindingContext as UserListViewModel)
                    .ActualizarTipoPerfilAsync(usuarioSeleccionado, selectedPerfil);
            }
        }
    }
}