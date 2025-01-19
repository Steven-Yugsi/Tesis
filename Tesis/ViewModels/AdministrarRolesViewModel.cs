using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Tesis.Models;
using System.Linq;
using System;
using Tesis.Conexion;
using Xamarin.Forms;
using System.Collections.Generic;

namespace Tesis.ViewModels
{
    public class AdministrarRolesViewModel
    {
        public ObservableCollection<Roles> Roles { get; set; }
        public Command CargarRolesCommand { get; set; }
        public bool IsRefreshing { get; set; }  // Lista de roles para vincular con la UI

        public AdministrarRolesViewModel()
        {
            Roles = new ObservableCollection<Roles>();
            CargarRolesCommand = new Command(async () => await CargarRolesAsync());
        }

        // Método para cargar roles desde Firebase
        public async Task CargarRolesAsync()
        {
            try
            {
                var rolesFirebase = await Conexionfirebase.firebase
                    .Child("Roles")
                    .OnceAsync<Roles>(); // Descarga los datos desde Firebase

                // Limpiar la lista y agregar los roles
                Roles.Clear();
                foreach (var rol in rolesFirebase)
                {
                    // Filtrar objetos válidos (que tienen Nombre y Descripcion)
                    if (!string.IsNullOrEmpty(rol.Object.Nombre) && !string.IsNullOrEmpty(rol.Object.Descripcion))
                    {
                        Roles.Add(rol.Object);
                    }
                }
                Console.WriteLine($"Total de roles agregados: {Roles.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar los roles: {ex.Message}");
            }
        }
    }
}
