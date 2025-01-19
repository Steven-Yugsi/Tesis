using Xamarin.Forms;
using Tesis.ViewModels;

namespace Tesis.Views
{
    public partial class CrearRolesPage : ContentPage
    {
        public CrearRolesPage()
        {
            InitializeComponent();
            BindingContext = new RolesViewModel();
        }
    }
}
