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

            // Ejecuta los comandos al mostrar la página
            viewModel.CargarUsuariosCommand.Execute(null);
            if (_isFirstLoad)
            {
                // Evitar disparar el evento por la asignación de TipoPerfil
                
                _isFirstLoad = false; // Cambiar a falso para futuras cargas
            }
        }
        private async void OnTipoPerfilChanged(object sender, EventArgs e)
        {
            // Manejar el evento de cambio de selección de perfil

            // Evitar que se ejecute en la primera carga
            if (_isFirstLoad)
            {
                _isFirstLoad = false;
                return;
            }

            // El "sender" es el Picker dentro de la celda del CollectionView
            var picker = sender as Picker;

            if (picker != null && picker.SelectedItem != null)
            {
                // El BindingContext del Picker es el usuario de la celda
                var usuarioSeleccionado = picker.BindingContext as MUsuarios;

                if (usuarioSeleccionado != null)
                {
                    var selectedPerfil = picker.SelectedItem as string;

                    // Llama al método en el ViewModel para actualizar el tipo de perfil
                    await (BindingContext as UserListViewModel)
                        .ActualizarTipoPerfilAsync(usuarioSeleccionado, selectedPerfil);
                }
            }
        }
    }
}