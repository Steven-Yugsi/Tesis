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
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();            
            BindingContext = new RegisterViewModel(); // Esto debe estar antes de InitializeComponent()


        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        private void OnNameTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = (Entry)sender;
            string newText = e.NewTextValue;

            // Permitir solo letras y espacios
            if (!string.IsNullOrEmpty(newText))
            {
                entry.Text = new string(newText.Where(c => char.IsLetter(c) || char.IsWhiteSpace(c)).ToArray());
            }
        }

        // Método para validar el teléfono
        private void OnPhoneTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = (Entry)sender;
            string newText = e.NewTextValue;

            // Permitir solo números y limitar a 10 caracteres
            if (!string.IsNullOrEmpty(newText))
            {
                entry.Text = new string(newText.Where(char.IsDigit).Take(10).ToArray());
            }
        }
    }
}