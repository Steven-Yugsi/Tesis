using Tesis.Models;
using Tesis.ViewModels;
using Xamarin.Forms;

namespace Tesis.Views
{
    public partial class ObservacionPage : ContentPage
    {
        public ObservacionPage(Alerta alerta)
        {
            InitializeComponent();
            BindingContext = new ObservacionViewModel(alerta, this.Navigation);
        }
    }
}