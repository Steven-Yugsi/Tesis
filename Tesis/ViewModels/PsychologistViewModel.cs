//using System.Collections.ObjectModel;
//using System.Windows.Input;
//using Xamarin.Forms;
//using Tesis.Models;
//using Tesis.ViewModels;
//using System.Reactive;

//public class PsychologistViewModel : BaseViewModel
//{
//    public ObservableCollection<Notification> Notificaciones { get; set; }
//    public ICommand VerEstadisticasCommand { get; }

//    public PsychologistViewModel()
//    {
//        Notificaciones = new ObservableCollection<  Notificacion>();
//        VerEstadisticasCommand = new Command(VerEstadisticas);

//        // Cargar notificaciones (puedes reemplazar con datos reales)
//        CargarNotificaciones();
//    }

//    private void CargarNotificaciones()
//    {
//        // Simulación de notificaciones
//        Notificaciones.Add(new Notificacion
//        {
//            Estudiante = "Juan Pérez",
//            Mensaje = "Posible caso de bullying detectado.",
//            FechaHora = "07/01/2025 10:15 AM"
//        });
//    }

//    private void VerEstadisticas()
//    {
//        // Navegar a la página de estadísticas (a implementar)
//    }
//}
