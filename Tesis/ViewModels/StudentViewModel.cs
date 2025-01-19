//using System.Collections.ObjectModel;
//using System.Windows.Input;
//using Tesis.ViewModels;
//using Xamarin.Forms;

//public class StudentViewModel : BaseViewModel
//{
//    public ObservableCollection<string> Mensajes { get; set; }
//    public ICommand EnviarMensajeCommand { get; }

//    public StudentViewModel()
//    {
//        Mensajes = new ObservableCollection<string>();
//        EnviarMensajeCommand = new Command(EnviarMensaje);
//    }

//    private void EnviarMensaje()
//    {
//        if (!string.IsNullOrWhiteSpace(NuevoMensaje))
//        {
//            Mensajes.Add($"Tú: {NuevoMensaje}");
//            NuevoMensaje = string.Empty;

//            // Aquí puedes integrar la lógica para enviar el mensaje a la base de datos
//        }
//    }

//    private string _nuevoMensaje;
//    public string NuevoMensaje
//    {
//        get => _nuevoMensaje;
//        set => SetProperty(ref _nuevoMensaje, value);
//    }
//}
